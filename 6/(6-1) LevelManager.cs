using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using LevelManagement;


public class LevelManager : MonoBehaviour
{
    public static int numofLevels = 5;
    private DataManager m_dataManager;
    private SoundManager m_soundManager;
    public Button[] button = new Button[numofLevels];
    public Text[] buttonText = new Text[numofLevels];
    public List<int> _score;


    protected void Awake()
    {
     
        m_dataManager = Object.FindObjectOfType<DataManager>();
        m_soundManager = Object.FindObjectOfType<SoundManager>();
    }


    private void Start()
    { 
        GameMenu.Open(); 
        setLevelNum();
        LoadData();
       
    }


    public void setLevelNum()
    { 
        for (int i = 0; i < numofLevels; i++)
        {
            int copy = i;
            buttonText[i].text = (i + 1).ToString();
            //button[i].onClick.AddListener(()=>MoveToLevel(i));
           button[i].onClick.AddListener( () => SceneManager.LoadScene(copy + 3));
       

            if (m_dataManager.unlocked[i] == false)
            {
                button[i].interactable = false;
            }
            else
            {
                button[i].interactable = true;
            }
        }
    }
     
 

    public void LoadData()
    { 
       m_dataManager.Load(); 
    }
      

}
