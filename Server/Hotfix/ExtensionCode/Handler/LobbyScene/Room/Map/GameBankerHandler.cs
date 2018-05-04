using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class GameBankerHandler : AMActorRpcHandler<SPlayer, GameBankerRequest, GameBankerResponse>
    {
        protected override async Task Run(SPlayer player, GameBankerRequest message, Action<GameBankerResponse> reply)
        {
            await Task.CompletedTask;
            
            var response = new GameBankerResponse();
                        
            try
            {
                if (message == null || message?.RoomId == 0)
                {
                    response.Error = -1;
                }
                else 
                {
                    var room = RoomManageComponent.Instance.GetRoom(message.RoomId);
                    
                    room?.SendMessages(player, 0);
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