using System;
using System.Collections.Generic;
using DG.Tweening;
using ETModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class NiuNiuMainComponentAwakeArgSystem : AwakeSystem<NiuNiuMainComponent,object[]>
    {
        public override void Awake(NiuNiuMainComponent self,object[] args)
        {
            self.Awake(args);
        }
    }
    

    public class NiuNiuMainComponent:Component
    {
        private Text _roomNum;                        //房间号码文本
        private long _mRoomId;                         //房间号码
        private UI _showCardUi;                       //卡牌存放窗口
        private Button _sitDownBt;                    //坐下按钮
        private Button _startGameBt;                  //开始按钮
        private GetAccountInfoResponse _player;        //当前玩家数据
        private RoomInfoResponse _roomInfo;            //房间信息
        private Button _betsButton1;
        private Button _betsButton2;
        private Text _bottomScoreText;                //底分文本
        private string _zhuangJiaName;

        public void Awake(object[] args)
        {
            SetRoomInfo(Convert.ToInt64(args[0]),Convert.ToBoolean(args[1]));
        }
        
        //初始化数据
        private async void SetRoomInfo(long roomId,bool isSitDown)
        {
            #region GetRoomNeedComponent

            var rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            
            //房间分线图
            var bg = rc.Get<GameObject>("BgImg");
            //房间信息按钮
            var roomInfoButton  = rc.Get<GameObject>("RoomInfoButton");
            //下拉窗口按钮
            var selectButton=rc.Get<GameObject>("SelectButton"); 
            //托管按钮
            var trusteeshipButton=rc.Get<GameObject>("Trusteeship");
            //表情按钮
            var expressionButton=rc.Get<GameObject>("Expression");
            //声音按钮
            var audioButton=rc.Get<GameObject>("AudioButton");
            //开始游戏按钮
            _startGameBt=rc.Get<GameObject>("StartGameBt").GetComponent<Button>();
            //坐下按钮
            _sitDownBt=rc.Get<GameObject>("SitDownBt").GetComponent<Button>();
            //不抢庄按钮
            var noBobButton=rc.Get<GameObject>("NoBobButton");
            //抢庄按钮
            var qiangzhuangBt=rc.Get<GameObject>("qiangzhuangBt");
            //下注选择按钮1
            _betsButton1=rc.Get<GameObject>("BetsButton1").GetComponent<Button>();
            //下注选择按钮2
            _betsButton2=rc.Get<GameObject>("BetsButton2").GetComponent<Button>();
            //复制房间号按钮
            var copyNumButton=rc.Get<GameObject>("CopyNumButton");
            //邀请微信好友按钮
            var invitingFriendsButton=rc.Get<GameObject>("InvitingFriendsButton");
            //搓牌按钮
            var shuffleButton=rc.Get<GameObject>("ShuffleButton");
            //翻牌按钮
            var flopButton=rc.Get<GameObject>("FlopButton");
            //准备按钮
            var readyButton=rc.Get<GameObject>("readyButton");
            //表情按钮
            var exitButton=rc.Get<GameObject>("ExitButton");
            //时间
            var timeTxt=rc.Get<GameObject>("TimeTxt");
            //电池
            var batterySlider=rc.Get<GameObject>("BatterySlider");
            //wifi
            var wiFiImg=rc.Get<GameObject>("WiFiImg");
            //自动翻牌
            var automaticFlopToggle=rc.Get<GameObject>("AutomaticFlopToggle");
            //提示按钮
            var tipsButton=rc.Get<GameObject>("tipsButton");
            //亮牌按钮
            var brightButton=rc.Get<GameObject>("brightButton");
            
            _roomNum = rc.Get<GameObject>("roomNum").GetComponent<Text>();
            //庄位信息
            var zhuangWeiTxt  = rc.Get<GameObject>("zhuangWei").GetComponent<Text>();
            //底分信息
            _bottomScoreText  = rc.Get<GameObject>("BottomScoreText").GetComponent<Text>();
            //房间局数
            var roomCountText  = rc.Get<GameObject>("roomCountText").GetComponent<Text>();

            #endregion
         
            //获取当前账号
            var accountResponse = (GetAccountInfoResponse) await SceneHelperComponent.Instance.Session.Call(new GetAccountInfoRequest());
            _player = accountResponse;
            
            //请求获得当前房间准备好的玩家信息
            var response =(RoomInfoResponse) await SceneHelperComponent.Instance.Session.Call(new RoomInfoRequest() {RoomId = roomId});
            _roomInfo = response;
            //获得房间规则信息
            var rules = response.Rules == null ? null : ProtobufHelper.FromBytes<NNChess>(response.Rules);
            //设置房间号
            Debug.Log("roomId"+roomId);
            _mRoomId = roomId;
            _roomNum.text = roomId.ToString();
            //TODo.....庄位信息
            //zhuangWeiTxt=rules.

            //设置房间底分
            if (rules != null)
            {
                _bottomScoreText.text = rules.Score.ToString();
                //设置房间局数
                roomCountText.text = "1/" + rules.Dish.ToString();
                //获得房间显示卡牌窗口
                _showCardUi = Game.Scene.GetComponent<UIComponent>().Create(UIType.NNShowCard, UiLayer.Medium);
                //设置房间人数
                _showCardUi.GetComponent<NnShowCardComponent>().RoomPeople = rules.PlayerCount;
            }

            //加载所需要的位置信息
            _showCardUi.GetComponent<NnShowCardComponent>().GetCurrentTablePos();
            
            //房间信息窗口事件注册
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(roomInfoButton.GetComponent<Button>(), () =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.NNRoomRuleInfoUIForm,UiLayer.Top);
            });
       
            
            //坐下按钮事件注册
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(_sitDownBt.GetComponent<Button>(), GetRoomInfo);
            
            //下拉按钮注册
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(selectButton.GetComponent<Button>(), () =>
            {
                object[] arg = new object[] {_mRoomId,this};
                Game.Scene.GetComponent<UIComponent>().Create(UIType.NNRoomOperation, UiLayer.Top,arg);
            });
            
            //开始按钮注册
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(_startGameBt.GetComponent<Button>(), StartGameOclick);
            

            //获取房间准备号玩家的数据
            GetAllReadyInfo();
            
