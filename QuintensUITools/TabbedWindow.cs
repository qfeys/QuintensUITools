using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuintensUITools
{
    /// <summary>
    /// Appely this on a Unity ui object (like a panel) to transform it into a panel with tabs. Ener the names of the tabs into 'TabNames'
    /// and a prefab of the subwindows for each tab into 'TabContent'.
    /// 'ExampleText' should be a gameobject with a 'Text' component. This will be used as a template for the tab names.
    /// 'TabImageHigh' and 'TabImageLow' are teh background of the tab titles when they are selected and when not.
    /// 'CanBeMinimised' alows for the minimalisation of the window.
    /// </summary>
    class TabbedWindow : MonoBehaviour
    {
        public bool CanBeMinimised;
        public Sprite TabImageLow;
        public Sprite TabImageHigh;
        public GameObject ExampleText;
        public List<string> TabNames;
        public List<GameObject> TabContent;

        List<GameObject> buttons;
        List<GameObject> windows;
        float standardHeight;
        bool isMinimised = false;
        

        public void Awake()
        {
            if (TabNames.Count != TabContent.Count)
                throw new ArgumentException("You gave " + TabNames.Count + " tab names and " + TabContent.Count + " tab contents.");
            if (ExampleText.GetComponentInChildren<Text>() == null)
                throw new ArgumentException("You did not include a Text component in your exampleText.");
            //if (tabPrefab.GetComponentInChildren<Button>() == null)
            //    throw new ArgumentException("You did not include a Button component in your tab prefab.");
            //if (tabPrefab.GetComponentInChildren<LayoutElement>() == null)
            //    throw new ArgumentException("You did not include a LayoutElement component in your tab prefab.");
        }

        public void Start()
        {
            standardHeight = ((RectTransform)gameObject.transform).rect.height;
            var VLayGr = gameObject.GetComponent<VerticalLayoutGroup>();
            if(VLayGr == null)
                VLayGr = gameObject.AddComponent<VerticalLayoutGroup>();
            VLayGr.childForceExpandHeight = false;
            VLayGr.childForceExpandWidth = false;
            GameObject buttonLine = new GameObject("Tab Line");
            buttonLine.transform.parent = transform;
            var LOE = buttonLine.AddComponent<LayoutElement>();
            LOE.minHeight = ExampleText.GetComponent<Text>().fontSize * 3 / 2;
            LOE.flexibleHeight = 0;
            LOE.flexibleWidth = 1;
            var HLayGr = buttonLine.AddComponent<HorizontalLayoutGroup>();
            HLayGr.childForceExpandHeight = false;
            HLayGr.childForceExpandWidth = false;
            GameObject mainWindow = new GameObject("Main Window");
            mainWindow.transform.parent = transform;
            mainWindow.AddComponent<LayoutElement>().flexibleHeight = 1;
            mainWindow.AddComponent<HorizontalLayoutGroup>();

            buttons = new List<GameObject>();
            windows = new List<GameObject>();
            for (int i = 0; i < TabNames.Count; i++)
            {
                GameObject tab = new GameObject("Tab");
                tab.transform.SetParent(buttonLine.transform);
                Image img = tab.AddComponent<Image>();
                img.sprite = TabImageLow;
                img.raycastTarget = true;
                img.type = Image.Type.Sliced;
                img.fillCenter = true;

                GameObject text = Instantiate(ExampleText);
                text.transform.SetParent(tab.transform,false);
                Text t = text.GetComponent<Text>();
                t.text = TabNames[i];
                float width = t.preferredWidth + t.fontSize;
                float height = t.fontSize * 3 / 2;

                int j = i;
                if (CanBeMinimised)
                {
                    tab.AddComponent<Button>().onClick.AddListener(() => { MaximiseWindow(); SetTab(j); });
                }
                else
                {
                    tab.AddComponent<Button>().onClick.AddListener(() => SetTab(j));
                }
                buttons.Add(tab);

                tab.AddComponent<LayoutElement>().flexibleHeight = 1;
                tab.GetComponent<LayoutElement>().preferredWidth = width;

                GameObject window = Instantiate(TabContent[i]);
                window.transform.SetParent(mainWindow.transform);
                window.SetActive(false);
                windows.Add(window);
            }
            if (CanBeMinimised)
            {
                GameObject tab = new GameObject("Tab");
                tab.transform.SetParent(buttonLine.transform);
                Image img = tab.AddComponent<Image>();
                img.sprite = TabImageLow;
                img.raycastTarget = true;
                img.type = Image.Type.Sliced;
                img.fillCenter = true;

                GameObject text = Instantiate(ExampleText);
                text.transform.SetParent(tab.transform, false);
                Text t = text.GetComponent<Text>();
                t.text = "X";
                float width = t.preferredWidth + t.fontSize;
                float height = t.fontSize * 3 / 2;
                
                tab.AddComponent<Button>().onClick.AddListener(() => { SetTab(TabNames.Count); MinimiseWindow(); });
                buttons.Add(tab);

                tab.AddComponent<LayoutElement>().flexibleHeight = 1;
                tab.GetComponent<LayoutElement>().preferredWidth = width;

                GameObject window = new GameObject();
                window.transform.SetParent(mainWindow.transform);
                window.SetActive(false);
                windows.Add(window);
            }
            SetTab(0);
        }

        private void SetTab(int n)
        {
            for (int i = 0; i < TabNames.Count; i++)
            {
                if(i != n)
                {
                    if (windows[i].activeSelf)
                    {
                        windows[i].SetActive(false);
                        buttons[i].GetComponent<Image>().sprite = TabImageLow;
                    }
                }
            }
            if (windows[n].activeSelf == false)
            {
                windows[n].SetActive(true);
                buttons[n].GetComponent<Image>().sprite = TabImageHigh;
            }

        }

        private void MinimiseWindow()
        {
            if(isMinimised == false)
            {
                ((RectTransform)gameObject.transform).sizeDelta = new Vector2(
                    ((RectTransform)gameObject.transform).rect.width,
                    ((RectTransform)transform.GetChild(0).transform).rect.height);
                isMinimised = true;
            }
        }

        private void MaximiseWindow()
        {
            if (isMinimised == true)
            {
                ((RectTransform)gameObject.transform).sizeDelta = new Vector2(
                    ((RectTransform)gameObject.transform).rect.width,
                    standardHeight);
                isMinimised = false;
            }
        }
    }
}
