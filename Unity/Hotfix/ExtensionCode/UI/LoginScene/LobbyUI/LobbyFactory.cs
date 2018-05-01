using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.Lobby)]
    public class LobbyFactory : AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent, params object[] args)
        {
            var ui =  base.Create(scene, type, parent, args);

            ui.AddComponent<LobbyComponent, object[]>(args);
            
            return ui;
        }

        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);
            
            ui.AddComponent<LobbyComponent>();
            
            return ui;
        }
    }
}