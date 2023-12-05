using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVis : MonoBehaviour
{
    public Transform player;
    public float smoothing = 1f;
    public float alpha = 0.2f;


    private Camera cam;
    private GameObject frontObject;
    private bool isBehind;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    private void Update()
    {
        Vector3 dir = player.position - transform.position;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            if(hit.collider.tag != "Player")
            {
                if(frontObject == null)
                {
                    frontObject = hit.collider.gameObject;
                    if(frontObject.GetComponent<VisObject>() != null)
                        frontObject.GetComponent<VisObject>().reduceAlpha = true;
                }
            }
            else {
                if(frontObject != null){
                    if(frontObject.GetComponent<VisObject>() != null)
                        frontObject.GetComponent<VisObject>().reduceAlpha = false;
                    frontObject = null;
                }
            }
        }
    }
}
