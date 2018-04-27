using System;
using System.Linq;
using System.Net;
using ETModel;
using Model.ExtensionCode.DB;

namespace ETHotfix
{
    [MessageHandler(AppType.Realm)]
    public class LoginHandler: AMRpcHandler<LoginRequest, LoginResponse>
    {
        protected override async void Run(Session session, LoginRequest message, Action<LoginResponse> reply)
        {
            // 检测是否是空包
            
            if (string.IsNullOrEmpty(message?.UserName) || string.IsNullOrEmpty(message.Password))
            {
                reply(null);

                return;
            }
            
            var response = new LoginResponse();
            
            try
            {
                // 数据库查询账号和密码
                
                var account =
                    (await Game.Scene.GetComponent<DBProxyComponent>()
                        .QueryJson<Account>(
                            $"{{\'UserName\':\'{message.UserName}\',\'Password\':\'{message.Password}\'}}"))
                    .FirstOrDefault();
                
                if (account == null)
                {
                    response.Error = -1;

                    response.Message = "账号不存在、或账号和密码错误！";
                }
                else
                {
                    #region 登录Gate服务器（如果有多个随机分配）

                    // 随机分配网关服务器
                    
                    var gateConfig = Game.Scene.GetComponent<RealmGateAddressComponent>().GetAddress();
                    
                    var gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gateConfig.GetComponent<InnerConfig>().IPEndPoint);

                    var gateOutConfig = gateConfig.GetComponent<OuterConfig>().IPEndPoint2;
                    
                    var gateresponse =
                        (JionPlayerByGateResponse) await gateSession.Call(
                            new JionPlayerByGateRequest() {AccountId = account.Id});

                    if (gateresponse.Error != 0)
                    {
                        response.Error = -1;

                        response.Message = "无法连接到服务器";
                    }
                    else
                    {
                        response.Address = gateOutConfig.Address.ToString()+ ":" + gateOutConfig.Port;

                        response.Key = gateresponse.Key;
                        
                        response.Error = 0;
                    }

                    #endregion
                }
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
            
            reply(response);
        }
    }
}