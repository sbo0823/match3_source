using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; //method to read and wirte from filestreams. load/save/delete data
using System;
using System.Text;
using System.Security.Cryptography; //make hash function available


public class JsonSaver
{
    //whenever you save and load anything, use Application.persistentDataPath
    private static readonly string _filename = "saveData1.sav";

    public static string GetSaveFilename()
    {
        return Application.persistentDataPath + "/" + _filename;
    }

    public void Save(SaveData data)
    {
        data.hashValue = String.Empty; //start hash value as empty string 

        string json = JsonUtility.ToJson(data); //new file stream needed

        data.hashValue = GetSHA256(json);
        json = JsonUtility.ToJson(data); //this will make sure that hash value written to disk properly,.
        // we emptied out hash string before, so we need to make sure that acrully saved as file.
        // once we do that, we have save data in the disk, and storing it as extra hash value associated with it. 


        // Debug.Log("hash string = " + hashString);


        string saveFilename = GetSaveFilename(); //full file name including the path.

        FileStream filestream = new FileStream(saveFilename, FileMode.Create); //create file on disk.

        using (StreamWriter writer = new StreamWriter(filestream)) // streamwriter : temporary object you'll going to write to file. StreamWriter. filestream : tell where we wanna write data
        {
            writer.Write(json); //write on json string.
            //using syntax tells program that we're going to dispose streamwrite once we finish with it.
            // generate json object on disk and automatically opens and close by using - streamwriter.
        } 
    }

    public bool Load(SaveData data) //bool : loaded or not loaded
    {

        //We'll gonna use it to store information once it's loaded. loading the file reads what we have in disks, then replace the values inside saved data object.


        string loadFilename = GetSaveFilename();

        if(File.Exists(loadFilename))
        {
            using (StreamReader reader = new StreamReader(loadFilename))
            {
                string json = reader.ReadToEnd();

                // verify data before override current data. 
                //check hash before reading 
                if (CheckData(json))
                {
                    JsonUtility.FromJsonOverwrite(json, data);
                     Debug.Log("hashes are equal");
                }
                else
                {
                    Debug.Log("JSONSAVER Load : invalid hash. Aborting file read");
                }

                
            }
            return true;
        } 
        return false; //if we didn't find valid file at the bottom of the method...check load was successful. 
    }

    private bool CheckData(string json)
    {
        SaveData tempSaveData = new SaveData(); //make temporary savedata object first to read in the contents in the JSON string.  
        JsonUtility.FromJsonOverwrite(json, tempSaveData);  //Read json data into tempsavedata object using Jsonutility.  
        //  to comapre to hash values by storing this object.

        string oldHash = tempSaveData.hashValue; //we've already saved hash value with this data. Let's extract the string. and store in local variable. "oldHash" 
        tempSaveData.hashValue = String.Empty; //and t hen clear out the data in tempSaveData.hashValue.

        //Now we can run the hashing algorithm again.
        //And remember we're always hashing the data with the hash value elements set to an empty string
        // and let's convert the data into a Jason formatted string string temp Jasen equals Jaison utility to Jaison passing
        //in the temp save data.

        string tempJson = JsonUtility.ToJson(tempSaveData); //Generate Jason representation of the public fields of an object and stor e in local value.
        string newHash = GetSHA256(tempJson); //once we have that temporary string, we recompute the hash and store it in string called "newHash" 

        return (oldHash == newHash); //return true/false 
    }

    public void Delete()
    {
        File.Delete(GetSaveFilename());
    }

    public string GetHexStringFromHash(byte[] hash)
    {
        string hexString = String.Empty;
        foreach (byte b in hash )
        {
            hexString += b.ToString("x2"); //x:hexidecimal string  2:2 digits and makes one big string.
            //byte.ToString : converts the numeric value of the current byte object to its equivalent string representation using the specified culgure specific formatting information.
        }
        return hexString;
    }

    private string GetSHA256(string text) //encode
    {
        byte[] textToBytes = Encoding.UTF8.GetBytes(text); //covert text to array of bytes.
                                                           //we need to convert data to format that functions expect.
                                                           //sha function expects data as not string, as array of bytes.
                                                           //to handle conversion, we use encoding library from system.text UTF-8
        SHA256Managed mySHA256 = new SHA256Managed(); //temporary instance to calculate hash value.

        //resulting hash value also array of bytes.

        byte[] hashValue = mySHA256.ComputeHash(textToBytes); //it's not format friendly saving to disk,
        //we really want to return string not array of bytes.

        return GetHexStringFromHash(hashValue);
        //return hex string

    }

}