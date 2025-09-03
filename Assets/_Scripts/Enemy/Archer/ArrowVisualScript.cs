using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class ArrowVisualScript : MonoBehaviour
{
    public Transform arrowVisual;
    public Transform arrowShadow;
    public ArrowScript arrowScript;

    private Vector3 arrowRange;
    private Vector3 startPos;
    private Vector3 target;

    private float shadowPos = 6f;

    void Update()
    {
        UpdateRotationVisual();
        UpdateShadow();

        float progress = (transform.position - startPos).magnitude;
        float arrowMagnitude = (target - startPos).magnitude;
        float normalized = progress / arrowMagnitude;

        if (normalized < .8f)
        {
            UpdateRotationShadow();
        }
    }

    void UpdateRotationVisual()
    {
        Vector3 moveDir = arrowScript.GetArrowMoveDirection();
        arrowVisual.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg);
    }

     void UpdateRotationShadow()
    {
        Vector3 moveDir = arrowScript.GetArrowMoveDirection();
        arrowShadow.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg);
    }

    void UpdateShadow()
    {
        Vector3 newPos = transform.position;

        if (Mathf.Abs(arrowRange.normalized.x) < Mathf.Abs(arrowRange.normalized.y))
        {
            newPos.x = startPos.x + arrowScript.GetNextPosX() / shadowPos + arrowScript.AbsCorrectionX();
        }
        else
        {
            newPos.y = startPos.y + arrowScript.GetNextPosY() / shadowPos + arrowScript.AbsCorrectionY();
        }

        arrowShadow.position = newPos;
    }

    public void Initialize(Vector3 range, Vector3 startPos, Vector3 target)
    {
        arrowRange = range;
        this.startPos = startPos;
        this.target = target;
    } 
}

