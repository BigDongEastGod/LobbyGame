using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class NNTspxComponentAwakeSystem : AwakeSystem<NNTspxComponent>
    {
        public override void Awake(NNTspxComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class NNTspxComponentStartSystem : StartSystem<NNTspxComponent>
    {
        public override void Start(NNTspxComponent self)
        {
            self.Start();
        }
    }

    public class NNTspxComponent : Component
    {
        private GameObject _osbPanel;

        private GameObject nnOptionItemPrefab;

        private GameObject optionShowBoxGrid;

        private GameObject nnOptionShowBox;

        // 勾选的选项索引集合
        public List<int> SelectedOptions;

        public UI NNGjxx;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            nnOptionShowBox = rc.Get<GameObject>("NiuNiuTspx");
            var nnTeShuPaiXingDp = rc.Get<GameObject>("TeShuPaiXingDp");
            _osbPanel = rc.Get<GameObject>("OptionShowBoxPanel");

            var allSelectBtn = rc.Get<GameObject>("AllSelectBtn");
            optionShowBoxGrid = rc.Get<GameObject>("OptionShowBoxGrid");

            nnOptionItemPrefab = rc.Get<GameObject>("NN_OptionItem");

            SelectedOptions = new List<int>();

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nnOptionShowBox.GetComponent<Button>(), CloseMask);
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nnTeShuPaiXingDp.GetComponent<Button>(), () =>
            {
                NNGjxx.GetComponent<NNGjxxComponent>().CloseMask();
                _osbPanel.SetActive(true);
                nnOptionShowBox.GetComponent<Image>().raycastTarget = true;
            });

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(allSelectBtn.GetComponent<Button>(), () =>
            {
                if (allSelectBtn.transform.Find("UnSelected").gameObject.activeSelf)
                {
                    allSelectBtn.transform.Find("UnSelected").gameObject.SetActive(false);
                    allSelectBtn.transform.Find("Selected").gameObject.SetActive(true);
                    int itemIndex = 0;
                    foreach (Transform item in optionShowBoxGrid.transform)
                    {
                        item.Find("UnSelected").gameObject.SetActive(false);
                        item.Find("Selected").gameObject.SetActive(true);
                        if (!SelectedOptions.Contains(itemIndex)) SelectedOptions.Add(itemIndex);
                        itemIndex++;
                    }
                }
                else
                {
                    allSelectBtn.transform.Find("Selected").gameObject.SetActive(false);
                    allSelectBtn.transform.Find("UnSelected").gameObject.SetActive(true);
                    int itemIndex = 0;
                    foreach (Transform item in optionShowBoxGrid.transform)
                    {
                        item.Find("UnSelected").gameObject.SetActive(true);
                        item.Find("Selected").gameObject.SetActive(false);
                        if (SelectedOptions.Contains(itemIndex)) SelectedOptions.Remove(itemIndex);
                        itemIndex++;
                    }
                }
            });
        }

        public void CloseMask()
        {
            _osbPanel.SetActive(false);
            nnOptionShowBox.GetComponent<Image>().raycastTarget = false;
        }

        public void Start()
        {
            NNGjxx = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuGjxx);

            for (int i = 0; i < NiuNiuRuleInstance.ZiYouQiangZhuang.ListTeShuPaiXing.Count; i++)
            {
                GameObject gameobject = UnityEngine.Object.Instantiate(nnOptionItemPrefab, optionShowBoxGrid.transform);
                gameobject.transform.Find("Text").GetComponent<Text>().text = NiuNiuRuleInstance.ZiYouQiangZhuang.ListTeShuPaiXing[i];
                gameobject.name = i.ToString();
                _osbPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(1124f, 269f);
                _osbPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 269f / 2 + 0.5f);

                SceneHelperComponent.Instance.MonoEvent.AddButtonClick(gameobject.GetComponent<Button>(), () =>
                {
                    if (gameobject.transform.Find("UnSelected").gameObject.activeSelf)
                    {
                        gameobject.transform.Find("UnSelected").gameObject.SetActive(false);
                        gameobject.transform.Find("Selected").gameObject.SetActive(true);
                        SelectedOptions.Add(Convert.ToInt32(gameobject.name));
                    }
                    else
                    {
                        gameobject.transform.Find("UnSelected").gameObject.SetActive(true);
                        gameobject.transform.Find("Selected").gameObject.SetActive(false);
                        SelectedOptions.Remove(Convert.ToInt32(gameobject.name));
                    }
                });
            }
        }

        public void RefreshShowItem(int ruleCount)
        {
            // 清除额外勾选选项
//            for (int i = 0; i < SelectedOptions.Count; i++)
//            {
//                SelectedOptions.RemoveAt(0);
////                optionShowBoxGrid.transform.Find(i.ToString() + "/UnSelected").gameObject.SetActive(true);
////                optionShowBoxGrid.transform.Find(i.ToString() + "/Selected").gameObject.SetActive(false);
//            }
//
//            foreach (Transform item in optionShowBoxGrid.transform)
//            {
//                item.Find("/UnSelected").gameObject.SetActive(true);
//                item.Find("/Selected").gameObject.SetActive(false);
//            }
            foreach (Transform item in optionShowBoxGrid.transform)
            {
                if (SelectedOptions.Contains(Convert.ToInt32(item.name)))
                    SelectedOptions.Remove(Convert.ToInt32(item.name));
                item.Find("/UnSelected").gameObject.SetActive(true);
                item.Find("/Selected").gameObject.SetActive(false);
            }


            if (ruleCount <= 3)
            {
                _osbPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(1124f, 187f);
                _osbPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 187f / 2 + 0.5f);
            }
            else if (ruleCount > 3 && ruleCount <= 6)
            {
                _osbPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(1124f, 269f);
                _osbPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 269f / 2 + 0.5f);
            }
            else
            {
                _osbPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(1124f, 350f);
                _osbPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 350f / 2 + 0.5f);
            }

            for (int i = 0; i < optionShowBoxGrid.transform.childCount; i++)
            {
                optionShowBoxGrid.transform.GetChild(i).gameObject.SetActive(i < ruleCount);
            }

            foreach (var VARIABLE in SelectedOptions)
            {
                Debug.Log(VARIABLE);
            }
        }
    }
}