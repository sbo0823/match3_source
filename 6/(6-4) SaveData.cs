using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI; move UI elements to UIManager.cs
using UnityEngine.SceneManagement;
using LevelManagement;
using UnityEngine.UI;


// Local Data Location : C:\Users\son\AppData\LocalLow\oki\match3

public class SaveData  
{
    //this class dosen't have to be inherited from Monobehavior, it's just plain data class / c# object.
    public string playerName;
    private readonly string defaultPlayerName = "Player";
 

    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;

    public string hashValue; //store large hexadecimal string use for the hash value

    public int Test;

    public List<int> m_score = new List<int>{ 0, 0, 0, 0, 0 };
    public List<bool> m_unlocked = new List<bool> { true, false, false, false, false };

    public SaveData()
    {
        playerName = defaultPlayerName;
        masterVolume = 0.5f;
        sfxVolume = 0.3f;
        musicVolume = 0.2f;
        hashValue = string.Empty;
        Test = 5;
        List<int> score = m_score;
        List<bool> unlocked = m_unlocked;
    }


}
