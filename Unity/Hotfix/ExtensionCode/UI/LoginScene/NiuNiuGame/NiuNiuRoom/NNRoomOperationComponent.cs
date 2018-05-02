using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    
    [ObjectSystem]
    public class NNRoomOperationComponentArgSystem : AwakeSystem<NNRoomOperationComponent,object[]>
    {
        public override void Awake(NNRoomOperationComponent self,object[] args)
        {
            self.Awake(args);
        }
    }
    
    
    
    public class NNRoomOperationComponent:Component
    {
        public void Awake(object[] args)
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            
            var exitButton = rc.Get<GameObject>("ExitButton");
            var dissolutionButton = rc.Get<GameObject>("DissolutionButton");
            var settingButton = rc.Get<GameObject>("SettingButton");
            var playback = rc.Get<GameObject>("playback");
            
            //离开房间按照注册
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(exitButton.GetComponent<Button>(), () =>
                {
                    QuitRoom(Convert.ToInt64(args[0]));
                });
        }

        
        
        private async void QuitRoom(long roomId)
        {
            var quitResponse = (QuitRoomResponse) await SceneHelperComponent.Instance.Session.Call(new QuitRoomRequest(){RoomId = roomId});
            if (quitResponse.Error == 0)
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuLobby, UiLayer.Bottom);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuMain);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.NNShowCard);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.NNRoomOperation);
            }
        }
    }
}