using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class CollectibleTotalText : MonoBehaviour
{
    [SerializeField] public Image _memoryImage;
    private TextMeshProUGUI _memoryTotalText;
    private CollectibleManager _collectibleManager;
    
    void Start()
    {
        _memoryTotalText = GetComponent<TextMeshProUGUI>();
        _collectibleManager = FindObjectOfType<CollectibleManager>();

        _memoryTotalText.text += $" \n{_collectibleManager.CollectibleCount} memories.\n";

        switch (_collectibleManager.CollectibleCount)
        {
            case {} when _collectibleManager.CollectibleCount is >= 5 and < 10:
                _memoryTotalText.text += "It warms your heart.";
                _memoryImage.enabled = false;
                break;
            case {} when _collectibleManager.CollectibleCount is >= 10 and < 15:
                _memoryTotalText.text += "It puts a smile on your face - and \ncan't help but reminiscence back home.";
                _memoryImage.enabled = true;
                break;
            case {} when _collectibleManager.CollectibleCount is >= 15 and < 20:
                _memoryTotalText.text += "It brings immense joy to your heart.";
                _memoryImage.enabled = true;
                break;
            default:
                _memoryTotalText.text += "It brings a little sadness to your heart.";
                _memoryImage.enabled = false;
                break;
        }
    }
}
