using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    public LayerMask touchInputMask;

    private List<GameObject> touchList = new List<GameObject>();
    private GameObject[] touchesOld;
    private RaycastHit hit;

    void Update()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            touchesOld = new GameObject[touchList.Count];
            touchList.CopyTo(touchesOld);
            touchList.Clear();

            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, touchInputMask))
            {
                GameObject recipient = hit.transform.gameObject;
                touchList.Add(recipient);

                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    Vector4 newPoint = new Vector4(hit.point.x, hit.point.y, hit.point.x, touchList.Count);
                    recipient.SendMessage("OnTouchDown", newPoint, SendMessageOptions.DontRequireReceiver);
                }
                if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
                {
                    Vector4 newPoint = new Vector4(hit.point.x, hit.point.y, hit.point.x, touchList.Count);
                    recipient.SendMessage("OnTouchUp", newPoint, SendMessageOptions.DontRequireReceiver);
                }
                if (Input.GetMouseButton(0))
                {
                    Vector4 newPoint = new Vector4(hit.point.x, hit.point.y, hit.point.x, touchList.Count);
                    recipient.SendMessage("OnTouchStay", newPoint, SendMessageOptions.DontRequireReceiver);
                }
            }

            foreach (GameObject g in touchesOld)
            {
                if (!touchList.Contains(g) && g != null)
                {
                    g.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
#endif


        if (Input.touchCount > 0)
        {
            touchesOld = new GameObject[touchList.Count];
            touchList.CopyTo(touchesOld);
            touchList.Clear();

            foreach (Touch touch in Input.touches)
            {
                Ray ray = GetComponent<Camera>().ScreenPointToRay(touch.position);

                if (Physics.Raycast(ray, out hit, touchInputMask))
                {
                    GameObject recipient = hit.transform.gameObject;
                    touchList.Add(recipient);

                    if (touch.phase == TouchPhase.Began)
                    {
                        //recipient.SendMessage("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
                        Vector4 newPoint = new Vector4(hit.point.x, hit.point.y, hit.point.x, touch.fingerId);
                        recipient.SendMessage("OnTouchDown", newPoint, SendMessageOptions.DontRequireReceiver);
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        Vector4 newPoint = new Vector4(hit.point.x, hit.point.y, hit.point.x, touch.fingerId);
                        recipient.SendMessage("OnTouchUp", newPoint, SendMessageOptions.DontRequireReceiver);
                    }
                    if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                    {
                        Vector4 newPoint = new Vector4(hit.point.x, hit.point.y, hit.point.x, touch.fingerId);
                        recipient.SendMessage("OnTouchStay", newPoint, SendMessageOptions.DontRequireReceiver);
                    }
                    if (touch.phase == TouchPhase.Canceled)
                    {
                        Vector4 newPoint = new Vector4(hit.point.x, hit.point.y, hit.point.x, touch.fingerId);
                        recipient.SendMessage("OnTouchExit", newPoint, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            foreach (GameObject g in touchesOld)
            {
                if (!touchList.Contains(g))
                {
                    g.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

    }
}