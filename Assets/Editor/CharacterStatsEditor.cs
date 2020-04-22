using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterStats),true)]

public class CharacterStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        //Properties to be excluded from drawing
        string[] propertiesInBaseClass = new string[] { "defaultStats", "AttackPower", "Armour"};
        
        //get an instance of the target
        CharacterStats instance = (CharacterStats)target;

        //Exclude defined properties from drawing
        DrawPropertiesExcluding(serializedObject, propertiesInBaseClass);

        //Get custom properties (Struct default stats)
        var defaultStats = serializedObject.FindProperty("defaultStats");
        
        //Display it
        EditorGUILayout.PropertyField(defaultStats, true);

        //If character is Hostile display additional stats to be set
        if(instance.defaultStats.Hostile)
        {
            var attackStat = serializedObject.FindProperty("AttackPower");
            EditorGUILayout.PropertyField(attackStat, true);
            var Armour = serializedObject.FindProperty("Armour");
            EditorGUILayout.PropertyField(Armour, true);
            //var maxHealth = serializedObject.FindProperty("maxHealth");
            //EditorGUILayout.PropertyField(maxHealth, true);

        }

        //Save Changes
        serializedObject.ApplyModifiedProperties();

        //Update
        EditorApplication.update.Invoke();
    }
}
