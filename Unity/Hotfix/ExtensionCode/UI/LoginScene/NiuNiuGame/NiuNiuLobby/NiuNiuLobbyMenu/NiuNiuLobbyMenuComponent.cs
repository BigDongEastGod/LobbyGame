using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class NiuNiuLobbyMenuComponentAwakeSystem : AwakeSystem<NiuNiuLobbyMenuComponent>
    {
        public override void Awake(NiuNiuLobbyMenuComponent self)
        {
            self.Awake();
        }
    }

    public class NiuNiuLobbyMenuComponent : Component
    {
        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            var nnLobbyMenu = rc.Get<GameObject>("NiuNiuLobbyMenu");
            var backLobbyBtn = rc.Get<GameObject>("BackLobbyBtn");
            var settingBtn = rc.Get<GameObject>("SettingBtn");
            var feedbackBtn = rc.Get<GameObject>("FeedbackBtn");
            var ruleBtn = rc.Get<GameObject>("RuleBtn");

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nnLobbyMenu.GetComponent<Button>(), () => { nnLobbyMenu.SetActive(false); });
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(backLobbyBtn.GetComponent<Button>(), () =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.Lobby,UiLayer.Bottom);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuLobby);
            });
        }
    }
}