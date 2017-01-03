using UnityEngine;

public sealed class PrefabSpawner : MonoBehaviour
{
    [System.Serializable]
    public class PrefabSpawnEvent : Finsternis.Events.CustomEvent<GameObject> { }
    public PrefabSpawnEvent onPrefabSpawned;
    public void SpawnPrefab(GameObject prefab)
    {
        var spawn = Instantiate(prefab);
        if(onPrefabSpawned)
            onPrefabSpawned.Invoke(spawn);
    }
}
