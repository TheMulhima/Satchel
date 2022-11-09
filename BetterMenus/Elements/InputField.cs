using Modding.Menu;
using Modding.Menu.Config;
using UnityEngine.UI;


namespace Satchel.BetterMenus
{
    /// <summary>
    /// An input field.
    /// </summary>
    public class InputField : Element
    {
        private static GameObject _prefab;
        public static GameObject prefab
        {
            get
            {
                if (_prefab == null)
                {
                    var inputFieldPrefab = new GameObject();
                    GameObject.DontDestroyOnLoad(inputFieldPrefab);
                    var inputFieldPrefabRt = inputFieldPrefab.AddComponent<RectTransform>();
                    inputFieldPrefabRt.sizeDelta = new Vector2(1000f, 60f);

                    var inputFieldObj = DefaultControls.CreateInputField(new UnityEngine.UI.DefaultControls.Resources());
                    inputFieldObj.transform.SetParent(inputFieldPrefab.transform, false);
                    inputFieldObj.name = "InputField";
                    inputFieldObj.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 60f); //to be set on build
                    inputFieldObj.RemoveComponent<Image>();
                    inputFieldObj.transform.localPosition = new Vector3(375f, 0f); //to be set on build

                    var field = inputFieldObj.GetComponent<UnityEngine.UI.InputField>();
                    field.textComponent.font = MenuResources.TrajanRegular;
                    field.textComponent.color = Color.white;
                    field.textComponent.alignment = TextAnchor.MiddleCenter;
                    field.textComponent.text = "";
                    //to be set on build
                    field.textComponent.fontSize = 46;
                    field.contentType = UnityEngine.UI.InputField.ContentType.Alphanumeric;
                    field.characterLimit = 10;

                    var placeHolder = inputFieldObj.Find("Placeholder").GetComponent<Text>();
                    placeHolder.font = MenuResources.TrajanRegular;
                    placeHolder.color = Color.grey;
                    placeHolder.alignment = TextAnchor.MiddleCenter;
                    placeHolder.text = "";
                    placeHolder.fontStyle = FontStyle.Normal;
                    placeHolder.fontSize = 46;

                    var underlineObj = new GameObject("Underline");
                    underlineObj.AddComponent<CanvasRenderer>();
                    var undLineImg = underlineObj.AddComponent<Image>();
                    undLineImg.color = Color.white;
                    underlineObj.transform.SetParent(inputFieldPrefab.transform, false);
                    underlineObj.transform.position += new Vector3(375f, -25f); //to be set on build

                    var undLineRect = underlineObj.GetAddComponent<RectTransform>();
                    undLineRect.sizeDelta = new Vector2(125f, 3f); //to be set on build

                    // Label object
                    var label = DefaultControls.CreateText(new DefaultControls.Resources());
                    var text = label.GetComponentInChildren<Text>();
                    label.GetComponent<RectTransform>().sizeDelta = new Vector2(700f, 60f);
                    text.color = Color.white;
                    text.fontSize = 46;
                    text.resizeTextMaxSize = 46;
                    text.font = MenuResources.TrajanBold;
                    text.alignment = TextAnchor.MiddleLeft;
                    label.name = "Label";
                    label.transform.localPosition += new Vector3(-150f, 0f);
                    label.transform.SetParent(inputFieldPrefab.transform, false);

                    var menuSelectable = inputFieldPrefab.GetAddComponent<MenuSelectable>();
                    menuSelectable.rightCursor = Utils.CreateRegularMenuCursor(inputFieldPrefab, leftSide: false);
                    menuSelectable.leftCursor = Utils.CreateRegularMenuCursor(inputFieldPrefab, leftSide: true);
                    menuSelectable.cancelAction = CancelAction.CustomCancelAction;

                    inputFieldPrefab.SetActive(false);
                    _prefab = inputFieldPrefab;
                }
                return _prefab;
            }
        }
        public GameObject currentField;
        private string _userInput = string.Empty;
        public string userInput
        {
            get => _userInput;
            set
            {
                _userInput = value;
                inputField.text = userInput;
            }
        }

        private Text _label;
        public Text label
        {
            get
            {
                if (_label == null)
                {
                    _label = currentField.Find("Label")?.GetComponent<Text>();
                }

                return _label;
            }
        }

        private Text _placeholderText;
        public Text placeholderText
        {
            get
            {
                if (_placeholderText == null)
                {
                    _placeholderText = currentField.Find("InputField")?.Find("Placeholder")?.GetComponent<Text>();
                }

                return _placeholderText;
            }
        }

        private UnityEngine.UI.InputField _inputField;
        public UnityEngine.UI.InputField inputField
        {
            get
            {
                if (_inputField == null)
                {
                    _inputField = currentField.Find("InputField")?.GetComponent<UnityEngine.UI.InputField>();
                }
                return _inputField;
            }
        }

        private RectTransform _underLine;
        public RectTransform underLine
        {
            get
            {
                if (_underLine == null)
                {
                    _underLine = currentField.Find("Underline").GetComponent<RectTransform>();
                }
                return _underLine;
            }
        }
        /// <summary>
        /// The font size of the text component of the input field.
        /// </summary>
        public int fontSize;

