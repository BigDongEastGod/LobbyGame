using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class LoginPanelComponentAwakeSystem : AwakeSystem<LoginPanelComponent>
    {
        public override void Awake(LoginPanelComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class LoginPanelComponentStartSystem : StartSystem<LoginPanelComponent>
    {
        public override void Start(LoginPanelComponent self)
        {
            self.Start();
        }
    }


    public class LoginPanelComponent : Component
    {
        private UI _registPanelUI;
        private GameObject loginSubmitBtn;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            // 登录窗口
            var loginPanel = rc.Get<GameObject>("LoginPanel");
            var loginCloseBtn = rc.Get<GameObject>("LoginCloseBtn");
            var loginNameInputField = rc.Get<GameObject>("LoginNameInputField");
            var loginPwdInputField = rc.Get<GameObject>("LoginPwdInputField");
            loginSubmitBtn = rc.Get<GameObject>("LoginSubmitBtn");

            // 关闭登录窗口按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(loginCloseBtn.GetComponent<Button>(),
                () =>
                {
                    loginPanel.SetActive(false);
                    loginNameInputField.GetComponent<InputField>().text = "";
                    loginPwdInputField.GetComponent<InputField>().text = "";
                });

            // 登录帐号按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(loginSubmitBtn.GetComponent<Button>(),
                () => OnLoginSubmitBtn(loginNameInputField.GetComponent<InputField>(), loginPwdInputField.GetComponent<InputField>()));
            
        }

        public void Start()
        {
            _registPanelUI = Game.Scene.GetComponent<UIComponent>().Get(UIType.RegistPanel);
        }

        /// <summary>
        /// 登录方法
        /// </summary>
        /// <param name="loginNameText"></param>
        /// <param name="loginPwdText"></param>
        public async void OnLoginSubmitBtn(InputField loginNameText, InputField loginPwdText)
        {
            if (String.IsNullOrWhiteSpace(loginPwdText.text) || String.IsNullOrWhiteSpace(loginNameText.text))
            {
                GameTools.ShowDialogMessage("帐号或密码不能为空!", "LoginCanvas");
                return;
            }

            try
            {
                var loadingUI = Game.Scene.GetComponent<UIComponent>().Get(UIType.LoadingPanel);

                Transform parent = GameObject.Find("Global/UI/" + "LoginCanvas" + "/TopMost").transform;

                loadingUI.GameObject.transform.SetParent(parent);

                loadingUI.GameObject.SetActive(true);

                loadingUI.GetComponent<LoadingComponent>().SetText("正在登录,请稍候...");
                
                
//                SceneHelperComponent.Instance.Session.Dispose();
//                SceneHelperComponent.Instance.Session = null;
                
                
                var session = _registPanelUI.GetComponent<RegistPanelComponent>().Session ?? SceneHelperComponent.Instance.CreateRealmSession();

                SceneHelperComponent.Instance.MonoEvent.RemoveButtonClick(loginSubmitBtn.GetComponent<Button>());
                LoginResponse response = (LoginResponse) await session.Call(
                    new LoginRequest()
                    {
                        UserName = loginNameText.text,
                        Password = loginPwdText.text
                    });
                
                if (response.Error == 0)
                {
                    PlayerPrefs.SetString("username", loginNameText.text);
                    PlayerPrefs.SetString("password", loginPwdText.text);

                    session.Dispose();

                    // 连接网关服务器
                    await SceneHelperComponent.Instance.CreateGateSession(response.Address, response.Key);
                    
                    // 获取用户信息
                    var accountResponse = (GetAccountInfoResponse) await SceneHelperComponent.Instance.Session.Call(new GetAccountInfoRequest());
                    if (accountResponse.AccountInfo.RoomId == 0)
                    {
                        Game.Scene.GetComponent<UIComponent>().Create(UIType.Lobby, UiLayer.Bottom);
                        Game.Scene.GetComponent<UIComponent>().Remove(UIType.Login);
                    }
                    else
                    {
                        Debug.Log("重新连接,房间号: " + accountResponse.AccountInfo.RoomId);
                        loadingUI.GetComponent<LoadingComponent>().SetText("正在恢复加入的牌局,请稍候...");
                        JoinPaiJu(accountResponse.AccountInfo.RoomId);
                    }
                }
                else if (response.Error == -1)
                {
                    // 登录失败
                    GameTools.ShowDialogMessage(response.Message, "LoginCanvas");
                }

                SceneHelperComponent.Instance.MonoEvent.AddButtonClick(loginSubmitBtn.GetComponent<Button>(),
                    () => OnLoginSubmitBtn(loginNameText, loginPwdText));
                
                loadingUI.GameObject.SetActive(false);
                
            }
            catch (Exception e)
            {
                GameTools.ShowDialogMessage("网络连接错误!" + e.Message, "LoginCanvas");
            }
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        private async void JoinPaiJu(long roomId)
        {
            // TODO 加入牌局
            var joinRoomResponse = (JoinRoomResponse) await SceneHelperComponent.Instance.Session.Call(
                new JoinRoomRequest() {RoomId = roomId});

            if (joinRoomResponse.Error == 0)
            {
                Debug.Log("加入房间成功,跳转至游戏主场景");

                Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuMain, UiLayer.Bottom, roomId, true);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.Login);
            }
            else
            {
                Debug.Log("加入房间失败: " + joinRoomResponse.Message);
                GameTools.ShowDialogMessage("加入房间失败!", "LoginCanvas");
            }
        }
    }
}