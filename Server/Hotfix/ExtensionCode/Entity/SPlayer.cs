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

        /// <summary>
        /// 加入过的房间列表
        /// </summary>
        public readonly List<RoomComponent> RoomList = new List<RoomComponent>();

        /// <summary>
        /// 房间记录、存放创建过的房间和加入的房间
        /// </summary>
        public readonly Dictionary<string, List<GameRoomInfo>> RoomsRecords = new Dictionary<string, List<GameRoomInfo>>();

        /// <summary>
        /// 添加浏览房间记录
        /// </summary>
        /// <param name="roominfo"></param>
        /// <param name="room"></param>
        public void AddRoomsRecord(GameRoomInfo roominfo,RoomComponent room)
        {
            // 如果不存在该游戏类型就创建一个

            if (!RoomsRecords.ContainsKey(room.RoomLogicComponent.RoomType)) 
                RoomsRecords.Add(room.RoomLogicComponent.RoomType, new List<GameRoomInfo>());
            
            // 检查是否有当前房间信息

            var gameRoomInfo = RoomsRecords[room.RoomLogicComponent.RoomType].FirstOrDefault(d => d.RoomId == room.Id);
            
            // 如果有就删除

            if (gameRoomInfo != null) RoomsRecords[room.RoomLogicComponent.RoomType].Remove(gameRoomInfo);
            
            // 添加房间信息到玩家浏览房间列表
            
            RoomsRecords[room.RoomLogicComponent.RoomType].Add(roominfo);
            
            // 添加委托
            
            room.DissolveRoomAction -= DissolveRoomABackCall;

            room.DissolveRoomAction += DissolveRoomABackCall;
            
            RoomList.Add(room);
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

            RoomList.ForEach(d => d.DissolveRoomAction -= DissolveRoomABackCall);

            RoomList.Clear();
        }

        public ActorProxy GetActorProxy => Game.Scene.GetComponent<ActorProxyComponent>().Get(this.GateSessionId);

        public Account Account => GetComponent<Account>();
    }
}