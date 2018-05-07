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

                Log.Debug("玩家ID：" + player.Id);
                
                Log.Debug("开始测试");
                
                response.Rooms.ForEach(d=>Log.Debug(d.RoomId.ToString()));
                
                Log.Debug("结束测试");
                
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