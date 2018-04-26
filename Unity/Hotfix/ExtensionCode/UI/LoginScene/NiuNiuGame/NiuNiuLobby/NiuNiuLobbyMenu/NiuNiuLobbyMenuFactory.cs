using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.NiuNiuLobbyMenu)]
    public class NiuNiuLobbyMenuFactory:AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            ui.AddComponent<NiuNiuLobbyMenuComponent>();

            return ui;
        }
    }
}