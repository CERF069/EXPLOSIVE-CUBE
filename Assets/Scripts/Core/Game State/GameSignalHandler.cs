using Zenject;
using System;
public class GameSignalHandler : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly GameStateManager _stateManager;

    public GameSignalHandler(SignalBus signalBus, GameStateManager stateManager)
    {
        _signalBus = signalBus;
        _stateManager = stateManager;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<StartGameSignal>(_stateManager.OnStart);
        _signalBus.Subscribe<PauseGameSignal>(_stateManager.OnPause);
        _signalBus.Subscribe<ResumeGameSignal>(_stateManager.OnResume);
        _signalBus.Subscribe<WinGameSignal>(_stateManager.OnWin);
        _signalBus.Subscribe<LoseGameSignal>(_stateManager.OnLose);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<StartGameSignal>(_stateManager.OnStart);
        _signalBus.Unsubscribe<PauseGameSignal>(_stateManager.OnPause);
        _signalBus.Unsubscribe<ResumeGameSignal>(_stateManager.OnResume);
        _signalBus.Unsubscribe<WinGameSignal>(_stateManager.OnWin);
        _signalBus.Unsubscribe<LoseGameSignal>(_stateManager.OnLose);
    }
}