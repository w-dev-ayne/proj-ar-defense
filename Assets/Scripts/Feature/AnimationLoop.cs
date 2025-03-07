using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationLoop : MonoBehaviour
{
    private Animation animation;

    private void OnEnable()
    {
        animation = this.GetComponent<Animation>();
    }

    private void Update()
    {
        if (animation.isPlaying)
        {
            return;
        }
        else
        {
            animation.Play();
        }
    }
}
