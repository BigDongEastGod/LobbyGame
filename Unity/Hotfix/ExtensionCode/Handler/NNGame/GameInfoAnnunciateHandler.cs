using System;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class GameInfoAnnunciateHandler : AMHandler<GameInfoAnnunciate>
    {
        public static event Action<GameInfoAnnunciate> GameAction;
        
        protected override void Run(Session session, GameInfoAnnunciate message)
        {
            GameAction?.Invoke(message);
        }
    }
}