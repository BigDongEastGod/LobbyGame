using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class NNOptionsComponentAwakeSystem : AwakeSystem<NNOptionsComponent>
    {
        public override void Awake(NNOptionsComponent self)
        {
            self.Awake();
        }
    }

    public class NNOptionsComponent : Component
    {
        private GameObject _osbPanel;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            var nnOptionShowBox = rc.Get<GameObject>("NiuNiuOptions");
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