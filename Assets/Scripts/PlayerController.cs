﻿using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public short activeBlock = 2;
    public float movementSpeed = 1f;
    public float mouseSensitivity = 3f;
    public KeyCode speedUpButton = KeyCode.LeftShift;
    public GameObject blockFrame;
    public float maxYRotation = 80;
    public float minYRotation = 10;
    float Yrotation;

    Camera mainCam;
    TerrainManager terrain;
    TerrainRaycaster raycaster;
    LabelRenderer labelRenderer;

    private void Start() {
        raycaster = GlobalObject.Get<TerrainRaycaster>();
        labelRenderer = GlobalObject.Get<LabelRenderer>();
        terrain = GlobalObject.Get<TerrainManager>();
        mainCam = GetComponentInChildren<Camera>();

        Yrotation = transform.rotation.eulerAngles.x;
    }

    private void Update() {
        TerrainRaycaster.RaycastResult? lookingAt = raycaster.LookingAt(mainCam, 10);
        if (lookingAt.HasValue)
        {
            Block block = BlockManager.blocks[lookingAt.Value.value.index];
            labelRenderer.AddLabel($"looking at: {lookingAt.Value.point} - {block.name} - {lookingAt.Value.face}");
            blockFrame.GetComponent<BoundsRenderer>().bounds = block.boundingBoxes[0];
            blockFrame.transform.position = lookingAt.Value.point;
            blockFrame.SetActive(true);

            if (Input.GetMouseButton(0))
            {
                terrain.SetCell(lookingAt.Value.point, 0);
            }
            else if (Input.GetMouseButton(1))
            {
                terrain.SetCell(lookingAt.Value.point + CellFace.Faces[lookingAt.Value.face], activeBlock);
            }
        }
        else
        {
            blockFrame.SetActive(false);
        }

        float dt = Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X");
        float rotY = Mathf.Clamp(-Input.GetAxis("Mouse Y") * mouseSensitivity * dt + Yrotation, minYRotation, maxYRotation) - Yrotation;
        Yrotation += rotY;

        transform.Rotate(new Vector3(rotY, 0, 0), Space.Self);
        transform.Rotate(new Vector3(0, mouseX, 0) * mouseSensitivity * dt, Space.World);

        float right = Input.GetAxisRaw("Horizontal");
        float forward = Input.GetAxisRaw("Vertical");

        transform.position += (right * transform.right + forward * transform.forward) * movementSpeed * dt * (Input.GetKey(speedUpButton) ? 5 : 1);
    }

    private void OnEnable() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