        /// <summary>
        /// The amount of characters that the input field can accept.
        /// </summary>
        public int characterLimit;

        /// <summary>
        /// The text to be displayed when there hasn't been any user input yet.
        /// </summary>
        public string placeholder;

        /// <summary>
        /// The width of the input field.
        /// </summary>
        public float inputBoxWidth;

        /// <summary>
        /// The Action(string) that gets Invoked when the value is changed.
        /// </summary>
        public Action<string> storeValue;

        /// <summary>
        /// The Func(string) to Invoke when loading the current setting.
        /// </summary>
        public Func<string> loadValue;

        /// <summary>
        /// The content type of the input field. Only change this if there is no Blueprint that does what you want.
        /// </summary>
        public UnityEngine.UI.InputField.ContentType contentType = UnityEngine.UI.InputField.ContentType.Alphanumeric;

        /// <summary>
        /// Creates a new InputField.
        /// </summary>
        /// <param name="name">The name to be displayed.</param>
        /// <param name="storeValue">The Action(string) that gets Invoked when the value is changed.</param>
        /// <param name="loadValue">The Func(string) to Invoke when loading the current setting.</param>
        /// <param name="placeholder">The text to be displayed when the user input is empty.</param>
        /// <param name="characterLimit">The amount of characters that the input field can accept.</param>
        /// <param name="fontSize">The font size of the text component of the input field.</param>
        /// <param name="inputBoxWidth">The width of the input box.</param>
        /// <param name="Id">The Id of the element that can be used to search for it.</param>
        public InputField(
            string name,
            Action<string> storeValue,
            Func<string> loadValue,
            string placeholder = "",
            int characterLimit = 10,
            int fontSize = 46,
            int inputBoxWidth = 300,
            string Id = "__Usename"
        ) : base(Id, name)
        {
            Name = name;
            this.storeValue = storeValue;
            this.loadValue = loadValue;
            this.placeholder = placeholder;
            this.characterLimit = characterLimit;
            this.fontSize = fontSize;
            this.inputBoxWidth = inputBoxWidth;
        }

        private GameObject AddField(GameObject fieldParent)
        {
            currentField = UnityEngine.Object.Instantiate(prefab, fieldParent.transform);
            currentField.name = base.Name;
            currentField.transform.parent = fieldParent.transform;

            var FieldEvent = new UnityEngine.UI.InputField.OnChangeEvent();
            FieldEvent.AddListener(storeValue.Invoke);
            inputField.onValueChanged = FieldEvent;

            currentField.SetActive(true);

            return currentField;
        }

        /// <summary>
        /// Creates a GameObjectRow based on the current variables.
        /// </summary>
        /// <param name="c">The ContentArea on which the InputField is created.</param>
        /// <param name="Instance">The current Menu instance.</param>
        /// <param name="AddToList">Should this element be added to the MenuOrder (All non IShadowElements).</param>
        /// <returns>The created GameObjectRow which can be used to add to the corresponding Lists.</returns>
        public override GameObjectRow Create(ContentArea c, Menu Instance, bool AddToList = true)
        {
            _ = Name ?? throw new ArgumentNullException(nameof(Name), "Name cannot be null");
            _ = storeValue ?? throw new ArgumentNullException(nameof(storeValue), "StoreValue cannot be null");
            _ = loadValue ?? throw new ArgumentNullException(nameof(loadValue), "LoadValue cannot be null");
            _ = placeholder ?? throw new ArgumentNullException(nameof(placeholder), "PlaceHolder cannot be null");

            c.AddStaticPanel(Name + "Panel", new RelVector2(new Vector2(1000f, 60f)), out var panel);
            AddField(panel);

            c.NavGraph.AddNavigationNode(inputField);
            currentField.GetComponent<MenuSelectable>().customCancelAction = _ => Instance.CancelAction();

            if (AddToList)
            {
                Instance.MenuOrder.Add(new GameObjectRow(panel));
            }
            gameObject = panel;

            //to set value of fields
            OnBuilt += Update;

            ((IContainer)Parent).OnBuilt += (_, _) =>
            {
                OnBuiltInvoke();
            };
            return new GameObjectRow(panel);

        }


        public override void Update()
        {
            placeholderText.text = placeholder;
            label.text = base.Name;
            userInput = loadValue();

            inputField.contentType = contentType;
            inputField.textComponent.fontSize = fontSize;
            inputField.characterLimit = characterLimit;

            var inputFieldRt = inputField.GetComponent<RectTransform>();

            inputFieldRt.sizeDelta = inputFieldRt.sizeDelta with { x = inputBoxWidth };
            inputFieldRt.transform.localPosition = inputFieldRt.transform.localPosition with { x = 500 - inputBoxWidth * 0.45f };
            underLine.sizeDelta = underLine.sizeDelta with { x = inputBoxWidth * 0.9f };
            underLine.transform.localPosition = underLine.transform.localPosition with { x = 500 - inputBoxWidth * 0.45f };
        }
    }
}