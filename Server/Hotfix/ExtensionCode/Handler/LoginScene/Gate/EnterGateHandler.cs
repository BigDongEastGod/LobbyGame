using System;
using System.Linq;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class EnterGateHandler: AMRpcHandler<EntherGateRequest, EntherGateResponse>
    {
        protected override async void Run(Session session, EntherGateRequest message, Action<EntherGateResponse> reply)
        {
            if (message == null || message?.Key == 0)
            {
                reply(null);

                return;
            }
            
            var response = new EntherGateResponse();
            
            try
            {
                var id = Game.Scene.GetComponent<SessionTKeyComponent>()?.Get(message.Key);
                
                if (id != null)
                {
                    // 挂载Actor组件、进行消息转发
                    
                    var config = Game.Scene.GetComponent<StartConfigComponent>();

                    var mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(config.MapConfigs.First().GetComponent<InnerConfig>().IPEndPoint);

                    var regedResponse = (RegisterGameResponse) await mapSession.Call(new RegisterGameRequest() {AccountId = id ?? 0,GateSessionId = session.Id});
                    
                    if (regedResponse.Error == 0)
                    {
                        // 添加用户到用户管理组件

                        GatePlayerManageComponent.Instance.Add(session, id ?? 0);
                        
                        // 向Location服务器注册位置

                        await session.AddComponent<ActorComponent, string>(ActorType.GateSession).AddLocation();
                        
                        // 添加到心跳组件

                        if (Game.Scene.GetComponent<PingComponent>() == null) Game.Scene.AddComponent<PingComponent, int, long>(2000, 3);

                        Game.Scene.GetComponent<PingComponent>().AddSession(id ?? 0);
                    }
                    else
                    {
                        response.Error = -1;

                        response.Message = "无法连接到网关服务器";
                    }
                }
                else
                {
                    response.Error = -1;
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