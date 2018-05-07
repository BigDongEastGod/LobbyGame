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
                
                // 添加到玩家房间记录

                if (player.RoomsRecord.Contains(response.RoomId)) player.RoomsRecord.Add(response.RoomId);

                Log.Debug("用户：" + player.Id + "创建房间号：" + response.RoomId);

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