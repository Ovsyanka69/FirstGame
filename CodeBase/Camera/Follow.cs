using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform TargetToFollow;
    public float LerpRate;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, TargetToFollow.position, Time.deltaTime * LerpRate);
    }
}
