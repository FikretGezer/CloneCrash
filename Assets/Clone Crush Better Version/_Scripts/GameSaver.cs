using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class DataSaver{
    public bool[] isActive;
    public int currentPageForLevels = 0;
    public bool isMusicMuted;
    public bool isSFXMuted;
}
public class GameSaver : MonoBehaviour
{
    [SerializeField] private WorldScriptable levelsHolder;
    [SerializeField] private int levelsPerPage = 12;
    public DataSaver dataSaver;
    private string fileName = "/player.dat";

    //DEFAULT MAX LEVEL IS SET TO 100
    private const int maxLevel = 200;

    public static GameSaver Instance;
    private void Awake() {
        if(Instance == null) Instance = this;
        Load();

        // Debug.Log("Length: " + dataSaver.isActive.Length);
    }
    private void Save()
    {
        //Create a route from the program to the file
        var isExist = File.Exists(Application.persistentDataPath + fileName);
        FileStream file = isExist ? File.Open(Application.persistentDataPath + fileName, FileMode.Open) : File.Open(Application.persistentDataPath + fileName, FileMode.Create);

        // Open Current Page for Levels
        int lastActive()
        {
            for (int i = 0; i < dataSaver.isActive.Length; i++)
            {
                if (!dataSaver.isActive[i])
                    return i - 1;
            }
            return 0;
        };
        dataSaver.currentPageForLevels = lastActive() / levelsPerPage;

        //Create a copy of the save data
        DataSaver data = new DataSaver();
        data = dataSaver;

        //Creata a binary formatter that can read or write binary files
        BinaryFormatter formatter = new BinaryFormatter();
        //Save the data
        formatter.Serialize(file, data);

        //Close the data stream
        file.Close();

        // Debug.Log("Saved");
    }
    private void Load()
    {
        //Check if save file is exist
        if (File.Exists(Application.persistentDataPath + fileName))
        {
            //Open file
            FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);

            //Creata a binary formatter that can read or write binary files
            BinaryFormatter formatter = new BinaryFormatter();
            dataSaver = formatter.Deserialize(file) as DataSaver;

            //Close the data stream
            file.Close();

            // Debug.Log("Loaded");
        }
        else
        {
            dataSaver = new DataSaver();
            dataSaver.isActive = new bool[maxLevel];
            dataSaver.isActive[0] = true;
            dataSaver.currentPageForLevels = 0;
            dataSaver.isMusicMuted = false;
            dataSaver.isSFXMuted = false;
        }
    }
    public void IncreaseLevel(int level)
    {
        if(level + 1 < dataSaver.isActive.Length
        && level + 1 < levelsHolder.allLevels.Length
        && levelsHolder.allLevels[level + 1] != null)
        {
            dataSaver.isActive[level + 1] = true;
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
