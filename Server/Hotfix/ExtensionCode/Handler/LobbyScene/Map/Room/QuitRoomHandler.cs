using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class QuitRoomHandler : AMActorRpcHandler<SPlayer, QuitRoomRequest, QuitRoomResponse>
    {
        protected override async Task Run(SPlayer player, QuitRoomRequest message, Action<QuitRoomResponse> reply)
        {
            await Task.CompletedTask;
            
            var response = new QuitRoomResponse();
            
            try
            {
                if (message == null || message?.RoomId == 0)
                {
                    response.Error = -1;

                    reply(response);
                    
                    return;
                }

                RoomManageComponent.Instance.GetRoom(message.RoomId)?.QuitRoom(player);
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