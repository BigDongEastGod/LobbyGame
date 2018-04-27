using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class NNRoomRuleInfoComponentAwakeSystem : AwakeSystem<NNRoomRuleInfoComponent>
    {
        public override void Awake(NNRoomRuleInfoComponent self)
        {
            self.Awake();
        }
    }
    
    public class NNRoomRuleInfoComponent:Component
    {
        public void Awake()
        {
        }
    }
}