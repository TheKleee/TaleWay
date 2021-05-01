using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using EZCameraShake;

public enum ChangeDirection
{
    NoChange,

    forwardToRight,
    forwardToLeft,

    backwardToRight,
    backwardToLeft
}

public enum PlayerChangeDir
{
    NoChange,

    Forward,
    Backward,
    Right,
    Left
}

public enum HighGround
{
    min,
    med,
    max
}
public class Target : MonoBehaviour
{
    [HideInInspector] public bool occupied;   //If not occupied you can set the movement direction!
    [HideInInspector] public PlayerChangeDir changeDir;
    [HideInInspector] public PlayerDirection nextDir;
    private Target setNextTarget;
    private Target setNextTargetNpc;
    [Header("Dir ID:")]
    public int dirID;

    [Header("Set Npc Movement:")]
    public ChangeDirection npcChangeDir;
    [HideInInspector] public PlayerDirection npcNextDir;

    [Header("I Have The High Ground:")]
    public HighGround hGround;

    [Header("--- List Of Targets: --- F, R, B, L")]
    public Target[] targetList = new Target[4];
    private Target[] npcTargetList = new Target[4];

    [Header("Is Off Map")]
    public bool isOffMap;

    [Header("Pointer Model:")]
    public GameObject setPointer;

    [Header("Player Ref:")]
    public MainPlayer player;

    [Header("Ground Objects:")]
    public GameObject[] ground = new GameObject[3];

    [Header("Audio Source:")]
    public AudioSource aSource;



    private void Awake()
    {
        if(!isOffMap)
            SetStartGround();
    }
    private void Start()
    {
        npcTargetList = targetList;
    }

    public void SetStartGround()
    {
        switch (hGround)
        {
            case HighGround.min:
                transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
                GameObject min = Instantiate(ground[0], transform.position, Quaternion.identity);
                min.transform.SetParent(transform);
                break;

            case HighGround.med:
                transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                GameObject med = Instantiate(ground[1], transform.position, Quaternion.identity);
                med.transform.SetParent(transform);
                break;

            case HighGround.max:
                transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
                GameObject max = Instantiate(ground[2], transform.position, Quaternion.identity);
                max.transform.SetParent(transform);
                break;
        }
    }

    public void ChangePointer()
    {
        if (!occupied && !isOffMap)
        {
            switch (dirID)
            {
                //Nothing:
                case 0:
                    setPointer.SetActive(false);
                    aSource.Play();
                    changeDir = PlayerChangeDir.NoChange;
                    dirID = 1;
                    break;


                //Backward To Something:
                case 1:
                    setPointer.SetActive(true);
                    aSource.Play();
                    setPointer.transform.localEulerAngles = new Vector3(0, 0, 0);
                    changeDir = PlayerChangeDir.Backward;

                    dirID = 2;
                    break;

                case 2:
                    setPointer.SetActive(true);
                    aSource.Play();
                    setPointer.transform.localEulerAngles = new Vector3(0, 90, 0);
                    changeDir = PlayerChangeDir.Left;

                    dirID = 3;

                    break;


                //Forward To Something:
                case 3:
                    setPointer.SetActive(true);
                    aSource.Play();
                    setPointer.transform.localEulerAngles = new Vector3(0, 180, 0);
                    changeDir = PlayerChangeDir.Forward;

                    dirID = 4;
                    break;

                case 4:
                    setPointer.SetActive(true);
                    aSource.Play();
                    setPointer.transform.localEulerAngles = new Vector3(0, 270, 0);
                    changeDir = PlayerChangeDir.Right;
                    dirID = 0;
                    break;
            }
        }
    }

    public void ChangeDir()
    {
        if (player.type == PlayerType.player)
        {
            switch (changeDir)
            {
                //Nothing:
                case PlayerChangeDir.NoChange:
                    nextDir = player.playerDir;
                    break;

                //Backward To Something:
                case PlayerChangeDir.Forward:
                    nextDir = PlayerDirection.forward;
                    break;

                case PlayerChangeDir.Backward:
                    nextDir = PlayerDirection.backward;
                    break;


                //Forward To Something:
                case PlayerChangeDir.Right:
                    nextDir = PlayerDirection.right;
                    break;

                case PlayerChangeDir.Left:
                    nextDir = PlayerDirection.left;
                    break;
            }
        }
        else
        {
            switch (npcChangeDir)
            {
                //Nothing:
                case ChangeDirection.NoChange:
                    npcNextDir = player.playerDir;
                    break;

                //Backward To Something:
                case ChangeDirection.backwardToRight:
                    if (player.playerDir == PlayerDirection.forward)
                    {
                        npcNextDir = PlayerDirection.right;
                    }

                    else if (player.playerDir == PlayerDirection.left)
                    {
                        npcNextDir = PlayerDirection.backward;
                    }

                    else
                        npcNextDir = player.playerDir;
                    break;

                case ChangeDirection.backwardToLeft:
                    if (player.playerDir == PlayerDirection.forward)
                    {
                        npcNextDir = PlayerDirection.left;
                    }

                    else if (player.playerDir == PlayerDirection.right)
                    {
                        npcNextDir = PlayerDirection.backward;
                    }

                    else
                        npcNextDir = player.playerDir;
                    break;


                //Forward To Something:
                case ChangeDirection.forwardToLeft:
                    if (player.playerDir == PlayerDirection.backward)
                    {
                        npcNextDir = PlayerDirection.left;
                    }

                    else if (player.playerDir == PlayerDirection.right)
                    {
                        npcNextDir = PlayerDirection.forward;
                    }

                    else
                        npcNextDir = player.playerDir;
                    break;

                case ChangeDirection.forwardToRight:
                    if (player.playerDir == PlayerDirection.backward)
                    {
                        npcNextDir = PlayerDirection.right;
                    }

                    else if (player.playerDir == PlayerDirection.left)
                    {
                        npcNextDir = PlayerDirection.forward;
                    }

                    else
                        npcNextDir = player.playerDir;
                    break;
            }
        }
        SetNextTarget();
    }

