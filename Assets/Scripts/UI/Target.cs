using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    public float MeleeAttackRange;
    public float SphereCastRadius;
    public float SphereCastDistance;
    GameObject TargetHUD;
    Enemy currentTarget;
    // Animator TargetSelectedAnim;
    public LayerMask TargetableLayers;
    public GameObject player;
    public bool targetInRange;


    #region Singleton
    static public Target instance;


    RaycastHit hit;
    Ray ray;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("You have MORE than ONE TARGET instances!");
            return;
        }
        instance = this;

        // player = GameObject.FindGameObjectWithTag("Warrior");

    }
    #endregion

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(CharacterSelection.ChosenCharacter.breed.ToString());

        player.GetComponent<PlayerMovement>().enabled = true;

        switch (CharacterSelection.ChosenCharacter.breed)
        {
            case CharacterInfo.Breed.Mage:
                GameObject.FindObjectOfType<MageCombatSystem>().enabled = true;
                break;
            case CharacterInfo.Breed.Warrior:
                GameObject.FindObjectOfType<WarriorCombatSystem>().enabled = true;
                break;
            case CharacterInfo.Breed.Rogue:
                GameObject.FindObjectOfType<RogueCombatSystem>().enabled = true;
                break;

        }

        player.GetComponent<PlayerData>().enabled = true;
        TargetHUD = GameObject.Find(Constants.TARGET_HUD);

    }

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        {
            if (0 != (1 << hit.collider.gameObject.layer & TargetableLayers.value))
            {
                currentTarget = hit.collider.gameObject.GetComponent<Enemy>();

                //To modify - when implementing for more than just Enemies
                currentTarget.HealthBar = TargetHUD.transform.GetChild(0).gameObject;
            }
            else
                currentTarget = null;
        }

        if (currentTarget && currentTarget.GetComponent<Enemy>().defaultStats.Alive)
        {
            ShowHUD();
            currentTarget.UpdateBar(currentTarget.defaultStats.Health);
        }
        else HideHUD();
    }


    private void FixedUpdate()
    {
        if (currentTarget && player)
        {
            RaycastHit secondHit;
            targetInRange = Physics.Linecast(currentTarget.gameObject.transform.position, player.transform.position, out secondHit);
            if (Vector3.Distance(currentTarget.transform.position, player.transform.position) <= MeleeAttackRange)
                targetInRange = true;
            else
            {
                targetInRange = false;
            }

        }
        else targetInRange = false;
    }

    public Enemy getCurrEnemy()
    {
        if (currentTarget)
            return currentTarget;
        else
        {
            RaycastHit hit;
            //Cast Sphere forward for an enemy
            Physics.SphereCast(player.transform.position + new Vector3(0, 1, 0), SphereCastRadius, player.transform.forward, out hit, SphereCastDistance, TargetableLayers, QueryTriggerInteraction.UseGlobal);
            if (hit.collider)
            {
                currentTarget = hit.collider.gameObject.GetComponent<Enemy>();
                currentTarget.HealthBar = TargetHUD.transform.GetChild(0).gameObject;

                return currentTarget;
            }
            else return null;
        }
    }

    public void ShowHUD()
    {
        TargetHUD.GetComponent<CanvasGroup>().alpha = 1;

    }

    public void HideHUD()
    {
        TargetHUD.GetComponent<CanvasGroup>().alpha = 0;
    }

    public GameObject getCurrTarget()
    {
        if (currentTarget)
            return currentTarget.gameObject;
        else return null;
    }

    public void SetEnemy(Enemy enemy)
    {
        currentTarget = enemy;
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(player.transform.position + (player.transform.forward * SphereCastDistance), SphereCastRadius);
    }

}
