using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class BetGameHandler : AMActorRpcHandler<SPlayer, BetGameRequest, BetGameResponse>
    {
        protected override async Task Run(SPlayer player, BetGameRequest message, Action<BetGameResponse> reply)
        {
            await Task.CompletedTask;
            
            var response = new BetGameResponse();
                        
            try
            {
                if (message == null || message?.RoomId == 0)
                {
                    response.Error = -1;
                }
                else 
                {
                    var room = RoomManageComponent.Instance.GetRoom(message.RoomId);
                    
                    room?.SendMessages(player, 0, message.Bet);
                }
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