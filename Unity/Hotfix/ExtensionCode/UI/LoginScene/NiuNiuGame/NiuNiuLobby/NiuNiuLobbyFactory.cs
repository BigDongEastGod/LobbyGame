using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.NiuNiuLobby)]
    public class NiuNiuLobbyFactory : AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuCreateRoom, UiLayer.Medium).GameObject.SetActive(false);
            Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuJoinRoom, UiLayer.Top).GameObject.SetActive(false);
            Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuLobbyMenu, UiLayer.Top).GameObject.SetActive(false);
            Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuSetting, UiLayer.Top).GameObject.SetActive(false);
            
            ui.AddComponent<NiuNiuLobbyComponent>();
            
            return ui;
        }

        public override void Remove(string type)
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuCreateRoom);
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuJoinRoom);
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuLobbyMenu);
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuGjxx);
            
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuSetting);
            
            base.Remove(type);
        }
    }
}