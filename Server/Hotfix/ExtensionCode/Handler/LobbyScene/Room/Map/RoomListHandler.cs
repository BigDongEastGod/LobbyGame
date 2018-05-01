using System;
using System.Collections.Generic;
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
                var rooms = RoomManageComponent.Instance.GetRooms(player);

                var roomsResponse = new List<Room>();

                foreach (var room in rooms)
                {
                    switch (@room.RoomType)
                    {
                        case RoomType.NN:
                            
                            break;
                        
                        case RoomType.DDZ:
                            
                            break;
                    }
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