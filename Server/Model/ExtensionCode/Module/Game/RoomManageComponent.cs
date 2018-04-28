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
        public Dictionary<long, Room> Rooms;
        
        public static RoomManageComponent Instance;
        
        public void Awake()
        {
            Instance = this;

            Rooms = new Dictionary<long, Room>();
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
            
            Rooms.Values.ForEach(d=>d.DissolveRoom());
            
            Rooms.Clear();
        }
    }
}