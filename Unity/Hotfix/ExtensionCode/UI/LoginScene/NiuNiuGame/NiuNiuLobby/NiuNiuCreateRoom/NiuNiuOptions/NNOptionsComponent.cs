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
            var nnTeShuPaiXingDp = rc.Get<GameObject>("TeShuPaiXingDp");
            _osbPanel = rc.Get<GameObject>("OptionShowBoxPanel");

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nnOptionShowBox.GetComponent<Button>(), () => { _osbPanel.SetActive(false); });
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nnTeShuPaiXingDp.GetComponent<Button>(), () => { _osbPanel.SetActive(true); });
        }

        public void InitPosAndSize(Vector2 pos)
        {
            Debug.Log(pos);
            Debug.Log(_osbPanel.GetComponent<RectTransform>().anchoredPosition);
            _osbPanel.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
}