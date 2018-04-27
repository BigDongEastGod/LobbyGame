using System;
using System.Linq;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class JionPlayerByGateHandler: AMRpcHandler<JionPlayerByGateRequest, JionPlayerByGateResponse>
    {
        protected override void Run(Session session, JionPlayerByGateRequest message, Action<JionPlayerByGateResponse> reply)
        {
            if (message == null || message?.AccountId == 0)
            {
                reply(null);

                return;
            }

            var response = new JionPlayerByGateResponse();

            try
            {
                // 检查当前Gate里是否有该用户、如果有强制踢下线
                
                var player = GatePlayerManageComponent.Instance.GetSession(message.AccountId);

                if (player != null) GatePlayerManageComponent.Instance.RemoveSession(player);
                
                // 创建临时KEY

                if (Game.Scene.GetComponent<SessionTKeyComponent>() == null)
                {
                    Game.Scene.AddComponent<SessionTKeyComponent>();
                }
                
                response.Key  = Game.Scene.GetComponent<SessionTKeyComponent>().Add(message.AccountId);
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