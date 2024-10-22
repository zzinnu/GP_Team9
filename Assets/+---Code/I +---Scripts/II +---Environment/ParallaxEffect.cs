using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;

    // BG position
    Vector2 startingPosition;
    float startingZ;

    // Camera �̵� �ӵ� ����
    Vector2 camMoveSinceStart => (Vector2)cam.transform.position - startingPosition;

    // Target�� BG�� �Ÿ�
    float zDistanceFromTarget => transform.position.z - followTarget.position.z;

    float clippingPlane => (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane));
    float parallaxFactor => Mathf.Abs(zDistanceFromTarget) / clippingPlane;

    private void Start()
    {
        if(followTarget.IsUnityNull())
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            followTarget = player.transform;
        }

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
