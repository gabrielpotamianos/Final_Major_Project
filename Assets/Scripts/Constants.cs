using System.Reflection;
//using UnityEditor;
public static class Constants 
{
    public const string TARGET_HUD = "TargetInfoPanel";
    public const string OUT_OF_RANGE = "You are out of range";
    public const string NO_TARGET_SELECTED = "Pick a target";
    public const string LOOT_INVENTORY = "Loot Inventory";
    public const string MESSAGE_PANEL = "MessagePanel";
    public const string MAXIMUM_CHARACTERS_MESSAGE = "Character cannot be created! \nPlease delete a character before creating a new one!\n Character will not be saved!";

    public const int MAXIMUM_CHARACTERS = 4;

    public const float WARRIOR_DISCHARGE_RATE = 1.25f;
    public const float WARRIOR_MAX_RAGE = 100;
    //public static void ClearLog()
    //{
    //    var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
    //    var type = assembly.GetType("UnityEditor.LogEntries");
    //    var method = type.GetMethod("Clear");
    //    method.Invoke(new object(), null);
    //}

}
