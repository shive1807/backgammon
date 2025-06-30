using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton Prefab Manager to store and instantiate common prefabs globally.
/// </summary>
public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    [Header("Prefab Registry")]
    [Tooltip("Assign your prefabs here with unique names.")]
    public List<PrefabEntry> prefabEntries = new();

    private Dictionary<string, GameObject> _prefabMap;

    [System.Serializable]
    public class PrefabEntry
    {
        public string name;
        public GameObject prefab;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _prefabMap = new Dictionary<string, GameObject>();
        foreach (var entry in prefabEntries)
        {
            if (!_prefabMap.ContainsKey(entry.name))
            {
                _prefabMap.Add(entry.name, entry.prefab);
            }
            else
            {
                Debug.LogWarning($"Duplicate prefab name in PrefabManager: {entry.name}");
            }
        }
    }

    /// <summary>
    /// Instantiates a prefab by name at the given position and rotation.
    /// </summary>
    public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation)
    {
        if (!_prefabMap.TryGetValue(prefabName, out var prefab))
        {
            Debug.LogError($"Prefab with name '{prefabName}' not found in PrefabManager.");
            return null;
        }

        return Instantiate(prefab, position, rotation);
    }

    /// <summary>
    /// Instantiates a prefab by name at the given position and default rotation.
    /// </summary>
    public GameObject Instantiate(string prefabName, Vector3 position)
    {
        return Instantiate(prefabName, position, Quaternion.identity);
    }

    /// <summary>
    /// Returns the prefab GameObject without instantiating.
    /// </summary>
    public GameObject GetPrefab(string prefabName)
    {
        if (!_prefabMap.TryGetValue(prefabName, out var prefab))
        {
            Debug.LogError($"Prefab with name '{prefabName}' not found in PrefabManager.");
            return null;
        }

        return prefab;
    }
}
