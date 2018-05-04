using System;
using System.Linq;
using System.Threading.Tasks;
using ETModel;
using Model.ExtensionCode.DB;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class GetAccountInfoHandler: AMActorRpcHandler<SPlayer, GetAccountInfoRequest, GetAccountInfoResponse>
    {
        protected override async Task Run(SPlayer player, GetAccountInfoRequest message, Action<GetAccountInfoResponse> reply)
        {
            var response = new GetAccountInfoResponse();
            
            try
            {
                if (player == null)
                {
                    response.Error = -1;
                }
                else
                {
                    var account = player.GetComponent<Account>();

                    if (account == null)
                    {
                        response.Error = -1;
                    
                        response.Message = "没有找到该账号的数据！";
                    }
                    else
                    {
                        response.AccountInfo = new AccountInfo()
                        {
                            UserName = account.UserName,
                            Password = account.Password,
                            Diamond = account.Diamond,
                            Gold = account.Gold,
                            RoomId = account.RoomId,
                            RegistrationTime = account.RegistrationTime.ToString(),
                            LoginTime = account.LoginTime.ToString()
                        };
                    }
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