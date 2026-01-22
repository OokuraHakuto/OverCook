using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCTRL : MonoBehaviour
{
    public Animator animator;
    public string animName;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.Play(animName);

    }
}