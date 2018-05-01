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
        public readonly List<Session> Sessions = new List<Session>();

        public static GatePlayerManageComponent Instance;

        public void Awake()
        {
            Instance = this;
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            Sessions.ForEach(d => d.Dispose());

            this.Sessions.Clear();
        }
    }
}