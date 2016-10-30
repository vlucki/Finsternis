namespace Finsternis
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityQuery;

    [CreateAssetMenu(fileName ="AlignToWall", menuName ="Finsternis/Features/Alignment/Align to Wall")]
    public class AlignToWall : FeatureAlignment
    {
        [SerializeField]
        private float yOffset = 2f;

        [SerializeField]
        [Range(0, 1)]
        private float offsetAmount = 0.25f;

        public override void Align(Dungeon dungeon, Vector3 dungeonScale, Vector2 position, GameObject gObject)
        {
            var walls = dungeon.GetNeighbours(position, false, Dungeon.wall);
            if (walls.IsNullOrEmpty())
            {
                Log.Error(this, "Could not find any wall around {0} to align {1}", position, gObject);
                return;
            }
            var wall = walls.GetRandom(Dungeon.Random.IntRange);
            var direction = wall.Towards(position);
            if (direction.y > 0)
                gObject.transform.Rotate(Vector3.up, 180, Space.World);
            else if (direction.x > 0)
                gObject.transform.Rotate(Vector3.up, 90, Space.World);
            else if (direction.x < 0)
                gObject.transform.Rotate(Vector3.up, -90, Space.World);

            var offset = new Vector3(-direction.x, 0, direction.y);
            offset.Scale(dungeonScale);
            gObject.transform.position += (offset * this.offsetAmount).WithY(this.yOffset);
        }
    }
}