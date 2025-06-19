// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;

/// First-person player controller for Resonance Audio demo scenes.
[RequireComponent(typeof(CharacterController))]
public class DemoPlayerController : MonoBehaviour
{
    [Header("Setup")]
    public Camera mainCamera;

    [Header("Control Toggle")]
    public bool enableInput = true;  // Toggle input on/off from Inspector

    private CharacterController characterController = null;

    // Rotation state
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    // Constants
    private const float clampAngleDegrees = 80.0f;
    private const float sensitivity = 2.0f;
    private const float movementSpeed = 5.0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (mainCamera == null)
        {
            Debug.LogWarning("MainCamera reference not set in DemoPlayerController.");
            return;
        }

        Vector3 rotation = mainCamera.transform.localRotation.eulerAngles;
        rotationX = rotation.x;
        rotationY = rotation.y;
    }

    void LateUpdate()
    {
        if (!enableInput) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            SetCursorLock(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorLock(false);
        }
#endif

        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            mouseX = 0.0f;
            mouseY = 0.0f;
        }

        rotationX += sensitivity * mouseY;
        rotationY += sensitivity * mouseX;
        rotationX = Mathf.Clamp(rotationX, -clampAngleDegrees, clampAngleDegrees);

        if (mainCamera != null)
        {
            mainCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        }
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(moveX, 0.0f, moveZ);

        if (mainCamera != null)
        {
            direction = mainCamera.transform.localRotation * direction;
        }

        direction.y = 0.0f;
        characterController.SimpleMove(movementSpeed * direction);
    }

    private void SetCursorLock(bool lockCursor)
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}