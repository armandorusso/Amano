using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableChildObjects : MonoBehaviour
{
    [SerializeField] public LayerMask ObjectLayers;

    private void OnEnable()
    {
        var parentTransform = gameObject.transform;

        foreach (Transform childTransform in parentTransform)
        {
            // https://youtu.be/VsmgZmsPV6w for explanation
            // Here you are taking the binary number of 1 and you are shifting it to the left by whatever is stored in the child object layer. If that number is equal to a layer in the ObjectLayers, we have our layer
            // Ex: childTransform.gameObject.layer = 8. 1 << 8 shifts the #1 8 spots to the left. So: 00000001 -> 10000000
            // this enables us to get a binary number where the 8th spot in the binary number has a 1 in it which means the layer is enabled
            // since 1 represents an active layer
            if (1 << childTransform.gameObject.layer == ObjectLayers)
            {
                childTransform.gameObject.SetActive(true);
            }
        }
    }
}
