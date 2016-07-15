using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConfirmationDialogController : MonoBehaviour
{
    public UnityEvent onConfirm;
    public UnityEvent onCancel;

    [SerializeField]
    private Text _messageField;

    [SerializeField]
    private EventSystem _evtSystem;

    [SerializeField]
    private GameObject _yes;

    [SerializeField]
    private GameObject _no;

    private static ConfirmationDialogController _this;

    public string Message
    {
        get { return _messageField.text; }
        set { _messageField.text = value; }
    }

    public ConfirmationDialogController()
    {
        ConfirmationDialogController._this = this;
    }

    void Awake()
    {
        _evtSystem = FindObjectOfType<EventSystem>();
        _messageField = GetComponentInChildren<Text>();
        foreach (Button b in transform.GetComponentsInChildren<Button>())
        {
            if (b.name.Equals("Yes"))
                _yes = b.gameObject;
            else if (b.name.Equals("No"))
                _no = b.gameObject;
        }
    }

    void Update()
    {
        if (_evtSystem.currentSelectedGameObject != _yes && _evtSystem.currentSelectedGameObject != _no)
            _evtSystem.SetSelectedGameObject(_yes);
    }

    public void Confirm()
    {
        onConfirm.Invoke();
    }

    public void Cancel()
    {
        onCancel.Invoke();
    }


    internal static void Show<T>(string message, UnityAction<T> confirmationCallback, T confirmationParameter, UnityAction cancelationCallbak = null)
    {
        _this.onConfirm.AddListener(() => confirmationCallback(confirmationParameter));
        if (cancelationCallbak != null)
            _this.onCancel.AddListener(cancelationCallbak);
        Show(message);
    }

    internal static void Show<T, K>(string message, UnityAction<T, K> callback, T parameterA, K parameterB, UnityAction cancelationCallbak = null)
    {
        _this.onConfirm.AddListener(() => callback(parameterA, parameterB));
        if (cancelationCallbak != null)
            _this.onCancel.AddListener(cancelationCallbak);
        Show(message);
    }

    private static void Show(string message)
    {
        _this.gameObject.SetActive(true);
        _this.Message = message;
    }

    void OnDisable()
    {
        onConfirm.RemoveAllListeners();
        onCancel.RemoveAllListeners();
    }
}
