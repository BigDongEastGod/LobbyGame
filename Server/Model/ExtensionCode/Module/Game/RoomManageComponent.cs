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
        public List<Room> Rooms;
        
        public static RoomManageComponent Instance;
        
        public void Awake()
        {
            Instance = this;

            Rooms = new List<Room>();
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            Rooms.ForEach(d => d.DissolveRoom(null));
            
            Rooms.Clear();
        }
    }
}