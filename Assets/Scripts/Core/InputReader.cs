using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    //总控
    public bool canReadInput = true;
    
    private readonly InputIntent _intent = new InputIntent();
    public InputIntent Intent => _intent;

    void Update()
    {
        _intent.Reset(); //清零
        if (!canReadInput) return;
        _intent.movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _intent.look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _intent.isHoldFiring = Input.GetKey(KeyCode.Mouse0);
        _intent.isHoldAiming = Input.GetKey(KeyCode.Mouse1);
        _intent.isSlash = Input.GetKeyDown(KeyCode.X);
        _intent.isPickup = Input.GetKeyDown(KeyCode.E);
        _intent.isDodge = Input.GetKeyDown(KeyCode.Space);

        
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 600, 40), $"movement: {_intent.movement}");
        GUI.Label(new Rect(10, 50, 600, 40), $"look: {_intent.look}");
        GUI.Label(new Rect(10, 90, 600, 40), $"isHoldFiring: {_intent.isHoldFiring}");
        GUI.Label(new Rect(10, 130, 600, 40), $"isHoldAiming: {_intent.isHoldAiming}");
        GUI.Label(new Rect(10, 170, 600, 40), $"isSlash: {_intent.isSlash}");
        GUI.Label(new Rect(10, 210, 600, 40), $"isPickup: {_intent.isPickup}");
        GUI.Label(new Rect(10, 250, 600, 40), $"isDodge: {_intent.isDodge}");
    }
}
