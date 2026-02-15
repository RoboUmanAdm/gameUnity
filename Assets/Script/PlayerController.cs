using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private PhotonView pv;
    [SerializeField] private float rotateSpeed;
    [SerializeField]private CameraFollow myCamScript;
    private PlayerInput inputActions;
    private CharacterController controller;
    private Animator animator;
    private Vector2 movementInput;
    private Vector3 currentMovement;
    private Quaternion rotateDir;
    private bool isRun;
    private bool isWalk;

    private void OnMovementActions(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        currentMovement.x = movementInput.x;
        currentMovement.z = movementInput.y;
        isWalk = movementInput.x != 0 || movementInput.y != 0;
    }

    private void Awake()
    {
        pv = GetComponentInParent<PhotonView>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        inputActions = new PlayerInput();
        inputActions.CharacterControls.Movement.started += OnMovementActions;
        inputActions.CharacterControls.Movement.performed += OnMovementActions;
        inputActions.CharacterControls.Movement.canceled += OnMovementActions;
        inputActions.CharacterControls.Run.started += OnRun;
        inputActions.CharacterControls.Run.canceled += OnRun;
        inputActions.CharacterControls.Movement.started += OnCameraMovement;
        inputActions.CharacterControls.Movement.performed += OnCameraMovement;
        inputActions.CharacterControls.Movement.canceled += OnCameraMovement;
        if(!pv.IsMine)
        {
            Destroy(myCamScript.gameObject);
        }
    }
    
    private void OnEnable() {
        inputActions.CharacterControls.Enable();
    }
    private void OnDisable() {
        inputActions.CharacterControls.Disable();
    }

    private void PlayerRotate()
    {
        if (isWalk)
        {
            rotateDir = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(currentMovement), Time.deltaTime * rotateSpeed);
            transform.rotation = rotateDir;
        }
    }
    private void AnimateControl()
    {
        animator.SetBool("isWalk", isWalk);
        animator.SetBool("isRun", isRun);
    }

    private void Update()
    {
        if(!pv.IsMine) return;
        AnimateControl();
        PlayerRotate();
    }
    private void FixedUpdate()
    {
        if(!pv.IsMine) return;
        controller.Move(currentMovement*Time.fixedDeltaTime);
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        isRun = context.ReadValueAsButton();
    }

public void Respawn()
    {
        controller.enabled = false;
        transform.position = Vector3.up;
        controller.enabled = true;
    }
    private void OnCameraMovement(InputAction.CallbackContext context)
    {
        myCamScript.SetOffset(currentMovement);
    }
}
