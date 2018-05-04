using System;
using System.Threading.Tasks;
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

        private GameObject _barText;
        private float _barMoveSpeed;
        private bool _isMoveBar;
        private RectTransform barTextTransform;
        private RectTransform posLeftTransform;
        private RectTransform posRightTransform;

        public async void Awake()
        {
            // 获取用户信息
            var response = (GetAccountInfoResponse) await SceneHelperComponent.Instance.Session.Call(new GetAccountInfoRequest());

            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            var niuniuStartBtn = rc.Get<GameObject>("NiuNiuStartBtn");
            var lobby = rc.Get<GameObject>("Lobby");
            _userIdText = rc.Get<GameObject>("UserIdText");
            _diamondText = rc.Get<GameObject>("DiamondText");
            InitUserInfo(response.AccountInfo.UserName, response.AccountInfo.Diamond.ToString());

            _barText = rc.Get<GameObject>("NoticeBarText");
            var _barPosLeft = rc.Get<GameObject>("NiuNiuNoticeBar").transform.Find("BarPosLeft").gameObject;
            var _barPosRight = rc.Get<GameObject>("NiuNiuNoticeBar").transform.Find("BarPosRight").gameObject;

            barTextTransform = _barText.GetComponent<RectTransform>();
            posLeftTransform = _barPosLeft.GetComponent<RectTransform>();
            posRightTransform = _barPosRight.GetComponent<RectTransform>();

            _barMoveSpeed = 10f;
            _isMoveBar = true;

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(niuniuStartBtn.GetComponent<Button>(), () =>
            {
                // TODO 启动牛牛
                Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuLobby, UiLayer.Bottom);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.Lobby);
            });

            Game.Scene.GetComponent<PingComponent>().PingBackCall = () =>
            {
                GameTools.ReLoading("LobbyCanvas");
            };
        }

        public void Update()
        {
            MoveBar();
        }


        private void InitUserInfo(string userName, string diamond)
        {
            _userIdText.GetComponent<Text>().text = userName;
            _diamondText.GetComponent<Text>().text = diamond;
        }

        private void MoveBar()
        {
            if (_isMoveBar && barTextTransform && posLeftTransform && posRightTransform)
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