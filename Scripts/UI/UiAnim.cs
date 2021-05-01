using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiAnim : MonoBehaviour
{
    private void Start()
    {
        LeanTween.scale(GetComponent<RectTransform>(), new Vector3(1.2f, 1.2f, 1.2f), .5f).setLoopPingPong();
    }
}
