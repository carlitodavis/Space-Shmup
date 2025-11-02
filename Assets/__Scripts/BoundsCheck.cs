using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  If you type /// in Visual Studio, it will auto-generate a summary block for you
/// Keeps a GameObject within the camera's bounds
/// Note that this script assumes an orthographic camera
/// </summary>
public class BoundsCheck : MonoBehaviour
{
    [System.Flags]
    public enum eScreenLocks
    {
        onScreen = 0,
        offRight = 1,
        offLeft = 2,
        offUp = 4,
        offDown = 8
    }
    public enum eType {center, inset, outset};

    [Header("Inscribed")]

    public eType boundsType = eType.center;
    public float radius = 1f;
    public bool keepOnScreen = true;
    [Header("Dynamic")]
    public eScreenLocks screenLocks = eScreenLocks.onScreen;
    // public bool isOnScreen = true;
    public float camWidth;
    public float camHeight;
    
    void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float checkRadius = 0;
        if (boundsType == eType.inset) checkRadius = -radius;
        if (boundsType == eType.outset) checkRadius = radius;

        Vector3 pos = transform.position;
        screenLocks = eScreenLocks.onScreen;
        // isOnScreen = true;

        if (pos.x > camWidth + checkRadius)
        {
            pos.x = camWidth + checkRadius;
            screenLocks |= eScreenLocks.offRight;
            // isOnScreen = false;
        }

        if (pos.x < -camWidth - checkRadius)
        {
            pos.x = -camWidth - checkRadius;
            screenLocks |= eScreenLocks.offLeft;
            // isOnScreen = false;
        }

        if (pos.y > camHeight + checkRadius)
        {
            pos.y = camHeight + checkRadius;
            screenLocks |= eScreenLocks.offUp;
            //  isOnScreen = false;
        }

        if (pos.y < -camHeight - checkRadius)
        {
            pos.y = -camHeight - checkRadius;
            screenLocks |= eScreenLocks.offDown;
            // isOnScreen = false;
        }
        if (keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            screenLocks = eScreenLocks.onScreen;
            //  isOnScreen = true;
        }


    }
    public bool isOnScreen
    {
        get
        {
            return (screenLocks == eScreenLocks.onScreen);
        }
    }

    public bool LocIs(eScreenLocks checkLoc)
    {
        if ( checkLoc == eScreenLocks.onScreen) return isOnScreen;
        return ((screenLocks & checkLoc) == checkLoc);
    }
}
