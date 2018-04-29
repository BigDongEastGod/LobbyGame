namespace ETModel
{
    /// <summary>
    /// Gate用户操作组件、后期添加操作数据库方法
    /// </summary>
    public class GatePlayer : Entity
    {
        public long ActorId;

        public long UnitId;
        
        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
        }

        /// <summary>
        /// 获取注册的Actor服务器的Session
        /// </summary>
        public Session ActorSession => Game.Scene.GetComponent<NetInnerComponent>().
            Get(Game.Scene.GetComponent<ActorProxyComponent>().Get(this.ActorId).Address);
    }
}