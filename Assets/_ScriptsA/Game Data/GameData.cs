using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


[Serializable]
public class SaveData
{
    public bool[] isActive;
}
public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public SaveData saveData;  
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        DontDestroyOnLoad(this);
        Load();
    }
    
    public void Save()
    {
        //Creata a binary formatter that can read binary files
        BinaryFormatter formatter = new BinaryFormatter();

        //Create a route from the program to the file
        FileStream file = null;
        if (File.Exists(Application.persistentDataPath + "/player.dat"))
            file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
        else
            File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create);


        //Create a copy of the save data
        SaveData data = new SaveData();
        data = saveData;

        //Save the data
        formatter.Serialize(file, data);
        
        //Close the data stream
        file.Close();

        Debug.Log("Saved");
    }
    public void Load()
    {
        //Check if save file is exist
        if (File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
            Debug.Log("Loaded");
        }
        else
        {
            saveData = new SaveData();
            saveData.isActive = new bool[100];
            saveData.isActive[0] = true;
        }
    }
    private void OnDisable()
    {
        Save();
    }
    private void OnApplicationQuit()
    {
        Save();
    }

}
