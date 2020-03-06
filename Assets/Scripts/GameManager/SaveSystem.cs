using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    static string path = Application.persistentDataPath + "/data.xoc";

    public static void SaveNewCharacterCreated(CharacterInSelection.Race race, CharacterInSelection.Class Class)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        CharacterInSelection.Race RACE = race;
        CharacterInSelection.Class CLASS = Class;
        formatter.Serialize(stream, RACE);
        formatter.Serialize(stream, CLASS);
        stream.Close();
    }

    //public static void SavePlayerData(PlayerData data)
    //{
    //    BinaryFormatter formatter = new BinaryFormatter();
    //    string path = Application.persistentDataPath + "/data.xoc";
    //    FileStream stream = new FileStream(path, FileMode.Create);

    //    PlayerData localData = new PlayerData(data);

    //    formatter.Serialize(stream, localData);
    //    stream.Close();

    //}

    //public static PlayerData LoadPlayerData()
    //{
    //    string path = Application.persistentDataPath + "/data.xoc";

    //    if (File.Exists(path))
    //    {
    //        BinaryFormatter formatter = new BinaryFormatter();

    //        FileStream stream = new FileStream(path, FileMode.Open);

    //        PlayerData data = formatter.Deserialize(stream) as PlayerData;
    //        stream.Close();

    //        return data;
    //    }
    //    else
    //    {
    //        Debug.LogError("Save file does not exist!");
    //        return null;
    //    }
    //}
}
