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

            if (Guest.Any(d => d == player)) return "InRomm";
            
            // 发送数据给房间所有人

            var response = new RoomInfoAnnunciate() {UserName = player.Account.UserName, Message = 0};

            Players.ForEach(d => d.GetActorProxy.Send(response));

            Guest.ForEach(d => d.GetActorProxy.Send(response));
            
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
            if (Players.Any(d => d == player)) return "InPrepare";  // 玩家已经准备了
            
            if (Guest.Any(d => d == player) == false) return "NoInGuest"; // 玩家没有在游客房间
            
            Players.Add(player);  // 添加到玩家列表

            Guest.Remove(player); // 从客人列表移除
            
            // 发送数据给房间所有人
            
            var response = new RoomInfoAnnunciate() {UserName = player.Account.UserName, Message = 1};

            Players.Where(d => d != player).ForEach(d => d.GetActorProxy.Send(response));

            Guest.ForEach(d => d.GetActorProxy.Send(response));

            return "Prepare";
        }

        public override void QuitRoom(SPlayer player)
        {
            Players.Remove(player);

            Guest.Remove(player);
            
            // 发送离开房间消息
            
            var response = new RoomInfoAnnunciate() {UserName = player.Account.UserName, Message = 2};

            Players.Where(d => d != player).ForEach(d => d.GetActorProxy.Send(response));
            
            Guest.Where(d => d != player).ForEach(d => d.GetActorProxy.Send(response));
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
            
            Guest.Clear();

            RoomManageComponent.Instance.Remove(this);
        }
    }
}