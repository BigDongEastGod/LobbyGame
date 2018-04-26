using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.HeadUIForm)]
    public class HeadUIFormFsctory:AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            ui.AddComponent<HeadUIFormComponent>();
            
            return ui;
        }
    }
}