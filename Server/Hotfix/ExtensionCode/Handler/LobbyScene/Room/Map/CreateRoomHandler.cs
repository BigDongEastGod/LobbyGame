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
                // 创建房间  
                
                var room = CreateRoom(message.RoomType, player.Id);
                
                // 随机生成房间号

                var roomId = int.Parse(RandomHelper.RandomNumber(100000,999999).ToString().Substring(0, 6));

                while (true)
                {
                    if (RoomManageComponent.Instance.GetRoom(roomId) == null) break;
                    
                    roomId = int.Parse(RandomHelper.RandomNumber(100000,999999).ToString().Substring(0, 6));
                }

                // 添加房间到房间管理组件

                if (RoomManageComponent.Instance.Add(roomId, player.Id, room))
                {
                    response.RoomId = roomId;

                    Log.Info("用户：" + player.Id + "创建房间号：" + roomId);
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

        public Room CreateRoom(string roomType,long playerId)
        {
            Room room = null;
            
            switch (roomType)
            {
                case "NN":
                    room = ComponentFactory.CreateWithId<NNRoom>(playerId);
                    break;
                case "DDZ":
                    break;
                case "SXMZ":
                    break;
            }

            return room;
        }
    }
}