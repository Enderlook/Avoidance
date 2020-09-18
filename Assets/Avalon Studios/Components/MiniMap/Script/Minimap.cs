﻿using AvalonStudios.Additions.Attributes;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AvalonStudios.Additions.Components.MinimapSystem
{
    public class Minimap : MonoBehaviour
    {
        // Properties
        public Transform Target
        {
            get
            {
                if (target != null)
                    return target.transform;
                return null;
            }
        }

        public Vector3 TargetPosition
        {
            get
            {
                Vector3 pos = Vector3.zero;
                if (target != null)
                    pos = target.transform.position;
                return pos;
            }
        }

        public float MinimapHeight => minimapHeight;

        public float Size => size;

        public float NearClipPlane => nearClippingPlanes;

        public float FarClipPlane => farClippingPlanes;

        public RenderTexture TargetTexture => targetTexture;

        public RectTransform MinimapRoot => minimapRoot;

        public Sprite MinimapRootFormMask => minimapRootFormMask;

        public GameObject TargetIcon => targetIcon;

        public Color TargetIconColor => targetIconColor;

        public Color TargetIconOutlineColor => targetIconOutlineColor;

        // Variables

        [StyledHeader("General")]
        [SerializeField, Tooltip("Target for the Minimap.")]
        private GameObject target;

        [SerializeField, Tooltip("Tag of the target.")]
        private string targetTag = "";

        [SerializeField, Tooltip("Camera Y position.")]
        private float minimapHeight = .1f;

        [SerializeField, Tooltip("Speed of the follow target.")]
        private float speedFollow = 10f;

        [StyledHeader("Camera Settings")]
        [SerializeField, Tooltip("The vertical size of the camera.")]
        private float size = 5;

        [SerializeField, Tooltip("The closest point relative to the camera that drawing occurs.")]
        private float nearClippingPlanes = .01f;

        [SerializeField, Tooltip("The furthest point relative to the camera that drawing occurs.")]
        private float farClippingPlanes = 500f;

        [SerializeField, Tooltip("The texture to render this camera into.")]
        private RenderTexture targetTexture = null;

        [StyledHeader("Rotation Setup")]
        [SerializeField, Tooltip("Smooth rotation?")]
        private bool smoothRotation = true;

        [SerializeField, Tooltip("Lerp rotation.")]
        private float lerpRotation = 8f;

        [StyledHeader("UI")]
        [SerializeField, Tooltip("Minimap canvas.")]
        private Canvas minimapCanvas = null;

        [SerializeField, Tooltip("Minimap root.")]
        private RectTransform minimapRoot = null;

        [SerializeField, Tooltip("Minimap form")]
        private Sprite minimapRootFormMask = null;

        [SerializeField, Tooltip("Target icon.")]
        private GameObject targetIcon = null;

        [SerializeField, Tooltip("Color of the target icon.")]
        private Color targetIconColor = Color.white;

        [SerializeField, Tooltip("Outline color of the target icon")]
        private Color targetIconOutlineColor = Color.red;

        [StyledHeader("Map Rect")]
        [SerializeField, Tooltip("Minimap position.")]
        private Vector3 minimapPosition = Vector2.zero;

        [SerializeField, Tooltip("Minimap rotation.")]
        private Vector3 minimapRotation = Vector3.zero;

        [SerializeField, Tooltip("Minimap size.")]
        private Vector2 minimapSize = Vector2.zero;

        private static Camera minimapCamera = null;
        private static RectTransform root = null;

        private RectTransform targetIconRect;
        private Vector3 currentVel = Vector3.zero;

        private void Awake()
        {
            if (target == null && targetTag.Length != 0)
                target = GameObject.FindGameObjectWithTag(targetTag);

            minimapCamera = transform.GetChild(0).GetComponent<Camera>();
            root = minimapRoot;

            targetIconRect = targetIcon.GetComponent<RectTransform>();
        }

        void Update()
        {
            if (target == null)
                return;

            FollowTarget(Time.deltaTime);
            MinimapRotation(Time.deltaTime);
        }

        private void FollowTarget(float time)
        {
            Vector3 pos = transform.position;

            pos.x = TargetPosition.x;
            pos.z = TargetPosition.z;

            if (Target != null)
            {
                Vector3 targetPoint = minimapCamera.WorldToViewportPoint(TargetPosition);
                targetIconRect.anchoredPosition = MinimapUtils.CalculatePosition(targetPoint, root);
            }
            
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref currentVel, time / speedFollow);
        }

        private void MinimapRotation(float time)
        {
            Vector3 rot = transform.eulerAngles;
            rot.y = Target.eulerAngles.y;
            if (smoothRotation)
            {
                targetIconRect.eulerAngles = Vector3.zero;

                if (transform.eulerAngles.y != rot.y)
                {
                    float dif = rot.y - transform.eulerAngles.y;
                    if (dif > 180 || dif < -180)
                        transform.eulerAngles = rot;
                    transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, rot, time * lerpRotation);
                }
            }
            else
                transform.eulerAngles = rot;
        }
    }
}