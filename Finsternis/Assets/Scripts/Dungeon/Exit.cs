using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Exit : MonoBehaviour
{
    [SerializeField]
    private BoxCollider _trigger;

    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private GameObject _camera;

    [SerializeField]
    private SimpleDungeon _dungeon;

    private bool _locked;

    private bool _triggered;

    public bool Locked { get { return _locked; } }

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _dungeon = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<SimpleDungeon>();

        if (!(_trigger = GetComponent<BoxCollider>())) {
            _trigger = gameObject.AddComponent<BoxCollider>();
        }
        _trigger.isTrigger = true;
        _trigger.size = new Vector3(1, 1, 1);
        _trigger.center = Vector3.forward / 2;
        _trigger.enabled = false;
        _locked = true;

        _triggered = false;
    }

    void Update()
    {
        if (_dungeon.killsUntilNext <= 0)
            Unlock();
    }

    public void Unlock()
    {
        if (!_trigger.enabled)
        {
            _trigger.enabled = true;
            Follow camFollow = _camera.GetComponentInParent<Follow>();
            camFollow.SetTarget(transform);
            camFollow.OnTargetReached.AddListener(BeginOpen);
        }
    }

    private void BeginOpen()
    {
        GetComponent<Animator>().SetTrigger("Open");
    }

    public void Open()
    {
        Follow camFollow = _camera.GetComponentInParent<Follow>();
        camFollow.OnTargetReached.RemoveListener(BeginOpen);
        camFollow.SetTarget(_player.transform);
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
        _player.transform.forward = -Vector3.forward;
        yield return new WaitForSeconds(1);

        _dungeon.Generate();

        Vector3 currOffset = _player.transform.position - _camera.transform.position;
        _player.transform.position = new Vector3((int) (_dungeon.Entrance.x * _dungeon.GetComponent<SimpleDungeonDrawer>().overallScale.x) + _dungeon.GetComponent<SimpleDungeonDrawer>().overallScale.x/2, 30, (int)- ( _dungeon.Entrance.y * _dungeon.GetComponent<SimpleDungeonDrawer>().overallScale.z) - _dungeon.GetComponent<SimpleDungeonDrawer>().overallScale.z/2);

        _camera.transform.position = _player.transform.position - currOffset;

    }
}
