using UnityEngine;
using UnityEngine.UI;

public class EnemyHUDController : MonoBehaviour
{

    [SerializeField]
    private GameObject _mainCamera;
    [SerializeField]
    private GameObject _hpMeter;
    [SerializeField]
    private Text _txtName;
    [SerializeField]
    private Character _enemy;


    private RangedValueAttribute _health;
    void Awake()
    {
        if(!_mainCamera)
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        if(!_enemy)
           _enemy = GetComponentInParent<Character>();

        if (!_hpMeter)
        {
            Image[] children = GetComponentsInChildren<Image>();
            foreach (Image child in children)
            {
                if (child.name.Equals("HpMeter"))
                {
                    _hpMeter = child.gameObject;
                    break;
                }
            }
        }

        if (!_txtName)
        {
            Text[] children = GetComponentsInChildren<Text>();
            foreach (Text child in children)
            {
                if (child.name.Equals("Name"))
                {
                    _txtName = child;
                    break;
                }
            }
        }
    }

    void Start()
    {
        if (_enemy)
        {
            transform.position = _enemy.transform.position + new Vector3(0, 2, 0.2f);
            _txtName.text = _enemy.name;
            _enemy.onDeath.AddListener(Enemy_death);
            _health = _enemy.GetAttribute("hp") as RangedValueAttribute;
        }
    }

    private void Enemy_death()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (!_enemy)
        {
            _enemy = GetComponentInParent<Character>();
            Start();
        }
        transform.rotation = Quaternion.LookRotation(transform.position - _mainCamera.transform.position, Vector3.up);
        if(_health)
            _hpMeter.transform.localScale = new Vector3(_health.Value / _health.Max, 1, 1);
    }
}
   
