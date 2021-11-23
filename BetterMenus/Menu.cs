using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Modding;
using Modding.Menu;
using Modding.Menu.Config;
using Satchel.BetterMenus;
using UnityEngine;
using UnityEngine.UI;
namespace Satchel.BetterMenus{
    public class Menu : MenuElement{
        private readonly List<Element> Elements = new();


        #region Fields
        //some private atributes we need because we intent to reorder the menu
        private int Columns = 1;
        private int Index = 0;
        private Menu Instance;
        private RelVector2 ItemAdvance = new RelVector2(new Vector2(0.0f, -105f));
        private AnchoredPosition Start = new AnchoredPosition
        {
            ChildAnchor = new Vector2(0.5f, 1f),
            ParentAnchor = new Vector2(0.5f, 1f),
            Offset = default
        };

        //list that stores the order. max items per column is 2 thats why we use tuple
        internal List<GameObjectPair> MenuOrder = new List<GameObjectPair>();
        internal static GameObject TempObj = new GameObject("SatchelTempObj");
        #endregion

        //not really sure if we need this or not
        /// <summary>
        /// Creates a new Menu instance. Generally, this is not needed.
        /// <para/>Use Menu.Create for creating custom menus instead.
        /// </summary>
        public Menu()
        {
            Parent = null; // menu has no Parent
            gameObject = null; // no go

            Instance = this;
            MenuOrder.Clear();
            ResetPositioners();
        }

        /// <summary>
        /// Creates a new MenuScreen with the provided variables.
        /// </summary>
        /// <param name="Title">The title that should be displayed.</param>
        /// <param name="modListMenu">The MenuScreen to add.</param>
        /// <param name="AllMenuOptions">All the Elements that should be added.</param>
        /// <returns>The created MenuScreen.</returns>
        public MenuScreen GetMenuScreen(string Title, MenuScreen modListMenu, Element[] AllMenuOptions)
        {
            MenuBuilder Menu = Utils.CreateMenuBuilder(Title); //create main screen
            UnityEngine.UI.MenuButton backButton = null; //just so we can use it in scroll bar
            
            //mapi code from IMenuMod
            if (AllMenuOptions.Count() > 5)
            {
                Menu.AddContent(new NullContentLayout(), c => c.AddScrollPaneContent(
                    new ScrollbarConfig
                    {
                        CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                        Navigation = new Navigation
                        {
                            mode = Navigation.Mode.Explicit,
                            selectOnUp = backButton,
                            selectOnDown = backButton
                        },
                        Position = new AnchoredPosition
                        {
                            ChildAnchor = new Vector2(0f, 1f),
                            ParentAnchor = new Vector2(1f, 1f),
                            Offset = new Vector2(-310f, 0f)
                        }
                    },
                    new RelLength(AllMenuOptions.Count() * 105f),
                    RegularGridLayout.CreateVerticalLayout(105f),
                    d => AddModMenuContent(AllMenuOptions, d, modListMenu)
                ));
            }
            else
            {
                Menu.AddContent(
                    RegularGridLayout.CreateVerticalLayout(105f),
                    c => AddModMenuContent(AllMenuOptions, c, modListMenu)
                );
            }

            Menu.AddBackButton(modListMenu, out backButton); // add a back button
            return Menu.Build();
        }

        private void AddModMenuContent(Element[] AllMenuOptions, ContentArea c, MenuScreen modListMenu)
        {
            //go through the list given to us by user
            foreach (var menuOption in AllMenuOptions)
            {
                menuOption.Create(c, modListMenu, Instance);
            }
        }

        private void ResetPositioners()
        {
            ItemAdvance = new RelVector2(new Vector2(0.0f, -105f));
            Start = new AnchoredPosition
            {
                ChildAnchor = new Vector2(0.5f, 1f),
                ParentAnchor = new Vector2(0.5f, 1f),
                Offset = default
            };

            Index = 0;
        }

        //from mapi
        private void ModifyNext(GameObject go)
        {
            if (go.gameObject.activeInHierarchy)
            {
                (Start + ItemAdvance * (Vector2)IndexPos).Reposition(go.gameObject.GetComponent<RectTransform>());
                Index++;
            }
        }

        /// <summary>
        /// Reorders the GameObjectPairs in this instance.
        /// </summary>
        public void Reorder()
        {
            foreach (GameObjectPair pair in Instance.MenuOrder)
            {
                if (pair.RightGo == TempObj)
                {
                    ModifyNext(pair.LeftGo);
                }
                else if (pair.LeftGo != TempObj && pair.RightGo != TempObj)
                {
                    var l = ItemAdvance;
                    l.x = new RelLength(750f);
                    ChangeColumns(2, 0.5f, l, 0.5f);

                    ModifyNext(pair.LeftGo);
                    ModifyNext(pair.RightGo);

                    var k = ItemAdvance;
                    k.x = new RelLength(0f);
                    ChangeColumns(1, 0.25f, k, 0.5f);
                }
            }
            ResetPositioners();
        }

        //from  mapi
        private Vector2Int IndexPos => new Vector2Int(Index % Columns, Index / Columns);

        //from mapi
        private void ChangeColumns(int columns, float originalAnchor = 0.5f, RelVector2? newSize = null, float newAnchor = 0.5f)
        {
            RelVector2 relVector2 = newSize ?? ItemAdvance;
            RelVector2 itemAdvance = ItemAdvance;
            RelLength y = itemAdvance.y * (float)IndexPos.y;
            itemAdvance = ItemAdvance;
            RelLength relLength = itemAdvance.x * (float)Columns * originalAnchor - relVector2.x * (float)columns * newAnchor;
            RelLength x = Start.ChildAnchor.x * relVector2.x + relLength;
            Index = 0;
            Columns = columns;
            Start += new RelVector2(x, y);
            ItemAdvance = relVector2;
        }

        /// <summary>
        /// Gets all the GameObjects in this Menu instance.
        /// </summary>
        /// <returns>The List of GameObjects.</returns>
        public List<GameObject> GetGameObjectList()
        {
            List<GameObject> GoList = new List<GameObject>();
            foreach (var menuOptionGos in MenuOrder)
            {
                if (menuOptionGos.LeftGo != TempObj) GoList.Add(menuOptionGos.LeftGo);
                if (menuOptionGos.RightGo != TempObj) GoList.Add(menuOptionGos.RightGo);
                
            }
            return GoList;
        }
        
        /// <summary>
        /// Generates a new MenuScreen.
        /// </summary>
        /// <param name="Title">The title to be displayed.</param>
        /// <param name="modListMenu">The MenuScreen to add.</param>
        /// <param name="MenuOptions">The Elements to add.</param>
        /// <param name="betterMenuMod">The created Menu. (Can in most cases be assigned to discard.)</param>
        /// <returns>The generated MenuScreen.</returns>
        public static MenuScreen Create(string Title, MenuScreen modListMenu, Element[] MenuOptions, out Menu betterMenuMod)
        {
            betterMenuMod = new Menu();
            return betterMenuMod.GetMenuScreen(Title, modListMenu, MenuOptions); 
        }
        /// <summary>
        /// Generates a new MenuScreen.
        /// </summary>
        /// <param name="Title">The title to be displayed.</param>
        /// <param name="modListMenu">The MenuScreen to add.</param>
        /// <param name="MenuOptions">The Elements to add.</param>
        /// <returns>The generated MenuScreen.</returns>
        public static MenuScreen Create(string Title, MenuScreen modListMenu, Element[] MenuOptions) {
            return new Menu().GetMenuScreen(Title, modListMenu, MenuOptions);
        }
    }
}