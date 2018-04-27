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
        /// 添加房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerId">玩家ID</param>
        /// <param name="roomType">房间类型</param>
        /// <returns></returns>
        public static bool Add(this RoomManageComponent self,long playerId,string roomType)
        {
            if (self.Rooms.Any(d => d.PlayerId == playerId)) return false;
            
            // 创建房间  
                
            var room = CreateRoom(roomType, playerId);
            
            // 随机生成房间号

            var roomId = int.Parse(RandomHelper.RandomNumber(100000,999999).ToString().Substring(0, 6));

            while (true)
            {
                if (self.GetRoom(roomId) == null) break;
                    
                roomId = int.Parse(RandomHelper.RandomNumber(100000,999999).ToString().Substring(0, 6));
            }

            self.Rooms.Add(new RoomManageComponent.RoomModel() {RoomId = roomId, PlayerId = playerId, Room = room});

            return true;
        }

        /// <summary>
        /// 获取房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roomId">房间ID</param>
        /// <returns></returns>
        public static Room GetRoom(this RoomManageComponent self,long roomId)
        {
            return self.Rooms.FirstOrDefault(d => d.RoomId == roomId)?.Room;
        }

        /// <summary>
        /// 获取房间ID
        /// </summary>
        /// <param name="self"></param>
        /// <param name="room">房间</param>
        /// <returns></returns>
        public static int? GetRoomId(this RoomManageComponent self,Room room)
        {
            return self.Rooms.FirstOrDefault(d => d.Room == room)?.RoomId;
        }

        /// <summary>
        /// 获取房间ID
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerId">用户ID</param>
        /// <returns></returns>
        public static int? GetRoomId(this RoomManageComponent self,long playerId)
        {
            return self.Rooms.FirstOrDefault(d => d.PlayerId == playerId)?.RoomId;
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="room">房间</param>
        public static void Remove(this RoomManageComponent self,Room room)
        {
            var removeroom = self.Rooms.FirstOrDefault(d => d.Room == room);

            if (removeroom == null) return;
            
            removeroom.Room.DissolveRoom();

            self.Rooms.Remove(removeroom);
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roomId">房间ID</param>
        public static void Remove(this RoomManageComponent self,long roomId)
        {
            var removeroom = self.Rooms.FirstOrDefault(d => d.RoomId == roomId);
            
            if (removeroom == null) return;
            
            removeroom.Room.DissolveRoom();

            self.Rooms.Remove(removeroom);
        }
        
        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="roomType"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        private static Room CreateRoom(string roomType,long playerId)
        {
            Room room = null;
            
            switch (roomType)
            {
                case "NN":
                    room = ComponentFactory.CreateWithId<NNRoom>(playerId);
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