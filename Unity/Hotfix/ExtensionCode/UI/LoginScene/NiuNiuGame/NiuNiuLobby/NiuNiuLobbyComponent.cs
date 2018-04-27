using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class NiuNiuLobbyComponentAwakeSystem : AwakeSystem<NiuNiuLobbyComponent>
    {
        public override void Awake(NiuNiuLobbyComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class NiuNiuLobbyComponentStartSystem : StartSystem<NiuNiuLobbyComponent>
    {
        public override void Start(NiuNiuLobbyComponent self)
        {
            self.Start();
        }
    }

    public class NiuNiuLobbyComponent : Component
    {

        private UI _nnCreateRoom;
        private UI _nnLobbyMenu;
        private UI _nnJoinRoom;

        private GameObject _userIdText;
        private GameObject _diamondText;

        public async void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            #region 获取游戏物体

            // 用户信息
            _userIdText = rc.Get<GameObject>("UserIdText");
            _diamondText = rc.Get<GameObject>("DiamondText");

            // 页面
            var nnLobby = rc.Get<GameObject>("NiuNiuLobby");

            // 大厅主要按钮
            var lobbyCreateButton = rc.Get<GameObject>("LobbyCreateButton");
            var lobbyJoinRoomButton = rc.Get<GameObject>("LobbyJoinRoomButton");
            var lobbyNiuFriendButton = rc.Get<GameObject>("LobbyNiuFriendButton");

            // 大厅次要按钮
            var ruleBtn = rc.Get<GameObject>("RuleBtn");
            var feedbackBtn = rc.Get<GameObject>("FeedbackBtn");
            var settingBtn = rc.Get<GameObject>("SettingBtn");
            var backLobbyBtn = rc.Get<GameObject>("BackLobbyBtn");
            var portraitBtn = rc.Get<GameObject>("PortraitBtn");
            var buyDiamondBtn = rc.Get<GameObject>("BuyDiamondBtn");
            var yaoQingMaBtn = rc.Get<GameObject>("YaoQingMaBtn");
            var zhanJiBtn = rc.Get<GameObject>("ZhanJiBtn");
            var mailBtn = rc.Get<GameObject>("MailBtn");
            var shareBtn = rc.Get<GameObject>("ShareBtn");
            var menuBtn = rc.Get<GameObject>("MenuBtn");

            #endregion

            #region 大厅页面点击事件

            // 创建房间按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(lobbyCreateButton.GetComponent<Button>(), () =>
            {
                nnLobby.SetActive(false);
                _nnCreateRoom.GameObject.SetActive(true);
            });

            // 加入房间按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(lobbyJoinRoomButton.GetComponent<Button>(), () =>
            {
                _nnJoinRoom.GameObject.SetActive(true);
            });

//            // 牛友群按钮
//            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(lobbyNiuFriendButton.GetComponent<Button>(), () => { });

            // 菜单按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(menuBtn.GetComponent<Button>(), () =>
            {
                _nnLobbyMenu.GameObject.SetActive(true);
            });

            #endregion
        }

        public void Start()
        {
            _nnCreateRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuCreateRoom);
            _nnJoinRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuJoinRoom);
            _nnLobbyMenu = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuLobbyMenu);
        }

        public void InitUserInfo(string userName, string diamond)
        {
            _userIdText.GetComponent<Text>().text = userName;
            _diamondText.GetComponent<Text>().text = diamond;
        }

        public string GetUserName()
        {
            return _userIdText.GetComponent<Text>().text;
        }
        public string GetDiamond()
        {
            return _diamondText.GetComponent<Text>().text;
        }
    }
}