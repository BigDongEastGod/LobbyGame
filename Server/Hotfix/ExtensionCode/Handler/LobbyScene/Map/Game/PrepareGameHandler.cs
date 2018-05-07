using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class PrepareGameHandler : AMActorRpcHandler<SPlayer, PrepareGameRequest, PrepareGameResponse>
    {
        protected override async Task Run(SPlayer player, PrepareGameRequest message, Action<PrepareGameResponse> reply)
        {
            await Task.CompletedTask;
            
            var response = new PrepareGameResponse();
            
            try
            {
                if (message == null || message?.RoomId == 0)
                {
                    response.Error = -1;
                }
                else 
                {
                    var room = RoomManageComponent.Instance.GetRoom(message.RoomId);

                    if (room != null) response.Message = room.Prepare(player);
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