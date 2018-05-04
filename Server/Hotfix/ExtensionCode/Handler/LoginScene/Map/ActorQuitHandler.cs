using System;
using System.Data.Common;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Map)]
    public class ActorQuitHandler: AMHandler<ActorQuitRequest>
    {
        protected override async void Run(Session session, ActorQuitRequest message)
        {
            if (message ==null || message?.AccountId == 0) return;

            await PlayerManageComponent.Instance.Remove(PlayerManageComponent.Instance.GetPlayer(message.AccountId));
        }
    }
}