using System;
using ETModel;
using Model.ExtensionCode.DB;

namespace ETHotfix
{
    [MessageHandler(AppType.Map)]
    public class RegisterGameHandler: AMRpcHandler<RegisterGameRequest, RegisterGameResponse>
    {
        protected override async void Run(Session session, RegisterGameRequest message, Action<RegisterGameResponse> reply)
        {
            var response = new RegisterGameResponse();
            
            if (message?.AccountId == 0)
            {
                response.Error = -1;
                
                reply(response);
                
                return;
            }
            
            try
            {
                if (message != null)
                {
                    // 添加到Player管理组件里

                    var player = PlayerManageComponent.Instance.Add(message.AccountId);

                    player.GateSessionId = message.GateSessionId;

                    // 数据库读取账号数据，并挂载到Player组件下

                    player.AddComponent(await Game.Scene.GetComponent<DBProxyComponent>().Query<Account>(player.Id));

                    // 挂载Actor

                    var actor = player.GetComponent<ActorComponent>();

                    if (actor == null)
                    {
                        await player.AddComponent<ActorComponent>().AddLocation();
                    }
                    else
                    {
                        await actor.AddLocation();
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