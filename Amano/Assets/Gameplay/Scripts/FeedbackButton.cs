using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackButton : MonoBehaviour
{
    [SerializeField] private string Url;
    
    public void OnFeedbackClicked()
    {
        Application.OpenURL(Url);
    }
}
