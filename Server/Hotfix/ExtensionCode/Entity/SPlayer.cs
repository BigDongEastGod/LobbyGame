using System.Collections.Generic;
using System.Linq;
using ETHotfix;
using Model.ExtensionCode.DB;

namespace ETModel
{
    /// <summary>
    /// 用户操作组件、后期添加操作数据库方法
    /// </summary>
    public class SPlayer : Entity
    {
        public long GateSessionId;

        public bool IsActivity;
        
        /// <summary>
        /// 用户房间浏览记录
        /// </summary>
        public Dictionary<string, List<GameRoomInfo>> RoomsRecord = new Dictionary<string, List<GameRoomInfo>>(); // 房间记录、存放创建过的房间和加入的房间

        /// <summary>
        /// 解散房间的回调方法
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="roomtype"></param>
        public void DissolveRoomABackCall(long roomId,string roomtype)
        {
            if (!RoomsRecord.TryGetValue(roomtype, out var rooms)) return;

            var roominfo = rooms.FirstOrDefault(d => d.RoomId == roomId);

            if (roominfo != null) rooms.Remove(roominfo);
        }
        
        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
        }

        public ActorProxy GetActorProxy => Game.Scene.GetComponent<ActorProxyComponent>().Get(this.GateSessionId);

        public Account Account => GetComponent<Account>();
    }
}