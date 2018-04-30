using System;
using ETModel;

namespace ETHotfix
{
    public class SessionGatePlayerComponent : Component
    {
        public override async void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
        }
    }
}