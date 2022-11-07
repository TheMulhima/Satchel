using Modding.Menu;
using Modding.Menu.Config;
using Satchel;
using Satchel.BetterMenus;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UEInputField = UnityEngine.UI.InputField;

namespace Satchel.BetterMenus.Elements
{
    public class NumInputField : Element
    {
        public GameObject CurrentField { get; set; }
        public float Value { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public bool WholeNumbers { get; set; }

        public Action<float> StoreValue { get; set; }
        public Func<float> LoadValue { get; set; }

        private Text _nameLabel;
        public Text NameLabel
        {
            get
            {
                _nameLabel ??= CurrentField.Find("TextObj")?.GetComponentInChildren<Text>();
                return _nameLabel;
            }
        }

        private static GameObject _prefab;
        public static GameObject Prefab
        {
            get
            {
                if (_prefab is not null)
                {
                    return _prefab;
                }
                var prefab = new GameObject();
                prefab.AddComponent<RectTransform>().sizeDelta = new Vector2(1000f, 0f);
                var menuSelectable = prefab.AddComponent<MenuSelectable>();
                var inputFieldObj = DefaultControls.CreateInputField(new DefaultControls.Resources());
                inputFieldObj.name = "InputFieldObj";
                inputFieldObj.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(300f, 60f);
                inputFieldObj.RemoveComponent<Image>();
                var field = inputFieldObj.GetComponentInChildren<UEInputField>();
                field.textComponent.fontSize = 40;
                field.textComponent.font = MenuResources.TrajanRegular;
                field.textComponent.color = Color.white;
                field.textComponent.alignment = TextAnchor.MiddleCenter;
                inputFieldObj.transform.SetParent(prefab.transform, true);
                inputFieldObj.transform.position = new Vector3(inputFieldObj.transform.localPosition.x + 415f, inputFieldObj.transform.localPosition.y);

                var underlineObj = new GameObject("UnderlineObj");
                underlineObj.AddComponent<CanvasRenderer>();
                var undLineRect = underlineObj.AddComponent<RectTransform>();
                var undLineImg = underlineObj.AddComponent<Image>();
                undLineImg.color = Color.white;
                undLineRect.sizeDelta = new Vector2(125f, 1f);
                underlineObj.transform.SetParent(prefab.transform, true);
                underlineObj.transform.position = new Vector3(underlineObj.transform.position.x + 415f, underlineObj.transform.position.y - 25f);

                // the text to be displayed next to the input field
                var textObj = DefaultControls.CreateText(new DefaultControls.Resources());
                var text = textObj.GetComponentInChildren<Text>();
                textObj.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(700f, 60f);
                text.color = Color.white;
                text.fontSize = 46;
                text.resizeTextMaxSize = HorizontalOptionStyle.VanillaStyle.LabelTextSize;
                text.font = MenuResources.TrajanBold;
                text.alignment = TextAnchor.MiddleLeft;
                textObj.name = "TextObj";
                textObj.transform.localPosition = new Vector3(textObj.transform.localPosition.x - 150f, textObj.transform.localPosition.y);
                textObj.transform.SetParent(prefab.transform, true);

                // LeftCursor object
                var cursorL = new GameObject("CursorLeft");
                GameObject.DontDestroyOnLoad(cursorL);
                cursorL.transform.SetParent(prefab.transform, false);
                // CanvasRenderer
                cursorL.AddComponent<CanvasRenderer>();
                // RectTransform
                var cursorLRt = cursorL.AddComponent<RectTransform>();
                cursorLRt.sizeDelta = new Vector2(164f, 119f);
                cursorLRt.pivot = new Vector2(0.5f, 0.5f);
                cursorLRt.anchorMin = new Vector2(0f, 0.5f);
                cursorLRt.anchorMax = new Vector2(0f, 0.5f);
                cursorLRt.anchoredPosition = new Vector2(-65f, 0f);
                cursorLRt.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                // Animator
                var cursorLAnimator = cursorL.AddComponent<Animator>();
                cursorLAnimator.runtimeAnimatorController = MenuResources.MenuCursorAnimator;
                cursorLAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                cursorLAnimator.applyRootMotion = false;
                // Image
                cursorL.AddComponent<Image>();
                menuSelectable.leftCursor = cursorLAnimator;

                // RightCursor object
                var cursorR = new GameObject("CursorRight");
                GameObject.DontDestroyOnLoad(cursorR);
                cursorR.transform.SetParent(prefab.transform, false);
                // CanvasRenderer
                cursorR.AddComponent<CanvasRenderer>();
                // RectTransform
                var cursorRRt = cursorR.AddComponent<RectTransform>();
                cursorRRt.sizeDelta = new Vector2(164f, 119f);
                cursorRRt.pivot = new Vector2(0.5f, 0.5f);
                cursorRRt.anchorMin = new Vector2(1f, 0.5f);
                cursorRRt.anchorMax = new Vector2(1f, 0.5f);
                cursorRRt.anchoredPosition = new Vector2(65f, 0f);
                cursorRRt.localScale = new Vector3(-0.4f, 0.4f, 0.4f);
                // Animator
                var cursorRAnimator = cursorR.AddComponent<Animator>();
                cursorRAnimator.runtimeAnimatorController = MenuResources.MenuCursorAnimator;
                cursorRAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                cursorRAnimator.applyRootMotion = false;
                // Image
                cursorR.AddComponent<Image>();
                menuSelectable.rightCursor = cursorRAnimator;

                prefab.transform.position = new Vector3(prefab.transform.position.x, prefab.transform.position.y - 52.5f);
                prefab.SetActive(false);
                _prefab = UnityEngine.Object.Instantiate(prefab);
                return _prefab;
            }
        }
        private UEInputField _field;
        public UEInputField InputField
        {
            get
            {
                _field ??= CurrentField.Find("InputFieldObj")?.GetComponentInChildren<UEInputField>();
                return _field;
            }
        }
        public NumInputField(
            string name,
            Action<float> storeValue,
            Func<float> loadValue,
            float minValue = 0f,
            float maxValue = 9999f,
            bool wholeNumbers = false,
            string Id = "__Usename"
            ) : base(Id, name)
        {
            Name = name;
            WholeNumbers = wholeNumbers;
            StoreValue = storeValue;
            LoadValue = loadValue;
            if (minValue >= 0f && minValue <= 9999f && maxValue >= minValue) MinValue = minValue;
            if (maxValue >= 0f && maxValue <= 9999f && maxValue >= minValue) MaxValue = maxValue;
        }

        private GameObject AddField(GameObject fieldParent)
        {
            CurrentField = UnityEngine.Object.Instantiate(Prefab, fieldParent.transform);
            CurrentField.name = base.Name;
            CurrentField.transform.parent = fieldParent.transform;

            Action<string> updateOnEvent = newValue =>
            {
                float newFloat = 0.0f;
                int newInt = 0;

                if (WholeNumbers)
                {
                    InputField.contentType = UEInputField.ContentType.IntegerNumber;
                    InputField.characterLimit = 4;
                    // same as float
                    // also jank code
                    var isInt = int.TryParse(newValue, out newInt);
                    if (!isInt) return;
                    newFloat = newInt;
                }
                else
                {
                    InputField.contentType = UEInputField.ContentType.DecimalNumber;
                    InputField.characterLimit = 8;
                    // returns from the delegate if inputted thing is not a number. means that it won't save anything if the input is bad
                    var isNum = float.TryParse(newValue, out newFloat);
                    if (!isNum) return;

                }
                // what is done when the input is changed
                if (MinValue <= newFloat && newFloat <= MaxValue)
                {
                    Value = newFloat;
                    StoreValue?.Invoke(newFloat);
                }
            };
            var FieldEvent = new UEInputField.OnChangeEvent();
            FieldEvent.AddListener(updateOnEvent.Invoke);
            InputField.onValueChanged = FieldEvent;

            CurrentField.SetActive(true);

            return CurrentField;
        }
        public override GameObjectRow Create(ContentArea c, Menu Instance, bool AddToList = true)
        {
            _ = Name ?? throw new ArgumentNullException(nameof(Name), "Name cannot be null");
            _ = StoreValue ?? throw new ArgumentNullException(nameof(StoreValue), "StoreValue cannot be null");
            _ = LoadValue ?? throw new ArgumentNullException(nameof(LoadValue), "LoadValue cannot be null");
            // 200 105
            c.AddStaticPanel(Name + "Panel", new RelVector2(new Vector2(1000f, 60f)), out GameObject panel);
            AddField(panel);

            //no idea how you'd cancel this input honestly
            c.NavGraph.AddNavigationNode(InputField);
            if (AddToList)
            {
                Instance.MenuOrder.Add(new GameObjectRow(panel));
            }
            gameObject = panel;
            ((IContainer)Parent).OnBuilt += (_, _) => Update();
            return new GameObjectRow(panel);

        }
        public override void Update()
        {
            NameLabel.text = base.Name;
            Value = LoadValue.Invoke();
            CurrentField.GetComponentInChildren<UEInputField>().text = Convert.ToString(Value);
        }
    }
}
