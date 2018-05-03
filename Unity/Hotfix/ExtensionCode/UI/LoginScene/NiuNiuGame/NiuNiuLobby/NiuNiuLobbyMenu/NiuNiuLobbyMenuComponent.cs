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

    [ObjectSystem]
    public class NiuNiuLobbyMenuComponmentStartSystem : StartSystem<NiuNiuLobbyMenuComponent>
    {
        public override void Start(NiuNiuLobbyMenuComponent self)
        {
            self.Start();
        }
    }

    public class NiuNiuLobbyMenuComponent : Component
    {
        
        private UI _settingPanel;
        private GameObject _nnLobbyMenu;
        
        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            _nnLobbyMenu = rc.Get<GameObject>("NiuNiuLobbyMenu");
            var backLobbyBtn = rc.Get<GameObject>("BackLobbyBtn");
            var settingBtn = rc.Get<GameObject>("SettingBtn");
            var feedbackBtn = rc.Get<GameObject>("FeedbackBtn");
            var ruleBtn = rc.Get<GameObject>("RuleBtn");

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(_nnLobbyMenu.GetComponent<Button>(), () => { _nnLobbyMenu.SetActive(false); });
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(backLobbyBtn.GetComponent<Button>(), () =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.Lobby,UiLayer.Bottom);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuLobby);
            });

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(settingBtn.GetComponent<Button>(), ShowSettingPanel);
        }

        public void Start()
        {
            _settingPanel = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuSetting);
        }

        private void ShowSettingPanel()
        {
            _nnLobbyMenu.SetActive(false);
            _settingPanel.GameObject.SetActive(true);
        }
    }
}