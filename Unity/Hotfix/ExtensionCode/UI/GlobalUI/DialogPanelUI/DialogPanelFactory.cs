using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.DialogPanel)]
    public class DialogPanelFatory : AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            ui.AddComponent<DialogPanelComponent>();

            return ui;
        }
    }
}