namespace Finsternis
{
    using UnityEngine;

    public class TargetMarker : MonoBehaviour
    {
        [SerializeField]
        private GameObject targetMarkerPrefab;

        private GameObject targetMarker;

        private void DisableMarker()
        {
            if (this.targetMarker)
            {
                this.targetMarker.transform.SetParent(null);
                this.targetMarker.SetActive(false);
            }
        }

        public void SetTarget(GameObject target)
        {
            if (target)
                MarkTarget(target);
            else
                DisableMarker();
        }

        private void MarkTarget(GameObject target)
        {
            if (!this.targetMarkerPrefab)
                return;

            if (!this.targetMarker)
                this.targetMarker = Instantiate(this.targetMarkerPrefab);

            this.targetMarker.SetActive(true);
            this.targetMarker.transform.SetParent(target.transform);
            this.targetMarker.transform.localPosition = Vector3.up * 4;
        }
    }
}