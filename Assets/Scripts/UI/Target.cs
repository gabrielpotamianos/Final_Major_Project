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
    public CharacterData currentTarget;
    int currentTargetIndex = 0;


    // Animator TargetSelectedAnim;
    public LayerMask TargetableLayers;
    public GameObject player;
    public bool targetInRange;

    RaycastHit hit;
    Ray ray;

    [SerializeField]
    public List<RaycastHit> ClosestTargets;

    RawImage EnemyAvatar;

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

        //THINK OF IT
        GameObject.Find("PlayerInfoPanel").transform.GetChild(2).GetComponent<RawImage>().texture = player.GetComponentInChildren<Camera>().targetTexture;




        player.GetComponent<PlayerMovement>().enabled = true;


        player.GetComponent<PlayerData>().enabled = true;
        TargetHUD = GameObject.Find(Constants.TARGET_HUD);
        TargetGameObject = Instantiate(TargetPrefab, TargetPrefab.transform.position, TargetPrefab.transform.rotation);

        TargetGameObject.SetActive(false);
        EnemyAvatar = TargetHUD.transform.GetChild(2).GetComponent<RawImage>();
        HideHUD();

        //THINK OF IT
        //  StartCoroutine(StopPlayerCameraAvatar());
    }


    //THINK OF IT
    IEnumerator StopPlayerCameraAvatar()
    {
        yield return new WaitForSeconds(3);
        player.GetComponentInChildren<Camera>().enabled = false;

    }




    private void Update()
    {

        GetAllTargets();
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, TargetableLayers))
            {
                //SelectTarget(hit);

                if (TargetableLayers == (TargetableLayers.value | 1 << hit.collider.gameObject.layer))
                {
                    DeselectTarget();
                    SelectTarget(hit);
                    ShowHUD();
                }
            }
            else
            {
                DeselectTarget();
                HideHUD();
            }
        }

        if (currentTarget && !currentTarget.Alive)
        {
            DeselectTarget();
            HideHUD();
        }
    }





    public EnemyCombat getCurrEnemy()
    {
        if (currentTarget && !currentTarget.GetComponent<EnemyCombat>())
        {
            MessageManager.instance.DisplayMessage("You cannot do that!");
            return null;
        }
        if (currentTarget.GetComponent<EnemyCombat>() && currentTarget.Alive)
            return currentTarget.GetComponent<EnemyCombat>();
        else
        {
            //Cast Sphere forward for an enemy
            Physics.SphereCast(player.transform.position + new Vector3(0, 1, 0), SphereCastRadius, player.transform.forward, out hit, SphereCastDistance, TargetableLayers, QueryTriggerInteraction.UseGlobal);
            if (hit.collider.GetComponent<EnemyCombat>())
            {
                SelectTarget(hit);
                return hit.collider.GetComponent<EnemyCombat>();
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
        if (target.collider.GetComponent<CharacterData>())
        {
            if (currentTarget)
                DeselectTarget();
            currentTarget = GetTarget(target);
            EnableAvatarCamera();
            EnemyAvatar.texture = currentTarget.GetComponentInChildren<Camera>().targetTexture;

            //THINK OF IT
            // currentTarget.GetComponentInChildren<Camera>().enabled = false;


            //To modify - when implementing for more than just Enemies
            currentTarget.SetHealthBar(TargetHUD.transform.GetChild(0).gameObject.GetComponent<Slider>());

            TargetGameObject.SetActive(true);
            TargetGameObject.transform.position = new Vector3(currentTarget.transform.position.x, TargetGameObject.transform.position.y, currentTarget.transform.position.z);
            TargetGameObject.transform.parent = currentTarget.transform;
        }

    }


    private void DeselectTarget()
    {
        DisableAvatarCamera();
        TargetGameObject.SetActive(false);
        TargetGameObject.transform.parent = null;
        if (currentTarget)
        {
            currentTarget.SetHealthBar(null);
            currentTarget = null;
        }
        EnemyAvatar.texture = null;

    }

    public CharacterData GetTarget(RaycastHit target)
    {
        if (target.collider.GetComponent<EnemyData>())
            return target.collider.GetComponent<EnemyData>();

        return target.collider.GetComponent<CharacterData>();
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

    public CharacterData GetCurrentTarget()
    {
        return currentTarget;
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

    private void DisableAvatarCamera()
    {
        if (currentTarget)
            currentTarget.GetComponentInChildren<Camera>().enabled = false;
    }

    private void EnableAvatarCamera()
    {
        if (currentTarget)
            currentTarget.GetComponentInChildren<Camera>().enabled = true;
    }


}
