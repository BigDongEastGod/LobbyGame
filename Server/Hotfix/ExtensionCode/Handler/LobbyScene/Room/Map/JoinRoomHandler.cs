using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class JoinRoomHandler: AMActorRpcHandler<SPlayer, JoinRoomRequest, JoinRoomResponse>
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

                    reply(response);
                    
                    return;
                }
                
                // 获得需要加入的房间

                var room = RoomManageComponent.Instance.GetRoom(message.RoomId);

                // 如果找到就加入到该房间、否则提示没有找到房间
                
                if (room == null)
                {
                    response.Error = -1;

                    response.Message = "没有找到房间";
                }
                else
                {
                    room.JionRoom(player);
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