using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Swipe : MonoBehaviour
{
    public bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool isDragging = false;
    public Vector2 startTouch, swipeDelta;

    [HideInInspector] public bool swiping;
    [HideInInspector] public bool canSwipe;
    public void CheckSwipe()
    {
        Timing.RunCoroutine(_UseSwipe().CancelWith(gameObject));
    }

    private IEnumerator<float> _UseSwipe()
    {
        canSwipe = false;
        while (swiping)
        {
            tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

            #region PC inputs:

            if (Input.GetMouseButtonDown(0))
            {
                tap = true;
                isDragging = true;
                startTouch = Input.mousePosition;
            }

            else if (Input.GetMouseButtonUp(0))
            {
               isDragging = false;
               Reset();
            }

            #endregion

            #region Mobile inputs:

            if (Input.touches.Length > 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    tap = true;
                    isDragging = true;
                    startTouch = Input.touches[0].position;
                }

                else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                    Reset();
                }
            }

            #endregion

            //Calculate the distance:
            swipeDelta = Vector2.zero;
            if (isDragging)
            {
                if (Input.touches.Length > 0)
                    swipeDelta = Input.touches[0].position - startTouch;
                else if (Input.GetMouseButton(0))
                    swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }

            //Crossing the dead zone:
            if (swipeDelta.magnitude > 25)
            {
                //Calculate the direction:
                float x = swipeDelta.x;
                float y = swipeDelta.y;
                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    //left or right
                    if (x < 0)
                        swipeLeft = true;
                    else
                        swipeRight = true;
                }
                else
                {
                    //up or down
                    if (y < 0)
                        swipeDown = true;
                    else
                        swipeUp = true;
                }

                Reset();
            }

            yield return 0;
        }
        canSwipe = true;
    }

    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
    }


    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }
}
