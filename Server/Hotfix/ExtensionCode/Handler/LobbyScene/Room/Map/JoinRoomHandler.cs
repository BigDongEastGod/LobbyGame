using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class JoinRoomHandler : AMActorRpcHandler<SPlayer, JoinRoomRequest, JoinRoomResponse>
    {
        protected override async Task Run(SPlayer player, JoinRoomRequest message, Action<JoinRoomResponse> reply)
        {
            await Task.CompletedTask;
            
            var response = new JoinRoomResponse();
                        
            try
            {
                if (message == null || message?.RoomId == 0)
                {
                    response.Error = -1;
                }
                else 
                {
                    var room = RoomManageComponent.Instance.GetRoom(message.RoomId);

                    if (room != null) response.Message = room.JionRoom(player);
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