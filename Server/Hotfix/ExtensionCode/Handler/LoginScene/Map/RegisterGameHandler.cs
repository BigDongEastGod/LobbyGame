using System;
using ETModel;

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
                    var player = PlayerManageComponent.Instance.Add(message.AccountId);

                    player.GateSessionId = message.GateSessionId;

                    await player.AddComponent<ActorComponent>().AddLocation();
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