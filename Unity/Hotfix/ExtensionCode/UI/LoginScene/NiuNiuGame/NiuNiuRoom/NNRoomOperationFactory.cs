using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.NNRoomOperation)]
    public class NNRoomOperationFactory:AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent, params object[] args)
        {
            var ui =  base.Create(scene, type, parent, args);
            
            ui.AddComponent<NNRoomOperationComponent,object[]>(args);
            
            return ui;
        }
    }
}