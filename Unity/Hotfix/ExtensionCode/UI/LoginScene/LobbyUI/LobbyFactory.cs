using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.Lobby)]
    public class LobbyFactory : AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);
            
            ui.AddComponent<LobbyComponent>();
            
            return ui;
        }

    }
}