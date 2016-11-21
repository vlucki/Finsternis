namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;
    using UnityEngine.Events;
    using System.Linq;

    public class MenusHolder : MonoBehaviour
    {
        private List<MenuController> menus;

        public UnityEvent onMenuActive;
        public UnityEvent onMenuClosed;
        public UnityEvent onEveryMenuClosed;

        void Awake()
        {
            this.menus = new List<MenuController>();
            foreach(Transform child in this.transform)
            {
                var menu = child.GetComponent<MenuController>();
                if (menu)
                {
                    this.menus.Add(menu);
                    menu.OnClose.AddListener(MenuClosed);
                    menu.OnBeganOpening.AddListener(onMenuActive.Invoke);
                }
            }
        }


        private void MenuClosed(MenuController menu)
        {
            onMenuClosed.Invoke();
            if (!this.menus.Any(menuInList => (menuInList.IsOpen || menuInList.IsOpening)))
                onEveryMenuClosed.Invoke();
        }
    }
}