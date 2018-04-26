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
    public class NiuNiuLobbyMenuComponentStartSystem : StartSystem<NiuNiuLobbyMenuComponent>
    {
        public override void Start(NiuNiuLobbyMenuComponent self)
        {
            self.Start();
        }
    }

    public class NiuNiuLobbyMenuComponent : Component
    {
        private UI _niuniuLobby;

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
                string username = _niuniuLobby.GetComponent<NiuNiuLobbyComponent>().GetUserName();
                string diamond = _niuniuLobby.GetComponent<NiuNiuLobbyComponent>().GetDiamond();
                
                Game.Scene.GetComponent<UIComponent>().Create(UIType.Lobby,UiLayer.Bottom);
                
                var lobby = Game.Scene.GetComponent<UIComponent>().Get(UIType.Lobby);
                lobby.GetComponent<LobbyComponent>().InitUserInfo(username, diamond);

                Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuLobby);
            });
        }

        public void Start()
        {
            _niuniuLobby = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuLobby);
        }
    }
}