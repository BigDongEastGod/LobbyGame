using System;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class RoomInfoHandler: AMActorRpcHandler<SPlayer, RoomInfoRequest, RoomInfoResponse>
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

                    if (player != null)
                    {
                        var playerroom = RoomManageComponent.Instance.GetRommByPlayer(player);

                        response.RoomId = RoomManageComponent.Instance.GetRoomId(playerroom);

                        response.Rules = playerroom.Rules;
                        
                        Log.Debug(response.Rules.Length.ToString());
                    }

                    reply(response);
                    
                    return;
                }
                
                // 获得房间
                
                var room = RoomManageComponent.Instance.GetRoom(message.RoomId);
                
                // 如果找到就加入到该房间、否则提示没有找到房间
                
                if (room == null)
                {
                    response.Error = -1;

                    response.Message = "没有找到房间";
                    
                    reply(response);
                    
                    return;
                }
                
                // 根据消息类型

                switch (@message.Message)
                {
                    case 0: // 加入房间
                        room.JionRoom(player);
                        break;

                    case 1: // 准备
                        room.Prepare(player);
                        break;
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