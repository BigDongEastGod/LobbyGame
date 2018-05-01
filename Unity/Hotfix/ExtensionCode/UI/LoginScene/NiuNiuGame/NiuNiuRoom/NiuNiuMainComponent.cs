using System;
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
        private Text roomNum;                        //房间号码文本
        private long m_roomId;                         //房间号码
        private UI showCardUI;                       //卡牌存放窗口
        private Button sitDownBt;                    //坐下按钮
        private Button startGameBt;                  //开始按钮
        public GetAccountInfoResponse Player;        //当前玩家数据
        public RoomInfoResponse roomInfo;            //房间信息

        public async void Awake(object[] args)
        {
            SetRoomInfo(Convert.ToInt64(args[0]));
        }
        
        //初始化数据
        private async void SetRoomInfo(long roomId)
        {
            Debug.Log("加入房间"+roomId);
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            
            //房间分线图
            var bg = rc.Get<GameObject>("BgImg");
            //房间号码
            
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
            startGameBt=rc.Get<GameObject>("StartGameBt").GetComponent<Button>();
            //坐下按钮
            sitDownBt=rc.Get<GameObject>("SitDownBt").GetComponent<Button>();
            //不抢庄按钮
            var noBobButton=rc.Get<GameObject>("NoBobButton");
            //抢庄按钮
            var qiangzhuangBt=rc.Get<GameObject>("qiangzhuangBt");
            //下注选择按钮1
            var betsButton1=rc.Get<GameObject>("BetsButton1");
            //下注选择按钮2
            var betsButton2=rc.Get<GameObject>("BetsButton2");
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
            var AutomaticFlopToggle=rc.Get<GameObject>("AutomaticFlopToggle");
            
            roomNum = rc.Get<GameObject>("roomNum").GetComponent<Text>();
            //庄位信息
            var zhuangWeiTxt  = rc.Get<GameObject>("zhuangWei").GetComponent<Text>();
            //底分信息
            var bottomScoreText  = rc.Get<GameObject>("BottomScoreText").GetComponent<Text>();
            //房间局数
            var roomCountText  = rc.Get<GameObject>("roomCountText").GetComponent<Text>();
            
            //获取当前账号
            var accountResponse = (GetAccountInfoResponse) await SceneHelperComponent.Instance.Session.Call(new GetAccountInfoRequest());
            Player = accountResponse;
            
            //请求获得当前房间准备好的玩家信息
            var response =(RoomInfoResponse) await SceneHelperComponent.Instance.Session.Call(new RoomInfoRequest() {RoomId = roomId});
            roomInfo = response;
            //获得房间规则信息
            var rules = response.Rules == null ? null : ProtobufHelper.FromBytes<NNChess>(response.Rules);
            //设置房间号
            Debug.Log("roomId"+roomId);
            m_roomId = roomId;
            roomNum.text = roomId.ToString();
            //TODo.....庄位信息
            //zhuangWeiTxt=rules.

            //设置房间底分
              bottomScoreText.text = rules.Score.ToString();
            //设置房间局数
              roomCountText.text = "1/"+rules.Dish.ToString();
            //获得房间显示卡牌窗口
            showCardUI=Game.Scene.GetComponent<UIComponent>().Create(UIType.NNShowCard, UiLayer.Medium);
            //设置房间人数
            showCardUI.GetComponent<NNShowCardComponent>().roomPeople =(ushort)rules.PlayerCount;
            //加载所需要的位置信息
            showCardUI.GetComponent<NNShowCardComponent>().GetCurrentTablePos();
            
            //房间信息窗口事件注册
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(roomInfoButton.GetComponent<Button>(), () =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.NNRoomRuleInfoUIForm,UiLayer.Top);
            });
       
            //房间信息窗口事件注册
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(selectButton.GetComponent<Button>(), () =>
            {
                
                Game.Scene.GetComponent<UIComponent>().Create(UIType.NNRoomOperation,UiLayer.Top);
            });
            
            
            //房间信息窗口事件注册
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(sitDownBt.GetComponent<Button>(), () =>
            {
                GetRoomInfo();
            });

            //获取房间准备号玩家的数据
            GetAllReadyInfo();
            
            RoomInfoAnnunciateHandler.RoomAction += RoomInfo;
        }

        private void RoomInfo(RoomInfoAnnunciate obj)
        {
            
        }

        private async void GetRoomInfo()
        {
            var response =(PrepareGameResponse) await SceneHelperComponent.Instance.Session.Call(new PrepareGameRequest(){RoomId = m_roomId});
            if (response.Error==0)
            {
                Debug.Log("成功坐下");
                sitDownBt.gameObject.SetActive(false);
                startGameBt.GetComponent<Animator>().SetInteger("IsMiddle",1);

                AccountInfo accountInfo=new AccountInfo(){UserName = Player.AccountInfo.UserName};
                showCardUI.GetComponent<NNShowCardComponent>().CreateHead(-1,accountInfo);
            }
        }

        private void GetAllReadyInfo()
        {
            for (int i = 0; i < roomInfo.Players.Count; i++)
            {
                Debug.Log("我进来了索引是"+i);
                showCardUI.GetComponent<NNShowCardComponent>().CreateHead(i,roomInfo.Players[i]);
            }
        }


    }
}