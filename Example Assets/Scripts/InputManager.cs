using UnityEngine;
using UnityEngine.InputSystem;

namespace UniversalRangedWeaponSystem.Examples
{
    [RequireComponent(typeof(PlayerInput)), AddComponentMenu("Weapon System/Examples/Input Manager", 0)]
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;

        public bool doPauseAction { get; private set; }
        public Vector2 moveActionValue { get; private set; }
        public Vector2 lookActionValue { get; private set; }
        public bool hasJumpedThisFrame { get; private set; }
        public bool wasPrimaryFireActionPressedThisFrame { get; private set; }   
        public bool wasPrimaryFireActionReleasedThisFrame { get; private set; }   
        public bool wasSecondaryFireActionPressedThisFrame { get; private set; }
        public bool wasSecondaryFireActionReleasedThisFrame { get; private set; }
        public bool doMenuAction { get; private set; }      

        private PlayerInput playerInput;

        private InputAction pauseAction;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction jumpAction;
        private InputAction primaryFireAction;    
        private InputAction secondaryFireAction;    
    

        private void Awake()
        {
            // Singleton pattern
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
            // If there's already an instance then destroy



            playerInput = GetComponent<PlayerInput>();
            SetupInputActions();
        }

        private void Update()
        {
            doPauseAction = pauseAction.WasPressedThisFrame() && pauseAction.IsPressed();
            moveActionValue = moveAction.ReadValue<Vector2>();
            lookActionValue = lookAction.ReadValue<Vector2>();

            hasJumpedThisFrame = jumpAction.WasPressedThisFrame() && jumpAction.IsPressed();

            wasPrimaryFireActionPressedThisFrame = primaryFireAction.WasPressedThisFrame() && primaryFireAction.IsPressed();        
            wasPrimaryFireActionReleasedThisFrame = primaryFireAction.WasReleasedThisFrame();      
        
            wasSecondaryFireActionPressedThisFrame = secondaryFireAction.WasPressedThisFrame() && secondaryFireAction.IsPressed();
            wasSecondaryFireActionReleasedThisFrame = secondaryFireAction.WasReleasedThisFrame();
        }

        private void SetupInputActions()
        {
            pauseAction = playerInput.actions["Pause"];
            moveAction = playerInput.actions["Move"];
            lookAction = playerInput.actions["Look"];
            jumpAction = playerInput.actions["Jump"];
            primaryFireAction = playerInput.actions["PrimaryFire"];        
            secondaryFireAction = playerInput.actions["SecondaryFire"];        

            pauseAction.Enable();
            moveAction.Enable();
            lookAction.Enable();
            jumpAction.Enable();
            primaryFireAction.Enable();
            secondaryFireAction.Enable();        
        }
    }
}
