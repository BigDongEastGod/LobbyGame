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

                if (!player.RoomsRecord.TryGetValue(message.GameType, out var roomInfos))
                {
                    reply(response);
                    
                    return;
                }

                response.Rooms = roomInfos;
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