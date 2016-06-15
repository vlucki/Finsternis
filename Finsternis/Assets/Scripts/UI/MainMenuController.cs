using System.Collections;
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

    public void FocusButton(BaseEventData data)
    {
        StartCoroutine(Rotate(Quaternion.Euler(new Vector3(0, 0, 360 - data.selectedObject.transform.parent.localRotation.eulerAngles.z))));
    }

    private IEnumerator Rotate(Quaternion target)
    {
        _eventSystem.sendNavigationEvents = false;
        while (Quaternion.Angle(target, _optionsContainer.transform.rotation) > _rotationThreshold)
        {
            _optionsContainer.transform.rotation = Quaternion.Slerp(_optionsContainer.transform.rotation, target, _slerpAmount);
            yield return new WaitForEndOfFrame();
        }
        _optionsContainer.transform.rotation = target;
        _eventSystem.sendNavigationEvents = true;
    }
}

