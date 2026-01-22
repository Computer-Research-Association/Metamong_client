using UnityEngine;

public class InputDeviceManager : MonoBehaviour
{

    public GameObject mobileJoystick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bool isMobile = Application.isMobilePlatform || Input.touchSupported;

        mobileJoystick.SetActive(isMobile);
    }

}
