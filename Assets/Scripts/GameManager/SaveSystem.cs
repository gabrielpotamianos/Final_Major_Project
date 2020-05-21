using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static string path = Application.persistentDataPath + "/data.zocc";
    static int currCharacters = 0;


    public static void SaveNewCharacterCreated(CharacterInfo characterInfo)
    {
        if (!MaximumCharactersReached())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Append);
            CharacterInfo info = characterInfo;

            string jsonFile = JsonUtility.ToJson(info);

            formatter.Serialize(stream, jsonFile);
            stream.Close();
            currCharacters++;
        }
    }

    public static void SaveAllCharactersOverwrite(List<CharacterInfo> charInfoList)
    {

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        currCharacters = 0;
        for (int i = 0; i < charInfoList.Count; i++)
        {
            CharacterInfo info = charInfoList[i];
            string jsonFile = JsonUtility.ToJson(info);

            formatter.Serialize(stream, jsonFile);
            currCharacters++;

        }
        stream.Close();

    }

    public static void SaveCharacterOverwrite(CharacterInfo charInfo)
    {
        List<CharacterInfo> CharArray = LoadAllCharacters();

        for (int i = 0; i < CharArray.Count; i++)
        {
            if (CharArray[i].IsTheSame(charInfo))
            {
                CharArray[i] = charInfo;
                break;
            }
        }

        SaveAllCharactersOverwrite(CharArray);

    }

    public static CharacterInfo LoadNewCharacter()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            string JsonFile = (string)formatter.Deserialize(stream);


            CharacterInfo info = JsonUtility.FromJson<CharacterInfo>(JsonFile);
            stream.Close();
            return info;
        }
        else
        {
            CharacterInfo info = new CharacterInfo();
            return info;
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
            while (stream.Length > stream.Position)
            {

                string JsonFile = (string)formatter.Deserialize(stream);

                CharacterInfo info = JsonUtility.FromJson<CharacterInfo>(JsonFile);

                InfoArray.Add(info);
                currCharacters++;

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


    public static bool MaximumCharactersReached()
    {
        if (File.Exists(path))
        {
            currCharacters = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            while (stream.Length > stream.Position)
            {
                string jsonFile = (string)formatter.Deserialize(stream);
                currCharacters++;
            }
            stream.Close();
        }
        return currCharacters >= Constants.MAXIMUM_CHARACTERS;
    }

    public static bool HasSave()
    {
        if (File.Exists(path))
            return true;
        return false;
    }




}
