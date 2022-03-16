using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{

    private Camera _myCamera;

    [SerializeField] private float fieldOfView = 24;
    private void Awake()
    {
        _myCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        _myCamera.fieldOfView = Camera.HorizontalToVerticalFieldOfView(fieldOfView, Screen.width / (float)Screen.height);
    }


}
