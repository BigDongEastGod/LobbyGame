using System;

namespace Model.ExtensionCode.DB
{
    public class Account: ETModel.Entity
    {
        public string UserName;

        public string Password;
        
        public long Diamond;
        
        public long Gold;
        
        public long RoomId;  // 所在房间ID

        public DateTime RegistrationTime;

        public DateTime LoginTime;
    }
}