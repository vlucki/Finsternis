namespace Finsternis
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityQuery;

    [CreateAssetMenu(fileName = "AlignToCorridor", menuName = "Finsternis/Features/Alignment/Align to Corridor")]
    public class AlignToCorridor : FeatureAlignment
    {
        [SerializeField]
        [Range(0, 0.5f)]
        private float offsetAmount = 0.25f;

        [SerializeField]
        private bool perpendicular = false;

        public override void Align(Dungeon dungeon, Vector3 dungeonScale, Vector2 position, GameObject gObject, int count = 0)
        {
            var corridor = dungeon[position] as Corridor;
            if (!corridor)
            {
#if DEBUG
                Log.E(this, "No corridor found at {0} when trying to align {1}", position, gObject);
#endif
                return;
            }
            var direction = corridor.Direction;
            bool nearStart = position.Distance(corridor[0]) >= position.Distance(corridor.End);
            if(count > 1 && corridor.Length == 1)
            {
                nearStart = false;
            }

            if (direction.y != 0)
            {
                if(nearStart)
                    gObject.transform.Rotate(Vector3.up, 180);

            }
            else
            {
                if (nearStart)
                    gObject.transform.Rotate(Vector3.up, 90);
                else
                    gObject.transform.Rotate(Vector3.up, -90);
            }

            if (perpendicular)
                gObject.transform.Rotate(Vector3.up, 90);

            var offset = gObject.transform.forward;
            offset.Scale(dungeonScale);
            gObject.transform.position += (offset * offsetAmount);
        }
    }
}