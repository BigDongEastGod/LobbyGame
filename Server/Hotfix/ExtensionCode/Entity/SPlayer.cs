using System;
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

        public Room CurrentRoom;

        /// <summary>
        /// 房间记录、存放创建过的房间和加入的房间
        /// </summary>
        public Dictionary<string, List<GameRoomInfo>> RoomsRecords = new Dictionary<string, List<GameRoomInfo>>();

        /// <summary>
        /// 添加浏览房间记录
        /// </summary>
        /// <param name="roominfo"></param>
        /// <param name="room"></param>
        public void AddRoomsRecord(GameRoomInfo roominfo,Room room)
        {
            // 添加到玩家房间记录

            var roomtype = Enum.GetName(typeof(RoomType), room.RoomType);
            
            // 如果不存在该游戏类型就创建一个

            if (!RoomsRecords.ContainsKey(roomtype)) RoomsRecords.Add(roomtype, new List<GameRoomInfo>());
            
            // 检查是否有当前房间信息

            var gameRoomInfo= RoomsRecords[roomtype].FirstOrDefault(d => d.RoomId == this.Id);
            
            // 如果有就删除

            if (gameRoomInfo != null) RoomsRecords[roomtype].Remove(roominfo);
            
            // 添加房间信息到玩家浏览房间列表
            
            RoomsRecords[roomtype].Add(roominfo);
            
            // 添加委托
            
            room.DissolveRoomAction -= DissolveRoomABackCall;

            room.DissolveRoomAction += DissolveRoomABackCall;

            CurrentRoom = room;
        }

        /// <summary>
        /// 解散房间的回调方法
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="roomtype"></param>
        public void DissolveRoomABackCall(long roomId,string roomtype)
        {
            if (!RoomsRecords.TryGetValue(roomtype, out var rooms)) return;

            var roominfo = rooms.FirstOrDefault(d => d.RoomId == roomId);

            if (roominfo != null) rooms.Remove(roominfo);
        }
        
        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
            
            RoomsRecords.Clear();

            CurrentRoom.DissolveRoomAction -= DissolveRoomABackCall;
        }

        public ActorProxy GetActorProxy => Game.Scene.GetComponent<ActorProxyComponent>().Get(this.GateSessionId);

        public Account Account => GetComponent<Account>();
    }
}