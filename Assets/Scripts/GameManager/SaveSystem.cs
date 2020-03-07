using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    static string path = Application.persistentDataPath + "/data.txt";

    public static void SaveNewCharacterCreated(CharacterInfo characterInfo)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Append);
        CharacterInfo info=characterInfo;
        
        formatter.Serialize(stream, info);
        stream.Close();
    }

    public static CharacterInfo LoadNewCharacter()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            CharacterInfo deserializeInfo = (CharacterInfo)formatter.Deserialize(stream);

            CharacterInfo info = new CharacterInfo(deserializeInfo.race, deserializeInfo.breed);
            stream.Close();
            return info;
        }
        else
        {
            CharacterInfo info = new CharacterInfo();
            return info ;
            throw new System.NullReferenceException("LOAD FILE DO NOT EXIST");

        }


    }
    public static List<CharacterInfo> LoadAllCharacters()
    {
        List<CharacterInfo> InfoArray = new List<CharacterInfo>();
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            while(stream.Length>stream.Position)
            {
                CharacterInfo deserializeInfo = (CharacterInfo)formatter.Deserialize(stream);

                CharacterInfo temporary = new CharacterInfo(deserializeInfo.race, deserializeInfo.breed);

                InfoArray.Add(temporary);

            }
            stream.Close();
            return InfoArray;
        }
        else
        {
            return InfoArray;
            throw new System.NullReferenceException("LOAD FILE DO NOT EXIST");

        }


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
