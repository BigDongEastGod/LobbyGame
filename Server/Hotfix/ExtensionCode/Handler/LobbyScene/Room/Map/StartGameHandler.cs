using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class StartGameHandler : AMActorRpcHandler<SPlayer, StartGameRequest, StartGameResponse>
    {
        protected override async Task Run(SPlayer player, StartGameRequest message, Action<StartGameResponse> reply)
        {
            await Task.CompletedTask;
            
            var response = new StartGameResponse();
                        
            try
            {
                if (message == null || message?.RoomId == 0)
                {
                    response.Error = -1;
                }
                else 
                {
                    var room = RoomManageComponent.Instance.GetRoom(message.RoomId);

                    response.Message = room?.StartGame(player);
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