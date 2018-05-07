using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class CalculateCardHandler : AMActorRpcHandler<SPlayer, CalculateCardRequest, CalculateCardResponse>
    {
        protected override async Task Run(SPlayer player, CalculateCardRequest message, Action<CalculateCardResponse> reply)
        {
            await Task.CompletedTask;
            
            var response = new CalculateCardResponse();
                        
            try
            {
                if (message == null || message?.RoomId == 0)
                {
                    response.Error = -1;
                }
                else 
                {
                    var room = RoomManageComponent.Instance.GetRoom(message.RoomId);

                    room?.SendMessages(player, 2);
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