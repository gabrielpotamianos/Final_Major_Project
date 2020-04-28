using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float DestoryTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SelfDestroyIn(DestoryTime));
    }
    IEnumerator SelfDestroyIn(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}
