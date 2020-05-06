using UnityEngine;

public static class SpellChecks
{
    private static bool EnemyOnSight(CharacterData enemy)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(enemy.transform.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

        return onScreen;
    }

    public static bool CheckSpell(bool boolean, string Message)
    {
        if (boolean)
            MessageManager.instance.DisplayMessage(Message);
        else return true;

        return false;
    }

    public static bool CheckSpell(CharacterData enemy, PlayerData playerData, float range, bool SpellAssigned)
    {

        if (enemy == null)
            MessageManager.instance.DisplayMessage(Constants.NO_TARGET_SELECTED);
        else if (!enemy.GetComponent<EnemyCombat>())
            MessageManager.instance.DisplayMessage("You cannot do that!");
        else if (SpellAssigned)
            MessageManager.instance.DisplayMessage("You cannot use that now!");
        else if (!enemy.GetComponent<EnemyCombat>().enemyData.Alive)
            MessageManager.instance.DisplayMessage(Constants.NO_TARGET_SELECTED);
        else if (!EnemyOnSight(enemy))
            MessageManager.instance.DisplayMessage("You must look at the enemy!");
        else if (Vector3.Distance(playerData.transform.position, enemy.transform.position) > range)
            MessageManager.instance.DisplayMessage(Constants.OUT_OF_RANGE);
        else return true;

        return false;
    }

    public static bool CheckSpell(CharacterData enemy, bool cooldown, bool SpellAssigned)
    {
        if (CheckSpell(cooldown, "Spell on cooldown!"))
        {
            if (enemy == null)
                MessageManager.instance.DisplayMessage(Constants.NO_TARGET_SELECTED);
            else if (!enemy.GetComponent<EnemyCombat>())
                MessageManager.instance.DisplayMessage("You cannot do that!");
            else if (SpellAssigned)
                MessageManager.instance.DisplayMessage("You cannot use that now!");
            else if (!enemy.GetComponent<EnemyCombat>().enemyData.Alive)
                MessageManager.instance.DisplayMessage(Constants.NO_TARGET_SELECTED);
            else if (!EnemyOnSight(enemy))
                MessageManager.instance.DisplayMessage("You must look at the enemy!");
            else return true;
        }
        return false;
    }

    public static bool CheckSpell(PlayerData playerData, bool cooldown, float AbilityCost, bool SpellAssigned)
    {
        if (CheckSpell(cooldown, "Spell on cooldown!"))
        {
            if (AbilityCost > playerData.statistics.CurrentSpellResource)
                MessageManager.instance.DisplayMessage("Not enough " + "Rage");
            else if (SpellAssigned)
                MessageManager.instance.DisplayMessage("You cannot use that now!");
            else return true;
        }

        return false;

    }

    public static bool CheckSpell(CharacterData enemy, PlayerData playerData, float range, bool cooldown, bool SpellAssigned)
    {
        if (CheckSpell(enemy, cooldown, SpellAssigned))
        {
            if (Vector3.Distance(playerData.transform.position, enemy.transform.position) > range)
                MessageManager.instance.DisplayMessage(Constants.OUT_OF_RANGE);
            else return true;
        }
        return false;
    }

    public static bool CheckSpell(CharacterData enemy, PlayerData playerData, float range, bool cooldown, float AbilityCost, bool SpellAssigned)
    {
        if (CheckSpell(enemy, playerData, range, cooldown, SpellAssigned))
        {
            if (AbilityCost > playerData.statistics.CurrentSpellResource)
                MessageManager.instance.DisplayMessage("Not enough" + "Ability Resource");
            else return true;
        }
        return false;
    }

    public static bool CheckSpell(CharacterData enemy, PlayerData playerData, bool cooldown, float AbilityCost, bool SpellAssigned)
    {
        if (CheckSpell(enemy, cooldown, SpellAssigned))
        {
            if (AbilityCost > playerData.statistics.CurrentSpellResource)
                MessageManager.instance.DisplayMessage("Not enough" + "Ability Resource");
            else return true;
        }
        return false;
    }

    public static bool CheckSpell(CharacterData enemy, PlayerData playerData, float range, bool cooldown, float AbilityCost,bool SpellAssigned, float ComboPoints = 0)
    {
        if (CheckSpell(enemy, playerData, range, cooldown, AbilityCost, SpellAssigned))
        {
            if (ComboPoints <= 0)
                MessageManager.instance.DisplayMessage("You need combo points to cast that!");
            else return true;
        }
        return false;
    }

    public static bool CheckSpell(CharacterData enemy, PlayerData playerData, float DirectionDotProductTreshold, float PositionDotProductThreshold, float Distance, bool cooldown, float AbilityCost, bool SpellAssigned)
    {
        if (CheckSpell(enemy, playerData, cooldown, AbilityCost, SpellAssigned))
        {
            float DirectionDotProduct = Vector3.Dot(enemy.transform.forward, playerData.transform.forward);
            float PositionDotProduct = Vector3.Dot((enemy.transform.position - playerData.transform.position).normalized, playerData.transform.forward);
            float BackstabDistance = Vector3.Distance(enemy.transform.position, playerData.transform.position);

            if (DirectionDotProduct < DirectionDotProductTreshold)
                MessageManager.instance.DisplayMessage("YOU MUST BE BEHIND THE TARGET!");
            else if (PositionDotProduct < PositionDotProductThreshold)
                MessageManager.instance.DisplayMessage("YOU MUST BE BEHIND THE TARGET!");
            else if (BackstabDistance > Distance)
                MessageManager.instance.DisplayMessage("YOU MUST BE BEHIND THE TARGET!");
            else return true;

        }
        return false;
    }


    public static bool CheckSpell(CharacterData enemy, PlayerData playerData, float DirectionDotProductTreshold, float PositionDotProductThreshold, float Distance, bool cooldown, float AbilityCost, bool Stealth, bool SpellAssigned)
    {
        if (CheckSpell(enemy, playerData, DirectionDotProductTreshold, PositionDotProductThreshold, Distance, cooldown, AbilityCost, SpellAssigned))
        {
            if (!Stealth)
                MessageManager.instance.DisplayMessage("You must be invisible for that!");
            else return true;
        }
        return false;
    }

}