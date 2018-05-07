using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 牛牛房间系统
    /// </summary>
    public class NNRoom : ETModel.Room
    {
        #region 成员变量
        
        // 当前局数

        public int CurrentDish;
        
        // 游戏规则

        public NNChess ChessRules;
        
        // 卡牌数组

        public List<PokerCard> Cards { get; private set; }
        
        // 游戏状态实体类
        
        public class PlayerState
        {
            public bool IsBanker;
            
            public bool IsReceive;

            public int Bet;

            public List<PokerCard> Cards;
        }

        // 游戏逻辑委托队列

        public Queue<Action<SPlayer, object[]>> GameQueue = new Queue<Action<SPlayer, object[]>>();

        /// <summary>
        /// 玩家游戏状态
        /// </summary>
        
        public readonly Dictionary<SPlayer, PlayerState> GameStates = new Dictionary<SPlayer, PlayerState>();

        
        /// <summary>
        /// 解散房间委托
        /// </summary>
        public event Action<long,string> DissolveRoomAction;

        #endregion

        #region 规则和其他

        /// <summary>
        /// 添加游戏规则到该房间
        /// </summary>
        /// <param name="rules"></param>
        public override void AddRules(byte[] rules)
        {
            Rules = rules;
            
            ChessRules = ProtobufHelper.FromBytes<NNChess>(rules);
            
            // 清空队列
            
            GameQueue.Clear();
            
            GameQueue.Enqueue(CreateBankerMessage);  // 随机创建一个庄家
            
            GameQueue.Enqueue(StartBetMessage);  // 给玩家发送下注消息
            
            GameQueue.Enqueue(SendBetMessage);  // 发送下注消息给其他玩家
            
            GameQueue.Enqueue(DealPokerMessage);  // 给玩家发牌
            
            GameQueue.Enqueue(CalculateCardsMessage);  // 计算玩家手里卡牌
        }

        /// <summary>
        /// 检查玩家是否在这个房间中
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public override long PlayerIsInRomm(SPlayer player)
        {
            return Players.FirstOrDefault(d => d == player && d.IsActivity) != null ? this.Id : 0;
        }

        #endregion

        #region 游戏逻辑实现

        /// <summary>
        /// 随机创建一个庄家
        /// </summary>
        /// <param name="player"></param>
        /// <param name="args"></param>
        private void CreateBankerMessage(SPlayer player, params object[] args)
        {
            // 把玩家接收信息标记为false
            
            GameStates.Values.ForEach(d => d.IsReceive = false);
            
            // 随机选择出庄家

            var randomPlayerId = RandomHelper.RandomNumber(0, Players.Count);

            var banker = Players.ElementAt(randomPlayerId);

            var response = new GameInfoAnnunciate {Message = 0, Arg = null, UserName = banker.Account.UserName};
            
            // 并发送消息给其他玩家

            GameStates.Keys.ForEach(d => d.GetActorProxy.Send(response));
        }

        /// <summary>
        /// 给玩家发送下注消息
        /// </summary>
        /// <param name="player"></param>
        /// <param name="args"></param>
        private void StartBetMessage(SPlayer player, params object[] args)
        {
            // 把玩家接收信息标记为false
            
            GameStates.Values.ForEach(d => d.IsReceive = false);
            
            var response = new GameInfoAnnunciate {Message = 1, Arg = null};
            
            GameStates.Keys.ForEach(d =>
            {
                response.UserName = d.Account.UserName;
                
                d.GetActorProxy.Send(response);
            });
        }

        /// <summary>
        /// 发送下注消息给其他玩家
        /// </summary>
        private void SendBetMessage(SPlayer player, params object[] args)
        {
            if (GameStates.ContainsKey(player) == false || args[1] == null || args[1].Equals("")) return;
            
            // 存放玩家下注的点数
            
            GameStates[player].Bet = Convert.ToInt32(args[1]);
            
            // 发送下注消息给其他玩家

            var response = new GameInfoAnnunciate() {Arg = SerializeHelper.Instance.SerializeObject(args[1]), Message = 2};

            GameStates.Keys.Where(d => d != player).ForEach(d =>
            {
                response.UserName = d.Account.UserName;

                d.GetActorProxy.Send(response);
            });
        }

        /// <summary>
        /// 给玩家发牌
        /// </summary>
        /// <param name="player"></param>
        /// <param name="args"></param>
        private void DealPokerMessage(SPlayer player, params object[] args)
        {
            // 把玩家接收信息标记为false
            
            GameStates.Values.ForEach(d => d.IsReceive = false);
            
            var poker = GetComponent<Poker>();
                            
            // 生成卡牌

            Cards = poker.CreateCards(1);
                            
            // 分发卡牌

            var playerPokers = poker.Deal(Cards, Players.Count, 5, true);
                            
            // 创建发送协议

            var response = new GameInfoAnnunciate() {Message = 3};
                            
            // 获取所有参展玩家

            var activityPlayers = GameStates.Keys.Where(d => d.IsActivity).ToArray();
                            
            // 每个玩家分发卡牌

            for (var i = 0; i < activityPlayers.Count(); i++)
            {
                response.UserName = activityPlayers.ElementAt(i).Account.UserName;

                response.Arg = SerializeHelper.Instance.SerializeObject(playerPokers[i]);
                                
                activityPlayers[i].GetActorProxy.Send(response);

                GameStates[activityPlayers[i]].Cards = playerPokers[i];
            }
        }

        /// <summary>
        /// 计算玩家手里卡牌
        /// </summary>
        /// <param name="player"></param>
        /// <param name="args"></param>
        private void CalculateCardsMessage(SPlayer player, params object[] args)
        {
            // 把玩家接收信息标记为false
            
            GameStates.Values.ForEach(d => d.IsReceive = false);
            
            var poker = GetComponent<Poker>();
            
            // 计算好牌，并排序（如果有牛的话）

            var pokerStr = 0;

            var cards = poker.CalculateCards(GameStates[player].Cards).ToList();

            if (cards.Count > 0)
            {
                GameStates[player].Cards = cards;

                pokerStr = poker.Calculate(
                    (GameStates[player].Cards.ElementAt(3).CardNumber > 10 ? 10 : GameStates[player].Cards.ElementAt(3).CardNumber) +
                    (GameStates[player].Cards.ElementAt(4).CardNumber > 10 ? 10 : GameStates[player].Cards.ElementAt(4).CardNumber)
                );
            }

            // 创建发送协议

            var response = new GameInfoAnnunciate() {Message = 4, Arg = SerializeHelper.Instance.SerializeObject(pokerStr), UserName = player.Account.UserName};
            
            GameStates.Keys.ForEach(d =>
            {
                d.GetActorProxy.Send(response);
            });
        }

        #endregion

        #region 加入房间

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public override string JionRoom(SPlayer player)
        {
            // 当玩家已经在这个房间里

            if (Players.Contains(player) || Guest.Contains(player)) return "InRomm";
            
            // 发送数据给房间所有人

            var response = new RoomInfoAnnunciate() {UserName = player.Account.UserName, Message = 0};

            Players.Where(d => d != player && d.IsActivity).ForEach(d => d.GetActorProxy.Send(response));

            Guest.Where(d => d != player && d.IsActivity).ForEach(d => d.GetActorProxy.Send(response));
            
            // 添加玩家到客人列表
            
            Guest.Add(player);
            
            // 添加到玩家房间记录

            AddRoomsRecord(player);

            return "JionRoom";
        }

        /// <summary>
        /// 添加到玩家房间记录
        /// </summary>
        /// <param name="player"></param>
        private void AddRoomsRecord(SPlayer player)
        {
            // 添加到玩家房间记录

            var roomtype = Enum.GetName(typeof(RoomType), this.RoomType);

            if (!player.RoomsRecord.ContainsKey(roomtype)) player.RoomsRecord.Add(roomtype, new List<GameRoomInfo>());

            var gameroominfo = new GameRoomInfo()
            {
                RoomId = this.Id,
                PlayerMode = ChessRules.PlayerMode,
                Score = ChessRules.Score,
                Dish = ChessRules.Dish,
                PayMode = ChessRules.PayMode,
                PlayerCount = this.Players.Count + "/" + ChessRules.PlayerCount
            };

            var roominfo = player.RoomsRecord[roomtype].FirstOrDefault(d => d.RoomId == this.Id);

            if (roominfo == null)
            {
                player.RoomsRecord[roomtype].Add(gameroominfo);
            }
            else
            {
                roominfo = gameroominfo;
            }
            
            // 添加委托

            DissolveRoomAction += player.DissolveRoomABackCall;
        }

        #endregion

        #region 准备游戏

        /// <summary>
        /// 准备游戏
        /// </summary>
        /// <param name="player"></param>
        public override string Prepare(SPlayer player)
        {
            if (Players.Contains(player))
            {
                //TODO:断线重连需要发送重连消息

                player.Account.RoomId = this.Id;
                
                return "InPrepare"; // 玩家已经准备了
            }

            if (Guest.Contains(player)) Guest.Remove(player);  // 从客人列表移除
            
            // 发送数据给房间所有人
            
            var response = new RoomInfoAnnunciate() {UserName = player.Account.UserName, Message = 1};

            Players.Where(d => d != player && d.IsActivity).ForEach(d => d.GetActorProxy.Send(response));

            Guest.Where(d => d != player && d.IsActivity).ForEach(d => d.GetActorProxy.Send(response));
            
            // 添加到玩家列表
            
            player.Account.RoomId = this.Id;
            
            Players.Add(player);  
            
            // 发送给玩家开始游戏权限

            var startplayer = Players.FirstOrDefault(d => d.IsActivity);

            if (startplayer != null && startplayer == player)
            {
                player.GetActorProxy.Send(new RoomInfoAnnunciate() {UserName = startplayer.Account.UserName, Message = 3});
            }

            return "Prepare";
        }

        #endregion

        #region 退出房间

        public override void QuitRoom(SPlayer player)
        {
            if (player == null) return;
            
            // 发送离开房间消息

            var response = new RoomInfoAnnunciate();

            if (Players.Contains(player))
            {
                // 更改游戏玩家开始权限

                if (Players.FirstOrDefault(d => d.IsActivity) == player)
                {
                    var startPlayer = Players.FirstOrDefault(d => d != player && d.IsActivity);
                    
                    if (startPlayer != null)
                    {
                        response.UserName = startPlayer.Account?.UserName;

                        response.Message = 3;

                        // 发送给玩家开始游戏权限

                        startPlayer.GetActorProxy.Send(response);
                    }
                }
            
                Players.Remove(player);
            }
            
            // 如果是游客就删除游客

            if (Guest.Contains(player)) Guest.Remove(player);
            
            // 发送退出消息给其他玩家

            response.UserName = player.Account.UserName;

            response.Message = 2;

            Players.Where(d => d != player && d.IsActivity).ForEach(d => d.GetActorProxy.Send(response));

            Guest.Where(d => d != player && d.IsActivity).ForEach(d => d.GetActorProxy.Send(response));

            player.Account.RoomId = 0;
        }

        #endregion

        #region 开始游戏

        public override string StartGame(SPlayer player)
        {
            if (Players.Count < 2) return "CantStartGame";
            
            // 添加玩家到游戏状态列表
            
            GameStates.Clear();

            Players.Where(d => d.IsActivity).ForEach(d => GameStates.Add(d, new PlayerState()));
            
            // 执行栈中的逻辑

            GameQueue.Dequeue()?.Invoke(player, null);
            
            return "StartGame";
        }

        #endregion

        #region 结束游戏

        public override void EndGame()
        {

        }

        #endregion

        #region 消息回调

        /// <summary>
        /// 消息回调
        /// </summary>
        /// <param name="player"></param>
        /// <param name="args"></param>
        public override void SendMessages(SPlayer player, params object[] args)
        {
            /*       提示消息
                0:玩家接收庄家信息
                1:接收玩家下注消息
                2:接收玩家发牌消息
             */
            
            if (!GameStates.ContainsKey(player)) return;
            
            // 设置已接收标记

            GameStates[player].IsReceive = true;

            // 接收玩家下注消息
            
            if (Convert.ToInt32(args[0]) == 1)
            {
                // 发送下注消息给其他玩家

                GameQueue.Peek()?.Invoke(player, args);
                
                if (GameStates.Values.Count(d => d.IsReceive) == GameStates.Count)
                {
                    GameQueue.Dequeue();
                        
                    GameQueue.Dequeue()?.Invoke(player, args);
                }
            }
            else
            {
                // 如果该队列所有玩家都已经接受到消息、移除当前栈中的队列
                
                if (GameStates.Values.Count(d => d.IsReceive) == GameStates.Count) GameQueue.Dequeue()?.Invoke(player, args);
            }
        }

        #endregion

        #region 解散和Dispose

        public override void DissolveRoom(SPlayer player)
        {
            base.DissolveRoom(player);
            
            DissolveRoomAction?.Invoke(this.Id, Enum.GetName(typeof(RoomType), this.RoomType));

            if (DissolveRoomAction != null) DissolveRoomAction -= player.DissolveRoomABackCall;

            this.Dispose();
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            
            base.Dispose();
            
            Players.Clear();
            
            Guest.Clear();

            RoomManageComponent.Instance.Remove(this);
        }

        #endregion
    }
}