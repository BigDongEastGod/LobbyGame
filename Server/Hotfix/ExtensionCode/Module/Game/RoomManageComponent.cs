using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 房间组件扩展类
    /// </summary>
    public static class RoomManageComponentEx
    {
        
        /// <summary>
        /// 获取用户所在的房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="player">用户SPlayer</param>
        /// <returns></returns>
        public static Room GetRommByPlayer(this RoomManageComponent self,SPlayer player)
        {
            return self.Rooms.Values.FirstOrDefault(d => d.PlayerIsInRomm(player) != 0);
        }
        
        /// <summary>
        /// 添加房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerId">玩家ID</param>
        /// <param name="roomType">房间类型</param>
        /// <returns></returns>
        public static Room Add(this RoomManageComponent self,long playerId,string roomType)
        {
            if (self.Rooms.ContainsKey(playerId)) return null;
            
            // 随机生成房间号

            var roomId = int.Parse(IdGenerater.GenerateId().ToString().Substring(0, 6));

            while (true)
            {
                if (self.GetRoom(roomId) == null) break;
                    
                roomId = int.Parse(IdGenerater.GenerateId().ToString().Substring(0, 6));
            }
            
            // 创建房间  

            var room = CreateRoom(roomType, roomId);

            if (room != null) self.Rooms.Add(playerId, room);

            return room;
        }

        /// <summary>
        /// 获取房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roomId">房间ID</param>
        /// <returns></returns>
        public static Room GetRoom(this RoomManageComponent self,long roomId)
        {
            return self.Rooms.Values.FirstOrDefault(d => d.Id == roomId);
        }

        /// <summary>
        /// 获取房间ID
        /// </summary>
        /// <param name="self"></param>
        /// <param name="room">房间</param>
        /// <returns></returns>
        public static long GetRoomId(this RoomManageComponent self,Room room)
        {
            return self.Rooms.Values.FirstOrDefault(d => d == room)?.Id ?? 0;
        }

        /// <summary>
        /// 获取房间ID
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerId">用户ID</param>
        /// <returns></returns>
        public static long GetRoomId(this RoomManageComponent self,long playerId)
        {
            return self.Rooms.TryGetValue(playerId, out var room) ? room.Id : 0;
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="room">房间</param>
        public static void Remove(this RoomManageComponent self,Room room)
        {
            var removeroom = self.Rooms.FirstOrDefault(d => d.Value == room);

            if (default(KeyValuePair<long, Room>).Equals(removeroom)) return;
            
            removeroom.Value.DissolveRoom();

            self.Rooms.Remove(removeroom.Key);
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roomId">房间ID</param>
        public static void Remove(this RoomManageComponent self,long roomId)
        {
            var removeroom = self.Rooms.FirstOrDefault(d => d.Value.Id == roomId);
            
            if (default(KeyValuePair<long, Room>).Equals(removeroom)) return;

            self.Remove(removeroom.Value);
        }
        
        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="roomType"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        private static Room CreateRoom(string roomType,int roomId)
        {
            Room room = null;
            
            switch (roomType)
            {
                case "NN":
                    room = ComponentFactory.CreateWithId<NNRoom>(roomId);
                    break;
                case "DDZ":
                    break;
                case "SXMZ":
                    break;
            }

            return room;
        }
    }
}