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

        public override void Remove(string type)
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuTspx);
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuGjxx);
            
            base.Remove(type);
        }
    }
}