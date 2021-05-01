using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Goal : MonoBehaviour
{
    private MainPlayer playerRef;

    [Header("Main Map info:")]
    public RotMap mainMap;

    [Header("Start Target:")]
    public Target startTarget;



    private void OnCollisionEnter(Collision player)
    {
        if(player.transform.GetComponent<MainPlayer>() != null)
        {
            playerRef = player.transform.GetComponent<MainPlayer>();

            if (playerRef.status == WolfOrPray.pray)
            {
                if (playerRef.hasKey)
                    Win();
                else
                    Lose();
            }
            else
                Lose();
        }
    }

    public void Win()
    {
        Timing.RunCoroutine(_HouseAnim().CancelWith(gameObject));
    }

    IEnumerator<float> _HouseAnim()
    {
        playerRef.Win();
        LeanTween.scale(gameObject, new Vector3(1.25f, 1.25f, 1.25f), .45f);
        yield return Timing.WaitForSeconds(.5f);
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), .45f);
        yield return Timing.WaitForSeconds(.5f);
    }
    public void Lose()
    {
        playerRef.Die();
    }
}
