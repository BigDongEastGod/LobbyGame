using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.NNShowCard)]
    public class NnShowCardFactory : AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent,params object[] args)
        {
            var ui = base.Create(scene, type, parent);

            ui.AddComponent<NnShowCardComponent,object[]>(args);

            return ui;
        }
    }
}