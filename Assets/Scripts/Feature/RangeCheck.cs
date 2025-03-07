using System;
using System.Collections;
using System.Collections.Generic;
using Lovatto.MobileInput;
using UnityEngine;

public class RangeCheck : MonoBehaviour
{
    private List<Target> targets = new List<Target>();
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Target>(out Target target))
        {
            if (!targets.Contains(target))
            {
                targets.Add(target);
            }
        }
    }
    
    public void DestroyAll()
    {
        foreach (Target target in targets)
        {
            target.Got(3);
        }
    }
}
