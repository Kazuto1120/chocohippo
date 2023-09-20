// Designed by KINEMATION, 2023

using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.FPSAnimator;
using Kinemation.FPSFramework.Runtime.Layers;
using Kinemation.FPSFramework.Runtime.Recoil;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Demo.Scripts.Runtime
{
    public enum FPSMovementState
    {
        Idle,
        Walking,
        Running,
        Sprinting
    }

    public enum FPSPoseState
    {
        Standing,
        Crouching
    }

    public enum FPSActionState
    {
        None,
        Ready,
        Aiming,
        PointAiming,
    }

    public enum FPSCameraState
    {
        Default,
        Barrel,
        InFront
    }

    // An example-controller class
    public class FPSController : FPSAnimController
    {
        [Tab("Animation")] [Header("General")] [SerializeField]
        private Animator animator;

        [SerializeField] private float turnInPlaceAngle;
        [SerializeField] private AnimationCurve turnCurve = new AnimationCurve(new Keyframe(0f, 0f));
        [SerializeField] private float turnSpeed = 1f;
        
        [Header("Dynamic Motions")]
        [SerializeField] private IKAnimation aimMotionAsset;
        [SerializeField] private IKAnimation leanMotionAsset;
        [SerializeField] private IKAnimation crouchMotionAsset;
        [SerializeField] private IKAnimation unCrouchMotionAsset;
        [SerializeField] private IKAnimation onJumpMotionAsset;
        [SerializeField] private IKAnimation onLandedMotionAsset;

        // Animation Layers
        [SerializeField] [HideInInspector] private LookLayer lookLayer;
        [SerializeField] [HideInInspector] private AdsLayer adsLayer;
        [SerializeField] [HideInInspector] private SwayLayer swayLayer;
        [SerializeField] [HideInInspector] private LocomotionLayer locoLayer;
        [SerializeField] [HideInInspector] private SlotLayer slotLayer;
        // Animation Layers

        [Tab("Controller")] 
        [Header("General")] 
        [SerializeField] private CharacterController controller;
        [SerializeField] private float gravity = 8f;
        [SerializeField] private float jumpHeight = 9f;
        private bool _isInAir = false;

        [Header("Camera")]
        [SerializeField] private Transform mainCamera;
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private Transform firstPersonCamera;
        [SerializeField] private Transform freeCamera;
        [SerializeField] private float sensitivity;
        [SerializeField] private Vector2 freeLookAngle;
        
        [Header("Movement")] 
        [SerializeField] private float curveLocomotionSmoothing = 2f;
        [SerializeField] private float moveSmoothing = 2f;
        [SerializeField] private float sprintSpeed = 3f;
        [SerializeField] private float walkingSpeed = 2f;
        [SerializeField] private float crouchSpeed = 1f;
        [SerializeField] private float crouchRatio = 0.5f;
        private float speed;

        [Tab("Weapon")] 
        [SerializeField] private List<Weapon> weapons;
        [SerializeField] private FPSCameraShake shake;

        private bool disableInput = false;
        private Vector2 _playerInput;

        // Used for free-look
        private Vector2 _freeLookInput;
        private Vector2 _smoothAnimatorMove;
        private Vector2 _smoothMove;

        private int _index;
        private int _lastIndex;

        private float _fireTimer = -1f;
        private int _bursts;
        private bool _aiming;
        private bool _freeLook;
        private bool _hasActiveAction;
        
        private FPSActionState actionState;
        private FPSMovementState movementState;
        private FPSPoseState poseState;
        private FPSCameraState cameraState = FPSCameraState.Default;
        
        private float originalHeight;
        private Vector3 originalCenter;

        private float smoothCurveAlpha = 0f;

        private static readonly int Crouching = Animator.StringToHash("Crouching");
        private static readonly int Moving = Animator.StringToHash("Moving");
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int OverlayType = Animator.StringToHash("OverlayType");
        private static readonly int TurnRight = Animator.StringToHash("TurnRight");
        private static readonly int TurnLeft = Animator.StringToHash("TurnLeft");
        private static readonly int InAir = Animator.StringToHash("InAir");
        private static readonly int Equip = Animator.StringToHash("Equip");
        private static readonly int UnEquip = Animator.StringToHash("Unequip");

        private void InitLayers()
        {
            InitAnimController();

            controller = GetComponentInChildren<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            lookLayer = GetComponentInChildren<LookLayer>();
            adsLayer = GetComponentInChildren<AdsLayer>();
            locoLayer = GetComponentInChildren<LocomotionLayer>();
            swayLayer = GetComponentInChildren<SwayLayer>();
            slotLayer = GetComponentInChildren<SlotLayer>();
        }

        private void Start()
        {
            Time.timeScale = 1f;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            speed = walkingSpeed;
            
            originalHeight = controller.height;
            originalCenter = controller.center;

            moveRotation = transform.rotation;

            InitLayers();
            EquipWeapon();
        }
        
        private void StartWeaponChange()
        {
            DisableAim();
            
            _hasActiveAction = true;
            animator.CrossFade(UnEquip, 0.1f);
        }

        public void SetActionActive(int isActive)
        {
            _hasActiveAction = isActive != 0;
        }

        public void RefreshStagedState()
        {
            GetGun().stagedReloadSegment++;
        }
        
        public void ResetStagedState()
        {
            GetGun().stagedReloadSegment = 0;
        }

        public void EquipWeapon()
        {
            if (weapons.Count == 0) return;

            weapons[_lastIndex].gameObject.SetActive(false);
            var gun = weapons[_index];

            _bursts = gun.burstAmount;
            
            StopAnimation(0.1f);
            InitWeapon(gun);
            gun.gameObject.SetActive(true);

            animator.SetFloat(OverlayType, (float) gun.overlayType);
            animator.Play(Equip);
        }
        
        private void ChangeWeapon_Internal()
        {
            if (movementState == FPSMovementState.Sprinting) return;
            if (_hasActiveAction) return;
            
            OnFireReleased();
            
            int newIndex = _index;
            newIndex++;
            if (newIndex > weapons.Count - 1)
            {
                newIndex = 0;
            }

            _lastIndex = _index;
            _index = newIndex;

            StartWeaponChange();
        }

        private void DisableAim()
        {
            _aiming = false;
            OnInputAim(_aiming);
            
            actionState = FPSActionState.None;
            adsLayer.SetAds(false);
            adsLayer.SetPointAim(false);
            swayLayer.SetFreeAimEnable(true);
            swayLayer.SetLayerAlpha(1f);
            slotLayer.PlayMotion(aimMotionAsset);
        }

        public void ToggleAim()
        {
            if (_hasActiveAction)
            {
                //return;
            }
            
            _aiming = !_aiming;

            if (_aiming)
            {
                actionState = FPSActionState.Aiming;
                adsLayer.SetAds(true);
                swayLayer.SetFreeAimEnable(false);
                swayLayer.SetLayerAlpha(0.3f);
                slotLayer.PlayMotion(aimMotionAsset);
                OnInputAim(_aiming);
            }
            else
            {
                DisableAim();
            }

            recoilComponent.isAiming = _aiming;
        }

        public void ChangeScope()
        {
            InitAimPoint(GetGun());
        }

        private void Fire()
        {
            if (_hasActiveAction) return;
            
            GetGun().OnFire();
            recoilComponent.Play();
            PlayCameraShake(shake);
        }

        private void OnFirePressed()
        {
            if (weapons.Count == 0) return;
            if (_hasActiveAction) return;

            Fire();
            _bursts = GetGun().burstAmount - 1;
            _fireTimer = 0f;
        }

        private Weapon GetGun()
        {
            return weapons[_index];
        }

        private void OnFireReleased()
        {
            recoilComponent.Stop();
            _fireTimer = -1f;
        }

        private void SprintPressed()
        {
            if (poseState == FPSPoseState.Crouching || _hasActiveAction || _isInAir)
            {
                return;
            }

            OnFireReleased();
            lookLayer.SetLayerAlpha(0.5f);
            adsLayer.SetLayerAlpha(0f);
            locoLayer.SetReadyWeight(0f);

            movementState = FPSMovementState.Sprinting;
            actionState = FPSActionState.None;

            recoilComponent.Stop();

            speed = sprintSpeed;
        }

        private void SprintReleased()
        {
            if (poseState == FPSPoseState.Crouching)
            {
                return;
            }

            lookLayer.SetLayerAlpha(1f);
            adsLayer.SetLayerAlpha(1f);
            movementState = FPSMovementState.Walking;

            speed = walkingSpeed;
        }

        private void Crouch()
        {
            //todo: crouching implementation
            
            float crouchedHeight = originalHeight * crouchRatio;
            float heightDifference = originalHeight - crouchedHeight;

            controller.height = crouchedHeight;

            // Adjust the center position so the bottom of the capsule remains at the same position
            Vector3 crouchedCenter = originalCenter;
            crouchedCenter.y -= heightDifference / 2;
            controller.center = crouchedCenter;

            speed = crouchSpeed;

            lookLayer.SetPelvisWeight(0f);

            poseState = FPSPoseState.Crouching;
            animator.SetBool(Crouching, true);
            slotLayer.PlayMotion(crouchMotionAsset);
        }

        private void Uncrouch()
        {
            //todo: crouching implementation
            controller.height = originalHeight;
            controller.center = originalCenter;

            speed = walkingSpeed;

            lookLayer.SetPelvisWeight(1f);

            poseState = FPSPoseState.Standing;
            animator.SetBool(Crouching, false);
            slotLayer.PlayMotion(unCrouchMotionAsset);
        }

        private void TryReload()
        {
            if (movementState == FPSMovementState.Sprinting || _hasActiveAction) return;

            var reloadClip = GetGun().reloadClip;

            if (reloadClip == null) return;
            
            OnFireReleased();
            //DisableAim();
            
            PlayAnimation(reloadClip);
            GetGun().Reload();
        }

        private void TryGrenadeThrow()
        {
            if (movementState == FPSMovementState.Sprinting || _hasActiveAction) return;

            if (GetGun().grenadeClip == null) return;
                
            OnFireReleased();
            DisableAim();
            PlayAnimation(GetGun().grenadeClip);
        }

        private void UpdateActionInput()
        {
            smoothCurveAlpha = FPSAnimLib.ExpDecay(smoothCurveAlpha, _aiming ? 0.4f : 1f, 10,
                Time.deltaTime);
            
            animator.SetLayerWeight(3, smoothCurveAlpha);
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                TryReload();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                TryGrenadeThrow();
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                StopAnimation(0.2f);
            }

            charAnimData.leanDirection = 0;

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                SprintPressed();
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                SprintReleased();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                ChangeWeapon_Internal();
            }

            if (movementState == FPSMovementState.Sprinting)
            {
                return;
            }
            
            if (actionState != FPSActionState.Ready)
            {
                if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyUp(KeyCode.Q)
                                                || Input.GetKeyDown(KeyCode.E) || Input.GetKeyUp(KeyCode.E))
                {
                    slotLayer.PlayMotion(leanMotionAsset);
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    charAnimData.leanDirection = 1;
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    charAnimData.leanDirection = -1;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    OnFirePressed();
                }

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    OnFireReleased();
                }

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    ToggleAim();
                }

                if (Input.GetKeyDown(KeyCode.V))
                {
                    ChangeScope();
                }

                if (Input.GetKeyDown(KeyCode.B) && _aiming)
                {
                    if (actionState == FPSActionState.PointAiming)
                    {
                        adsLayer.SetPointAim(false);
                        actionState = FPSActionState.Aiming;
                    }
                    else
                    {
                        adsLayer.SetPointAim(true);
                        actionState = FPSActionState.PointAiming;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (poseState == FPSPoseState.Standing)
                {
                    Crouch();
                }
                else
                {
                    Uncrouch();
                }
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                if (actionState == FPSActionState.Ready)
                {
                    actionState = FPSActionState.None;
                    locoLayer.SetReadyWeight(0f);
                    lookLayer.SetLayerAlpha(1f);
                }
                else
                {
                    actionState = FPSActionState.Ready;
                    locoLayer.SetReadyWeight(1f);
                    lookLayer.SetLayerAlpha(.5f);
                    OnFireReleased();
                }
            }
        }

        private Quaternion desiredRotation;
        private Quaternion moveRotation;
        private float turnProgress = 1f;
        private bool isTurning = false;

        private void TurnInPlace()
        {
            float turnInput = _playerInput.x;
            _playerInput.x = Mathf.Clamp(_playerInput.x, -90f, 90f);
            turnInput -= _playerInput.x;

            float sign = Mathf.Sign(_playerInput.x);
            if (Mathf.Abs(_playerInput.x) > turnInPlaceAngle)
            {
                if (!isTurning)
                {
                    turnProgress = 0f;
                    
                    animator.ResetTrigger(TurnRight);
                    animator.ResetTrigger(TurnLeft);
                    
                    animator.SetTrigger(sign > 0f ? TurnRight : TurnLeft);
                }
                
                isTurning = true;
            }

            transform.rotation *= Quaternion.Euler(0f, turnInput, 0f);
            
            float lastProgress = turnCurve.Evaluate(turnProgress);
            turnProgress += Time.deltaTime * turnSpeed;
            turnProgress = Mathf.Min(turnProgress, 1f);
            
            float deltaProgress = turnCurve.Evaluate(turnProgress) - lastProgress;

            _playerInput.x -= sign * turnInPlaceAngle * deltaProgress;
            
            transform.rotation *= Quaternion.Slerp(Quaternion.identity,
                Quaternion.Euler(0f, sign * turnInPlaceAngle, 0f), deltaProgress);
            
            if (Mathf.Approximately(turnProgress, 1f) && isTurning)
            {
                isTurning = false;
            }
        }

        private float _jumpState = 0f;

        private void UpdateLookInput()
        {
            _freeLook = Input.GetKey(KeyCode.X);

            float deltaMouseX = Input.GetAxis("Mouse X") * sensitivity;
            float deltaMouseY = -Input.GetAxis("Mouse Y") * sensitivity;

            if (_freeLook)
            {
                // No input for both controller and animation component. We only want to rotate the camera

                _freeLookInput.x += deltaMouseX;
                _freeLookInput.y += deltaMouseY;

                _freeLookInput.x = Mathf.Clamp(_freeLookInput.x, -freeLookAngle.x, freeLookAngle.x);
                _freeLookInput.y = Mathf.Clamp(_freeLookInput.y, -freeLookAngle.y, freeLookAngle.y);

                return;
            }

            _freeLookInput = FPSAnimLib.ExpDecay(_freeLookInput, Vector2.zero, 15f, Time.deltaTime);
            
            _playerInput.x += deltaMouseX;
            _playerInput.y += deltaMouseY;
            
            _playerInput.y = Mathf.Clamp(_playerInput.y, -90f, 90f);
            moveRotation *= Quaternion.Euler(0f, deltaMouseX, 0f);
            TurnInPlace();

            _jumpState = FPSAnimLib.ExpDecay(_jumpState, _isInAir ? 1f : 0f, 10f, Time.deltaTime);

            float moveWeight = Mathf.Clamp01(Mathf.Abs(_smoothMove.magnitude));
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, moveWeight);
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, _jumpState);
            _playerInput.x *= 1f - moveWeight;
            _playerInput.x *= 1f - _jumpState;
            
            charAnimData.SetAimInput(_playerInput);
            charAnimData.AddDeltaInput(new Vector2(deltaMouseX, charAnimData.deltaAimInput.y));
        }

        private void UpdateFiring()
        {
            if (recoilComponent == null) return;
            
            if (recoilComponent.fireMode != FireMode.Semi && _fireTimer >= 60f / GetGun().fireRate)
            {
                Fire();

                if (recoilComponent.fireMode == FireMode.Burst)
                {
                    _bursts--;

                    if (_bursts == 0)
                    {
                        _fireTimer = -1f;
                        OnFireReleased();
                    }
                    else
                    {
                        _fireTimer = 0f;
                    }
                }
                else
                {
                    _fireTimer = 0f;
                }
            }

            if (_fireTimer >= 0f)
            {
                _fireTimer += Time.deltaTime;
            }
        }

        private bool IsZero(float value)
        {
            return Mathf.Approximately(0f, value);
        }

        private float verticalVelocity = 0f;
        private float smoothIsMoving = 0f;

        private void UpdateMovement()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            charAnimData.moveInput = new Vector2(moveX, moveY);

            moveX *= 1f - _jumpState;
            moveY *= 1f - _jumpState;

            Vector2 rawInput = new Vector2(moveX, moveY);
            Vector2 normInput = new Vector2(moveX, moveY);
            normInput.Normalize();
            
            if ((IsZero(normInput.y) || !IsZero(normInput.x)) 
                && movementState == FPSMovementState.Sprinting)
            {
                SprintReleased();
            }
            
            if (movementState == FPSMovementState.Sprinting)
            {
                normInput.x = rawInput.x = 0f;
                normInput.y = rawInput.y = 2f;
            }

            _smoothMove = FPSAnimLib.ExpDecay(_smoothMove, normInput, moveSmoothing, Time.deltaTime);

            moveX = _smoothMove.x;
            moveY = _smoothMove.y;
            
            charAnimData.moveInput = normInput;

            _smoothAnimatorMove.x = FPSAnimLib.ExpDecay(_smoothAnimatorMove.x, rawInput.x, 5f, Time.deltaTime);
            _smoothAnimatorMove.y = FPSAnimLib.ExpDecay(_smoothAnimatorMove.y, rawInput.y, 4f, Time.deltaTime);
            
            bool idle = Mathf.Approximately(0f, normInput.magnitude);
            animator.SetBool(Moving, !idle);
            
            smoothIsMoving = FPSAnimLib.ExpDecay(smoothIsMoving, idle ? 0f : 1f, curveLocomotionSmoothing, 
                Time.deltaTime);
            
            animator.SetFloat(Velocity, Mathf.Clamp01(smoothIsMoving));
            animator.SetFloat(MoveX, _smoothAnimatorMove.x);
            animator.SetFloat(MoveY, _smoothAnimatorMove.y);

            Vector3 move = transform.right * moveX + transform.forward * moveY;

            if (_isInAir)
            {
                verticalVelocity -= gravity * Time.deltaTime;
                verticalVelocity = Mathf.Max(-30f, verticalVelocity);
            }
            
            move.y = verticalVelocity;
            controller.Move(move * speed * Time.deltaTime);

            bool bWasInAir = _isInAir;
            _isInAir = !controller.isGrounded;
            animator.SetBool(InAir, _isInAir);

            if (!_isInAir)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    verticalVelocity = jumpHeight;
                }
            }

            if (bWasInAir != _isInAir)
            {
                if (_isInAir)
                {
                    SprintReleased();
                }
                else
                {
                    verticalVelocity = -0.5f;
                }

                slotLayer.PlayMotion(_isInAir ? onJumpMotionAsset : onLandedMotionAsset);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                disableInput = !disableInput;
                Application.Quit(0);
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                if (disableInput)
                {
                    freeCamera.gameObject.SetActive(true);
                    mainCamera.gameObject.SetActive(false);
                }
                else
                {
                    freeCamera.gameObject.SetActive(false);
                    mainCamera.gameObject.SetActive(true);
                }
            }

            if (!disableInput)
            {
                UpdateActionInput();
                UpdateLookInput();
                UpdateFiring();
                UpdateMovement();
            }
            
            UpdateAnimController();
        }

        private Quaternion _smoothBodyCam;

        /*
        private void ApplyBodyCamRotation()
        {
            Vector2 finalInput = new Vector2(_playerInput.x, _playerInput.y);

            var holderOffset = Quaternion.Euler(finalInput.y, finalInput.x, 0f);
            var headRot = firstPersonCamera.rotation;
            var rootRot = transform.rotation * holderOffset;
            
            cameraHolder.rotation = Quaternion.Slerp(headRot, rootRot, stabilizationWeight);
            cameraHolder.position = firstPersonCamera.position;

            var target = cameraHolder.rotation * Quaternion.Euler(_freeLookInput.y, _freeLookInput.x, 0f);
            _smoothBodyCam = FPSAnimLib.ExpDecay(_smoothBodyCam, target, bodyCameraSpeed, Time.deltaTime);
            
            mainCamera.rotation = _smoothBodyCam;
        }
        */

        public void UpdateCameraRotation()
        {
            Vector2 finalInput = new Vector2(_playerInput.x, _playerInput.y);
            (Quaternion, Vector3) cameraTransform =
                (transform.rotation * Quaternion.Euler(finalInput.y, finalInput.x, 0f),
                    firstPersonCamera.position);

            if (cameraState == FPSCameraState.InFront)
            {
                //cameraTransform = (frontCamera.rotation, frontCamera.position);
            }

            if (cameraState == FPSCameraState.Barrel)
            {
                //cameraTransform = (barrelCamera.rotation, barrelCamera.position);
            }

            cameraHolder.rotation = cameraTransform.Item1;
            cameraHolder.position = cameraTransform.Item2;

            mainCamera.rotation = cameraHolder.rotation * Quaternion.Euler(_freeLookInput.y, _freeLookInput.x, 0f);
        }
    }
}