using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject _optionsContainer;

    [SerializeField]
    private EventSystem _eventSystem;

    [SerializeField]
    [Range(0, 1)]
    private float _slerpAmount = 0.1f;

    private Quaternion _targetRotation;

    private bool _targetRotationReached = true;

    [SerializeField]
    [Range(0, 360)]
    private float _rotationThreshold = 1;

    void Awake()
    {
        if (!_optionsContainer)
            _optionsContainer = transform.GetChild(0).gameObject;

        if (!_eventSystem)
            _eventSystem = FindObjectOfType<EventSystem>();
    }

    public void Rotate(GameObject reference)
    {
        if (_targetRotationReached)
        {
            _targetRotation = Quaternion.Euler(new Vector3(0, 0, 360 - reference.transform.localRotation.eulerAngles.z));
            _targetRotationReached = Quaternion.Angle(_targetRotation, _optionsContainer.transform.rotation) <= _rotationThreshold;
        }
    }

    void Update()
    {
        if (!_targetRotationReached)
        {
            _optionsContainer.transform.rotation = Quaternion.Slerp(_optionsContainer.transform.rotation, _targetRotation, _slerpAmount);
            _targetRotationReached = Quaternion.Angle(_targetRotation, _optionsContainer.transform.rotation) <= _rotationThreshold;
        }
        else if (!_eventSystem.sendNavigationEvents)
            _eventSystem.sendNavigationEvents = true;
    }

    void LateUpdate()
    {
        if (!_targetRotationReached && _eventSystem.sendNavigationEvents)
            _eventSystem.sendNavigationEvents = false;
    }

}

