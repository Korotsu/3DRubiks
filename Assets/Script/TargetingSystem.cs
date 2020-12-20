﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    Rubickscube rubick;

    [SerializeField] private float      rayCastLength = 1000.0f;
    [SerializeField] private LayerMask  layer;

    public Vector3 rotationVector;

    Vector3 refPosLocal;

    Vector3 refPosWorld;
    Vector3 refNormal;
    Vector3 axis;

    Plane   plane;

    float height    = 0;
    float direction = 0;
    float oldFactor = 0;
    float factor    = 1;

    bool axisInit = false;

    bool animating = false;

    void Start()
    {
        rubick = GetComponent<Rubickscube>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !animating)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayCastLength))
            {
                refPosLocal      = transform.InverseTransformPoint(hit.point);
                refPosWorld    = hit.point;
                refNormal   = hit.normal;
                plane.SetNormalAndPosition(refNormal , refPosWorld);
                animating  = true;
            }
        }

        else if (Input.GetMouseButton(0) && animating)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;

            if (plane.Raycast(ray,out distance))
            {
                Vector3 point = ray.GetPoint(distance);

                if ((oldFactor == 0 || oldFactor == 1) && (refPosLocal - transform.InverseTransformPoint(point)).magnitude > 0.005f)
                {
                    axis = SortVector(refNormal, (point - refPosWorld).normalized);

                    axisInit = true;
                }

                if (axisInit && (refPosLocal - transform.InverseTransformPoint(point)).magnitude > 0.000001f)
                {
                   Vector3 localAxis = transform.InverseTransformPoint(axis);

                    height = Vector3.Dot(localAxis, refPosLocal);

                    float oldDirection = direction;

                    direction = Direction(axis, refPosWorld, point);

                    if (direction != oldDirection)
                    {
                        rubick.RotateLineAroundAxis(axis, height, 0, oldFactor, oldDirection);
                        oldFactor = 0;
                    }

                    Vector3 dir = Vector3.Cross(refNormal, axis);
                    factor = Mathf.Abs(Vector3.Dot(dir, (point - refPosWorld)));

                    if (factor > 1)
                    {
                        factor = 1;
                    }

                    rubick.rotate = true;
                    rubick.RotateLineAroundAxis(axis,height, factor, oldFactor,direction);

                    oldFactor = factor;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (oldFactor != 0 && oldFactor != 1)
            {
                if (oldFactor >= 0.5)
                {
                    rubick.RotateLineAroundAxis(axis, height, 1, oldFactor, direction);
                    oldFactor = 0;
                    axisInit = false;
                }

                else
                {
                    rubick.RotateLineAroundAxis(axis, height, 0, oldFactor, direction);
                    oldFactor = 0;
                    axisInit = false;
                }
            }

            else
            {
                oldFactor = 0;
            }

            animating = false;
            rubick.rotate = false;
        }
    }

    //Find the Rotation Axis by finding which face of the cube we selected and getting the vector that is the closest to the direction selected
    Vector3 SortVector(Vector3 normal, Vector3 direction)
    {
        Vector3 localNormal = transform.InverseTransformPoint(normal);
        Vector3 localDirection = transform.InverseTransformPoint(direction);

        localNormal.x = Mathf.Abs(localNormal.x);
        localNormal.y = Mathf.Abs(localNormal.y);
        localNormal.z = Mathf.Abs(localNormal.z);

        if ((localNormal - Vector3.forward).magnitude <= 0.01f)
        {
            if (Mathf.Abs(Vector3.Dot(localDirection, Vector3.up)) < Mathf.Abs(Vector3.Dot(localDirection, Vector3.right)))
            {
                return transform.up;
            }
            else
            {
                return transform.right;
            }
        }
        else if ((localNormal - Vector3.up).magnitude <= 0.01f)
        {
            if (Mathf.Abs(Vector3.Dot(localDirection, Vector3.right)) < Mathf.Abs(Vector3.Dot(localDirection, Vector3.forward)))
            {
                return transform.right;
            }
            else
            {
                return transform.forward;
            }
        }
        else if ((localNormal - Vector3.right).magnitude <= 0.01f)
        {
            if (Mathf.Abs(Vector3.Dot(localDirection, Vector3.forward)) < Mathf.Abs(Vector3.Dot(localDirection, Vector3.up)))
            {
                return transform.forward;
            }
            else
            {
                return transform.up;
            }
        }

        return Vector3.zero;
    }

    float Direction(Vector3 axis, Vector3 start, Vector3 end)
    {
        return Vector3.SignedAngle(start, end, axis) / Mathf.Abs(Vector3.SignedAngle(start, end, axis));
    }
}


