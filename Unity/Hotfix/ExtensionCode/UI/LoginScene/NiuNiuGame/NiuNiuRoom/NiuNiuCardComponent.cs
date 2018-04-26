using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    
    [ObjectSystem]
    public class NiuNiuCardComponentAwakeSystem : AwakeSystem<NiuNiuCardComponent>
    {
        public override void Awake(NiuNiuCardComponent self)
        {
            self.Awake();
        }
    }
    
    public class NiuNiuCardComponent:Component
    {
        private Image jokerImg;
        
        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            
            jokerImg = rc.Get<GameObject>("jokerImg").GetComponent<Image>();
        }
    }
}