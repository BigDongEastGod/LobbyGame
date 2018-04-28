using ETModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    
    [ObjectSystem]
    public class NiuNiuMainComponentAwakeSystem : AwakeSystem<NiuNiuMainComponent>
    {
        public override void Awake(NiuNiuMainComponent self)
        {
            self.Awake();
        }
    }
    
    
    
    public class NiuNiuMainComponent:Component
    {
        private Text roomNum;               //房间号码文本
        private long roomId;                //房间号码
        private UI showCardUI;
        private Button sitDownBt;
        
        
       public async void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            
            //房间分线图
            var bg = rc.Get<GameObject>("BgImg");
            //房间号码
            roomNum = rc.Get<GameObject>("roomNum").GetComponent<Text>();
            //庄位信息
            var zhuangWeiTxt  = rc.Get<GameObject>("zhuangWei").GetComponent<Text>();
            //底分信息
            var bottomScoreText  = rc.Get<GameObject>("BottomScoreText").GetComponent<Text>();
            //房间局数
            var roomCountText  = rc.Get<GameObject>("roomCountText").GetComponent<Text>();
           
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
            var startGameBt=rc.Get<GameObject>("StartGameBt");
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
            //时间 电池 wifi
            var timeTxt=rc.Get<GameObject>("TimeTxt");
            var batterySlider=rc.Get<GameObject>("BatterySlider");
            var wiFiImg=rc.Get<GameObject>("WiFiImg");
            var AutomaticFlopToggle=rc.Get<GameObject>("AutomaticFlopToggle");
            
            var response =(RoomInfoResponse) await SceneHelperComponent.Instance.Session.Call(new RoomInfoRequest() {RoomId = 0, Message = -1});
    
            var rules = response.Rules == null ? null : ProtobufHelper.FromBytes<NNChess>(response.Rules);

            roomId = response.RoomId;
            roomNum.text = response.RoomId == 0 ? null : roomId.ToString();
             //zhuangWeiTxt=rules.
            bottomScoreText.text = rules.Score.ToString();
            roomCountText.text = rules.Dish.ToString();
            
            RoomInfoAnnunciateHandler.RoomAction += RoomInfo;
            //获得房间显示卡牌窗口
            showCardUI=Game.Scene.GetComponent<UIComponent>().Create(UIType.NNShowCard, UiLayer.Medium);
            //设置房间人数
            showCardUI.GetComponent<NNShowCardComponent>().roomPeople = 6;
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
        
            

        }

        private void RoomInfo(RoomInfoAnnunciate obj)
        {
            
        }


        private async void GetRoomInfo()
        {
            var response =(RoomInfoResponse) await SceneHelperComponent.Instance.Session.Call(new RoomInfoRequest() {RoomId = roomId, Message = 1});
            if (response.Error==0)
            {
                Debug.Log("成功坐下");
                sitDownBt.gameObject.SetActive(false);
                showCardUI.GetComponent<NNShowCardComponent>().CreateHead(-1);
            }
        }
        
        
       
    }
}