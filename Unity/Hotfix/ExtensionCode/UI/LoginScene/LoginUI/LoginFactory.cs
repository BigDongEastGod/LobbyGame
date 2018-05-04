using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.Login)]
    public class LoginFactory : AUIFactory
    {
        public override UI Create(Scene scene, string type, GameObject parent)
        {
            var ui = base.Create(scene, type, parent);

            Game.Scene.GetComponent<UIComponent>().Create(UIType.RegistPanel, UiLayer.Medium).GameObject.SetActive(false);
            Game.Scene.GetComponent<UIComponent>().Create(UIType.LoginPanel, UiLayer.Medium).GameObject.SetActive(false);
            Game.Scene.GetComponent<UIComponent>().Create(UIType.DialogPanel, UiLayer.Top).GameObject.SetActive(false);

            ui.AddComponent<LoginComponent>();

            return ui;
        }

        public override void Remove(string type)
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.RegistPanel);
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.LoginPanel);
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.DialogPanel);

            base.Remove(type);
        }
    }
}