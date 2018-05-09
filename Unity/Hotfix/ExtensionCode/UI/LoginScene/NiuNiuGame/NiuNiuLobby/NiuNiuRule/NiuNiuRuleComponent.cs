using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{

    [ObjectSystem]
    public class NiuNiuRuleComponentAwakeSystem : AwakeSystem<NiuNiuRuleComponent>
    {
        public override void Awake(NiuNiuRuleComponent self)
        {
            self.Awake();
        }
    }
    
    public class NiuNiuRuleComponent:Component
    {
        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            var nnRulePanel = rc.Get<GameObject>("NiuNiuRulePanel");
            var clostBtn = rc.Get<GameObject>("CloseBtn");
            
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nnRulePanel.GetComponent<Button>(),()=>{nnRulePanel.SetActive(false);});
            
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(clostBtn.GetComponent<Button>(),()=>{nnRulePanel.SetActive(false);});
        }
        
    }
}