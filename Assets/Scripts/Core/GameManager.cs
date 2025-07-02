using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class GameManager : MonoBehaviour
{
    [FormerlySerializedAs("missedLimit")] 
    [SerializeField, Tooltip("Количество пропущенных объектов для проигрыша"), Min(1)]
    private int _missedLimit = 5;

    private int _missedCount = 0;

    [Inject] private SignalBus _signalBus;
    [Inject] private GameStateManager _gameStateManager;
    
    [Inject] private UIController _uiController;

    [Inject]
    public void Construct(SignalBus signalBus, GameStateManager gameStateManager, UIController uiController)
    {
        _signalBus = signalBus;
        _gameStateManager = gameStateManager;
        _uiController = uiController;
    }


    [Inject]
    private void StartGameManager()
    {
        _signalBus.Subscribe<ObjectMissedSignal>(OnObjectMissed);
        _signalBus.Subscribe<WinGameSignal>(OnGameEnd);
        _signalBus.Subscribe<LoseGameSignal>(OnGameEnd);
        
        _signalBus.Subscribe<AllWavesCompletedSignal>(OnAllWavesCompleted);
        
        _uiController.UpdateMissesLeft(_missedLimit - _missedCount);
    }

    private void OnDisable()
    {
        _signalBus.Unsubscribe<ObjectMissedSignal>(OnObjectMissed);
        _signalBus.Unsubscribe<WinGameSignal>(OnGameEnd);
        _signalBus.Unsubscribe<LoseGameSignal>(OnGameEnd);
        
        _signalBus.Unsubscribe<AllWavesCompletedSignal>(OnAllWavesCompleted);
    }

    private void OnObjectMissed(ObjectMissedSignal signal)
    {
        _missedCount++;
        Debug.Log($"Missed objects: {_missedCount}/{_missedLimit}");

        _uiController.UpdateMissesLeft(_missedLimit - _missedCount);

        if (_missedCount >= _missedLimit)
        {
            Debug.Log("Missed limit reached! Changing state to Lose.");
            _gameStateManager.OnLose();
        }
    }
    public int GetMissedCount() => _missedCount;

    public void IncreaseMissedLimit(int value)
    {
        _missedLimit += value;
        _uiController.UpdateMissesLeft(_missedLimit - _missedCount);
    }

    public void DecreaseMissedCount(int value)
    {
        _missedCount = Mathf.Max(0, _missedCount - value);
        _uiController.UpdateMissesLeft(_missedLimit - _missedCount);
    }


    private void OnAllWavesCompleted(AllWavesCompletedSignal signal)
    {
        Debug.Log("All waves completed! Changing state to Win.");
        _gameStateManager.OnWin();
    }

    private void OnGameEnd()
    {
        ResetMissedCount();
    }

    [ContextMenu("Reset Missed Count")]
    public void ResetMissedCount()
    {
        _missedCount = 0;
        _missedLimit = 5;
        _uiController.UpdateMissesLeft(_missedLimit - _missedCount);
        Debug.Log("Missed count reset.");
    }

    [ContextMenu("Set State: Start")]
    public void SetStartState() => _gameStateManager.OnStart();

    [ContextMenu("Set State: Pause")]
    public void SetPauseState() => _gameStateManager.OnPause();

    [ContextMenu("Set State: Resume")]
    public void SetResumeState() => _gameStateManager.OnResume();

    [ContextMenu("Set State: Win")]
    public void SetWinState() => _gameStateManager.OnWin();

    [ContextMenu("Set State: Lose")]
    public void SetLoseState() => _gameStateManager.OnLose();
}
