using UnityEngine;
using UnityEngine.InputSystem;

public class BikeInput : MonoBehaviour
{
    public BikeController bike;

    bool waitForRelease;   // stops the start press from also triggering a skid

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // title screen: space to start
        if (!bike.started)
        {
            if (kb.spaceKey.wasPressedThisFrame)
            {
                bike.StartRun();
                waitForRelease = true;
            }
            return;
        }

        // after dying, space restarts
        if (bike.dead)
        {
            if (kb.spaceKey.wasPressedThisFrame) bike.Restart();
            return;
        }

        // ignore space until it's let go after starting
        if (waitForRelease)
        {
            if (kb.spaceKey.isPressed) return;
            waitForRelease = false;
        }

        if (kb.leftArrowKey.wasPressedThisFrame && !bike.skidding) bike.MoveLane(-1);
        if (kb.rightArrowKey.wasPressedThisFrame && !bike.skidding) bike.MoveLane(1);
        if (kb.upArrowKey.wasPressedThisFrame) bike.Pedal();

        // hold space to skid
        bike.skidding = kb.downArrowKey.isPressed;
    }
}