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
                var accountId = Game.Scene.GetComponent<SessionTKeyComponent>().Get(message.Key);

                if (accountId != 0)
                {
                    // 如果该账号已经登录过

                    var gateSession = GatePlayerManageComponent.Instance.GetSession(accountId);

                    if (gateSession != null && !gateSession.IsDisposed) GatePlayerManageComponent.Instance.Remove(gateSession);
                    
                    // 挂载Actor组件、进行消息转发

                    var config = Game.Scene.GetComponent<StartConfigComponent>();

                    var mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(config.MapConfigs.First().GetComponent<InnerConfig>().IPEndPoint);

                    var regedResponse = (RegisterGameResponse) await mapSession.Call(new RegisterGameRequest() {AccountId = accountId, GateSessionId = session.Id});

                    if (regedResponse.Error == 0)
                    {
                        // 挂载Session销毁触发组件、并设置UnitId方便销毁时删除Actor

                        session.AddComponent<SessionGatePlayerComponent>();

                        // 添加用户到用户管理组件

                        GatePlayerManageComponent.Instance.Add(session, accountId);

                        // 向Location服务器注册位置

                        await session.AddComponent<ActorComponent, string>(ActorType.GateSession).AddLocation();

                        // 添加到心跳组件

                        if (Game.Scene.GetComponent<PingComponent>() == null) Game.Scene.AddComponent<PingComponent, int, long>(5000, 7);

                        Game.Scene.GetComponent<PingComponent>().AddSession(accountId);
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