    public void SetNextTarget()
    {
        if (player.type == PlayerType.player)
        {
            switch (nextDir)
            {
                case PlayerDirection.forward:
                    setNextTarget = targetList[0];
                    break;

                case PlayerDirection.right:
                    setNextTarget = targetList[1];
                    break;

                case PlayerDirection.backward:
                    setNextTarget = targetList[2];
                    break;

                case PlayerDirection.left:
                    setNextTarget = targetList[3];
                    break;
            }
            if (nextDir != player.playerDir)
                Timing.RunCoroutine(_SetPointerToNoChange().CancelWith(gameObject));
        } else {
            switch (npcNextDir)
            {
                case PlayerDirection.forward:
                    setNextTargetNpc = npcTargetList[0];
                    break;

                case PlayerDirection.right:
                    setNextTargetNpc = npcTargetList[1];
                    break;

                case PlayerDirection.backward:
                    setNextTargetNpc = npcTargetList[2];
                    break;

                case PlayerDirection.left:
                    setNextTargetNpc = npcTargetList[3];
                    break;
            }
        }
    }

    public void CheckGroundHeight()
    {
        if(player.type == PlayerType.player)
        {
            switch (hGround)
            {
                case HighGround.min:
                    if(setNextTarget.hGround == HighGround.max)
                    {
                        player.Die();
                    }
                    break;

                case HighGround.max:
                    if (setNextTarget.hGround == HighGround.min)
                    {
                        player.Die();
                    }
                    break;
            }
        } else {

            switch (hGround)
            {
                case HighGround.min:
                    if (setNextTargetNpc.hGround == HighGround.max)
                    {
                        player.Die();
                    }
                    break;

                case HighGround.max:
                    if (setNextTargetNpc.hGround == HighGround.min)
                    {
                        player.Die();
                    }
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider movable)
    {
        if (movable.GetComponent<MainPlayer>() != null)
        {
            player = movable.GetComponent<MainPlayer>();
            
            if (!isOffMap)
            {
                if (!occupied)
                {
                    ChangeDir();
                    Timing.RunCoroutine(_Walking().CancelWith(gameObject));
                }
            }
        }
    }
    IEnumerator<float> _Walking()
    {
        yield return Timing.WaitForSeconds(.25f);

        if (player.type == PlayerType.player)
        {
            player.playerDir = nextDir;
            player.curTarget = setNextTarget;
        }
        else
        {
            player.playerDir = npcNextDir;
            player.curTarget = setNextTargetNpc;
        }

        Timing.RunCoroutine(_SetPlayerDir().CancelWith(gameObject));
    }

    public void NextTarget()
    {
        Timing.RunCoroutine(_SetNextTarget().CancelWith(gameObject));
    }
    IEnumerator<float> _SetNextTarget()
    {
        yield return Timing.WaitForSeconds(.25f);
        switch (player.playerDir)
        {
            case PlayerDirection.forward:
                player.curTarget = targetList[0];
                break;

            case PlayerDirection.right:
                player.curTarget = targetList[1];
                break;

            case PlayerDirection.backward:
                player.curTarget = targetList[2];
                break;

            case PlayerDirection.left:
                player.curTarget = targetList[3];
                break;
        }
        ChangeDir();
        CheckGroundHeight();
        player.Walk();
    }

    IEnumerator<float> _SetPlayerDir()
    {
        yield return Timing.WaitForSeconds(.25f);
        if (!player.stopSim)
        {
            CheckGroundHeight();
            player.Walk();
        }
    }
    [Header("Pointer Vfx:")]
    public GameObject pointerVfx;
    IEnumerator<float> _SetPointerToNoChange()
    {
        setPointer.SetActive(false);

        CameraShaker.Instance.ShakeOnce(1.5f, 1.5f, .1f, 1f);
        //Instantiate vfx
        Instantiate(
            pointerVfx,
            new Vector3(transform.position.x,
                        transform.position.y + 1.25f,
                        transform.position.z
                        ),
            Quaternion.identity
            );
        yield return Timing.WaitForSeconds(.5f);

        changeDir = PlayerChangeDir.NoChange;
    }


    [HideInInspector] public int rotID; //Rotation id { 0 = up, 1 = right, 2 = down, 3 = left}

    [Header("Rows and Columns:")]
    public int rId;     //Row id...
    public int cId;     //Column...
}
