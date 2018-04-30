namespace ETModel
{
    /// <summary>
    /// 用户操作组件、后期添加操作数据库方法
    /// </summary>
    public class SPlayer : Entity
    {
        public long GateSessionId;

        public bool IsActivity;
        
        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
        }

        public ActorProxy GetActorProxy => Game.Scene.GetComponent<ActorProxyComponent>().Get(this.GateSessionId);
    }
}