using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionToTouchPoint : MonoBehaviour
{
    public Transform _Target;
    public bool isCanvasObject;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 initialPosition;
    private float deltaX, deltaY;
    private void Start()
    {
        initialPosition = _Target.localPosition;
    }

    private void OnMouseDown()
    {
        if (isCanvasObject)
        {
            screenPoint = Camera.main.WorldToScreenPoint(Input.mousePosition); // I removed this line to prevent centring 
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            _Target.position = curPosition;
        }
        else
        {
            deltaX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
            deltaY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _Target.position = new Vector2(curPosition.x - deltaX, curPosition.y - deltaY);
        }

    }

    private void OnMouseUp()
    {
        _Target.localPosition = initialPosition;
    }
}
