using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float moveHardness = 1;
    [SerializeField] private float lookHardness = 1;

    [HideInInspector] public Transform placeHolder;
    [HideInInspector] public Transform target;
    
    void Update()
    {
        if (target)
        {
            Vector3 position = transform.position;
            position = Vector3.Lerp(position, placeHolder.position, moveHardness * Time.deltaTime);
            transform.position = position;

            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookHardness * Time.deltaTime);
        }
    }
}
