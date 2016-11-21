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

        public UnityEvent onMenuOpened;
        public UnityEvent onMenuClosed;
        public UnityEvent onEveryMenuClosed;

        void Awake()
        {
            this.menus = new List<MenuController>();
            GetComponentsInChildren<MenuController>(this.menus);
            foreach(var menu in this.menus)
            {
                menu.OnOpen.AddListener(MenuOpened);
            }
        }

        private void MenuOpened(MenuController menu)
        {
            onMenuOpened.Invoke();
        }


        private void MenuClosed(MenuController menu)
        {
            onMenuClosed.Invoke();
            if (!this.menus.Any(menuInList => (menuInList.IsOpen || menuInList.IsOpening)))
                onEveryMenuClosed.Invoke();
        }
    }
}