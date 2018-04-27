using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class RoomInfoAnnunciateHandler : AMHandler<RoomInfoAnnunciate>
    {
        public static Action<RoomInfoAnnunciate> RoomAction;
        
        protected override void Run(Session session, RoomInfoAnnunciate message)
        {
            RoomAction.Invoke(message);
        }
    }
}