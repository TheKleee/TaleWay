using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using EZCameraShake;

public enum PlayerDirection
{
    forward,
    backward,
    left,
    right
}

public enum PlayerType
{
    player,
    npc
}

public enum WolfOrPray
{
    pray,
    wolf
}

[RequireComponent(typeof(Rigidbody))]
public class MainPlayer : MonoBehaviour
{
    [HideInInspector] public bool simStarted;    //Check if simulation has started!!!
    [HideInInspector] public bool hasKey;
    [HideInInspector] public bool stopSim;
    [HideInInspector] public RotMap mainMap;
    private Rigidbody rb;

    [Header("Dir ID:")]
    public int dirID;

    [Header("Is Bot:")]
    public PlayerType type;

    [Header("Wolf Or Pray:")]
    public WolfOrPray status;

    [Header("Is Red:")]
    public bool isRed;          //We need to check this because of the key!

    [Header("Starting Direction:")]
    public PlayerDirection playerDir;

    [Header("Set Current Target:")]
    public Target curTarget;

    [Header("Vfx:")]
    public GameObject vfx;

    [Header("Audio Source:")]
    public AudioSource aSource;


    /// <summary>
    /// This is called from another script when map is being created!!!
    /// </summary>
    public void SetThingsUp()
    {
        rb = GetComponent<Rigidbody>();
        if (mainMap == null)
            mainMap = FindObjectOfType<RotMap>();

        curTarget.player = this;
        curTarget.occupied = true;
        Timing.RunCoroutine(_SetFallIntroDelay().CancelWith(gameObject));
        SetPlayerDir();
    }
    IEnumerator<float> _SetFallIntroDelay()
    {
        yield return Timing.WaitForSeconds(1f);
        LeanTween.move(gameObject, curTarget.transform, 1.5f).setEaseOutBounce();
        yield return Timing.WaitForSeconds(.5f);
        aSource.Play();
    }

    public void SetPlayerDir()
    {
        if (!simStarted)
        {
            if (type == PlayerType.player)
            {
                switch (dirID)
                {
                    case 0:
                        transform.localEulerAngles = new Vector3(0, 0, 0);
                        aSource.Play();
                        playerDir = PlayerDirection.forward;
                        rotID = 0;
                        curTarget.nextDir = playerDir;
                        dirID = 1;
                        break;

                    case 1:
                        transform.localEulerAngles = new Vector3(0, 90, 0);
                        aSource.Play();
                        playerDir = PlayerDirection.right;
                        rotID = 1;
                        curTarget.nextDir = playerDir;
                        dirID = 2;
                        break;

                    case 2:
                        transform.localEulerAngles = new Vector3(0, 180, 0);
                        aSource.Play();
                        playerDir = PlayerDirection.backward;
                        rotID = 2;
                        curTarget.nextDir = playerDir;
                        dirID = 3;
                        break;

                    case 3:
                        transform.localEulerAngles = new Vector3(0, 270, 0);
                        aSource.Play();
                        playerDir = PlayerDirection.left;
                        rotID = 3;
                        curTarget.nextDir = playerDir;
                        dirID = 0;
                        break;

                }
            }
            else
            {
                switch (playerDir)
                {
                    case PlayerDirection.forward:
                        curTarget.npcNextDir = playerDir;
                        transform.localEulerAngles = new Vector3(0, 0, 0);
                        break;

                    case PlayerDirection.backward:
                        curTarget.npcNextDir = playerDir;
                        transform.localEulerAngles = new Vector3(0, 180, 0);
                        break;

                    case PlayerDirection.right:
                        curTarget.npcNextDir = playerDir;
                        transform.localEulerAngles = new Vector3(0, 90, 0);
                        break;

                    case PlayerDirection.left:
                        curTarget.npcNextDir = playerDir;
                        transform.localEulerAngles = new Vector3(0, 270, 0);
                        break;
                }
            }
        }
        
    }

    public void RunSimulation()
    {
        //Start the simulation
        simStarted = true;

        curTarget.occupied = false;
        curTarget.NextTarget();

        //Do other stuff here too : D
    }

    public void Walk()
    {
        transform.LookAt(curTarget.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (curTarget.isOffMap)
            Timing.RunCoroutine(_StartFall().CancelWith(gameObject));


        aSource.Play();
        //Make Player Walk to the next target!!!
        Timing.RunCoroutine(_SetYMovement().CancelWith(gameObject));
        LeanTween.moveX(gameObject, curTarget.transform.position.x, .65f);
        LeanTween.moveZ(gameObject, curTarget.transform.position.z, .65f);

        //I honestly don't know >:|
    }

    IEnumerator<float> _SetYMovement()
    {
        LeanTween.moveY(gameObject, curTarget.transform.position.y + .65f, .325f).setEaseInOutSine();
        yield return Timing.WaitForSeconds(.3f);
        LeanTween.moveY(gameObject, curTarget.transform.position.y, .325f).setEaseInOutSine();
    }

    IEnumerator<float> _StartFall()
    {
        yield return Timing.WaitForSeconds(.5f);
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddTorque(transform.right * 3f, ForceMode.Impulse);
        rb.AddForce((transform.forward + transform.up) * 1.5f, ForceMode.Impulse);
        yield return Timing.WaitForSeconds(2f);
        Die();
    }

    #region Status Control:

    public void Die()
    {
        if (type == PlayerType.player)
            mainMap.LvlFailed();

        CameraShaker.Instance.ShakeOnce(4f, 4f, .1f, 1f);
        Instantiate(vfx, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void Win()
    {
        if(type == PlayerType.player)
        {
            mainMap.LvlCleared();
        } else {
            mainMap.LvlFailed();
        }

        if(hasKey)
            Destroy(gameObject);
    }

    #endregion


    private void OnCollisionEnter(Collision goal)
    {
        if(status == WolfOrPray.wolf)
        {
            if (goal.transform.GetComponent<MainPlayer>() != null)
                if (goal.transform.GetComponent<MainPlayer>().status == WolfOrPray.pray)
                {
                    if(type == PlayerType.player)
                        Win();
                    
                    goal.transform.GetComponent<MainPlayer>().Die();
                }
        }
    }

    [HideInInspector] public int rotID; //You need this in PointerController.cs >:C
}
