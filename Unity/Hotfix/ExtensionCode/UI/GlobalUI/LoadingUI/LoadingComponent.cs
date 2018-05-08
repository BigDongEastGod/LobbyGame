using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class LoadingComponentAwakeSystem : AwakeSystem<LoadingComponent>
    {
        public override void Awake(LoadingComponent self)
        {
            self.Awake();
        }
    }

    public class LoadingComponent : Component
    {
        private GameObject _text;

        public void Awake()
        {
            var ui = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            _text = ui.Get<GameObject>("Text");
        }

        public void SetText(string message)
        {
            _text.GetComponent<Text>().text = message;
        }

    }
}