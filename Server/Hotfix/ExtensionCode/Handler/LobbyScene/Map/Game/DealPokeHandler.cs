using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class DealPokeHandler : AMActorRpcHandler<SPlayer, DealPokeRequest, DealPokeResponse>
    {
        protected override async Task Run(SPlayer player, DealPokeRequest message, Action<DealPokeResponse> reply)
        {
            await Task.CompletedTask;
            
            var response = new DealPokeResponse();
                        
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