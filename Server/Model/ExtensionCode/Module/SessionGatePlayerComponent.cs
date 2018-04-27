namespace ETModel
{
    public class SessionGatePlayerComponent : Component
    {
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            var session = this.GetParent<Session>();

            // 删除Actor

            if (session.GetComponent<ActorComponent>() != null)
            {
                //                await Game.Scene.GetComponent<ActorProxyComponent>().Get(Player.UnitId)
                //                    .Call(new MapQuitRequest() { ActorId = Player.UnitId });
                //
                //                await Game.Scene.GetComponent<LocationProxyComponent>().Remove(session.Id);
            }

            Game.Scene.GetComponent<ActorManagerComponent>()?.Remove(session.Id);

            Game.Scene.GetComponent<NetOuterComponent>()?.Remove(session.Id);

            Game.Scene.GetComponent<NetInnerComponent>()?.Remove(session.Id);
            
            Game.Scene.GetComponent<GatePlayerManageComponent>()?.RemoveSession(session);
        }
    }
}