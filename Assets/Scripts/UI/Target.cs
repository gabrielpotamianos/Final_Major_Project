using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    const string TARGET_HUD = "TargetHUD";
    GameObject TargetHUD;
    Enemy currentTarget;
    // Animator TargetSelectedAnim;
    RaycastHit lastHit;
    public LayerMask TargetableLayers;


    #region Singleton
    static public Target instance;

    private void Awake()
    {
        if(instance!=null)
        {
            Debug.LogError("You have MORE than ONE TARGET instances!");
            return;
        }
        instance = this;


        TargetHUD = GameObject.Find(TARGET_HUD);
    }
    #endregion

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))  //,Mathf.Infinity,1<<LayerMask.NameToLayer("Player")
        {
            if (hit.collider.gameObject.layer == (hit.collider.gameObject.layer | 1 << TargetableLayers.value))
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
            currentTarget.UpdateBar(currentTarget.Health);
        }
        else HideHUD();
    }

    public void ShowHUD()
    {
        TargetHUD.GetComponent<CanvasGroup>().alpha = 1;

    }

    public void HideHUD()
    {
        TargetHUD.GetComponent<CanvasGroup>().alpha = 0;
    }

    //private void SearchInChildren(ref GameObject obj)
    //{
    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        if (transform.GetChild(i).name.Equals(TARGET_SELECTED_GO))
    //        {
    //            obj = transform.GetChild(i).gameObject;
    //            break;
    //        }
    //    }
    //}


}
