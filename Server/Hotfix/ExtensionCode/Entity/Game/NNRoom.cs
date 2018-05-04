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
        public int CurrentDish { get; private set; }

        public NNChess ChessRules { get; private set; }
        
        private class GameState
        {
            public bool IsBanker;
            
            public bool IsSend;

            public int Bet;
        }

        /// <summary>
        /// 玩家游戏状态
        /// </summary>
        
        private readonly Dictionary<SPlayer, GameState> _gameStates = new Dictionary<SPlayer, GameState>();

        public override void AddRules(byte[] rules)
        {
            Rules = rules;
            
            ChessRules = ProtobufHelper.FromBytes<NNChess>(rules);
        }

        public override long PlayerIsInRomm(SPlayer player)
        {
            return Players.FirstOrDefault(d => d == player && d.IsActivity) != null ? this.Id : 0;
        }

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

            return "JionRoom";
        }

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

        public override string StartGame(SPlayer player)
        {
            if (Players.Count < 2) return "CantStartGame";

            // 随机选择出庄家

            var randomPlayerId = new Random().Next(0, Players.Count);

            var response = new GameInfoAnnunciate {Message = 2, Arg = null, UserName = Players.ElementAt(randomPlayerId).Account.UserName};
            
            // 添加玩家到游戏状态中、并发送消息给其他玩家
            
            _gameStates.Clear();
            
            Players.ForEach(d =>
            {
                d.GetActorProxy.Send(response);

                _gameStates.Add(d, d == Players.ElementAt(randomPlayerId) ? new GameState() {IsBanker = true} : new GameState());
            });
            
            return "StartGame";
        }

        public override void EndGame()
        {
//            if (MaxDish > 0 && _currentDish > MaxDish)
//            {
//                //TODO:结束游戏、结算数据
//            }
//            else
//            {
//                //TODO:开始下一轮游戏
//
//                StartGame();
//            }
        }

        public override void SendMessages(SPlayer player, params object[] args)
        {
            // 0 : 庄家（抢庄完成） 0 : 下注完成

            switch (Convert.ToInt32(args[0]))
            {
                case 0:

                    // 如果庄家（抢庄）成功、给玩家发送开始下注消息
                    
                    if (_gameStates.Values.FirstOrDefault(d => d.IsBanker) != null)
                    {
                        if (_gameStates.FirstOrDefault(d => d.Value.IsBanker).Key == player)
                        {
                            // 给玩家发送下注消息

                            var response = new GameInfoAnnunciate {Message = 0, Arg = null};

                            Players.Where(d => d.IsActivity).ForEach(d =>
                            {
                                response.UserName = d.Account.UserName;

                                d.GetActorProxy.Send(response);
                            });
                        }
                    }
                    
                    break;
                case 1:
                    
                    // 用户下注
                    
                    if (_gameStates.ContainsKey(player))
                    {
                        _gameStates[player].Bet = Convert.ToInt32(args[1]);
                        
                        var response = new GameInfoAnnunciate() {Arg = SerializeHelper.Instance.SerializeObject(args[1]), Message = 1};

                        // 发送下注消息给其他玩家
                        
                        Players.Where(p => p != player).ForEach(d =>
                            {
                                response.UserName = player.Account.UserName;

                                d.GetActorProxy.Send(response);
                            }
                        );

                        _gameStates[player].IsSend = true;

                        // 判断是否全部下注成功
                        
                        if (_gameStates.Values.Count(d => !d.IsSend) == 0)
                        {
                            //TODO:全部下注成功、开始发牌了
                            
                            Log.Debug("全部下注成功、开始发牌了");
                        }
                    }
                    
                    break;
            }
        }

        public override void DissolveRoom(SPlayer player)
        {
            base.DissolveRoom(player);

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
    }
}