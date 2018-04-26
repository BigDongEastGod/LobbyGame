using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class LobbyComponentAwakeSystem : AwakeSystem<LobbyComponent>
    {
        public override void Awake(LobbyComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class LobbyComponentUpdateSystem : UpdateSystem<LobbyComponent>
    {
        public override void Update(LobbyComponent self)
        {
            self.Update();
        }
    }

    public class LobbyComponent : Component
    {
        private GameObject _userIdText;
        private GameObject _diamondText;
        
        public async void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            var niuniuStartBtn = rc.Get<GameObject>("NiuNiuStartBtn");
            _userIdText = rc.Get<GameObject>("UserIdText");
            _diamondText = rc.Get<GameObject>("DiamondText");

            var response = (GetAccountInfoResponse) await SceneHelperComponent.Instance.Session.Call(new GetAccountInfoRequest());
            InitUserInfo(response.AccountInfo.UserName,response.AccountInfo.Diamond.ToString());
            
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(niuniuStartBtn.GetComponent<Button>(), () =>
            {
                // TODO 启动牛牛
                Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuLobby, UiLayer.Bottom);

                UI niuniuLobbyUi = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuLobby);
                niuniuLobbyUi.GetComponent<NiuNiuLobbyComponent>().InitUserInfo(response.AccountInfo.UserName, response.AccountInfo.Diamond.ToString());

                Game.Scene.GetComponent<UIComponent>().Remove(UIType.Lobby);
            });
        }

        public void Update()
        {
            
        }


        public void InitUserInfo(string userName,string diamond)
        {
            _userIdText.GetComponent<Text>().text = userName;
            _diamondText.GetComponent<Text>().text = diamond;
        }
    }
}