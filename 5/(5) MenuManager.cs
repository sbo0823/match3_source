using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace LevelManagement
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private MainMenu mainMenuPrefab;
        [SerializeField]
        private SettingsMenu settingMenuPrefab;
        [SerializeField]
        private CreditsScreen creditScreenprefab;
        [SerializeField]
        private GameMenu gameMenuPrefab;
        [SerializeField]
        private PauseMenu pauseMenuPrefab;

        [SerializeField] //choose in inspecter
        private Transform m_menuParent;

        private Stack<Menu> _menuStack = new Stack<Menu>(); //PRIVATE INSTANCE _(UNDERBAR)

        private static MenuManager _instance;

        public static MenuManager Instance { get { return _instance; } }



        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                InitializeMenus();
                
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }


        private void InitializeMenus()
        {
            if (m_menuParent == null)
            {
                GameObject menuParentObject = new GameObject("Menus");
                m_menuParent = menuParentObject.transform;
            }

           DontDestroyOnLoad(m_menuParent.gameObject);

            // Object(Menus) don't destroy onload, save so they persist    when level 1 load.

            //  Menu[] menuPrefabs = { mainMenuPrefab, settingMenuPrefab, creditScreenprefab, gameMenuPrefab, pauseMenuPrefab };

            // Store 'type' variable.  
            // [1] System.Type[] 'myType' = this.GetType().Attribute; 
                                                //this object invoke get type.
                                                 //now store our reference to the menu manager type
                                                 //Once we have a type, we can call method GetFields. returns information about each fields and turns it to class fieldInfo.
                                                 //We'll normally use GetFields in conjunction with what we call BindingFlags. enumeration that control how you search through reflection.
                                                 //We need to chain bindingflags together to search for the proper menu fields, and we have an option to bind a fields either. static or non-static...

            BindingFlags myFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;


            //search the field in menumanager only, not the field from inheritives.

            // [2] FieldInfo[] fields = myType.GetFields(myFlags); //We'll return in array of field info object. 

            FieldInfo[] fields = this.GetType().GetFields(myFlags); //We'll return in array of field info object. 


            foreach (FieldInfo field in fields)
            {
                Menu prefab = field.GetValue(this) as Menu; //this specific menu object.(field-this)
                if (prefab != null)
                {
                    Menu menuInstance = Instantiate(prefab, m_menuParent); //set menuInstance with gameobject and transform info of m_menuParent
                    if (prefab != mainMenuPrefab) //exclude menu isn't in menuPrefabs
                    {
                        menuInstance.gameObject.SetActive(false);
                    }
                    else
                    {
                        // open main menu
                        OpenMenu(menuInstance); 
                    }

                }
            }
        }

        public void OpenMenu(Menu menuInstance)
        {
            if (menuInstance == null)
            {
                Debug.LogWarning("MENUMANAGER OpenMenu ERROR: invalid menu");
                return;
            }

            if (_menuStack.Count > 0)
            {
                foreach (Menu menu in _menuStack)
                {
                    menu.gameObject.SetActive(false);
                } //activate only one menu
            }

            menuInstance.gameObject.SetActive(true); //set active selected menuInstance
            _menuStack.Push(menuInstance); // push activated menu to stack (top of the stack)
        }

        public void CloseMenu()
        {
            if (_menuStack.Count == 0)
            {
                Debug.LogWarning("MENUMANAGER CloseMenu ERROR: No menus in stack!");
                return;
            }

            Menu topMenu = _menuStack.Pop(); //Removes and returns the object at the top of the Stack.
            topMenu.gameObject.SetActive(false);

            if (_menuStack.Count > 0)
            {
                Menu nextMenu = _menuStack.Peek(); //Peek : Returns the object at the top of the Stack without removing it.
                nextMenu.gameObject.SetActive(true);
            }
        }
    }
}
