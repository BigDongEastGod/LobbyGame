using System;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class PingHandler: AMRpcHandler<PingRequest, PingResponse>
    {
        protected override void Run(Session session, PingRequest message, Action<PingResponse> reply)
        {
            var response = new PingResponse();

            try
            {
                var player = session.GetComponent<SessionGatePlayerComponent>()?.GetComponent<Player>();
                
                if (player != null) Game.Scene.GetComponent<PingComponent>().UpdateSession(player.Id);
                
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}