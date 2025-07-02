using UnityEngine;

[CreateAssetMenu(fileName = "WaveSpawnerConfigSO", menuName = "Config/WaveSpawnerConfigSO")]
public class WaveSpawnerConfigSO : ScriptableObject
{
    [Header("Общее количество волн")]
    [Tooltip("Сколько волн всего будет в спавнере (минимум 1)")]
    [SerializeField, Min(1)] private int _waveCount = 5;

    [Header("Параметры длительности и интервалов")]
    [Tooltip("Длительность одной волны в секундах (минимум 0.1)")]
    [SerializeField, Min(0.1f)] private float _waveDuration = 30f;

    [Tooltip("Интервал между спавнами объектов внутри волны (минимум 0.01)")]
    [SerializeField, Min(0.01f)] private float _spawnInterval = 1f;

    [Tooltip("Множитель сложности, влияет на скорость спавна (от 1 до 5)")]
    [SerializeField, Range(1f, 5f)] private float _difficultyMultiplier = 1f;

    [Space(10)]
    [Header("Максимальное количество активных объектов")]
    [Tooltip("Базовое максимальное количество активных объектов на волне (минимум 1)")]
    [SerializeField, Min(1)] private int _baseMaxActiveObjectsPerWave = 10;

    [Tooltip("Увеличение максимального количества активных объектов с каждой следующей волной")]
    [SerializeField, Min(0)] private int _increasePerWave = 5;
    
    public int WaveCount => _waveCount;
    public float WaveDuration => _waveDuration;
    public float SpawnInterval => _spawnInterval;
    public float DifficultyMultiplier => _difficultyMultiplier;
    
    public int BaseMaxActiveObjectsPerWave => _baseMaxActiveObjectsPerWave;
    public int IncreasePerWave => _increasePerWave;
    
    public int GetMaxActiveObjectsForWave(int waveIndex)
    {
        return _baseMaxActiveObjectsPerWave + _increasePerWave * waveIndex;
    }
    
    public int GetDynamicSpawnCount(int waveIndex)
    {
        int count = 1;
        for (int i = 0; i < waveIndex; i++)
        {
            if (Random.value < 0.5f)
                count++;
            else
                break;
        }

        return count;
    }
}
