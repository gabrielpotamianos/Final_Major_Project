using UnityEngine;

public static class SpellChecks
{
    private static bool EnemyOnSight(EnemyCombat enemy)
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

    public static bool CheckSpell(EnemyCombat enemy, PlayerData playerData, float range)
    {
        if (enemy == null || !enemy.enemyData.Alive)
            MessageManager.instance.DisplayMessage(Constants.NO_TARGET_SELECTED);
        else if (!EnemyOnSight(enemy))
            MessageManager.instance.DisplayMessage("You must look at the enemy!");
        else if (Vector3.Distance(playerData.transform.position, enemy.transform.position) > range)
            MessageManager.instance.DisplayMessage(Constants.OUT_OF_RANGE);
        else return true;

        return false;
    }

    public static bool CheckSpell(EnemyCombat enemy, bool cooldown)
    {
        if (CheckSpell(cooldown, "Spell on cooldown!"))
        {
            if (enemy == null || !enemy.enemyData.Alive)
                MessageManager.instance.DisplayMessage(Constants.NO_TARGET_SELECTED);
            else if (!EnemyOnSight(enemy))
                MessageManager.instance.DisplayMessage("You must look at the enemy!");
            else return true;
        }
        return false;
    }

    public static bool CheckSpell(PlayerData playerData, bool cooldown, float AbilityCost)
    {
        if (CheckSpell(cooldown, "Spell on cooldown!"))
        {
            if (AbilityCost > playerData.statistics.CurrentSpellResource)
                MessageManager.instance.DisplayMessage("Not enough " + "Rage");
            else return true;
        }

        return false;

    }

    public static bool CheckSpell(EnemyCombat enemy, PlayerData playerData, float range, bool cooldown)
    {
        if (CheckSpell(enemy, cooldown))
        {
            if (Vector3.Distance(playerData.transform.position, enemy.transform.position) > range)
                MessageManager.instance.DisplayMessage(Constants.OUT_OF_RANGE);
            else return true;
        }
        return false;
    }

    public static bool CheckSpell(EnemyCombat enemy, PlayerData playerData, float range, bool cooldown, float AbilityCost)
    {
        if (CheckSpell(enemy, playerData, range, cooldown))
        {
            if (AbilityCost > playerData.statistics.CurrentSpellResource)
                MessageManager.instance.DisplayMessage("Not enough" + "Ability Resource");
            else return true;
        }
        return false;
    }

    public static bool CheckSpell(EnemyCombat enemy, PlayerData playerData, bool cooldown, float AbilityCost)
    {
        if (CheckSpell(enemy, cooldown))
        {
            if (AbilityCost > playerData.statistics.CurrentSpellResource)
                MessageManager.instance.DisplayMessage("Not enough" + "Ability Resource");
            else return true;
        }
        return false;
    }

    public static bool CheckSpell(EnemyCombat enemy, PlayerData playerData, float range, bool cooldown, float AbilityCost, float ComboPoints = 0)
    {
        if (CheckSpell(enemy, playerData, range, cooldown, AbilityCost))
        {
            if (ComboPoints <= 0)
                MessageManager.instance.DisplayMessage("You need combo points to cast that!");
            else return true;
        }
        return false;
    }

    public static bool CheckSpell(EnemyCombat enemy, PlayerData playerData, float DirectionDotProductTreshold, float PositionDotProductThreshold, float Distance, bool cooldown, float AbilityCost)
    {
        if (CheckSpell(enemy, playerData, cooldown, AbilityCost))
        {
            float DirectionDotProduct = Vector3.Dot(Target.instance.getCurrEnemy().transform.forward, playerData.transform.forward);
            float PositionDotProduct = Vector3.Dot((Target.instance.getCurrEnemy().transform.position - playerData.transform.position).normalized, playerData.transform.forward);
            float BackstabDistance = Vector3.Distance(Target.instance.getCurrEnemy().transform.position, playerData.transform.position);

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


    public static bool CheckSpell(EnemyCombat enemy, PlayerData playerData, float DirectionDotProductTreshold, float PositionDotProductThreshold, float Distance, bool cooldown, float AbilityCost, bool Stealth)
    {
        if (CheckSpell(enemy, playerData, DirectionDotProductTreshold, PositionDotProductThreshold, Distance, cooldown, AbilityCost))
        {
            if (!Stealth)
                MessageManager.instance.DisplayMessage("You must be invisible for that!");
            else return true;
        }
        return false;
    }

}