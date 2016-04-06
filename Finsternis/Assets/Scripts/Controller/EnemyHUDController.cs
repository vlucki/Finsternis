using UnityEngine;
using UnityEngine.UI;

public class EnemyHUDController : Follow
{

    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private GameObject hpMeter;

    private Character enemy;

    void Start()
    {
        if(!mainCamera)
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        enemy = target.GetComponent<Character>();
        enemy.death += Enemy_death;
        Image[] children = GetComponentsInChildren<Image>();
        foreach(Image child in children)
        {
            if(child.name.Equals("HpMeter"))
            {
                hpMeter = child.gameObject;
                break;
            }
        }
    }

    private void Enemy_death()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        offset.x = Mathf.Clamp((mainCamera.transform.position.x - target.position.x) / 10, -0.5f, 0.5f);
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);

        RangedValueAttribute health = enemy.Attributes.GetAttribute("health") as RangedValueAttribute;
        hpMeter.transform.localScale = new Vector3(health.Value / health.Max, 1, 1);
    }
}
   
