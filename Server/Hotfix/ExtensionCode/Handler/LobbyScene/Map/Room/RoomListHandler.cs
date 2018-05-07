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
                switch (message.GameType)
                {
                    case "NN":  // 牛牛

                        #region 牛牛游戏
                            
                        var roomInfos = new List<GameRoomInfo>();

                        for (var i = 0; i < player.RoomsRecord.Count; i++)
                        {
                            // 根据房间号获取房间
                                
                            var roominfo = RoomManageComponent.Instance.GetRoom(player.RoomsRecord.ElementAt(i));

                            // 如果房间为空或没有规则就删除浏览记录
                                
                            if (roominfo?.Rules == null)
                            {
                                player.RoomsRecord.RemoveAt(i);

                                continue;
                            }

                            // 判断房间类型
                            
                            if (Enum.GetName(typeof(RoomType), roominfo.RoomType) != "NN") continue;

                            // 获取规则并添加到发送房间列表里

                            var rule = ProtobufHelper.FromBytes<NNChess>(roominfo.Rules);

                            roomInfos.Add(new GameRoomInfo()
                            {
                                RoomId = roominfo.Id,
                                PlayerMode = rule.PlayerMode,
                                Score = rule.Score,
                                Dish = rule.Dish,
                                PayMode = rule.PayMode,
                                PlayerCount = roominfo.Players.Count + "/" + rule.PlayerCount
                            });
                        }

                        response.Rooms = roomInfos;
                            
                        #endregion

                        break;
                        
                    case "DDZ":
                            
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