using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.LoadingPanel)]
    public class LoadingFactory : AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui =  base.Create(scene, type, parent);

            ui.AddComponent<LoadingComponent>();

            return ui;
        }
    }
}