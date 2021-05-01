using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamStart : MonoBehaviour
{
    void Start()
    {
        LeanTween.moveY(gameObject, 8, 3f).setEaseOutBack();
    }
}
