namespace ETModel
{
    /// <summary>
    /// Gate用户操作组件、后期添加操作数据库方法
    /// </summary>
    public class GatePlayer : Entity
    {
        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
        }
    }
}