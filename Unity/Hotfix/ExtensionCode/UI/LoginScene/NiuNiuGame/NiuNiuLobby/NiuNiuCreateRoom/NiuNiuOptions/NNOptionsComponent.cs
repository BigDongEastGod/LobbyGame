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

    [ObjectSystem]
    public class NNOptionsComponentStartSystem : StartSystem<NNOptionsComponent>
    {
        public override void Start(NNOptionsComponent self)
        {
            self.Start();
        }
    }

    public class NNOptionsComponent : Component
    {
        private GameObject _osbPanel;
        private UI _nnCreateRoom;
        private GameObject nnOptionItemPrefab;

        private GameObject optionShowBoxGrid;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            var nnOptionShowBox = rc.Get<GameObject>("NiuNiuOptions");
            var nnTeShuPaiXingDp = rc.Get<GameObject>("TeShuPaiXingDp");
            _osbPanel = rc.Get<GameObject>("OptionShowBoxPanel");

            var allSelectBtn = rc.Get<GameObject>("AllSelectBtn");
            optionShowBoxGrid = rc.Get<GameObject>("OptionShowBoxGrid");

            nnOptionItemPrefab = rc.Get<GameObject>("NN_OptionItem");

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nnOptionShowBox.GetComponent<Button>(), () => { _osbPanel.SetActive(false); });
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nnTeShuPaiXingDp.GetComponent<Button>(), ShowTspxItem);
        }

        public void Start()
        {
            _nnCreateRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuCreateRoom);
        }

        private void ShowTspxItem()
        {
            _osbPanel.SetActive(true);
//
//                foreach (string itemText in _nnCreateRoom.GetComponent<NiuNiuCRComponent>()._curretNiuNiuRule.ListTeShuPaiXing)
//                {
//                    GameObject gameobject = UnityEngine.Object.Instantiate(nnOptionItemPrefab, optionShowBoxGrid.transform);
//                    gameobject.transform.Find("Text").GetComponent<Text>().text = itemText;
//                }
                
        }
    }
}