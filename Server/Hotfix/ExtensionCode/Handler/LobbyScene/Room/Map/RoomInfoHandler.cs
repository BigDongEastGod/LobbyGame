using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class RoomInfoHandler : AMActorRpcHandler<SPlayer, RoomInfoRequest, RoomInfoResponse>
    {
        protected override async Task Run(SPlayer player, RoomInfoRequest message, Action<RoomInfoResponse> reply)
        {
            await Task.CompletedTask;
            
            var response = new RoomInfoResponse();
            
            try
            {
                if (message == null || message?.RoomId == 0)
                {
                    response.Error = -1;

                    reply(response);
                    
                    return;
                }
                
                var room = RoomManageComponent.Instance.GetRoom(message.RoomId);

                if (room != null)
                {
                    response.RoomId = room.Id;

                    response.Rules = room.Rules;

                    response.Players = await RoomManageComponent.Instance.GetRoomPlayers(room, player);
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