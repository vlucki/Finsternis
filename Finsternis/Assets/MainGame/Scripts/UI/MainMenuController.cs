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

    private GameObject selectedButton;

    void Awake()
    {
        if (!_optionsContainer)
            _optionsContainer = transform.GetChild(0).gameObject;

        if (!_eventSystem)
            _eventSystem = FindObjectOfType<EventSystem>();

        selectedButton = EventSystem.current.firstSelectedGameObject;
    }

    public void FocusButton(BaseEventData data)
    {
        Transform toRotate = data.selectedObject.transform;
        Transform parent = toRotate.parent;
        while(parent != null && parent.name != "OptionsContainer")
        {
            toRotate = parent;
            parent = toRotate.parent;
        }
        StartCoroutine(Rotate(Quaternion.Euler(new Vector3(0, 0, 360 - toRotate.localRotation.eulerAngles.z)), data.selectedObject));
    }

    private IEnumerator Rotate(Quaternion targetRotation, GameObject currentlySelected)
    {
        CanvasGroup lastSelectedDisplay = GameObject.Find(selectedButton.name + "Display").GetComponent<CanvasGroup>();
        CanvasGroup currentlySelectedDisplay = GameObject.Find(currentlySelected.name + "Display").GetComponent<CanvasGroup>();
        _eventSystem.sendNavigationEvents = false;
        float startAngle = Quaternion.Angle(targetRotation, _optionsContainer.transform.rotation);
        float angle = Quaternion.Angle(targetRotation, _optionsContainer.transform.rotation);
        while (angle > _rotationThreshold)
        {
            _optionsContainer.transform.rotation = Quaternion.Slerp(_optionsContainer.transform.rotation, targetRotation, _slerpAmount);
            if (lastSelectedDisplay)
                lastSelectedDisplay.alpha = angle / startAngle;
            if (currentlySelectedDisplay)
                currentlySelectedDisplay.alpha = 1 - angle / startAngle;
            angle = Quaternion.Angle(targetRotation, _optionsContainer.transform.rotation);
            yield return new WaitForEndOfFrame();
        }
        selectedButton = currentlySelected;
        _optionsContainer.transform.rotation = targetRotation;
        _eventSystem.sendNavigationEvents = true;
    }
}

