using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    
    [ObjectSystem]
    public class NNRoomOperationComponentAwakeSystem : AwakeSystem<NNRoomOperationComponent>
    {
        public override void Awake(NNRoomOperationComponent self)
        {
            self.Awake();
        }
    }
    
    
    
    public class NNRoomOperationComponent:Component
    {
        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            
            var exitButton = rc.Get<GameObject>("ExitButton");
            var dissolutionButton = rc.Get<GameObject>("DissolutionButton");
            var settingButton = rc.Get<GameObject>("SettingButton");
            var playback = rc.Get<GameObject>("playback");
            
            //离开房间按照注册
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(exitButton.GetComponent<Button>(), () =>
            {
                
            });
        }
    }
}