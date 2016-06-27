using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Follow))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Follow _follow;

    [SerializeField]
    [Range(1, 100)]
    private float _shakeDamping = 2;

    [SerializeField]
    [Range(1, 20)]
    private float _shakeFrequency = 20;

    [SerializeField]
    [Range(1, 100)]
    private float _shakeAmplitude = 20;

    private bool shaking;

    internal void Shake(float time, float damping, float amplitude, float frequency)
    {
        if (!shaking)
        {
            _shakeDamping = damping;
            _shakeFrequency = frequency;
            _shakeAmplitude = amplitude;
            StartCoroutine(Shake(time));
        }
    }

    public bool occludeWalls = false;
    [SerializeField]
    private GameObject _occludingObject;

    public GameObject OccludingObject { get { return _occludingObject; } }

    void Awake()
    {
        if (!_follow)
            _follow = GetComponent<Follow>();
        shaking = false;
    }

    void Update()
    {
        if (!_follow.Target)
            return;
        CharacterController cc = _follow.Target.GetComponentInParent<CharacterController>();
        if (cc)
        {
            if (cc.IsFalling() && cc.GetComponent<Rigidbody>().velocity.y >= 0.5f)
            {
                _follow.offset = new Vector3(0, _follow.offset.y, -1);
            }
            else
            {
                if (!shaking && !_follow.WasOffsetChanged)
                {
                    _follow.ResetOffset();
                    StartCoroutine(Shake(1f));
                }
            }
        }
    }

    void FixedUpdate()
    {
        GameObject occludingObject;
        RaycastHit hit;
        if(Physics.Raycast(
            _follow.Target.position, 
            transform.position - _follow.Target.position, 
            out hit, 
            Vector3.Distance(transform.position, _follow.Target.position), 
            (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Invisible")), 
            QueryTriggerInteraction.Ignore))
        {
            occludingObject = hit.collider.gameObject;
            if (occludingObject)
            {
                Wall wall = occludingObject.GetComponent<Wall>();
                if (wall)
                {
                    _occludingObject = occludingObject;
                    wall.FadeOut();
                }
            }
        } else if (_occludingObject)
        {
            _occludingObject = null;
        }
    }

    IEnumerator Shake(float shakeTime)
    {
        shaking = true;
        float amplitude = _shakeAmplitude;
        while (shakeTime > 0)
        {

            yield return new WaitForSeconds(1/_shakeFrequency);
            shakeTime -= Time.deltaTime + 1 / _shakeFrequency;

            Vector3 explosionOffset = Random.insideUnitSphere;
            explosionOffset.z = 0;
            _follow.offset = _follow.OriginalOffset + explosionOffset * amplitude;

            amplitude /= _shakeDamping;
        }
        _follow.ResetOffset();
        shaking = false;
    }
}
