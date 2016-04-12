using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(Follow))]
public class MenuController : MonoBehaviour
{

    [SerializeField]
    private Canvas _canvas;

    private Follow _followBehaviour;
    
    private MenuItem[] _items;

    private int _selectedItem;

    [Range(0, 1)]
    public float itemChangeDelay = 0.25f;

    private float _elapsedTime;

    public bool Active {
        get { return _canvas.enabled; }
        set
        {
            _canvas.enabled = value;
            _followBehaviour.enabled = value;
        }
    }

    void Awake()
    {
        if (!this._canvas)
            this._canvas = GetComponent<Canvas>();

        _followBehaviour = GetComponent<Follow>();

        _items = GetComponentsInChildren<MenuItem>();
    }

    // Use this for initialization
    void Start()
    {
        ToggleMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }

        if (Active)
        {
            HandleInput();
        }
    }

    public void ToggleMenu()
    {
        Active = !Active;
        if (Active)
        {
            _followBehaviour.target.GetComponent<CharacterController>().Lock();

            SetSelectedItem(0);
            
            this.transform.position = this._followBehaviour.target.transform.position - 2 * this._followBehaviour.offset;
        }
        else
        {
            _followBehaviour.target.GetComponent<CharacterController>().Unlock();
        }
    }

    private void SetSelectedItem(int v)
    {
        if (v < 0)
            v = _items.Length - 1;
        else if (v >= _items.Length)
            v = 0;

        _items[_selectedItem].GetComponent<Image>().color = Color.gray; //gray out last selection
        _selectedItem = v;
        _items[_selectedItem].GetComponent<Image>().color = Color.white; //highlight current selection
    }

    private void HandleInput()
    {
        if (_items == null || _items.Length == 0)
            return;

        int f = Input.GetKey(KeyCode.W) ? -1 : Input.GetKey(KeyCode.S) ? 1 : 0;

        if(f == 0)
        {
            _elapsedTime = itemChangeDelay;
        } else if(_elapsedTime < itemChangeDelay)
        {
            _elapsedTime += Time.deltaTime;
        } else
        {
            _elapsedTime = 0;
            SetSelectedItem(_selectedItem + f);        
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _items[_selectedItem].activate.Invoke();
        }    
    }
}
