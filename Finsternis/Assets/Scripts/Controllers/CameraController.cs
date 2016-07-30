using UnityEngine;
using System.Collections.Generic;
using MovementEffects;

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

    public bool occludeWalls = false;
    [SerializeField]
    private GameObject _occludingObject;
    private bool shouldReset;
    private Vector3 lastTarget;
    private IEnumerator<float> shakeHandle;

    public GameObject OccludingObject { get { return _occludingObject; } }

    void Awake()
    {
        if (!_follow)
            _follow = GetComponentInParent<Follow>();
        shaking = false;
    }

    void FixedUpdate()
    {
        GameObject occludingObject;
        RaycastHit hit;
        Vector3 target = _follow.Target.position;

        if (Physics.SphereCast(
            target,
            0.25f,
            transform.position - target,
            out hit,
            Vector3.Distance(transform.position, target),
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
        }
        else if (_occludingObject)
        {
            _occludingObject = null;
        }

        if (target != lastTarget)
            lastTarget = target;
        else
            return;

        int mask = 1 << LayerMask.NameToLayer("Wall");
        float maxDistance = 2f;
        if (Physics.Raycast(target + Vector3.up / 2, Vector3.back, out hit, maxDistance, mask))
        {
            _follow.MemorizeOffset(_follow.OriginalOffset + (Vector3.up * 2 + Vector3.forward * 3) * (1 - hit.distance / maxDistance));
            if (!shaking)
                _follow.ResetOffset();
            shouldReset = true;
        }
        else if (shouldReset && !shaking)
        {
            _follow.translationInterpolation = 0.05f;
            _follow.OnTargetReached.AddListener(FinishedInterpolating);
            shouldReset = false;
            _follow.ResetOffset(true);
        }
    }

    private void FinishedInterpolating()
    {
        _follow.translationInterpolation = 0.1f;
        _follow.MemorizeOffset(_follow.OriginalOffset);
        _follow.OnTargetReached.RemoveListener(FinishedInterpolating);
    }

    internal void Shake(float time, float damping, float amplitude, float frequency, bool overrideShake = true)
    {
        if(overrideShake && shaking)
        {
            shaking = false;
            Timing.KillCoroutines(shakeHandle);
        }

        if (!shaking)
        {
            _shakeDamping = damping;
            _shakeFrequency = frequency;
            _shakeAmplitude = amplitude;
            shakeHandle = Timing.RunCoroutine(_Shake(time));
        }
    }

    IEnumerator<float> _Shake(float shakeTime)
    {
        shaking = true;
        float amplitude = _shakeAmplitude;
        while (shakeTime > 0)
        {
            yield return Timing.WaitForSeconds(1/_shakeFrequency);
            shakeTime -= Time.deltaTime + 1 / _shakeFrequency;

            Vector3 shakeOffset = Random.insideUnitSphere / 10;

            shakeOffset.z = 0;
            transform.localPosition = shakeOffset * amplitude;
            transform.localRotation = Quaternion.Euler(new Vector3(Random.value, Random.value, Random.value) * amplitude / 5);
            amplitude /= _shakeDamping;
        }

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        shaking = false;
    }
}
