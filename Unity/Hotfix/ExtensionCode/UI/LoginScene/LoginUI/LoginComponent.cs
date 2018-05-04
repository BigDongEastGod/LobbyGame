using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class LoginComponentAwakeSystem : AwakeSystem<LoginComponent>
    {
        public override void Awake(LoginComponent self)
        {
            self.Awake();
        }
    }

    public class LoginComponent : Component
    {
        private GameObject obj;
        
        public void Awake()
        {
            
            
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            // 登录注册按钮
            var registBtn = rc.Get<GameObject>("RegistBtn");
            var loginBtn = rc.Get<GameObject>("LoginBtn");

            // 注册按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(registBtn.GetComponent<Button>(),
                () => { Game.Scene.GetComponent<UIComponent>().Get(UIType.RegistPanel).GameObject.SetActive(true); });

            // 登录按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(loginBtn.GetComponent<Button>(),
                () => { Game.Scene.GetComponent<UIComponent>().Get(UIType.LoginPanel).GameObject.SetActive(true); });
        }
    }
}