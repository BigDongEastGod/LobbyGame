using ETModel;
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
        
       public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            
            //房间分线图
            var bg = rc.Get<GameObject>("BgImg");
            //房间号码
            var roomNum = rc.Get<GameObject>("roomNum");
            //庄位信息
            var zhuangWeiTxt  = rc.Get<GameObject>("zhuangWei");
            //底分信息
            var bottomScoreText  = rc.Get<GameObject>("BottomScoreText");
            //房间局数
            var roomCountText  = rc.Get<GameObject>("roomCountText");
            //当前客户端提示标题
            var mainTitleTxt = rc.Get<GameObject>("mainTitle");
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
            var sitDownBt=rc.Get<GameObject>("SitDownBt");
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
            
        }
    }
}