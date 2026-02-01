#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

using UnityEngine;

namespace UniversalRangedWeaponSystem.Examples
{
    [AddComponentMenu("Weapon System/Examples/Player Camera Controller", 1)]
    public class LookWithMouse : MonoBehaviour
    {
        [Tooltip("How much to rotate the camera when the mouse moves. Higher values mean the camera rotates faster.")]
        public float mouseSensitivity = 100f;

        [Tooltip("The GameObject that is considered 'the player', usually the parent of the camera this component is attached to.")]
        public Transform playerBody;

        [Tooltip("The speed at which the camera will move to the new position as set by the recoil.")]
        public float recoilSmoothSpeed = 10f;


        private RangedWeapon rangedWeapon;
        private float recoilLerpAlpha = 1f;

        private const float k_MouseSensitivityMultiplier = 0.01f;
        private float xRotation;
        private float yRotation;
        private Vector3 deviation = Vector3.zero;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            WeaponSocket.OnWeaponAttached += (RangedWeapon rangedWeapon) =>
            {
                this.rangedWeapon = rangedWeapon;
                rangedWeapon.OnFire.AddListener(OnFire);
            };
        }

        
        private void Update()
        {
    #if ENABLE_INPUT_SYSTEM
            float mouseX = 0, mouseY = 0;

            if (Mouse.current != null)
            {
                var delta = Mouse.current.delta.ReadValue() / 15.0f;
                mouseX += delta.x;
                mouseY += delta.y;
            }
            if (Gamepad.current != null)
            {
                var value = Gamepad.current.rightStick.ReadValue() * 2;
                mouseX += value.x;
                mouseY += value.y;
            }

            mouseX *= mouseSensitivity * k_MouseSensitivityMultiplier;
            mouseY *= mouseSensitivity * k_MouseSensitivityMultiplier;
#else
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * k_MouseSensitivityMultiplier;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * k_MouseSensitivityMultiplier;
#endif            
            
            if (rangedWeapon)
            {
                if (rangedWeapon.recoilExtension)
                {                    
                    deviation = Vector3.Lerp(Vector3.zero, rangedWeapon.recoilRotation, recoilLerpAlpha);
                    recoilLerpAlpha = Mathf.Clamp01(recoilLerpAlpha + (Time.deltaTime * recoilSmoothSpeed));
                }
            }

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            yRotation += mouseX;            

            // apply rotation
            playerBody.localRotation = Quaternion.Euler(yRotation * Vector3.up) * Quaternion.Euler(deviation.x * Vector3.up);
            transform.localRotation = Quaternion.Euler(xRotation * Vector3.right) * Quaternion.Euler(deviation.y * Vector3.right * -1f);
        }

        private void OnFire()
        {
            xRotation -= deviation.y;
            yRotation += deviation.x;
            deviation = Vector3.zero;

            recoilLerpAlpha = 0f;           
        }
    }
}
