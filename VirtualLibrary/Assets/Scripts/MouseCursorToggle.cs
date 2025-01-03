using UnityEngine;

public class MouseCursorToggle : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.LeftAlt; // The key to toggle the cursor
    private bool cursorVisible = false;

    void Update()
    {
        bool isAltPressed = Input.GetKey(KeyCode.LeftAlt); // Check if Alt is held down
        Cursor.visible = isAltPressed;
        Cursor.lockState = isAltPressed ? CursorLockMode.None : CursorLockMode.Locked;
    }

}
