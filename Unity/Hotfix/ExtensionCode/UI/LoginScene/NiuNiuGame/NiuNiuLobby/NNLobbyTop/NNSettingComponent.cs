using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class NNSettingComponentAwakeSystem : AwakeSystem<NNSettingComponent>
    {
        public override void Awake(NNSettingComponent self)
        {
            self.Awake();
        }
    }

    public class NNSettingComponent : Component
    {
        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();


            var settingCloseBtn = rc.Get<GameObject>("Setting_CloseBtn");

            var settingPanel = rc.Get<GameObject>("SettingPanel");

            var confirmBtn = rc.Get<GameObject>("ConfirmBtn");
            var changeAccountBtn = rc.Get<GameObject>("ChangeAccountBtn");

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(settingCloseBtn.GetComponent<Button>(), () => settingPanel.SetActive(false));
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(settingPanel.GetComponent<Button>(), () => settingPanel.SetActive(false));
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(confirmBtn.GetComponent<Button>(), () => settingPanel.SetActive(false));
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(changeAccountBtn.GetComponent<Button>(), () =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.Login, UiLayer.Bottom);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuLobby);
            });
        }
        
    }
}