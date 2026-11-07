using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputIntent
{
    public Vector2 movement{get;internal set;}
    public Vector2 look{get;internal set;}
    public bool isHoldFiring{get;internal set;}
    public bool isHoldAiming{get;internal set;}
    public bool isSlash{get;internal set;}
    public bool isPickup{get;internal set;}
    public bool isDodge{get;internal set;}

    //清零
    internal void Reset()
    {
        movement = Vector2.zero;
        look = Vector2.zero;
        isHoldFiring = false;
        isHoldAiming = false;
        isSlash = false;
        isPickup = false;
        isDodge = false;
    }
}
