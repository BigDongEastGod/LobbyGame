using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.NNRoomOperation)]
    public class NNRoomOperationFactory:AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            ui.AddComponent<NNRoomOperationComponent>();

            return ui;
        }
    }
}