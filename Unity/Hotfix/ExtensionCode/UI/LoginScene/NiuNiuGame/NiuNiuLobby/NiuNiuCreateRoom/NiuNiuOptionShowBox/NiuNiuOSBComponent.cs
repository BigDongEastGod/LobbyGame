using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class NiuNiuOSBComponentAwakeSystem : AwakeSystem<NiuNiuOSBComponent>
    {
        public override void Awake(NiuNiuOSBComponent self)
        {
            self.Awake();
        }
    }

    public class NiuNiuOSBComponent : Component
    {
        private GameObject _osbPanel;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            var nnOptionShowBox = rc.Get<GameObject>("NiuNiuOptionShowBox");
            _osbPanel = rc.Get<GameObject>("OptionShowBoxPanel");

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nnOptionShowBox.GetComponent<Button>(), () => { nnOptionShowBox.SetActive(false); });
        }

        public void InitPosAndSize(Vector2 pos)
        {
            Debug.Log(pos);
            Debug.Log(_osbPanel.GetComponent<RectTransform>().anchoredPosition);
            _osbPanel.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
}