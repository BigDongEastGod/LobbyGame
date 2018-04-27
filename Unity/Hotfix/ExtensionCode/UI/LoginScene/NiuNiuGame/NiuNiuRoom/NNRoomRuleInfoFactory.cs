using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.NNRoomRuleInfoUIForm)]
    public class NNRoomRuleInfoFactory:AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            ui.AddComponent<NNRoomRuleInfoComponent>();
            
            return ui;
        }
    }
}