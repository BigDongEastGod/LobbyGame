using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class LoadingComponentAwakeSystem : AwakeSystem<LoadingComponent>
    {
        public override void Awake(LoadingComponent self)
        {
            self.Awake();
        }
    }
    
    public class LoadingComponent:Component
    {
        public void Awake()
        {
            
        }
    }
}