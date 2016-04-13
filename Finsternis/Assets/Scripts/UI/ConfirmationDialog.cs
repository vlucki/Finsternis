using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ConfirmationDialog : MonoBehaviour
{
    public UnityEvent onConfirm;
    public UnityEvent onCancel;

    [SerializeField]
    private Text _messageField;

    [SerializeField]
    private EventSystem _evtSystem;

    [SerializeField]
    private GameObject yes;

    [SerializeField]
    private GameObject no;

    private static ConfirmationDialog _this;

    public string Message
    {
        get { return _messageField.text; }
        set { _messageField.text = value; }
    }

    public ConfirmationDialog()
    {
        ConfirmationDialog._this = this;
    }

    void Awake()
    {
        _evtSystem = FindObjectOfType<EventSystem>();
        _messageField = GetComponentInChildren<Text>();
        foreach(Button b in transform.GetComponentsInChildren<Button>())
        {
            if (b.name.Equals("Yes"))
                yes = b.gameObject;
            else if (b.name.Equals("No"))
                no = b.gameObject;
        }
    }

    void Update()
    {
        if (_evtSystem.currentSelectedGameObject != yes && _evtSystem.currentSelectedGameObject != no)
            _evtSystem.SetSelectedGameObject(yes);
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
