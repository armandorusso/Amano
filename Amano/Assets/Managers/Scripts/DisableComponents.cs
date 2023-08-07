using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableComponents : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TetsuoDisableMovement.Instance.EnableOrDisableInputActions(false);
    }

}
