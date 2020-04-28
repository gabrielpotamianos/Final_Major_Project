using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class Target : MonoBehaviour
{
    public GameObject TargetPrefab;
    GameObject TargetGameObject;
    public float MeleeAttackRange;
    public float SphereCastRadius;
    public float SphereCastDistance;

    public float SphereCastAllRadius;
    GameObject TargetHUD;
    public Enemy currentTarget;
    int currentTargetIndex = 0;


    // Animator TargetSelectedAnim;
    public LayerMask TargetableLayers;
    public GameObject player;
    public bool targetInRange;

    RaycastHit hit;
    Ray ray;

    [SerializeField]
    public List<RaycastHit> ClosestTargets;

    #region Singleton
    static public Target instance;



    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("You have MORE than ONE TARGET instances!");
            return;
        }
        instance = this;
    }
    #endregion

    private void Start()
    {
        ClosestTargets = new List<RaycastHit>();
        player = GameObject.FindGameObjectWithTag(CharacterSelection.ChosenCharacter.breed.ToString());

        player.GetComponent<PlayerMovement>().enabled = true;

        switch (CharacterSelection.ChosenCharacter.breed)
        {
            case CharacterInfo.Breed.Mage:
                GameObject.FindObjectOfType<MageCombatSystem>().enabled = true;
                Destroy(GameObject.FindObjectOfType<RogueCombatSystem>());
                Destroy(GameObject.FindObjectOfType<WarriorCombatSystem>());
                break;
            case CharacterInfo.Breed.Warrior:
                GameObject.FindObjectOfType<WarriorCombatSystem>().enabled = true;
                Destroy(GameObject.FindObjectOfType<RogueCombatSystem>());
                Destroy(GameObject.FindObjectOfType<MageCombatSystem>());
                break;
            case CharacterInfo.Breed.Rogue:
                GameObject.FindObjectOfType<RogueCombatSystem>().enabled = true;
                Destroy(GameObject.FindObjectOfType<WarriorCombatSystem>());
                Destroy(GameObject.FindObjectOfType<MageCombatSystem>());
                break;

        }

        player.GetComponent<PlayerData>().enabled = true;
        TargetHUD = GameObject.Find(Constants.TARGET_HUD);
        TargetGameObject = Instantiate(TargetPrefab, TargetPrefab.transform.position, TargetPrefab.transform.rotation);

        TargetGameObject.SetActive(false);
    }

    private void Update()
    {
        GetAllTargets();
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit,Mathf.Infinity,TargetableLayers))
            {
                if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
                {
                    SelectTarget(hit);
                    ShowHUD();
                }
                else DeselectTarget();
            }
        }

        if (!currentTarget || !currentTarget.Alive)
        {
            DeselectTarget();
            HideHUD();
        }
    }





    public Enemy getCurrEnemy()
    {
        if (currentTarget && currentTarget.Alive)
            return currentTarget;
        else
        {
            //Cast Sphere forward for an enemy
            Physics.SphereCast(player.transform.position + new Vector3(0, 1, 0), SphereCastRadius, player.transform.forward, out hit, SphereCastDistance, TargetableLayers, QueryTriggerInteraction.UseGlobal);
            if (hit.collider.GetComponent<Enemy>())
            {
                SelectTarget(hit);
                return currentTarget;
            }
            else
                return null;
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

    private void SelectTarget(RaycastHit target)
    {
        currentTarget = target.collider.gameObject.GetComponent<Enemy>();

        //To modify - when implementing for more than just Enemies
        currentTarget.HealthBar = TargetHUD.transform.GetChild(0).gameObject;

        TargetGameObject.SetActive(true);
        TargetGameObject.transform.position = new Vector3(currentTarget.transform.position.x, TargetGameObject.transform.position.y, currentTarget.transform.position.z);
        TargetGameObject.transform.parent = currentTarget.transform;

    }

    private void DeselectTarget()
    {
        TargetGameObject.SetActive(false);
        TargetGameObject.transform.parent = null;
        currentTarget = null;

    }

    private void GetAllTargets()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (currentTargetIndex >= ClosestTargets.Count)
            {
                ClosestTargets = Physics.SphereCastAll(player.transform.position, SphereCastAllRadius, player.transform.forward, SphereCastDistance, TargetableLayers, QueryTriggerInteraction.UseGlobal).ToList();

                for (int i = ClosestTargets.Count - 1; i >= 0; i--)
                {
                    Vector3 screenPoint = Camera.main.WorldToViewportPoint(ClosestTargets[i].transform.position);
                    bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                    if (!onScreen) ClosestTargets.Remove(ClosestTargets[i]);
                }


                currentTargetIndex = 0;
            }
            if (ClosestTargets.Count > 0)
            {
                SelectTarget(ClosestTargets[currentTargetIndex]);
                currentTargetIndex++;
            }
        }

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
                                                  
    public bool IsTargetInRange()
    {
        if (currentTarget && player)
        {
            RaycastHit secondHit;
            targetInRange = Physics.Linecast(currentTarget.gameObject.transform.position, player.transform.position, out secondHit);
            if (Vector3.Distance(currentTarget.transform.position, player.transform.position) <= MeleeAttackRange)
                targetInRange = true;
            else
                targetInRange = false;
        }
        else targetInRange = false;


        return targetInRange;
    }

}
