using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventCallers : MonoBehaviour
{
    [SerializeField] private LayerMask m_layerMask; 
    [SerializeField] private bool m_useLayerMask; 
    [SerializeField] private UnityEvent<Collision2D> m_onCollisionEnter2D;
    [SerializeField] private UnityEvent<Collision2D> m_onCollsionExit2D;
    [SerializeField] private UnityEvent<Collider2D> m_onTriggerEnter2D;
    [SerializeField] private UnityEvent<Collider2D> m_onTriggerExit2D;
    [SerializeField] private UnityEvent m_onAwake;
    [SerializeField] private UnityEvent m_onStart;
    [SerializeField] private UnityEvent m_onEnable;
    [SerializeField] public UnityEvent m_onDestroy;

    private void OnDestroy()
    {
        m_onDestroy?.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(LayerCheck(other.gameObject))
            m_onCollisionEnter2D?.Invoke(other);
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        if(LayerCheck(other.gameObject))
            m_onCollsionExit2D?.Invoke(other);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(LayerCheck(other.gameObject))
            m_onTriggerEnter2D?.Invoke(other);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if(LayerCheck(other.gameObject))
            m_onTriggerExit2D?.Invoke(other);
    }

    private bool LayerCheck(GameObject other)
    {
        return !m_useLayerMask || m_layerMask == (m_layerMask | (1 << other.layer));
    }

    private void Awake()
    {
        m_onAwake?.Invoke();
    }
    
    private void Start()
    {
        m_onStart?.Invoke();
    }
    
    private void OnEnable()
    {
        m_onEnable?.Invoke();
    }
}