using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTracker : MonoBehaviour
{
    [SerializeField] Transform targetObj, headTarget;
    [SerializeField] Vector3 offset;
    Transform myTransform;
    [SerializeField] bool isFakeHead;

    private void Awake()
    {
        myTransform = transform;
        if (isFakeHead)
        {
            transform.position = headTarget.position;
            transform.rotation = headTarget.rotation;
            transform.localScale = headTarget.localScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObj != null) {
            Vector3 temp = targetObj.position + offset;
            myTransform.position = new Vector3(temp.x, -4.79f, temp.z);
        }
        else
            Debug.Log("Error");
    }
}
