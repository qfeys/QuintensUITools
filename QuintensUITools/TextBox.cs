﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace QuintensUITools
{
    /// <summary>
    /// Creates a UI element that holds some text. Uses TextRef.
    /// </summary>
    public class TextBox
    {
        public TextRef Text;

        public float Width { get { return text.preferredWidth / SCALING_FACTOR; } }
        const int SCALING_FACTOR = 8;

        GameObject go;
        Text text;

        /// <summary>
        /// The gameobject that contains the text.
        /// WARNING: Do not attach a layout element to this gameobject. If you need one, use a container
        /// as parent for the TextBox.
        /// </summary>
        public GameObject gameObject { get { return go; } }
        public RectTransform transform { get { return go.transform as RectTransform; } }

        const float MOUSE_OVER_DISPLAY_TRESHOLD = 0.0f;

        public TextBox(Transform parent, TextRef Text, int size = 12, TextAnchor allignment = TextAnchor.MiddleLeft, Color? color = null)
        {
            this.Text = Text;
            go = new GameObject(Text, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform tr = (RectTransform)go.transform;
            float anchX = 0; float anchY = 0;
            switch (allignment)
            {
            case TextAnchor.LowerLeft: anchX = 0; anchY = 0; break;
            case TextAnchor.MiddleLeft: anchX = 0; anchY = 0.5f; break;
            case TextAnchor.UpperLeft: anchX = 0; anchY = 1; break;
            case TextAnchor.LowerCenter: anchX = 0.5f; anchY = 0; break;
            case TextAnchor.MiddleCenter: anchX = 0.5f; anchY = 0.5f; break;
            case TextAnchor.UpperCenter: anchX = 0.5f; anchY = 1; break;
            case TextAnchor.LowerRight: anchX = 1; anchY = 0; break;
            case TextAnchor.MiddleRight: anchX = 1; anchY = 0.5f; break;
            case TextAnchor.UpperRight: anchX = 1; anchY = 1; break;
            }
            tr.anchorMin = new Vector2(anchX, anchY);
            tr.anchorMax = new Vector2(anchX, anchY);
            tr.pivot = new Vector2(anchX, anchY);
            tr.anchoredPosition = new Vector2(0, 0);

            if (Text.link == null)      // Just text
            {
                text = go.AddComponent<Text>();
                text.font = Graphics.GetStandardFont();
                text.fontSize = size * SCALING_FACTOR;
                text.alignment = allignment;
                text.horizontalOverflow = HorizontalWrapMode.Overflow;
                text.verticalOverflow = VerticalWrapMode.Overflow;
                TextBoxScript tbs = go.AddComponent<TextBoxScript>();
                tbs.parent = this;
                tbs.hasMouseover = Text.AltText != null;
                text.text = Text;
                text.color = color ?? Graphics.Color_.text;

                tr.localScale = new Vector3(1f / SCALING_FACTOR, 1f / SCALING_FACTOR, 1);
                tr.sizeDelta = new Vector2(text.preferredWidth / SCALING_FACTOR + size, (size + 2)) * SCALING_FACTOR;

            }
            else                        // Make a button
            {
                Image img = go.AddComponent<Image>();
                img.sprite = Graphics.GetSprite("tb_button_bg");
                img.type = Image.Type.Sliced;
                Button but = go.AddComponent<Button>();
                but.onClick.AddListener(() => Text.link());

                GameObject sGo = new GameObject(Text, typeof(RectTransform));
                sGo.transform.SetParent(tr, false);
                RectTransform sTr = (RectTransform)sGo.transform;

                sTr.anchorMin = new Vector2(anchX, anchY);
                sTr.anchorMax = new Vector2(anchX, anchY);
                sTr.pivot = new Vector2(anchX, anchY);
                switch (allignment)
                {
                case TextAnchor.LowerLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.UpperLeft: sTr.anchoredPosition = new Vector2(4, 0); break;
                case TextAnchor.LowerCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.UpperCenter: sTr.anchoredPosition = new Vector2(0, 0); break;
                case TextAnchor.LowerRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.UpperRight: sTr.anchoredPosition = new Vector2(-4, 0); break;
                }
                text = sGo.AddComponent<Text>();
                text.font = Graphics.GetStandardFont();
                text.fontSize = size * SCALING_FACTOR;
                text.alignment = allignment;
                text.horizontalOverflow = HorizontalWrapMode.Overflow;
                text.verticalOverflow = VerticalWrapMode.Overflow;
                TextBoxScript tbs = go.AddComponent<TextBoxScript>();
                tbs.parent = this;
                tbs.hasMouseover = Text.AltText != null;
                text.text = Text;
                text.color = color ?? Graphics.Color_.text;

                tr.sizeDelta = new Vector2(text.preferredWidth / SCALING_FACTOR + size, (size + 2));
                sTr.localScale = new Vector3(1f / SCALING_FACTOR, 1f / SCALING_FACTOR, 1);
                sTr.sizeDelta = new Vector2(text.preferredWidth / SCALING_FACTOR + size, (size + 2));
            }
        }

        public void SetColor(Color col)
        {
            text.color = col;
        }

        private void AddButton(Action act)
        {
            GameObject butGo = new GameObject("button", typeof(RectTransform));
            butGo.transform.SetParent(go.transform);
            ((RectTransform)butGo.transform).anchorMin = new Vector2(0, 0);
            ((RectTransform)butGo.transform).anchorMax = new Vector2(1, 1);
        }

        private void Update()
        {
            if (Text.isChanging)
                text.text = Text;
        }

        public class TextBoxScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            public TextBox parent;
            public bool hasMouseover = false;
            public bool mouseActive = false;
            public float mouseTimeActive = 0;

            private void Update()
            {
                parent.Update();
                if (hasMouseover && mouseActive)
                {
                    mouseTimeActive += Time.unscaledDeltaTime;
                    if (mouseTimeActive >= MOUSE_OVER_DISPLAY_TRESHOLD)
                    {
                        MouseOver.Activate(parent.Text.AltText);
                    }
                }
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                mouseActive = true;
                mouseTimeActive = 0;
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                mouseActive = false;
                mouseTimeActive = 0;
                MouseOver.Deactivate();
            }
        }

        public class InputBox
        {

            GameObject go;
            InputField field;
            Text text;

            string default_;

            public RectTransform transform { get { return go.transform as RectTransform; } }

            public InputBox(Transform parent, string default_ = "", int size = 12, int width = 80, TextAnchor allignment = TextAnchor.MiddleLeft,
                InputField.ContentType contentType = InputField.ContentType.IntegerNumber, Color? color = null)
            {
                this.default_ = default_;
                go = new GameObject("InputBox", typeof(RectTransform));
                go.transform.SetParent(parent, false);
                RectTransform tr = (RectTransform)go.transform;
                float anchX = 0; float anchY = 0;
                switch (allignment)
                {
                case TextAnchor.LowerLeft: anchX = 0; anchY = 0; break;
                case TextAnchor.MiddleLeft: anchX = 0; anchY = 0.5f; break;
                case TextAnchor.UpperLeft: anchX = 0; anchY = 1; break;
                case TextAnchor.LowerCenter: anchX = 0.5f; anchY = 0; break;
                case TextAnchor.MiddleCenter: anchX = 0.5f; anchY = 0.5f; break;
                case TextAnchor.UpperCenter: anchX = 0.5f; anchY = 1; break;
                case TextAnchor.LowerRight: anchX = 1; anchY = 0; break;
                case TextAnchor.MiddleRight: anchX = 1; anchY = 0.5f; break;
                case TextAnchor.UpperRight: anchX = 1; anchY = 1; break;
                }
                tr.anchorMin = new Vector2(anchX, anchY);
                tr.anchorMax = new Vector2(anchX, anchY);
                tr.pivot = new Vector2(anchX, anchY);
                tr.anchoredPosition = new Vector2(0, 0);

                Image image = go.AddComponent<Image>();
                image.sprite = Graphics.GetSprite("input_box");
                image.raycastTarget = true;
                image.type = Image.Type.Sliced;
                image.fillCenter = true;

                field = go.AddComponent<InputField>();

                GameObject textGo = new GameObject("InputBox", typeof(RectTransform));
                textGo.transform.SetParent(tr);
                text = textGo.AddComponent<Text>();
                text.font = Graphics.GetStandardFont();
                text.fontSize = size * SCALING_FACTOR;
                text.alignByGeometry = true;
                switch (allignment)
                {
                case TextAnchor.LowerLeft: case TextAnchor.MiddleLeft: case TextAnchor.UpperLeft: text.alignment = TextAnchor.MiddleLeft; break;
                case TextAnchor.LowerCenter: case TextAnchor.MiddleCenter: case TextAnchor.UpperCenter: text.alignment = TextAnchor.MiddleCenter; break;
                case TextAnchor.LowerRight: case TextAnchor.MiddleRight: case TextAnchor.UpperRight: text.alignment = TextAnchor.MiddleRight; break;
                }
                text.horizontalOverflow = HorizontalWrapMode.Overflow;
                text.verticalOverflow = VerticalWrapMode.Overflow;
                text.color = color ?? Graphics.Color_.text;

                field.textComponent = text;
                field.text = default_;
                field.contentType = contentType;

                tr.sizeDelta = new Vector2(width, (size + 2));

                RectTransform trtx = textGo.transform as RectTransform;
                trtx.localScale = new Vector3(1f / SCALING_FACTOR, 1f / SCALING_FACTOR, 1);
                trtx.sizeDelta = new Vector2(width * SCALING_FACTOR, size + 2);
                switch (allignment)
                {
                case TextAnchor.LowerLeft: case TextAnchor.MiddleLeft: case TextAnchor.UpperLeft: trtx.anchoredPosition = new Vector2(10, 0); ; break;
                case TextAnchor.LowerCenter: case TextAnchor.MiddleCenter: case TextAnchor.UpperCenter: trtx.anchoredPosition = new Vector2(0, 0); ; break;
                case TextAnchor.LowerRight: case TextAnchor.MiddleRight: case TextAnchor.UpperRight: trtx.anchoredPosition = new Vector2(-10, 0); ; break;
                }
            }

            public string PeekValue()
            {
                return field.text;
            }

            public string TakeValue()
            {
                string ret = field.text;
                field.text = default_;
                return ret;
            }
        }
    }

    /// <summary>
    /// Text reference class. This is a container class for all text that has to be send to
    /// the UI system. This can be a simple string, a reference to the localisation file or
    /// a reference to a value somewhere.
    /// You can use a TextRef implicitly as a string.
    /// </summary>
    public class TextRef
    {
        TextRef() { }

        string text;
        string text2nd;

        Func<object> script;
        Func<object> script2nd;

        public Action link { get; private set; }

        enum RefType { direct, localised, reference }
        RefType refType;
        RefType refType2nd;
        public bool isChanging { get { return refType == RefType.reference; } }

        /// <summary>
        /// Create a new text reference that can store a string which can be read from the localisation files.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="localised">Whether or not this value is a key in the localisation files.</param>
        /// <returns></returns>
        public static TextRef Create(string text, bool localised = true)
        {
            TextRef tr = new TextRef();
            tr.text = text;
            if (localised)
                tr.refType = RefType.localised;
            else
                tr.refType = RefType.direct;
            return tr;
        }

        /// <summary>
        /// Create a new text reference that can store a string and an alternative string which can be read
        /// from the localisation files.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="altText">This is mainly used for mouseover text.</param>
        /// <param name="localised">Whether or not this value is a key in the localisation files.</param>
        /// <returns></returns>
        public static TextRef Create(string text, string altText, bool localised = true)
        {
            TextRef tr = new TextRef() {
                text = text,
                text2nd = altText
            };
            if (localised)
                tr.refType = RefType.localised;
            else
                tr.refType = RefType.direct;
            tr.refType2nd = RefType.localised;
            return tr;
        }

        /// <summary>
        /// Create a new text reference that can store a string and an alternative string which can be read
        /// from the localisation files.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="altRef">This is mainly used for mouseover text.</param>
        /// <param name="localised">Whether or not this value is a key in the localisation files.</param>
        /// <returns></returns>
        public static TextRef Create(string text, Func<object> altRef, bool localised = true)
        {
            TextRef tr = new TextRef() {
                text = text,
                script2nd = altRef
            };
            if (localised)
                tr.refType = RefType.localised;
            else
                tr.refType = RefType.direct;
            tr.refType2nd = RefType.reference;
            return tr;
        }

        /// <summary>
        /// Create a new text reference that will remember the reference to a value in
        /// the program. Refer to the object, not to the ToString of the object. The
        /// TextRef object will make sure numbers are properly formatted.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static TextRef Create(Func<object> reference)
        {
            TextRef tr = new TextRef() {
                script = reference,
                refType = RefType.reference
            };
            return tr;
        }

        /// <summary>
        /// Create a new text reference that will remember the reference to a value in
        /// the program. Refer to the object, not to the ToString of the object. The
        /// TextRef object will make sure numbers are properly formatted. This version
        /// of Create can also link an alternative text.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="altText"></param>
        /// <returns></returns>
        public static TextRef Create(Func<object> reference, string altText)
        {
            TextRef tr = new TextRef() {
                script = reference,
                text2nd = altText,
                refType = RefType.reference,
                refType2nd = RefType.localised
            };
            return tr;
        }

        /// <summary>
        /// Create a new text reference that will remember the reference to a value in
        /// the program. Refer to the object, not to the ToString of the object. The
        /// TextRef object will make sure numbers are properly formatted. This version
        /// of Create can also link an alternative text.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="altRef"></param>
        /// <returns></returns>
        public static TextRef Create(Func<object> reference, Func<object> altRef)
        {
            TextRef tr = new TextRef() {
                script = reference,
                script2nd = altRef,
                refType = RefType.reference,
                refType2nd = RefType.reference
            };
            return tr;
        }

        public TextRef AddLink(Action link)
        {
            this.link = link;
            return this;
        }

        public static implicit operator string(TextRef tr)
        {
            switch (tr.refType)
            {
            case RefType.direct: return tr.text;
            case RefType.localised: return Localisation.GetText(tr.text);
            case RefType.reference: return tr.ExtractData();
            }
            throw new Exception("There exist another text ref type?");
        }

        public static implicit operator TextRef(string st)
        {
            return Create(st);
        }

        public static implicit operator TextRef(double d)
        {
            return Create(ToSI(d));
        }

        private string ExtractData(bool alt = false)
        {
            object d;
            if (alt)
                d = script2nd();
            else
                d = script();
            if (d == null) return "INVALID";
            Type t = d.GetType();
            if (t == typeof(double))
                return ToSI((double)d);
            else if (t == typeof(float))
                return ToSI((float)d);
            else if (t == typeof(int))
                return ToSI((int)d);
            else if (t == typeof(long))
                return ToSI((long)d);
            else
                return d.ToString();
        }

        public string Text { get { return this; } }

        public string AltText
        {
            get
            {
                if (text2nd == null && script2nd == null) return null;
                switch (refType2nd)
                {
                case RefType.direct: return text2nd;
                case RefType.localised: return Localisation.GetText(text2nd);
                case RefType.reference: return ExtractData(true);
                }
                throw new Exception("There exist another text ref type?");
            }
        }


        /// <summary>
        /// Found on stackoverflow
        /// https://stackoverflow.com/questions/12181024/formatting-a-number-with-a-metric-prefix
        /// </summary>
        /// <param name="d"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        static public string ToSI(double d, string format = null)
        {
            if (d == 0 || (d >= 0.1 && d < 10000)) return d.ToString(format ?? (d < 10 ? "0.##" : "0.#"));

            char[] incPrefixes = new[] { 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' };
            char[] decPrefixes = new[] { 'm', '\u03bc', 'n', 'p', 'f', 'a', 'z', 'y' };

            int degree = (int)Math.Floor(Math.Log10(Math.Abs(d)) / 3);
            double scaled = d * Math.Pow(1000, -degree);

            if (degree - 1 >= incPrefixes.Length) return "~inf";
            if (-degree - 1 >= decPrefixes.Length) return "~0";
            char? prefix = null;
            switch (Math.Sign(degree))
            {
            case 1: prefix = incPrefixes[degree - 1]; break;
            case -1: prefix = decPrefixes[-degree - 1]; break;
            }

            if(format == null)
            {
                format = scaled < 10 ? "0.##" : "0.#";
            }

            return scaled.ToString(format) + prefix;
        }
    }
}
