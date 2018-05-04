using System;
using ETModel;

namespace ETHotfix
{
    public class SessionGatePlayerComponent : Entity
    {
        public override async void Dispose()
        {
            if (IsDisposed) return;

            var session = this.GetParent<Session>();

            if (session == null) return;

            Log.Debug("有用户退出");

            try
            {
                // 发送给注册的Actor服务器下线消息

                var player = GetComponent<Player>();
                
                if (player != null)
                {
                    player.ActorSession?.Send(new ActorQuitRequest() {AccountId = player.Id, ActorId = player.UnitId});

                    // 删除Actor

                    Game.Scene.GetComponent<ActorProxyComponent>().Remove(player.UnitId);
                }
                
                Game.Scene.GetComponent<ActorManagerComponent>().Remove(session.Id);
                
                Game.Scene.GetComponent<NetOuterComponent>().Remove(session.Id);
                
                GatePlayerManageComponent.Instance.Remove(session);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            base.Dispose();
        }
    }
}