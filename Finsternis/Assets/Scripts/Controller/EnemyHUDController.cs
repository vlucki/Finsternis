using UnityEngine;
using UnityEngine.UI;

public class EnemyHUDController : MonoBehaviour
{

    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private GameObject hpMeter;

    [SerializeField]
    private Text txtName;

    [SerializeField]
    private Character enemy;

    void Awake()
    {
        if(!mainCamera)
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        enemy = GetComponentInParent<Character>();
        enemy.death += Enemy_death;

        if (!hpMeter)
        {
            Image[] children = GetComponentsInChildren<Image>();
            foreach (Image child in children)
            {
                if (child.name.Equals("HpMeter"))
                {
                    hpMeter = child.gameObject;
                    break;
                }
            }
        }

        if (!txtName)
        {
            Text[] children = GetComponentsInChildren<Text>();
            foreach (Text child in children)
            {
                if (child.name.Equals("Name"))
                {
                    txtName = child;
                    break;
                }
            }
        }


        txtName.text = enemy.name;
    }

    private void Enemy_death()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position, Vector3.up);

        RangedValueAttribute health = enemy.Attributes.GetAttribute("health") as RangedValueAttribute;
        hpMeter.transform.localScale = new Vector3(health.Value / health.Max, 1, 1);
    }
}
   
