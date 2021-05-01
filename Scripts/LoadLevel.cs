using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;

public class LoadLevel : MonoBehaviour
{
    //Load the level you are currently at!!!
    [Header("Story ID:")]
    public int storyId;     // 0 = rrh, 1 = three little pigs, 2 = robin hood, etc...

    [Header("Currently Max Level:")]
    public int curMaxLvl = 10;

   
    private void Start()
    {
        Timing.RunCoroutine(_CallLevel().CancelWith(gameObject));   //Move this to another function later...

        //deal with points later... you'll probably use something like progress of unlocking the next story etc, etc...
    }

    IEnumerator<float> _CallLevel()
    {
        yield return 0;

        if (SaveData.instance.lvl > curMaxLvl)
        {
            int nextLvl = Random.Range(0, curMaxLvl - 1);
            SceneManager.LoadSceneAsync(nextLvl);
        }

        else if (SaveData.instance.lvl > 0)
        {
            SceneManager.LoadSceneAsync(SaveData.instance.lvl);
        }

        else
            SceneManager.LoadSceneAsync(1);
    }

    public void ChooseStory()
    {
        //Chose the selected story...
    }
}
