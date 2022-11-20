using UnityEngine;
using UnityEngine.InputSystem;

public class TetsuoShuriken : MonoBehaviour
{
    [SerializeField] public SpriteRenderer shuriken;
    [SerializeField] public Transform shurikenPosition;
    [SerializeField] public Transform rotationPosition;
    [SerializeField] public MousePosTracker mouseTracker;

    // Update is called once per frame
    void Update()
    {
        Vector3 rotation = mouseTracker.GetMousePositionInWorld() - rotationPosition.position;
        float zRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rotationPosition.rotation = Quaternion.Euler(0, 0, zRotation);
    }

    public void ShootShuriken(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        var shurikenObj = Instantiate(shuriken.gameObject, shurikenPosition.position, Quaternion.identity);
    }
}
