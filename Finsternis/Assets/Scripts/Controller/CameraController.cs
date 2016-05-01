using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Follow))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Follow _follow;

    [Range(1, 100)]
    public float shakeDamping = 2;

    [Range(1, 20)]
    public float shakeFrequency = 20;

    private bool shaking;

    public GameObject _occludingObject;

    void Awake()
    {
        if (!_follow)
            _follow = GetComponent<Follow>();
        shaking = false;
    }

    void Update()
    {
        if (!_follow.target)
            return;
        CharacterController cc = _follow.target.GetComponentInParent<CharacterController>();
        if (cc)
        {
            if (cc.IsFalling())
            {
                _follow.offset = new Vector3(0, _follow.offset.y, -1);
                _follow.focusTarget = true;
            }
            else
            {
                if (!shaking && !_follow.OffsetReset)
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
            transform.position, 
            transform.forward, 
            out hit, 
            Vector3.Distance(transform.position, _follow.target.position), 
            (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Invisible")), 
            QueryTriggerInteraction.Ignore))
        {
            occludingObject = hit.collider.gameObject;
            MeshRenderer renderer = occludingObject.GetComponentInParent<MeshRenderer>();
            if (!renderer)
                renderer = occludingObject.GetComponentInChildren<MeshRenderer>();
            if (renderer)
            {
                if (occludingObject != _occludingObject)
                {
                    if (_occludingObject)
                        _occludingObject.layer = LayerMask.NameToLayer("Wall");
                    _occludingObject = occludingObject;
                    _occludingObject.layer = LayerMask.NameToLayer("Invisible");
                }
            }
        } else if (_occludingObject)
        {
            _occludingObject.layer = LayerMask.NameToLayer("Wall");
            _occludingObject = null;
        }
    }

    IEnumerator Shake(float shakeTime)
    {
        shaking = true;
        System.Random r = new System.Random();
        while (shakeTime > 0)
        {

            yield return new WaitForSeconds(1/shakeFrequency);
            shakeTime -= Time.deltaTime + 1 / shakeFrequency;
            float x = (float)r.NextDouble();
            float y = (float)r.NextDouble();

            if (r.NextDouble() >= 0.5)
                x *= -1;
            if (r.NextDouble() <= 0.5)
                y *= -1;
            float noise = Mathf.PerlinNoise(x, y);
            
            _follow.offset = _follow.OriginalOffset + new Vector3(noise + (r.NextDouble() < 0.5 ? 1 : -1), noise + (r.NextDouble() < 0.5 ? 1 : -1)) / shakeDamping;
            x += y - shakeTime;
            y = shakeTime;
        }
        _follow.ResetOffset();
        shaking = false;
    }
}
