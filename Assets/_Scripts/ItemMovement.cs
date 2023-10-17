using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovement : MonoBehaviour
{
    private Camera mainCam;
    private void Awake()
    {
        mainCam = Camera.main;
    }
    Vector3 startPos, endPos;
    GameObject startObject, endObject;
    bool isDrawing = true;
    private void Update()
    {
        var pos = mainCam.ScreenPointToRay(Input.mousePosition);        
        if(Input.GetMouseButtonDown(0))
        {
            startPos = pos.origin;
            Collider2D[] col = Physics2D.OverlapCircleAll(startPos, 0.1f);
            startObject = col[0].gameObject;
        }
        if(Input.GetMouseButtonUp(0))
        {
            endPos = pos.origin;
            Collider2D[] col = Physics2D.OverlapCircleAll(endPos, 0.1f);
            endObject = col[0].gameObject;

            var dir = (endPos - startPos).normalized;

            if(startObject != null && endObject != null)
            {
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Debug.Log(angle);
            }

            isDrawing = false;
        }
        if(!isDrawing && startObject != null && endObject != null)
        {
            isDrawing = true;
            var tempPos = startObject.transform.position;
            startObject.transform.position = endObject.transform.position;
            endObject.transform.position = tempPos;
        }
        
        
        //if(col.Length > 0 )
        //    Debug.Log(col[0].name);

    }
    private void OnDrawGizmos()
    {
        if(!isDrawing)
            Gizmos.DrawLine(startPos, endPos);
    }
    //private float[,] angles = { {-10f, 12f}, {-174, 152}, {0,3 }, {0,4 } };
    /*
    right -> 12, -10
    left -> 152 -174
    up -> 98 69
    down -> -110 -71
     */
}
