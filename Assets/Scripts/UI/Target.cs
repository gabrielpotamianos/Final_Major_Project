using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    public float RangeDistance;
    GameObject TargetHUD;
    Enemy currentTarget;
    // Animator TargetSelectedAnim;
    RaycastHit lastHit;
    public LayerMask TargetableLayers;
    public GameObject player;
    public bool targetInRange;


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

       // player = GameObject.FindGameObjectWithTag(SelectCharacter.SelectedGameObject);
        player = GameObject.FindGameObjectWithTag("Warrior");

        TargetHUD = GameObject.Find(Constants.TARGET_HUD);
    }
    #endregion

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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

        if (currentTarget)
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
            if (Vector3.Distance(currentTarget.transform.position, player.transform.position) <= RangeDistance)
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
            Debug.LogError("Target Not Been Selected!");
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

    public GameObject getCurrTarget()
    {
        if (currentTarget)
            return currentTarget.gameObject;
        else return null;
    }


}
