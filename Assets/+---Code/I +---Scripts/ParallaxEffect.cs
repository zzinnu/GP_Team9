using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;

    // BG position
    Vector2 startingPosition;
    float startingZ;

    // Camera 이동 속도 계산용
    Vector2 camMoveSinceStart => (Vector2)cam.transform.position - startingPosition;

    // Target과 BG의 거리
    float zDistanceFromTarget => transform.position.z - followTarget.position.z;

    float clippingPlane => (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane));
    float parallaxFactor => Mathf.Abs(zDistanceFromTarget) / clippingPlane;

    private void Start()
    {
        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    private void Update()
    {
        // origin + (travel * parallaxFactor)
        Vector2 newPosition = startingPosition + camMoveSinceStart * parallaxFactor;

        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }
}
