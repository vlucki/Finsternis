using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float _slerpAmount = 0.1f;

    private Quaternion _targetRotation;

    public void Rotate(GameObject reference)
    {
        _targetRotation = Quaternion.Euler(new Vector3(0, 0, 360 - reference.transform.localRotation.eulerAngles.z));
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _slerpAmount);
    }

}

