using ETModel;
using UnityEngine;

namespace ETHotfix
{
    
    [UIFactory(UIType.NiuNiuOSB)]
    public class NNOSBFactory:AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);
           
            ui.AddComponent<NiuNiuOSBComponent>();
            
            return ui;
        }
    }
}