using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


namespace LevelManagement
{
// Menu Subclasses
// let set menu class as abstract class (i.e. template)
//abstract means the class isn't meant to be standalone object.
// if you want to make a new menu, create a subclass and use that instead.
  
//Curiously recurring template pattern. 
//method inside menu base class.
     
    public abstract class Menu<T>:Menu where T: Menu<T> //limit generic type to same Menu class T:class,struct or interface.
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        protected virtual void Awake() //outside of object can't see the method, but derived classes can. that means settings menu ,credits screen, mainmenu can access to Menu class.
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = (T)this; //casting.
            }
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }

        public static void Open()
        {
            if(MenuManager.Instance != null && Instance != null)
            {
                MenuManager.Instance.OpenMenu(Instance);
            }
        }

        public static void Close()
        {

            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.CloseMenu();
            }
        }
    }

      


       [RequireComponent(typeof(Canvas))]
     public abstract class Menu : MonoBehaviour
    {

        public virtual void OnBackPressed() //need to be modified or re-written by subclass -> add virtual keyword.
        { 
            if(MenuManager.Instance != null)
            {
                MenuManager.Instance.CloseMenu();
            }
        }
    }

}