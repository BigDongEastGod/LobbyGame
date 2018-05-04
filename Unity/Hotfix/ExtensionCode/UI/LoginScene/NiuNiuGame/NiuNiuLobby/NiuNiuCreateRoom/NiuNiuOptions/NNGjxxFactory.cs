using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.NiuNiuGjxx)]
    public class NNGjxxFactory : AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            ui.AddComponent<NNGjxxComponent>();

            return ui;
        }
    }
}