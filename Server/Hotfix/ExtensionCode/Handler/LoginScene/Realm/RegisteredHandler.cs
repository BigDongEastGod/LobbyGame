using System;
using System.Linq;
using ETModel;
using Model.ExtensionCode.DB;

namespace ETHotfix
{
    [MessageHandler(AppType.Realm)]
    public class RegisteredHandler: AMRpcHandler<RegisteredRequest, RegisteredResponse>
    {
        protected override async void Run(Session session, RegisteredRequest message, Action<RegisteredResponse> reply)
        {
            if (string.IsNullOrEmpty(message?.UserName) || string.IsNullOrEmpty(message.Password))
            {
                reply(null);

                return;
            }
            
            var response = new RegisteredResponse();
            
            try
            {
                var account =
                    (await Game.Scene.GetComponent<DBProxyComponent>()
                        .QueryJson<Account>($"{{\'UserName\':\'{message.UserName}\'}}")).FirstOrDefault();

                if (account == null)
                {
                    var accountmodel = ComponentFactory.Create<Account>();

                    accountmodel.UserName = message.UserName;

                    accountmodel.Password = message.Password;

                    accountmodel.RegistrationTime = DateTime.Now;

                    accountmodel.LoginTime = DateTime.Now;

                    accountmodel.Diamond = 0;

                    accountmodel.Gold = 0;

                    await Game.Scene.GetComponent<DBProxyComponent>().Save(accountmodel);

                    response.Error = 0;
                }
                else
                {
                    response.Error = -1;

                    response.Message = "该账号已经注册过了，请更换账号！";
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