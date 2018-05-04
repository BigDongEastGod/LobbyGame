using ETModel;

namespace ETHotfix
{
	[Event(EventIdType.InitSceneStart)]
	public class InitSceneStart_CreateLoginUI: AEvent
	{
		public override void Run()
		{
			UI ui = Game.Scene.GetComponent<UIComponent>().Create(UIType.Login, UiLayer.Bottom);
			
			Game.Scene.GetComponent<UIComponent>().Create(UIType.DialogPanel, UiLayer.Top).GameObject.SetActive(false);
			Game.Scene.GetComponent<UIComponent >().Create(UIType.LoadingPanel,UiLayer.TopMost).GameObject.SetActive(false);
		}
	}
}
