using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class RegistPanelComponentAwakeSystem : AwakeSystem<RegistPanelComponent>
    {
        public override void Awake(RegistPanelComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class RegistPanelComponentStartSystem : StartSystem<RegistPanelComponent>
    {
        public override void Start(RegistPanelComponent self)
        {
            self.Start();
        }
    }

    public class RegistPanelComponent : Component
    {
        public SessionWrap Session;

        private UI _loginPanelUI;
//        private UI _dialogPanelUI;


        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            // 注册窗口
            var registPanel = rc.Get<GameObject>("RegistPanel");
            var registCloseBtn = rc.Get<GameObject>("RegistCloseBtn");
            var registNameInputField = rc.Get<GameObject>("RegistNameInputField");
            var registPwdInputField = rc.Get<GameObject>("RegistPwdInputField");
            var registRePwdInputField = rc.Get<GameObject>("RegistRePwdInputField");
            var registSubmitBtn = rc.Get<GameObject>("RegistSubmitBtn");

            // 关闭注册窗口按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(registCloseBtn.GetComponent<Button>(),
                () =>
                {
                    registPanel.SetActive(false);
                    registNameInputField.GetComponent<InputField>().text = "";
                    registPwdInputField.GetComponent<InputField>().text = "";
                    registRePwdInputField.GetComponent<InputField>().text = "";
                });

            // 注册帐号按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(registSubmitBtn.GetComponent<Button>(),
                () => OnregistSubmitBtn(registNameInputField, registPwdInputField, registRePwdInputField));
        }

        public void Start()
        {
//            _dialogPanelUI = Game.Scene.GetComponent<UIComponent>().Get(UIType.DialogPanel);
            _loginPanelUI = Game.Scene.GetComponent<UIComponent>().Get(UIType.LoginPanel);
        }

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="registNameText"></param>
        /// <param name="registPwdText"></param>
        /// <param name="registRePwdText"></param>
        private async void OnregistSubmitBtn(GameObject registNameText, GameObject registPwdText,
            GameObject registRePwdText)
        {
            if (String.IsNullOrWhiteSpace(registNameText.GetComponent<InputField>().text) ||
                String.IsNullOrWhiteSpace(registPwdText.GetComponent<InputField>().text) ||
                String.IsNullOrWhiteSpace(registRePwdText.GetComponent<InputField>().text))
            {
//                _dialogPanelUI.GameObject.SetActive(true);
//                _dialogPanelUI.GetComponent<DialogPanelComponent>().ShowDialogBox("帐号或密码不能为空!");

                GameTools.ShowDialogMessage("帐号或密码不能为空!", "LoginCanvas");
                return;
            }

            if (registPwdText.GetComponent<InputField>().text != registRePwdText.GetComponent<InputField>().text)
            {
                // 两次密码不一致
//                _dialogPanelUI.GameObject.SetActive(true);
//                _dialogPanelUI.GetComponent<DialogPanelComponent>().ShowDialogBox("两次密码输入不一致!");
                GameTools.ShowDialogMessage("两次密码输入不一致!", "LoginCanvas");
                return;
            }


            try
            {
                if (this.Session == null)
                {
                    this.Session = SceneHelperComponent.Instance.CreateRealmSession();
                }

                RegisteredResponse response =
                    (RegisteredResponse) await this.Session.Call(
                        new RegisteredRequest()
                        {
                            UserName = registNameText.GetComponent<InputField>().text,
                            Password = registPwdText.GetComponent<InputField>().text
                        });

                if (response.Error == 0)
                {
                    // 注册成功,直接登录
                    _loginPanelUI.GetComponent<LoginPanelComponent>().OnLoginSubmitBtn(registNameText.GetComponent<InputField>(),
                        registPwdText.GetComponent<InputField>());
                }
                else if (response.Error == -1)
                {
                    // 注册失败
//                    _dialogPanelUI.GetComponent<DialogPanelComponent>().ShowDialogBox(response.Message);
                    GameTools.ShowDialogMessage(response.Message, "LoginCanvas");
                }
            }
            catch (Exception e)
            {
//                _dialogPanelUI.GetComponent<DialogPanelComponent>().ShowDialogBox("网络连接错误!");
                GameTools.ShowDialogMessage(e.Message, "LoginCanvas");
            }
        }
    }
}