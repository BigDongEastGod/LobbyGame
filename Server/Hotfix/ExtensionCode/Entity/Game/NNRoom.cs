using System.Collections.Generic;
using System.Linq;
using CSharpx;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 牛牛房间系统
    /// </summary>
    public class NNRoom : Room
    {
        public readonly Dictionary<SPlayer, bool> Players = new Dictionary<SPlayer, bool>();

        private int currentDish = 0;

        private NNChess chessRules;

        public override void AddRules(byte[] rules)
        {
            Rules = rules;
            
            chessRules = ProtobufHelper.FromBytes<NNChess>(Rules);
        }

        public override long PlayerIsInRomm(SPlayer player)
        {
            return Players.Keys.FirstOrDefault(d => d == player) != null ? this.Id : 0;
        }

        public override void JionRoom(SPlayer player)
        {
            // 当玩家已经在这个房间里

            if (Players.ContainsKey(player))
            {
                player.GetActorProxy.Send(new RoomInfoAnnunciate() {AccountId = player.Id, Message = -2});

                return;
            }

            // 如果房间小于指定人数，加入这个房间，并发送数据给房间里其他玩家
            
            if (Players.Count < chessRules.PlayerCount)
            {
                Players.Add(player, false);

                return;
            }
            
            // 发送房间人数满的消息

            player.GetActorProxy.Send(new RoomInfoAnnunciate() {AccountId = player.Id, Message = -1});
        }

        /// <summary>
        /// 准备游戏
        /// </summary>
        /// <param name="player"></param>
        public override void Prepare(SPlayer player)
        {
            if (!Players.ContainsKey(player)) return;
            
            Players[player] = true;

            Players.Keys.Where(d => d != player).ForEach(d => d.GetActorProxy.Send(new RoomInfoAnnunciate() {AccountId = player.Id, Message = 0}));
        }

        public override void QuitRoom(SPlayer player)
        {
            Players.Remove(player);
            
            // 发送离开房间消息

            Players.Keys.Where(d => d != player).ForEach(d => d.GetActorProxy.Send(new RoomInfoAnnunciate() {AccountId = player.Id, Message = -2}));
        }

        public override void StartGame()
        {
//            if (Players.Count < MaxPlayer)
//            {
//                //TODO:房间人数不够、无法开始游戏
//            }
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

        public override void DissolveRoom()
        {
            base.DissolveRoom();

            this.Dispose();
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            
            base.Dispose();
            
            Players.Clear();

            RoomManageComponent.Instance.Remove(this);
        }
    }
}