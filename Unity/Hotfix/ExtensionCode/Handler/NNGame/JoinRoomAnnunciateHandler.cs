using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class JoinRoomAnnunciateHandler : AMHandler<JoinRoomAnnunciate>
    {
        public static Action<JoinRoomAnnunciate> JoinRoomAction;
        
        protected override void Run(Session session, JoinRoomAnnunciate message)
        {
            JoinRoomAction.Invoke(message);
        }
    }
}