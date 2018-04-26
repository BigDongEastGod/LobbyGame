using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.NiuNiuCreateRoom)]
    public class NiuNiuCRFactory : AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            ui.AddComponent<NiuNiuCRComponent>();

            return ui;
        }

    }
}