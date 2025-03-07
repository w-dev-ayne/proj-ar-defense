using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rader : MonoBehaviour
{
    public Transform camTransform;

    private RectTransform rect;

    private void OnEnable()
    {
        rect = this.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rect.localEulerAngles = new Vector3(0, 0, -camTransform.localEulerAngles.y);
    }
}
