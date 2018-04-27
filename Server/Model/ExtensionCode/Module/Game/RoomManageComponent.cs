using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace ETModel
{
    [ObjectSystem]
    public class RoomManageAwakeSystem : AwakeSystem<RoomManageComponent>
    {
        public override void Awake(RoomManageComponent self)
        {
            self.Awake();
        }
    }
    
    public class RoomManageComponent : Component
    {
        public class RoomModel
        {
            public long RoomId;
            
            public long PlayerId;
            
            public Room Room;
        }
        
        public static RoomManageComponent Instance;
        
        private List<RoomModel> _rooms;
        
        public void Awake()
        {
            Instance = this;

            _rooms = new List<RoomModel>();
        }

        public bool Add(long roomId,long playerId,Room room)
        {
            if (_rooms.Any(d => d.PlayerId == playerId)) return false;

            _rooms.Add(new RoomModel() {RoomId = roomId, PlayerId = playerId, Room = room});

            return true;
        }

        public Room GetRoom(long roomId)
        {
            return _rooms.FirstOrDefault(d=>d.RoomId==roomId)?.Room;
        }

        public long? GetRoomId(Room room)
        {
            return _rooms.FirstOrDefault(d => d.Room == room)?.RoomId;
        }

        public void Remove(Room room)
        {
            var removeroom = _rooms.FirstOrDefault(d => d.Room == room);

            if (removeroom == null) return;
            
            removeroom.Room.DissolveRoom();

            _rooms.Remove(removeroom);
        }

        public void Remove(long roomId)
        {
            var removeroom = _rooms.FirstOrDefault(d => d.RoomId == roomId);
            
            if (removeroom == null) return;
            
            removeroom.Room.DissolveRoom();

            _rooms.Remove(removeroom);
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
            
            _rooms.ForEach(d=>d.Room.DissolveRoom());
            
            _rooms.Clear();
        }
    }
}