//            RoomInfoAnnunciateHandler.RoomAction += RoomBack;
//            GameInfoAnnunciateHandler.GameAction += GameBack;

            #region 短线处理

//            //如果是短线重连，判断是否已经坐下
//            if (isSitDown)
//            {
//                SitDown(-1,_player.AccountInfo.UserName);
//            }
//            else
//            {
//                _sitDownBt.gameObject.SetActive(true);
//            }
//
//            Game.Scene.GetComponent<PingComponent>().PingBackCall = () =>
//            {
//                GameTools.ReLoading("GameCanvas");
//            };

            #endregion

           //Test();


        }

      

        //房间回调
        public void RoomBack(RoomInfoAnnunciate obj)
        {
            switch (obj.Message)
            {
                case 0://加入房间
                    break;
                case 1://准备
                    var chairIndex= _showCardUi.GetComponent<NnShowCardComponent>().FindFreeChair(obj.UserName);
                    SitDown(chairIndex, obj.UserName);
                    break;
                case 2://离开房间
                    _showCardUi.GetComponent<NnShowCardComponent>().QuitRoom(obj.UserName);
                    break;
                case 3://显示开始游戏按钮
                    _startGameBt.gameObject.SetActive(true);
                    _startGameBt.GetComponent<Animator>().SetInteger("IsMiddle",1);
                    break;
            }
        }
        
        //房间游戏回调
        public void GameBack(GameInfoAnnunciate obj)
        {
           Debug.Log("收到回调"+obj.Message);
            switch (obj.Message)
            {
                case 0://显示下注按钮
                    if(_zhuangJiaName==_player.AccountInfo.UserName) return;
                    _startGameBt.gameObject.SetActive(false);
                    ShowBetsButton();
                    break;
                case 1://下注完成
                    ShowBet(obj.UserName,SerializeHelper.Instance.DeserializeObject<int>(obj.Arg));
                    break;
                case 2://抢庄
                    ShowZhuangJiaIcon(obj.UserName);
                    _zhuangJiaName = obj.UserName;
                    break;
                case 3://发牌
                    var pokerCard= SerializeHelper.Instance.DeserializeObject<List<PokerCard>>(obj.Arg);
                    Licensing(pokerCard);
                    break;
                    
            }
        }
        
        //创建本地玩家头像
        private async void GetRoomInfo()
        {
            var response =(PrepareGameResponse) await SceneHelperComponent.Instance.Session.Call(new PrepareGameRequest(){RoomId = _mRoomId});
            if (response.Error != 0) return;
            _sitDownBt.gameObject.SetActive(false);
            SitDown(-1,_player.AccountInfo.UserName);
        }

        //其他玩家的头像创建
        private void GetAllReadyInfo()
        {
            for (var i = 0; i < _roomInfo.Players.Count; i++)
            {
                if (_roomInfo.Players[i].UserName != _player.AccountInfo.UserName)
                {
                    _showCardUi.GetComponent<NnShowCardComponent>().CreateHead(i,_roomInfo.Players[i]);
                }
            }
        }

        //坐下
        private void SitDown(int chairIndex,string username)
        {
            var accountInfo=new AccountInfo(){UserName = username};
            _showCardUi.GetComponent<NnShowCardComponent>().CreateHead(chairIndex,accountInfo);
        }
        
        //开始游戏点
        private async void StartGameOclick()
        {
            var startResponse =(StartGameResponse) await SceneHelperComponent.Instance.Session.Call(new StartGameRequest(){RoomId = _mRoomId});
            Debug.Log(startResponse.Error == -1 ? "当前房间人数不够，不能开始游戏!!!" : "开始游戏成功!!!");
        }
        
        //显示下注按钮
        private void ShowBetsButton()
        {
            var scroeStr = _bottomScoreText.text.Split('/');
            if (scroeStr.Length <= 1) return;
            _betsButton1.gameObject.SetActive(true);
            _betsButton2.gameObject.SetActive(true);
            _betsButton1.transform.GetChild(0).GetComponent<Text>().text = scroeStr[0];
            _betsButton2.transform.GetChild(0).GetComponent<Text>().text = scroeStr[1];
            _betsButton1.onClick.AddListener(()=>AddBetsEvent(scroeStr[0]));
            _betsButton2.onClick.AddListener(()=>AddBetsEvent(scroeStr[1]));
        }

        //向服务器发送下注请求
        private async void AddBetsEvent(string score)
        {
            var betsResponse =(BetGameResponse) await SceneHelperComponent.Instance.Session.Call(new BetGameRequest(){Bet=int.Parse(score)});
            if (betsResponse.Error != 0) return;
            Debug.Log("下注成功，底分为:"+score);
            _betsButton1.gameObject.SetActive(false);
            _betsButton2.gameObject.SetActive(false);
            _showCardUi.GetComponent<NnShowCardComponent>().ShowBets(_player.AccountInfo.UserName,int.Parse(score));

        }

        //显示下注的分数
        private void ShowBet(string userName,int score)
        {
            _showCardUi.GetComponent<NnShowCardComponent>().ShowBets(userName, score);
        }
        
        //显示庄家
        private void ShowZhuangJiaIcon(string userName)
        {
            _startGameBt.gameObject.SetActive(false);
            _showCardUi.GetComponent<NnShowCardComponent>().ShowZhuangJiaIcon(userName);
        }
        
        //发牌
        private void Licensing(List<PokerCard> pokerCards)
        {
            _showCardUi.GetComponent<NnShowCardComponent>().Licensing(pokerCards);
        }


        private void Test()
        {
            var accountInfo=new AccountInfo(){UserName = "1"};
            _showCardUi.GetComponent<NnShowCardComponent>().CreateHead(-1,accountInfo);
            var accountInfo1=new AccountInfo(){UserName = "2"};
            _showCardUi.GetComponent<NnShowCardComponent>().CreateHead(0,accountInfo1);
            var accountInfo2=new AccountInfo(){UserName = "3"};
            _showCardUi.GetComponent<NnShowCardComponent>().CreateHead(1,accountInfo2);
            var accountInfo3=new AccountInfo(){UserName = "4"};
            _showCardUi.GetComponent<NnShowCardComponent>().CreateHead(2,accountInfo3);
            var accountInfo4=new AccountInfo(){UserName = "5"};
            _showCardUi.GetComponent<NnShowCardComponent>().CreateHead(3,accountInfo4);
            var accountInfo5=new AccountInfo(){UserName = "6"};
            _showCardUi.GetComponent<NnShowCardComponent>().CreateHead(4,accountInfo5);
            
            
            List<PokerCard> list=new List<PokerCard>();
            for (var i = 0; i < 5; i++)
            {
                PokerCard temp = new PokerCard
                {
                    CardNumber = i,
                    CardType = 0
                };
                list.Add(temp);
            }

            _player.AccountInfo.UserName = "1";
            Licensing(list);
        }

    }
}