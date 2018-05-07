using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class RoomRulesHandler: AMActorRpcHandler<SPlayer, RoomRulesRequest, RoomRulesResponse>
    {
        protected override async Task Run(SPlayer unit, RoomRulesRequest message, Action<RoomRulesResponse> reply)
        {
            await Task.CompletedTask;
            
            var response = new RoomRulesResponse();
            
            try
            {
                if (message == null || message?.RoomId == 0)
                {
                    response.Error = -1;

                    reply(response);
                    
                    return;
                }
                
                var room = RoomManageComponent.Instance.GetRoom(message.RoomId);

                room?.AddRules(message.Rules);
            }
            catch (Exception e)
            {
                response.Error = -1;

                ReplyError(response, e, reply);
            }
            
            reply(response);
        }
    }
}