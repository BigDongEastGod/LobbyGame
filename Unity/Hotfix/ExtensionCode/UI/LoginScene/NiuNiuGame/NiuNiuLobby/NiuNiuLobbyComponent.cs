using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    [ObjectSystem]
    public class NiuNiuLobbyComponentUpdateSystem : UpdateSystem<NiuNiuLobbyComponent>
    {
        public override void Update(NiuNiuLobbyComponent self)
        {
            self.Update();
        }
    }

    public class NiuNiuLobbyComponent : Component
    {
        private UI _nnCreateRoom;
        private UI _nnLobbyMenu;
        private UI _nnJoinRoom;

        private GameObject _userIdText;
        private GameObject _diamondText;

        private float _barMoveSpeed;
        private bool _isMoveBar;

        private RectTransform _barTextTransform;
        private RectTransform _posLeftTransform;
        private RectTransform _posRightTransform;

        private GameObject _refreshBtn;
        private GameObject _refreshImage;

        private GameObject _roomEmptyImg;
        private GameObject _roomContent;
        private GameObject _roomInfoItem;
        private GameObject _nnLobby;

        // 房间列表
        private bool _isNiuFriendRoom;
        private List<GameObject> _roomInfoList;

        public async void Awake()
        {
            // 获取用户信息
            var response = (GetAccountInfoResponse) await SceneHelperComponent.Instance.Session.Call(new GetAccountInfoRequest());

            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            #region 获取游戏物体

            _nnLobby = rc.Get<GameObject>("NiuNiuLobby");

            // 用户信息
            _userIdText = rc.Get<GameObject>("UserIdText");
            _diamondText = rc.Get<GameObject>("DiamondText");
            InitUserInfo(response.AccountInfo.UserName, response.AccountInfo.Diamond.ToString());

            var _barText = rc.Get<GameObject>("NiuNiuNoticeBar").transform.Find("mask/NoticeBarText").gameObject;
            var _barPosLeft = rc.Get<GameObject>("NiuNiuNoticeBar").transform.Find("BarPosLeft").gameObject;
            var _barPosRight = rc.Get<GameObject>("NiuNiuNoticeBar").transform.Find("BarPosRight").gameObject;

            _barTextTransform = _barText.GetComponent<RectTransform>();
            _posLeftTransform = _barPosLeft.GetComponent<RectTransform>();
            _posRightTransform = _barPosRight.GetComponent<RectTransform>();

            _barMoveSpeed = 10f;
            _isMoveBar = true;

            _roomInfoList = new List<GameObject>();
            _isNiuFriendRoom = false;

            // 页面
            var nnLobby = rc.Get<GameObject>("NiuNiuLobby");

            // 大厅主要按钮
            var lobbyCreateButton = rc.Get<GameObject>("LobbyCreateButton");
            var lobbyJoinRoomButton = rc.Get<GameObject>("LobbyJoinRoomButton");
//            var lobbyNiuFriendButton = rc.Get<GameObject>("LobbyNiuFriendButton");

            var friendToggle = rc.Get<GameObject>("FriendToggle");
            var niuFriendToggle = rc.Get<GameObject>("NiuFriendToggle");

            // 大厅次要按钮
//            var ruleBtn = rc.Get<GameObject>("RuleBtn");
//            var feedbackBtn = rc.Get<GameObject>("FeedbackBtn");
//            var settingBtn = rc.Get<GameObject>("SettingBtn");
//            var backLobbyBtn = rc.Get<GameObject>("BackLobbyBtn");
//            var portraitBtn = rc.Get<GameObject>("PortraitBtn");
//            var buyDiamondBtn = rc.Get<GameObject>("BuyDiamondBtn");
//            var yaoQingMaBtn = rc.Get<GameObject>("YaoQingMaBtn");
//            var zhanJiBtn = rc.Get<GameObject>("ZhanJiBtn");
//            var mailBtn = rc.Get<GameObject>("MailBtn");
//            var shareBtn = rc.Get<GameObject>("ShareBtn");
            var menuBtn = rc.Get<GameObject>("MenuBtn");
            _refreshBtn = rc.Get<GameObject>("RefreshBtn");

            _refreshImage = rc.Get<GameObject>("RefreshImg");

            // 已创建房间列表
            _roomEmptyImg = rc.Get<GameObject>("RoomEmptyImg"); //没有房间信息时候显示图片
            _roomContent = rc.Get<GameObject>("RoomContent"); //房间列表父物体
            _roomInfoItem = rc.Get<GameObject>("NiuNiuRoomInfoItem"); //房间信息预设

            #endregion

            #region 大厅页面点击事件

            // 创建房间按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(lobbyCreateButton.GetComponent<Button>(), () =>
            {
                nnLobby.SetActive(false);
                _nnCreateRoom.GameObject.SetActive(true);
            });

            // 加入房间按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(lobbyJoinRoomButton.GetComponent<Button>(), () => { _nnJoinRoom.GameObject.SetActive(true); });

//            // 牛友群按钮
//            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(lobbyNiuFriendButton.GetComponent<Button>(), () => { });

            // 菜单按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(menuBtn.GetComponent<Button>(), () => { _nnLobbyMenu.GameObject.SetActive(true); });

            niuFriendToggle.GetComponent<Toggle>().onValueChanged.AddListener((niuFriend) =>
            {
                _isNiuFriendRoom = niuFriend;
                GetRoomList();
            });

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(_refreshBtn.GetComponent<Button>(), GetRoomList);

            #endregion


            // 断线重连委托
            Game.Scene.GetComponent<PingComponent>().PingBackCall = () =>
            {
                GameTools.ReLoading("GameCanvas");
                GetRoomList();
            };


            GetRoomList();
        }

        public void Start()
        {
            _nnCreateRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuCreateRoom);
            _nnJoinRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuJoinRoom);
            _nnLobbyMenu = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuLobbyMenu);
        }

        private void InitUserInfo(string userName, string diamond)
        {
            _userIdText.GetComponent<Text>().text = userName;
            _diamondText.GetComponent<Text>().text = diamond;
        }

        public async void GetRoomList()
        {
            _roomListIsDone = false;
            _refreshBtn.GetComponent<Button>().interactable = false;
            _refreshImage.GetComponent<Image>().material.SetFloat("_Speed", 130);
            _isRotate = true;
            _rotateTime = 50;
            
            try
            {
                if (!_isNiuFriendRoom)
                {
                    var roomList = (RoomListResponse) await SceneHelperComponent.Instance.Session.Call(new RoomListRequest() {GameType = "NN"});

                    if (roomList.Error == 0)
                    {
                        int countDiff = roomList.Rooms.Count - _roomInfoList.Count;
                        if (countDiff > 0)
                        {
                            for (int i = 0; i < countDiff; i++)
                            {
                                var go = UnityEngine.Object.Instantiate(_roomInfoItem, _roomContent.transform);
                                _roomContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 95f * countDiff);
                                go.SetActive(false);
                                _roomInfoList.Add(go);
                            }
                        }

                        for (int i = 0; i < roomList.Rooms.Count; i++)
                        {
                            var rc = _roomInfoList[i].GetComponent<ReferenceCollector>();
                            var roomId = rc.Get<GameObject>("RoomId");
                            var playerMode = rc.Get<GameObject>("PlayerMode");
                            var score = rc.Get<GameObject>("Score");
                            var dish = rc.Get<GameObject>("Dish");
                            var payMode = rc.Get<GameObject>("PayMode");
                            var playerCount = rc.Get<GameObject>("PlayerCount");
                            var inviteBtn = rc.Get<GameObject>("InviteBtn"); //TODO

                            var room = roomList.Rooms[i];

                            roomId.GetComponent<Text>().text = room.RoomId.ToString();
                            playerMode.GetComponent<Text>().text = room.PlayerMode;
                            score.GetComponent<Text>().text = room.Score;
                            dish.GetComponent<Text>().text = room.Dish.ToString();
                            payMode.GetComponent<Text>().text = room.PayMode;
                            playerCount.GetComponent<Text>().text = room.PlayerCount;

                            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(_roomInfoList[i].GetComponent<Button>(), () => JoinPaiJu(room.RoomId));

                            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(inviteBtn.GetComponent<Button>(), () => GameTools.ShowDialogMessage("邀请好友", "GameCanvas"));

                            _roomInfoList[i].SetActive(true);
                        }

                        Debug.Log("房间列表加载完成");
                        _roomEmptyImg.SetActive(roomList.Rooms.Count == 0);
                    }
                    else
                    {
                        GameTools.ShowDialogMessage(roomList.Message, "GameCanvas");
                    }
                }
                else
                {
                    if (_roomInfoList != null)
                        foreach (var go in _roomInfoList)
                        {
                            go.SetActive(false);
                        }

                    _roomEmptyImg.SetActive(true);
                }
            }
            catch (Exception e)
            {
                Debug.Log("RoomList加载异常： " + e.Message);
                GameTools.ShowDialogMessage(e.Message, "GameCanvas");
            }

            _roomListIsDone = true;
        }

        /// <summary>
        /// 加入牌局
        /// </summary>
        /// <param name="roomId"></param>
        private async void JoinPaiJu(long roomId)
        {
            try
            {
                var joinRoomResponse = (JoinRoomResponse) await SceneHelperComponent.Instance.Session.Call(
                    new JoinRoomRequest() {RoomId = roomId});

                if (joinRoomResponse.Error == 0)
                {
                    Debug.Log("加入房间成功,跳转至游戏主场景");

                    Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuMain, UiLayer.Bottom, roomId, false);
                    Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuLobby);
                }
                else
                {
                    Debug.Log("加入房间失败: " + joinRoomResponse.Message);
                    GameTools.ShowDialogMessage("没有这个房间,请重新输入!", "GameCanvas");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private int _rotateTime = 50;
        private bool _isRotate = false;
        private bool _roomListIsDone = true;

        public void Update()
        {
            MoveBar();

            if (_isRotate)
            {
                _rotateTime--;
                if (_rotateTime <= 0 && _roomListIsDone)
                {
                    
                    _refreshBtn.GetComponent<Button>().interactable = true;
                    _isRotate = false;
                    _rotateTime = 50;
                    _refreshImage.GetComponent<Image>().material.SetFloat("_Speed", 0);
                }
            }
        }

        private void MoveBar()
        {
            if (_isMoveBar && _barTextTransform && _posLeftTransform && _posRightTransform)
            {
                Vector2 tempVec2 = new Vector2(_barTextTransform.anchoredPosition.x - Time.deltaTime * 10 * _barMoveSpeed, _barTextTransform.anchoredPosition.y);
                _barTextTransform.anchoredPosition = tempVec2;
                if (tempVec2.x < _posLeftTransform.anchoredPosition.x)
                {
                    _barTextTransform.anchoredPosition = _posRightTransform.anchoredPosition;
                }
            }
        }
    }
}