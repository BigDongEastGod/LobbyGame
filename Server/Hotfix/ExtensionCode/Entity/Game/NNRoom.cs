using System.Collections.Generic;
using ETHotfix;

namespace ETModel
{
    /// <summary>
    /// 牛牛房间系统
    /// </summary>
    public class NNRoom : Room
    {
        public readonly List<SPlayer> Players = new List<SPlayer>();

        public int MaxPlayer = 2;

        public int MaxDish = 0;

        private int _currentDish = 0;

        public NNChess ChessRules;

        public override void AddRules(byte[] rules)
        {
            ChessRules = ProtobufHelper.FromBytes<NNChess>(rules);
        }

        public override void JionRoom(SPlayer player)
        {
            if (Players.Count < MaxPlayer)
            {
                Players.ForEach(d => d.GetActorProxy.Send(new JoinRoomAnnunciate() {AccountId = player.Id, Error = 0}));
                
                Players.Add(player);
            }
            else
            {
                // 发送房间人数满的消息

                player.GetActorProxy.Send(new JoinRoomAnnunciate() {AccountId = player.Id, Error = -1});
            }
        }

        public override void QuitRoom(SPlayer player)
        {
            Players.Remove(player);
            
            // 发送离开房间消息
            
            Players.ForEach(d => d.GetActorProxy.Send(new JoinRoomAnnunciate() {AccountId = player.Id, Error = -2}));
        }

        public override void StartGame()
        {
            if (Players.Count < MaxPlayer)
            {
                //TODO:房间人数不够、无法开始游戏
            }
        }

        public override void EndGame()
        {
            if (MaxDish > 0 && _currentDish > MaxDish)
            {
                //TODO:结束游戏、结算数据
            }
            else
            {
                //TODO:开始下一轮游戏

                StartGame();
            }
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