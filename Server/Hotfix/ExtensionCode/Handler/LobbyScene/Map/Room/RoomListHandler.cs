using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class RoomListHandler: AMActorRpcHandler<SPlayer, RoomListRequest, RoomListResponse>
    {
        protected override async Task Run(SPlayer player, RoomListRequest message, Action<RoomListResponse> reply)
        {
            await Task.CompletedTask;
            
            var response = new RoomListResponse();
            
            try
            {
                if (!player.RoomsRecords.ContainsKey(message.GameType))
                {
                    reply(response);
                    
                    return;
                }
                
                response.Rooms = player.RoomsRecords[message.GameType];
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