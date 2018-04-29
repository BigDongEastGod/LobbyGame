using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace ETModel
{
    [ObjectSystem]
    public class GatePlayerManageAwakeSystem : AwakeSystem<GatePlayerManageComponent>
    {
        public override void Awake(GatePlayerManageComponent self)
        {
            self.Awake();
        }
    }

    public class GatePlayerManageComponent : Component
    {
        public readonly Dictionary<long, Player> Players = new Dictionary<long, Player>();
        
        public readonly Dictionary<long, Session> Sessions = new Dictionary<long, Session>();

        public static GatePlayerManageComponent Instance;

        public void Awake()
        {
            Instance = this;
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            Players.Values.ForEach(d => d.Dispose());

            this.Players.Clear();
        }
    }
}