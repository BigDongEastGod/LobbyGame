using ETModel;
using UnityEngine;

namespace ETHotfix.Regist
{
    [UIFactory(UIType.RegistPanel)]
    public class RegistPanelFactory : AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            ui.AddComponent<RegistPanelComponent>();

            return ui;
        }
    }
}