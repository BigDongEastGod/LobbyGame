using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class CreateRoomHandler: AMActorRpcHandler<SPlayer, CreateRoomRequest, CreateRoomResponse>
    {
        protected override async Task Run(SPlayer player, CreateRoomRequest message, Action<CreateRoomResponse> reply)
        {
            await Task.CompletedTask;
            
            var response = new CreateRoomResponse();
            
            try
            {
                // 添加房间到房间管理组件
                
                Log.Debug("房间类型：" + message.RoomType);
                
                // 玩家创建新房间
                
                var room = RoomManageComponent.Instance.Add(player.Id, message.RoomType);
                
                response.RoomId = room?.Id ?? 0;

                if (room != null)
                {
                    Log.Debug("用户：" + player.Id + "创建房间号：" + response.RoomId);
                }
                else
                {
                    response.Error = -2;

                    response.Message = "您已经创建了一个房间，不能再创建房间了";
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