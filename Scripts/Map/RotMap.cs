using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MEC;

public class RotMap : MonoBehaviour
{
    #region Levels (remove later):
    [Header("--- Select Levels: ---")]
    public int thisLvl;
    public int nextLvl;
    #endregion

    [Header("World Map Ref:")]
    public WorldObjects worldMap;

    private Swipe swipe;
    private bool isSwiping;
    private bool canSwipe = true;

    [Header("Camera Ref:")]
    public Camera cam;

    [Header("Rotation Speed:")]
    public float rotSpeed = 10;

    private MainPlayer player;

    [Header("UI Elements:")]
    public GameObject RunSimBtn;


    private void Start()
    {
        TinySauce.OnGameStarted(levelNumber: SaveData.instance.lvl.ToString());
        Timing.RunCoroutine(_ActivateUI().CancelWith(gameObject));

        swipe = GetComponent<Swipe>();

        if (worldMap.isRed)
        {
            player = allPlayers[0];
        } else {
            player = allPlayers[1];
        }

        player.mainMap = this;

        if (allPlayers.Length < 1)
            allPlayers = FindObjectsOfType<MainPlayer>();   //Some minor fix cause I'll forget about this thing anyway! >:C
    }

    void Update()
    {
        #region Mobile:

        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (!swipe.swiping)
                isSwiping = true;

            if (!player.simStarted)
            {
                Ray raycast = cam.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit rayHit;
                if (Physics.Raycast(raycast, out rayHit))
                {
                    if (rayHit.transform.GetComponent<Target>() != null && rayHit.collider.isTrigger)
                    {
                        //Do something...
                        rayHit.transform.GetComponent<Target>().ChangePointer();
                    }

                    if (rayHit.transform.GetComponent<MainPlayer>() == player)
                    {
                        player.SetPlayerDir();
                    }
                }
            }
        }

        #endregion

        #region pc:

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/fileName");
        }

        //the pc input => this should be removed before building the project (or commented out at least) : )
        if (Input.GetMouseButtonDown(0))
        {
            if (!swipe.swiping)
                isSwiping = true;

            if (!player.simStarted)
            {
                Ray clickcast = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(clickcast, out RaycastHit hit))
                {
                    if (hit.transform.GetComponent<Target>() != null && hit.collider.isTrigger)
                    {
                        //Do something...
                        hit.transform.GetComponent<Target>().ChangePointer();
                    }

                    if (hit.transform.GetComponent<MainPlayer>() == player)
                    {
                        player.SetPlayerDir();
                    }
                }
            }
        }

        #endregion

        #region Swiping Detection:

        if (isSwiping && !swipe.swiping)
        {
            isSwiping = false;
            swipe.swiping = true;
            swipe.CheckSwipe();
            Timing.RunCoroutine(_StopSwipeDetection().CancelWith(gameObject));
        }

        #endregion


        #region Rotation:
        if (swipe.SwipeLeft && canSwipe)
        {
            canSwipe = false;
            LeanTween.rotateAround(gameObject, -Vector3.up, 90, .45f);
        }

        if (swipe.swipeRight && canSwipe)
        {
            canSwipe = false;
            LeanTween.rotateAround(gameObject, Vector3.up, 90, .45f);
        }

        #endregion
    }

    private IEnumerator<float> _StopSwipeDetection()
    {
        yield return Timing.WaitForSeconds(.45f);
        swipe.swiping = false;
        canSwipe = true;
    }

    public void LvlFailed()
    {
        if (SaveData.instance != null)
        {
            SaveData.instance.lvl = thisLvl;
            SaveData.instance.SaveGame();
        }
        Timing.RunCoroutine(_Failed().CancelWith(gameObject));
    }

    public void LvlCleared()
    {
        if (SaveData.instance != null)
        {
            SaveData.instance.lvl = thisLvl + 1;
            SaveData.instance.SaveGame();
        }
        Timing.RunCoroutine(_Cleared().CancelWith(gameObject));
    }

    IEnumerator<float> _Failed()
    {
        yield return Timing.WaitForSeconds(1.5f);
        StopSimulation();
        //Set lvl failed canvas to true or whatever : /
        ThisLevel();
    }

    IEnumerator<float> _Cleared()
    {
        yield return Timing.WaitForSeconds(1.5f);
        StopSimulation();
        //Same as for lvlFailed but the win one instead -.-
        NextLevel();
        Debug.Log("You Won!!! :D"); //And ofc do something else as well... like loading new lvl, etc, etc...
    }

    [Header("List Of All Player Objects:")]
    public MainPlayer[] allPlayers = new MainPlayer[2];
    public void StartingSimulation()
    {
        RunSimBtn.SetActive(false);
        foreach (MainPlayer mPlayer in allPlayers)
            mPlayer.RunSimulation();    //I'm so gonna forget all about this xD
    }

    public void StopSimulation()
    {
        foreach (MainPlayer mPlayer in allPlayers)
            mPlayer.stopSim = true;

        LeanTween.moveY(worldMap.gameObject, 10, 1f).setEaseInBack();
    }

    IEnumerator<float> _ActivateUI()
    {
        yield return Timing.WaitForSeconds(4f);
        RunSimBtn.SetActive(true);
    }

    #region Level Control:

    public void NextLevel()
    {
        TinySauce.OnGameFinished(levelNumber: "Next Level: " + SaveData.instance.lvl, true, SaveData.instance.points);
        Timing.RunCoroutine(_LoadLvl(nextLvl).CancelWith(gameObject));
    }

    public void ThisLevel()
    {
        TinySauce.OnGameFinished(levelNumber: "Level Failed: " + SaveData.instance.lvl, false, SaveData.instance.points);
        Timing.RunCoroutine(_LoadLvl(thisLvl).CancelWith(gameObject));
    }

    IEnumerator<float> _LoadLvl(int lvlToLoad)
    {
        yield return Timing.WaitForSeconds(1.5f);
        SceneManager.LoadSceneAsync(lvlToLoad);
    }

    #endregion
}
