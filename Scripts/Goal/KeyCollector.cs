using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class KeyCollector : MonoBehaviour
{
    //Add some vfx...
    [Header("Vfx:")]
    public GameObject vfx;

    private void OnTriggerEnter(Collider player)
    {
        if(player.GetComponent<MainPlayer>() != null)
        {
            if (player.GetComponent<MainPlayer>().status == WolfOrPray.pray)
            {
                player.GetComponent<MainPlayer>().hasKey = true;
                //Instantiate vfx xD
                Instantiate(vfx, transform.position, Quaternion.identity);
                CameraShaker.Instance.ShakeOnce(2.5f, 2.5f, .1f, 1f);
                Destroy(gameObject);
            }
        }
    }
}
