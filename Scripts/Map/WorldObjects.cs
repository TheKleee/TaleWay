using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class WorldObjects : MonoBehaviour
{
    [Header("Map Setup: --- Read *TARGETS* Element Id List!!! ---")]
    [Range(0, 8)] public int redPosId;
    [Range(0, 8)] public int bluePosId;
    [Range(0, 8)] public int keyPosId;
    [Range(0, 8)] public int housePosId;


    ///////////////////////////////// SET IT UP >:D
    [Header("Choose Main Player:")]
    public bool isRed;

    [Header("Player List:")]
    [Header("------------------------------------------------")]
    [Space]
    [Space]
    public MainPlayer red;
    public MainPlayer blue;

    [Header("Key And House:")]
    public KeyCollector key;
    public Goal house;

    [Header("Main Target List:")]
    public Target[] targets = new Target[9];

    [Header("Indicators:")]
    //As Red:
    public GameObject redYou;
    public GameObject houseGoal;
    //As Blue:
    public GameObject blueYou;
    public GameObject redGoal;


    private void Start()
    {
        //Choose main player:
        if (isRed)
        {
            //pCont.player = red;
            red.type = PlayerType.player;
            blue.type = PlayerType.npc;
        } else {
            //pCont.player = blue;
            blue.type = PlayerType.player;
            red.type = PlayerType.npc;
        }
        Timing.RunCoroutine(_SummonIndicators().CancelWith(gameObject));    //Summoning the indicators!!! :D

        //Players:
        //Red:
        red.curTarget = targets[redPosId];
        targets[redPosId].player = red;
        red.transform.position =
            new Vector3(
                targets[redPosId].transform.position.x,
                red.transform.position.y,
                targets[redPosId].transform.position.z
            );
        red.SetThingsUp();
        //Blue:
        blue.curTarget = targets[bluePosId];
        targets[bluePosId].player = blue;
        blue.transform.position =
            new Vector3(
                targets[bluePosId].transform.position.x,
                blue.transform.position.y,
                targets[bluePosId].transform.position.z
            );
        blue.SetThingsUp();

        //Key:
        key.transform.position = targets[keyPosId].transform.position;

        //House:
        house.startTarget = targets[housePosId];
        house.startTarget.occupied = true;
        house.transform.position = targets[housePosId].transform.position;

    }

    IEnumerator<float> _SummonIndicators()
    {
        yield return Timing.WaitForSeconds(2.5f);
        if (isRed)
        {
            redYou.SetActive(true);
            houseGoal.SetActive(true);
        } else {
            blueYou.SetActive(true);
            redGoal.SetActive(true);
        }

    }
}
