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
            transform.position, 
            transform.forward, 
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
