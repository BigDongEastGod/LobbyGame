using System.Collections.Generic;
using Model.ExtensionCode.DB;

namespace ETModel
{
    /// <summary>
    /// 用户操作组件、后期添加操作数据库方法
    /// </summary>
    public class SPlayer : Entity
    {
        public long GateSessionId;

        public bool IsActivity;

        public List<long> RoomsRecord = new List<long>(); // 房间记录、存放创建过的房间和加入的房间
        
        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
        }

        public ActorProxy GetActorProxy => Game.Scene.GetComponent<ActorProxyComponent>().Get(this.GateSessionId);

        public Account Account => GetComponent<Account>();
    }
}