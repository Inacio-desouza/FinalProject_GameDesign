using UnityEngine;
using UnityEngine.InputSystem;

public class BikeInput : MonoBehaviour
{
    public BikeController bike;

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // after dying
        if (bike.dead)
        {
            if (kb.spaceKey.wasPressedThisFrame) bike.Restart();
            return;
        }
        //controls
        if (kb.leftArrowKey.wasPressedThisFrame) bike.MoveLane(-1);
        if (kb.rightArrowKey.wasPressedThisFrame) bike.MoveLane(1);
        if (kb.upArrowKey.wasPressedThisFrame) bike.Pedal();


        bike.skidding = kb.downArrowKey.isPressed;
    }
}