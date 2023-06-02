using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private float _jumpForceAmount;
    [SerializeField] private Vector2 _launchDirection;
    [SerializeField] public AudioClip TrampolineSound;

    private Animator _animator;
    private AudioSource _audioSource;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.rigidbody.velocity += _launchDirection * _jumpForceAmount;
            // Bug: jump + trampoline to gain a huge boost. Could be kept as a feature for fun levels?
            // The line below fixes this
            // col.rigidbody.velocity = new Vector2((_launchDirection.x + col.rigidbody.velocity.x) * _jumpForceAmount, _launchDirection.y);
            _animator.SetBool("hasSteppedOn", true);
            _audioSource.PlayOneShot(TrampolineSound);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _animator.SetBool("hasSteppedOn", false);
        }
    }
}
