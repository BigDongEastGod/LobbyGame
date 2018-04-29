using System;
using System.Data.Common;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Map)]
    public class ActorQuitHandler: AMHandler<ActorQuitRequest>
    {
        protected override void Run(Session session, ActorQuitRequest message)
        {
            if (message ==null || message?.AccountId == 0) return;
            
            var player = PlayerManageComponent.Instance.GetPlayer(message.AccountId);

            Task.Run(() => PlayerManageComponent.Instance.Remove(player));
        }
    }
}