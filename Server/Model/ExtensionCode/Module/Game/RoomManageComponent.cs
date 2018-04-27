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
        
        public List<RoomModel> Rooms;
        
        public void Awake()
        {
            Instance = this;

            Rooms = new List<RoomModel>();
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
            
            Rooms.ForEach(d=>d.Room.DissolveRoom());
            
            Rooms.Clear();
        }
    }
}