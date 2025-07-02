using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObjectMissedSignal
{
    public GameObject MissedObject;
    public ObjectMissedSignal(GameObject missedObject) => MissedObject = missedObject;
}

public class AllWavesCompletedSignal { }

public class WaveProgressSignal
{
    public int CurrentWave;
    public int TotalWaves;

    public WaveProgressSignal(int currentWave, int totalWaves)
    {
        CurrentWave = currentWave;
        TotalWaves = totalWaves;
    }
}

public class Spawner : MonoBehaviour
{
    [SerializeField] private SpawnerConfigSO _spawnerConfig;
    [SerializeField] private WaveSpawnerConfigSO _waveConfig;

    [Inject] private DiContainer _container;
    [Inject] private SignalBus _signalBus;

    private Camera _camera;
    private List<GameObject> _pooledInstances = new();
    private Coroutine _spawnCoroutine;
    private bool _isGameRunning = false;
    private float _currentDifficultyMultiplier;

    private UIController _uiController;

    [Inject]
    public void Initialize(UIController uiController)
    {
        _uiController = uiController;
        _camera = Camera.main;

        if (_camera == null)
        {
            Debug.LogError("[Spawner] Main Camera not found!");
            return;
        }

        if (_spawnerConfig == null || !_spawnerConfig.IsValid(out _))
        {
            Debug.LogError("[Spawner] Invalid spawner config");
            return;
        }

        _currentDifficultyMultiplier = _waveConfig.DifficultyMultiplier;
        
        foreach (var element in _spawnerConfig.SpawnerElements)
        {
            for (int i = 0; i < element.MaxCount; i++)
            {
                var instance = _container.InstantiatePrefab(element.Prefab, GetOffscreenPosition(), Quaternion.identity, null);
                instance.SetActive(false);
                _pooledInstances.Add(instance);
            }
        }
        
        _signalBus.Subscribe<ObjectMissedSignal>(OnObjectMissed);
        _signalBus.Subscribe<StartGameSignal>(OnStartSpawn);
        _signalBus.Subscribe<ResumeGameSignal>(OnResumeSpawn);
        _signalBus.Subscribe<PauseGameSignal>(OnStopSpawn);
        _signalBus.Subscribe<WinGameSignal>(OnStopSpawn);
        _signalBus.Subscribe<LoseGameSignal>(OnStopSpawn);
    }

    private void OnStartSpawn()
    {
        foreach (var obj in _pooledInstances)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        StartSpawning();
    }

    private void OnResumeSpawn()
    {
        _isGameRunning = true;
    }

    private void StartSpawning()
    {
        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);

