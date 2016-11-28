namespace Finsternis
{

    using UnityEngine;
    using UnityEngine.UI;

    public class DungeonInfo : MonoBehaviour
    {

        private void Awake()
        {
            this.GetComponentsInChildren<Text>()[1].text = "/ " + GameManager.Instance.DungeonsToClear.ToString();
        }
    }
}
