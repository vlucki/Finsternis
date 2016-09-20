namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    [RequireComponent(typeof(Text))]
    public class CreditsLoader : MonoBehaviour
    {
        [SerializeField]
        private TextAsset creditsAsset;
        void Awake()
        {
            GetComponent<Text>().text = creditsAsset.text;
        }
    }
}