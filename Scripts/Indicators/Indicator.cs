using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Indicator : MonoBehaviour
{
    #region Hidden >: D
    private Camera cam;
    #endregion

    [Header("Vfx")]
    public GameObject vfx;


    private void Awake()
    {
        cam = Camera.main;  //Too lazy to set it up manually >:C
    }
    private void Start()
    {
        transform.LookAt(cam.transform);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);


        Timing.RunCoroutine(_DeathDelay().CancelWith(gameObject));
    }


    IEnumerator<float> _DeathDelay()
    {
        yield return Timing.WaitForSeconds(1.5f);
        Instantiate(vfx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
