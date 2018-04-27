using System.Linq;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 房间组件扩展类
    /// </summary>
    public static class RoomManageComponentEx
    {
        public static bool Add(this RoomManageComponent self,long roomId,long playerId,Room room)
        {
            if (self.Rooms.Any(d => d.PlayerId == playerId)) return false;

            self.Rooms.Add(new RoomManageComponent.RoomModel() {RoomId = roomId, PlayerId = playerId, Room = room});

            return true;
        }

        public static Room GetRoom(this RoomManageComponent self,long roomId)
        {
            return self.Rooms.FirstOrDefault(d=>d.RoomId==roomId)?.Room;
        }

        public static long? GetRoomId(this RoomManageComponent self,Room room)
        {
            return self.Rooms.FirstOrDefault(d => d.Room == room)?.RoomId;
        }

        public static void Remove(this RoomManageComponent self,Room room)
        {
            var removeroom = self.Rooms.FirstOrDefault(d => d.Room == room);

            if (removeroom == null) return;
            
            removeroom.Room.DissolveRoom();

            self.Rooms.Remove(removeroom);
        }

        public static void Remove(this RoomManageComponent self,long roomId)
        {
            var removeroom = self.Rooms.FirstOrDefault(d => d.RoomId == roomId);
            
            if (removeroom == null) return;
            
            removeroom.Room.DissolveRoom();

            self.Rooms.Remove(removeroom);
        }
    }
}