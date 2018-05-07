using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.NNShowCard)]
    public class NnShowCardFactory : AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            ui.AddComponent<NnShowCardComponent>();
            
            return ui;
        }
    }
}