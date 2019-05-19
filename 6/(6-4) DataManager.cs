using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;


public class DataManager : MonoBehaviour
{
    // Playerprefs limitation : only stores int, float, string dta types (no lists or arrays)
    // ex. bool[] levelsUnlocked = new bool[] {true, true, false, false, false };
    // If we keep use playerprefs, we need to convert bool data to string and store them to PlayerPrefs seperately
    // Unsecured, stores data in plain text file.
    //benefit of json
    // object as value - object in turn store more datas key-value pairs.
    // ex ) {"firstname" : "John",
    //        "address" : {
    //        "streetAddress": "21 2nd Street",
    //        "city" : "New York" },
    // ...
    // "phoneNumbers" : [ { "type" : "home", "number" : "212 555-1234" } , { "type" : "office", "number" : "212 555-1234" } ], ...
    // "children" : [], "name"" : null
    // JSONUtility : convert public fields of object into jason format string
    // get,set : get - expose variable from private save data field.

    private SaveData m_saveData;
    private JsonSaver m_Jsonsaver;

    public static DataManager _instance;

    public static DataManager Instance { get { return _instance; } }

    public List<int> score
    {
        get { return m_saveData.m_score; }
        set { m_saveData.m_score = value; }
    }

    public List<bool> unlocked
    {
        get { return m_saveData.m_unlocked; }
        set { m_saveData.m_unlocked = value; }
    }

    public float MasterVolume
    {
        get { return m_saveData.masterVolume; }
        set { m_saveData.masterVolume = value; }
    }

    public float SfxVolume
    {
        get { return m_saveData.sfxVolume; }
        set { m_saveData.sfxVolume = value; }
    }

    public float MusicVolume
    {
        get { return m_saveData.musicVolume; }
        set { m_saveData.musicVolume = value; }
    }

    public string PlayerName
    {
        get { return m_saveData.playerName; }
        set { m_saveData.playerName = value; }
    }


    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;

        }

        // Debug.Log();
        m_saveData = new SaveData(); //external object can't access data directly. 
        m_Jsonsaver = new JsonSaver(); // we can generate new jsonSaver with the new keyword to create new instance. we can do that becaus ewe don't inherited from monobehavior. just plain c# obejct. 
                                       //each piece data from save data, needs a corresponding property here. that's we need to add properties for the datas, as if setting menus see them. 

    }
    public void Save()
    {
        m_Jsonsaver.Save(m_saveData);
    }

    public void Load()
    {
        m_Jsonsaver.Load(m_saveData);
    }
}