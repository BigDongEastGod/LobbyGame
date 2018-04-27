using System;

namespace Model.ExtensionCode.DB
{
    public class Account: ETModel.Entity
    {
        public string UserName;

        public string Password;
        
        public long Diamond;
        
        public long Gold;

        public DateTime RegistrationTime;

        public DateTime LoginTime;
    }
}