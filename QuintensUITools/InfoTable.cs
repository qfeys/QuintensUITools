﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuintensUITools
{
    /// <summary>
    /// This class allows for the quick creation of a two collum info panel that can be easely updated.
    /// To use this simply attach it to a UI object (that is big enough to display the text)
    /// 'ExampleText' should be a gameobject with a 'Text' component. This will be used as a template for all the other text.
    /// To set the info, call 'SetInfo' to change all the information, or 'AddInfo' to add to existion information.
    /// Make sure to always call 'Redraw()' after changing the information. This should also dynamically deactivate unused lines.
    /// 'FullRedraw()' can be used to delete all contets and recreate all gameobjects.
    /// </summary>
    public class InfoTable : MonoBehaviour
    {
        List<Tuple<string, string>> info;

        public GameObject ExampleText;

        public void Awake()
        {
            if (ExampleText.GetComponentInChildren<Text>() == null)
                throw new ArgumentException("You did not include a Text component in your example text.");
        }

        public void Start()
        {
            var VLayGr = gameObject.GetComponent<VerticalLayoutGroup>();
            if (VLayGr == null)
                VLayGr = gameObject.AddComponent<VerticalLayoutGroup>();
            VLayGr.childForceExpandHeight = false;
            VLayGr.childForceExpandWidth = false;
        }

        public void Redraw()
        {
            if (transform.childCount == 0)  // No child exist, so create the first line
            {
                GameObject line = new GameObject("Line");
                line.transform.SetParent(transform, false);
                var LayEl = line.AddComponent<LayoutElement>();
                LayEl.minHeight = ExampleText.GetComponent<Text>().fontSize;
                LayEl.preferredHeight = ExampleText.GetComponent<Text>().fontSize * 2;
                LayEl.flexibleWidth = 1;
                LayEl.flexibleHeight = 0;
                var HLayGr = line.AddComponent<HorizontalLayoutGroup>();
                HLayGr.childForceExpandWidth = true;
                HLayGr.childForceExpandHeight = true;
                HLayGr.padding = new RectOffset((int)LayEl.minHeight, (int)LayEl.minHeight, 0, 0);

                GameObject name = Instantiate(ExampleText);
                name.name = "Name";
                name.transform.SetParent(line.transform);
                LayEl = name.AddComponent<LayoutElement>();
                LayEl.flexibleWidth = 1;
                name.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

                GameObject data = Instantiate(ExampleText);
                data.name = "Data";
                data.transform.SetParent(line.transform);
                data.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
                //LayEl = name.AddComponent<LayoutElement>();
                //LayEl.flexibleWidth = 1;
            }
            if (info.Count == 0)
            {
                transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "#####";
                transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "#####";
            }
            int i;
            for (i = 0; i < info.Count; i++)
            {
                if (transform.childCount <= i)  // There are not enough childeren (lines)
                {
                    Transform nLine = Instantiate(transform.GetChild(0).gameObject).transform;
                    nLine.SetParent(transform);
                    nLine.SetAsLastSibling();
                }
                transform.GetChild(i).GetChild(0).GetComponent<Text>().text = info[i].Item1;
                transform.GetChild(i).GetChild(1).GetComponent<Text>().text = info[i].Item2;
            }
            if (i == 0) i++;
            while (transform.GetChild(1).childCount < i)
            {
                transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
                i++;
            }
        }

        public void SetInfo(List<Tuple<string,string>> newInfo)
        {
            info = newInfo;
        }

        public void SetInfo(Tuple<string, string> newInfo)
        {
            info = new List<Tuple<string, string>>();
            info.Add(newInfo);
        }

        public void AddInfo(Tuple<string, string> newInfo)
        {
            info.Add(newInfo);
        }

        public void FullRedraw()
        {
            for(int i = transform.childCount-1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
                transform.GetChild(i).SetParent(null);
            }
            Redraw();
        }
    }
}
