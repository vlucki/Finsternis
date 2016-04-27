using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Follow))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Follow _follow;

    [Range(1, 100)]
    public float shakeDamping = 5;
    public float shakeTime = 100;

    bool shaking = false;

    void Awake()
    {
        if (!_follow)
            _follow = GetComponent<Follow>();
    }

    void Start()
    {
        StartCoroutine(Shake());
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
                _follow.offset = new Vector3(0, _follow.offset.y, 0);
                _follow.focusTarget = true;
            }
            else if(!shaking && !_follow.OffsetReset)
            {
                _follow.ResetOffset();
            }
        }
    }

    IEnumerator Shake()
    {
        shaking = true;
        System.Random r = new System.Random();
        while (shakeTime > 0)
        {
            
            shakeTime -= Time.deltaTime;
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
            yield return new WaitForSeconds(0.1f);
        }
        _follow.ResetOffset();
        shaking = false;
    }
}
