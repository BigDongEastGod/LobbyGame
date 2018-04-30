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

    [ObjectSystem]
    public class LobbyComponentStartSystem : StartSystem<LobbyComponent>
    {
        public override void Start(LobbyComponent self)
        {
            self.Start();
        }
    }

    public class LobbyComponent : Component
    {
        private GameObject _userIdText;
        private GameObject _diamondText;

        private GameObject _barText;
        private float _barMoveSpeed;
        private bool _isMoveBar;
        private RectTransform barTextTransform;
        private RectTransform posLeftTransform;
        private RectTransform posRightTransform;

        public async void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            var niuniuStartBtn = rc.Get<GameObject>("NiuNiuStartBtn");
            _userIdText = rc.Get<GameObject>("UserIdText");
            _diamondText = rc.Get<GameObject>("DiamondText");

            _barText = rc.Get<GameObject>("NoticeBarText");
            var _barPosLeft = rc.Get<GameObject>("NiuNiuNoticeBar").transform.Find("BarPosLeft").gameObject;
            var _barPosRight = rc.Get<GameObject>("NiuNiuNoticeBar").transform.Find("BarPosRight").gameObject;

            barTextTransform = _barText.GetComponent<RectTransform>();
            posLeftTransform = _barPosLeft.GetComponent<RectTransform>();
            posRightTransform = _barPosRight.GetComponent<RectTransform>();

            _barMoveSpeed = 10f;
            _isMoveBar = true;

            var response = (GetAccountInfoResponse) await SceneHelperComponent.Instance.Session.Call(new GetAccountInfoRequest());
            InitUserInfo(response.AccountInfo.UserName, response.AccountInfo.Diamond.ToString());

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(niuniuStartBtn.GetComponent<Button>(), () =>
            {
                // TODO 启动牛牛
                Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuLobby, UiLayer.Bottom);

                UI niuniuLobbyUi = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuLobby);
                niuniuLobbyUi.GetComponent<NiuNiuLobbyComponent>().InitUserInfo(response.AccountInfo.UserName, response.AccountInfo.Diamond.ToString());

                Game.Scene.GetComponent<UIComponent>().Remove(UIType.Lobby);
            });
        }

        public void Start()
        {
        }


        public void Update()
        {
            MoveBar();
        }


        public void InitUserInfo(string userName, string diamond)
        {
            _userIdText.GetComponent<Text>().text = userName;
            _diamondText.GetComponent<Text>().text = diamond;
        }

        private void MoveBar()
        {
            if (_isMoveBar)
            {
                Vector2 tempVec2 = new Vector2(barTextTransform.anchoredPosition.x - Time.deltaTime * 10 * _barMoveSpeed, barTextTransform.anchoredPosition.y);
                barTextTransform.anchoredPosition = tempVec2;
                if (tempVec2.x < posLeftTransform.anchoredPosition.x)
                {
                    barTextTransform.anchoredPosition = posRightTransform.anchoredPosition;
                }
            }
        }
    }
}