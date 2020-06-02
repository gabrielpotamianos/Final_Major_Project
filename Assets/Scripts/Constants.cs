using System.Reflection;
//using UnityEditor;
public static class Constants
{
    #region Scene Init

    public const string SCENE_INIT_SINGLETON = "Scene init script is initialised twice or more!";

    #endregion


    #region Camera Collision

    public const float CAMERA_COLLISION_MAX_DISTANCE = 4.0f;
    public const float CAMERA_COLLISION_MIN_DISTANCE = 1.0f;
    public const float CAMERA_COLLISION_SMOOTH = 10.0f;
    public const float CAMERA_COLLISION_OFFSET = 0.85f;
    #endregion

    #region Character Creation

    public const string CHARACTER_CREATION_MAX_CHAR_MSG = "Character cannot be created! \nPlease delete a character before creating a new one!\n Character will not be saved!";
    public const float CHARACTER_CREATION_MAX_CHAR_TIME = 10;
    public const float CHARACTER_TO_CREATE_ROTATION_SPEED = 2f;
    #endregion

    public const string TARGET_HUD = "TargetInfoPanel";
    public const string OUT_OF_RANGE = "You are out of range";
    public const string NO_TARGET_SELECTED = "Pick a target";
    public const string LOOT_INVENTORY = "Loot Inventory";
    public const string PLAYER_INVENTORY = "Inventory";
    public const string VENDOR_INVENTORY = "Vendor Inventory";
    public const string MESSAGE_PANEL = "MessagePanel";
    public const string SINGLETON_ERROR = " Too Many Instances of ";
    public const string HIT_MISS = "Missed";


    public const float TICK = 2;
    public const float CAMERA_CLAMP_X = 70f;
    public const float CAMERA_SENSITIVITY = 150f;
    public const float CAMERA_MOVEMENT_SPEED = 120f;
    public const float CAMERA_SLIDER_CLAMP_MIN = 0.015f;
    public const float CAMERA_SLIDER_CAMP_MAX = 1f;

    #region Enemy Combat

    public const float ENEMY_COMBAT_TEXT_HEIGHT_POS = 2;
    public const float ENEMY_COMBAT_TEXT_RANGE = 300f;

    public const float ENEMY_COMBAT_LOOTINGBOX_DEPTH = -2;
    public const float ENEMY_COMBAT_LOOTINGBOX_HEIGHT = 1;

    #endregion


    #region Enemy Data

    public const int ENEMY_DATA_PATROL_FREQUENCY_MAX = 15;


    #endregion


    #region Spells

    public const string FIRST_SPELL = "Spell (1)";
    public const string SECOND_SPELL = "Spell (2)";
    public const string THIRD_SPELL = "Spell (3)";
    public const string FORTH_SPELL = "Spell (4)";

    #endregion

    public const int MAXIMUM_CHARACTERS = 4;

    #region Warrior

    public const float WARRIOR_DISCHARGE_RATE = 1.25f;
    public const float WARRIOR_MAX_RAGE = 100;

    public const float CHARGE_ABILITY_TIME = 25;
    public const float CHARGE_COOLDOWN_TIME = 5;
    public const float CHARGE_RANGE = 36;

    #endregion
    //public static void ClearLog()
    //{
    //    var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
    //    var type = assembly.GetType("UnityEditor.LogEntries");
    //    var method = type.GetMethod("Clear");
    //    method.Invoke(new object(), null);
    //}

}
