﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{

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
