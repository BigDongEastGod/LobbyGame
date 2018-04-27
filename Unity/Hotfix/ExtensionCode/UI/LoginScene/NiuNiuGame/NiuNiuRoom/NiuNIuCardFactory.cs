using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.NiuNIuCard)]
    public class NiuNIuCardFactory:AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            ui.AddComponent<NiuNiuCardComponent>();

            return ui;
        }
    }
}