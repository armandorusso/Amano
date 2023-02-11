using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmanoTimer : MonoBehaviour
{
    private float _duration;
    private bool _isInProgress;
    private bool _isTimerDone;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isInProgress)
        {
            _duration -= Time.deltaTime;
        }

        if (_duration < 0f)
        {
            _isTimerDone = true;
            StopTimer();
        }
    }

    public void StartTimer(float duration)
    {
        _isInProgress = true;
        _isTimerDone = false;
        _duration = duration;
    }

    public void StopTimer()
    {
        _isInProgress = false;
        _duration = 0f;
    }

    public void ResetTimer()
    {
        _duration = 0f;
        _isInProgress = false;
        _isTimerDone = true;
    }

    public bool IsTimerInProgress()
    {
        return _isInProgress;
    }
    
    public bool IsTimerDone()
    {
        return _isTimerDone;
    }
}

