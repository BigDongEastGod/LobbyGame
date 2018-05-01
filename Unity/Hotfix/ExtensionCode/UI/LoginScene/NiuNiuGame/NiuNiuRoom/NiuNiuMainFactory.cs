using ETModel;
using UnityEngine;

namespace ETHotfix
{

    [UIFactory(UIType.NiuNiuMain)]
    public class NiuNiuMainFactory:AUIFactory
    {
//        public override UI Create(Scene scene, string type, GameObject parent)
//        {
//            var ui = base.Create(scene, type, parent);
//
//            ui.AddComponent<NiuNiuMainComponent>();
//            
//            return ui;
//        }

        public override UI Create(Scene scene, string type, GameObject parent, params object[] args)
        {
            var ui =  base.Create(scene, type, parent, args);
            
            ui.AddComponent<NiuNiuMainComponent,object[]>(args);
            
            return ui;
        }
    }
}