﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupMovement : MonoBehaviour
{
    [SerializeField] private float mouseSpeed   = 1;
    [SerializeField] private float brakeSpeed   = 0;
    [SerializeField] private bool inertia       = false;

    Rubickscube rubick;

    Vector3 refPos;
    Vector3 refNormal;
    Vector3 lastLocation;

    Plane plane;

    float magnitude;
    Vector3 axis;

    bool rotating = false;

    [SerializeField] private float rayCastLength = 1000.0f;

    // Start is called before the first frame update
    void Start()
    {
        rubick = GetComponent<Rubickscube>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !rotating)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayCastLength))
            {
                refPos = hit.point;
                refNormal = hit.normal;
                plane.SetNormalAndPosition(refNormal, refPos);
                rotating = true;
            }
        }

        else if (Input.GetMouseButton(1) && rotating)
        {
            /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayCastLength) && !rubick.rotate)
            {*/


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;

            if (plane.Raycast(ray, out distance))
            {
                Vector3 point = ray.GetPoint(distance);
                /*if (lastLocation == point)
                {
                    return;
                }*/

                Vector3 vect = point - refPos;

                //print(vect.magnitude);

                axis = Vector3.Cross(refNormal, vect);

                transform.rotation = Quaternion.AngleAxis(vect.magnitude * mouseSpeed * Time.deltaTime, axis) * transform.rotation;

                lastLocation = point;

                magnitude = vect.magnitude;
            }
        }

        else
        {
            if (magnitude > 0 && inertia && !rubick.rotate)
            {
                transform.rotation = Quaternion.AngleAxis(magnitude * mouseSpeed * Time.deltaTime, axis) * transform.rotation;
                magnitude -= brakeSpeed;
            }

            else
            {
                magnitude = 0;
            }
            rotating = false;
        }
    }

}