        _isGameRunning = true;
        _currentDifficultyMultiplier = _waveConfig.DifficultyMultiplier;
        _spawnCoroutine = StartCoroutine(SpawnWavesRoutine());
    }

    private void OnStopSpawn()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }

        _isGameRunning = false;
    }

    private void Update()
    {
        CheckActiveObjects();
    }

    private IEnumerator SpawnWavesRoutine()
    {
        for (int wave = 0; wave < _waveConfig.WaveCount; wave++)
        {
            _signalBus.Fire(new WaveProgressSignal(wave + 1, _waveConfig.WaveCount));
            Debug.Log($"[Spawner] Wave {wave + 1}/{_waveConfig.WaveCount} started.");

            float elapsed = 0f;
            int waveLimit = _waveConfig.GetMaxActiveObjectsForWave(wave);
            float waveDuration = _waveConfig.WaveDuration;

            while (elapsed < waveDuration)
            {
                if (!_isGameRunning) yield break;

                float normalizedProgress = Mathf.Clamp01(elapsed / waveDuration);
                _uiController.SetWaveProgressSlider(normalizedProgress);

                int currentActive = GetActiveObjectCount();
                int availableToSpawn = waveLimit - currentActive;

                if (availableToSpawn > 0)
                {
                    int spawnCount = Mathf.Min(_waveConfig.GetDynamicSpawnCount(wave), availableToSpawn);
                    for (int i = 0; i < spawnCount; i++)
                        SpawnRandomElement();
                }

                float delay = _waveConfig.SpawnInterval / _currentDifficultyMultiplier;
                yield return new WaitForSeconds(delay);
                elapsed += delay;
            }

            _uiController.SetWaveProgressSlider(1f);
            Debug.Log($"[Spawner] Wave {wave + 1} finished.");
            _currentDifficultyMultiplier += 0.3f;
        }

        Debug.Log("[Spawner] All waves completed.");
        _isGameRunning = false;
        _signalBus.Fire<AllWavesCompletedSignal>();
    }

    private void CheckActiveObjects()
    {
        float screenBottomY = _camera.ViewportToWorldPoint(new Vector3(0, 0, 10)).y;
        float despawnY = screenBottomY - _spawnerConfig.DespawnDistanceBelowScreen;

        foreach (var obj in _pooledInstances)
        {
            if (obj != null && obj.activeInHierarchy && obj.transform.position.y < despawnY)
            {
                ClearTrails(obj);
                obj.SetActive(false);
                if (_isGameRunning)
                    _signalBus.Fire(new ObjectMissedSignal(obj));
            }
        }
    }

    private void OnObjectMissed(ObjectMissedSignal signal)
    {
        if (!_isGameRunning || signal.MissedObject == null)
            return;

        ClearTrails(signal.MissedObject);
        signal.MissedObject.SetActive(false);
    }

    private Vector3 GetSpawnPosition()
    {
        float yMin = _camera.ViewportToWorldPoint(new Vector3(0, _spawnerConfig.MinSpawnYViewport, 10)).y;
        float yMax = _camera.ViewportToWorldPoint(new Vector3(0, _spawnerConfig.MaxSpawnYViewport, 10)).y;
        float y = Random.Range(yMin, yMax);

        bool spawnLeft = Random.value < 0.5f;
        float x = _camera.ViewportToWorldPoint(new Vector3(spawnLeft ? 0 : 1, 0, 10)).x;
        x += spawnLeft ? -_spawnerConfig.HorizontalScreenOffset : _spawnerConfig.HorizontalScreenOffset;

        return new Vector3(x, y, 0f);
    }

    private Vector3 GetOffscreenPosition() => new(1000f, 1000f, 0f);

    private void SpawnRandomElement()
    {
        float rand = Random.Range(0f, 100f);
        float cumulative = 0f;

        foreach (var element in _spawnerConfig.SpawnerElements)
        {
            cumulative += element.SpawnChance;
            if (rand <= cumulative)
            {
                var instance = _pooledInstances.Find(obj =>
                    !obj.activeInHierarchy && obj.name.Contains(element.Prefab.name));

                if (instance == null)
                {
                    instance = _container.InstantiatePrefab(element.Prefab, GetOffscreenPosition(), Quaternion.identity, null);
                    instance.SetActive(false);
                    _pooledInstances.Add(instance);
                    Debug.Log($"[Spawner] Created new instance for prefab {element.Prefab.name} because pool was empty.");
                }

                instance.transform.position = GetSpawnPosition();
                
                ClearTrails(instance);
                
                
                instance.SetActive(true);

                if (instance.TryGetComponent<ISpawner>(out var spawner))
                    spawner.OnSpawn(instance.transform.position);

                return;
            }
        }
    }
    private void ClearTrails(GameObject obj)
    {
        var trailRenderers = obj.GetComponentsInChildren<TrailRenderer>();
        foreach (var trail in trailRenderers)
        {
            trail.Clear();
        }
    }

    private int GetActiveObjectCount()
    {
        int count = 0;
        foreach (var obj in _pooledInstances)
            if (obj != null && obj.activeInHierarchy)
                count++;
        return count;
    }
}
