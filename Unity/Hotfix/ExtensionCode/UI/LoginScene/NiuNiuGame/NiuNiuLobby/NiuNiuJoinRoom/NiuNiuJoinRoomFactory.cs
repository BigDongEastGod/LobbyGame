using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.NiuNiuJoinRoom)]
    public class NiuNiuJoinRoomFactory:AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui =  base.Create(scene, type, parent);

            ui.AddComponent<NiuNiuJoinRoomComponent>();

            return ui;
        }
    }
}