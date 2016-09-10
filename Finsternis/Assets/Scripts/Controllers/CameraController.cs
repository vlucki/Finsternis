using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityQuery;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Follow follow;

    [SerializeField]
    [Range(1, 100)]
    private float shakeDamping = 2;

    [SerializeField]
    [Range(1, 20)]
    private float shakeFrequency = 20;

    [SerializeField]
    [Range(1, 100)]
    private float shakeAmplitude = 20;

    private bool shaking;

    public bool occludeWalls = false;
    [SerializeField]
    private GameObject occludingObject;
    private bool shouldReset;
    private Vector3 lastTarget;
    private Coroutine shakeHandle;

    public GameObject OccludingObject { get { return occludingObject; } }

    void Awake()
    {
        if (!this.follow)
            this.follow = GetComponentInParent<Follow>();
        shaking = false;
    }

    void FixedUpdate()
    {
        if (!this.follow)
            return;
        if (!this.follow.Target)
            return;

        GameObject occludingObject;
        RaycastHit hit;
        Vector3 target = this.follow.Target.position;

        if (Physics.SphereCast(
            target,
            0.25f,
            transform.position - target,
            out hit,
            transform.position.Distance(target),
            (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Invisible")),
            QueryTriggerInteraction.Ignore))
        {
            occludingObject = hit.collider.gameObject;
            if (occludingObject)
            {
                Wall wall = occludingObject.GetComponent<Wall>();
                if (wall)
                {
                    this.occludingObject = occludingObject;
                    wall.FadeOut();
                }
            }
        }
        else if (this.occludingObject)
        {
            this.occludingObject = null;
        }

        if (target != lastTarget)
            lastTarget = target;
        else
            return;

        int mask = 1 << LayerMask.NameToLayer("Wall");
        float maxDistance = 2f;
        if (Physics.Raycast(target + Vector3.up / 2, Vector3.back, out hit, maxDistance, mask))
        {
            this.follow.MemorizeOffset(this.follow.OriginalOffset + (Vector3.up * 2 + Vector3.forward * 3) * (1 - hit.distance / maxDistance));
            if (!this.shaking)
                this.follow.ResetOffset();
            shouldReset = true;
        }
        else if (shouldReset && !this.shaking)
        {
            this.follow.translationInterpolation = 0.05f;
            this.follow.OnTargetReached.AddListener(FinishedInterpolating);
            shouldReset = false;
            this.follow.ResetOffset(true);
        }
    }

    private void FinishedInterpolating()
    {
        this.follow.translationInterpolation = 0.1f;
        this.follow.MemorizeOffset(this.follow.OriginalOffset);
        this.follow.OnTargetReached.RemoveListener(FinishedInterpolating);
    }

    internal void Shake(float time, float damping, float amplitude, float frequency, bool overrideShake = true)
    {
        if(overrideShake && this.shaking)
        {
            this.shaking = false;
            StopCoroutine(this.shakeHandle);
        }

        if (!this.shaking)
        {
            this.shakeDamping = damping;
            this.shakeFrequency = frequency;
            this.shakeAmplitude = amplitude;
            this.shakeHandle = StartCoroutine(_Shake(time));
        }
    }

    IEnumerator _Shake(float shakeTime)
    {
        this.shaking = true;
        float amplitude = this.shakeAmplitude;
        while (shakeTime > 0)
        {
            yield return Yields.SEC(1 / this.shakeFrequency);
            shakeTime -= Time.deltaTime + 1 / this.shakeFrequency;

            Vector3 shakeOffset = Random.insideUnitSphere / 10;

            shakeOffset.z = 0;
            transform.localPosition = shakeOffset * amplitude;
            transform.localRotation = Quaternion.Euler(new Vector3(Random.value, Random.value, Random.value) * amplitude / 5);
            amplitude /= this.shakeDamping;
        }

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        this.shaking = false;
    }
}
