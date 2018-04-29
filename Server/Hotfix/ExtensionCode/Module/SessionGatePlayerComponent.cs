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

            try
            {
                var session = this.GetParent<Session>();

                // 删除Actor

                if (session.GetComponent<ActorComponent>() != null)
                {
                    // 获取GatePlayer组件，并发送给注册的Actor服务器下线消息

                    var player = GatePlayerManageComponent.Instance.GatePlayer(session);

                    if (player != null)
                    {
                        player.ActorSession.Send(new ActorQuitRequest() {AccountId = player.Id, ActorId = player.UnitId});
                        
                        // 删除在Location的注册
                        
                        await session.GetComponent<ActorComponent>().RemoveLocation();
                
                        // 删除Actor
                
                        Game.Scene.GetComponent<ActorProxyComponent>().Remove(player.UnitId);
                    }
                }
                
                Game.Scene.GetComponent<ActorManagerComponent>().Remove(session.Id);
                
                Game.Scene.GetComponent<NetOuterComponent>().Remove(session.Id);
                
               GatePlayerManageComponent.Instance.Remove(session); 
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}