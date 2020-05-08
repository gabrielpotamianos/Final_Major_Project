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

            formatter.Serialize(stream, info);
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

            formatter.Serialize(stream, info);
            currCharacters++;

        }
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
                CharacterInfo deserializeInfo = (CharacterInfo)formatter.Deserialize(stream);
                currCharacters++;
                Debug.Log(currCharacters);
            }
            stream.Close();
        }
        return currCharacters>=Constants.MAXIMUM_CHARACTERS;
    }

    public static bool HasSave()
    {
        if(File.Exists(path))
            return true;
        return false;
    }


}
