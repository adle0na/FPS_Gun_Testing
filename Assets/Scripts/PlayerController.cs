using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    
    // Editable Variables
    [SerializeField] private float walkSpeed;

    [SerializeField] private float runSpeed;
    
    [SerializeField] private float lookSensitivity;

    [SerializeField] private float cameraRotationLimit;
    
    [SerializeField] private float crouchSpeed;
        
    [SerializeField] private float crouchPosY;
    
    [SerializeField] private float jumpForce;
    
    // 
    private float applySpeed;
    private float currentCameraRotationX = 0f;
    private float originPosY;
    private float applyCrouchPosY;
    
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;
    
    [SerializeField] private Camera theCamera;
    private Rigidbody playerRb;
    private CapsuleCollider capsuleCollider;
    
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerRb = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    void Update()
    {
        IsGround();
         TryJump();
         TryRun();
         TryCrouch();
         Move();
         CameraRotation();
         CharactorRotation();
    }
    #endregion
    
    #region Functions
    
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancle();
        }

    }

    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGround)
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        isCrouch = !isCrouch;

        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());
    }

    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while (_posY != applyCrouchPosY)
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.2f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15)
                break;
            yield return null;
        }

        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }

    private void IsGround()
    {
        isGround = Physics.Raycast
            (transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if(isCrouch)
           Crouch();

        playerRb.velocity = transform.up * jumpForce;
    }

    private void Running()
    {
        if(isCrouch)
            Crouch();
        isRun = true;
        applySpeed = runSpeed;
    }

    private void RunningCancle()
    {
        isRun = false;

        applySpeed = walkSpeed;
    }

    private void Move()
    {
        float _moveDirx = Input.GetAxisRaw("Horizontal");
        float _moveDirz = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirx;
        Vector3 _moveVertical = transform.forward * _moveDirz;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;
        
        playerRb.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;

        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp
            (currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private void CharactorRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");

        Vector3 _charactorRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        playerRb.MoveRotation(playerRb.rotation * Quaternion.Euler(_charactorRotationY));
    }
    #endregion
}