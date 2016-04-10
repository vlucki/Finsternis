using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(BoxCollider))]
public class Exit : MonoBehaviour
{
    [SerializeField]
    private BoxCollider _collider;

    [SerializeField]
    private GameObject _player;
    private GameObject _camera;
    private SimpleDungeon _dungeon;
    
    private bool _triggered;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _dungeon = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<SimpleDungeon>();
        _triggered = false;

        if(!(_collider = GetComponent<BoxCollider>())){
            _collider = gameObject.AddComponent<BoxCollider>();
        }
    }

    void Start()
    {
        _collider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_triggered && other.gameObject.Equals(_player))
        {
            _triggered = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (_triggered)
        {
            if (other.gameObject.Equals(_player))
            {
                _triggered = false;
                if (other.transform.position.y < transform.position.y)
                {
                    StartCoroutine(EndLevel());
                }
            }

        }
    }

    private IEnumerator EndLevel()
    {
        _player.GetComponent<Rigidbody>().velocity = new Vector3(0, _player.GetComponent<Rigidbody>().velocity.y, 0);

        yield return new WaitForSeconds(1);

        _dungeon.Generate();

        Vector3 currOffset = _player.transform.position - _camera.transform.position;
        _player.transform.position = new Vector3((int) (_dungeon.Entrance.x * _dungeon.GetComponent<SimpleDungeonDrawer>().scale.x) + 1, 25, (int)- ( _dungeon.Entrance.y * _dungeon.GetComponent<SimpleDungeonDrawer>().scale.z) - 1);
        
        _camera.transform.position = _player.transform.position - currOffset;

        _dungeon.GetComponent<SimpleDungeonDrawer>().Draw();

    }
}
