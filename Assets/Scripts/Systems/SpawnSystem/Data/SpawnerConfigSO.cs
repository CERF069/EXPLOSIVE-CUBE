using UnityEngine;

[System.Serializable]
public class SpawnerElement
{
    [Tooltip("Префаб для спауна")]
    [SerializeField] private GameObject _prefab;

    [Tooltip("Максимальное количество экземпляров этого префаба")]
    [SerializeField, Min(1)] private int _maxCount = 1;

    [Tooltip("Шанс спавна этого префаба в процентах (0-100)")]
    [SerializeField, Range(0f, 100f)] private float _spawnChance = 0f;

    public GameObject Prefab => _prefab;
    public int MaxCount => _maxCount;
    public float SpawnChance => _spawnChance;
}

[CreateAssetMenu(fileName = "SpawnerConfigSO", menuName = "Config/SpawnerConfigSO")]
public class SpawnerConfigSO : ScriptableObject
{
    [SerializeField] private SpawnerElement[] _spawnerElements;
    public SpawnerElement[] SpawnerElements => _spawnerElements;

    public bool IsValid(out string errorMessage)
    {
        if (_spawnerElements == null || _spawnerElements.Length == 0)
        {
            errorMessage = $"[{nameof(SpawnerConfigSO)}] Конфиг не содержит ни одного элемента спауна!";
            return false;
        }

        float totalChance = 0f;

        for (int i = 0; i < _spawnerElements.Length; i++)
        {
            var element = _spawnerElements[i];

            if (element.Prefab == null)
            {
                errorMessage = $"[{nameof(SpawnerConfigSO)}] Префаб в элементе #{i} не задан!";
                return false;
            }

            if (element.MaxCount <= 0)
            {
                errorMessage = $"[{nameof(SpawnerConfigSO)}] MaxCount в элементе #{i} должен быть больше нуля!";
                return false;
            }

            if (element.SpawnChance < 0f || element.SpawnChance > 100f)
            {
                errorMessage = $"[{nameof(SpawnerConfigSO)}] SpawnChance в элементе #{i} должен быть от 0 до 100!";
                return false;
            }

            totalChance += element.SpawnChance;
        }

        if (totalChance > 100f)
        {
            errorMessage = $"[{nameof(SpawnerConfigSO)}] Сумма шансов спавна всех элементов превышает 100% ({totalChance}%)!";
            return false;
        }

        errorMessage = null;
        return true;
    }
    
    
    [Header("Spawn Position Settings")]
    [SerializeField, Range(0f, 1f)] private float _minSpawnYViewport = 0.2f;
    [SerializeField, Range(0f, 1f)] private float _maxSpawnYViewport = 0.8f;
    [SerializeField] private float _despawnDistanceBelowScreen = 5f;
    [SerializeField] private float _horizontalScreenOffset = 2f;
    
    public float MinSpawnYViewport => _minSpawnYViewport;
    public float MaxSpawnYViewport => _maxSpawnYViewport;
    public float DespawnDistanceBelowScreen => _despawnDistanceBelowScreen;
    public float HorizontalScreenOffset => _horizontalScreenOffset;
}
