using Zenject;
using UnityEngine;

public class GameStateManager
{
    private GameState currentState;
    private readonly SignalBus _signalBus;
    private readonly UIController _uiController;

    [Inject]
    public GameStateManager(SignalBus signalBus, UIController uiController)
    {
        _signalBus = signalBus;
        _uiController = uiController;
    }

    public void ChangeState(GameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void OnStart() => ChangeState(new StartState(this, _signalBus, _uiController));
    public void OnPause() => ChangeState(new PauseState(this, _signalBus, _uiController));
    public void OnResume() => ChangeState(new ResumeState(this, _signalBus, _uiController));
    public void OnWin() => ChangeState(new WinState(this, _signalBus, _uiController));
    public void OnLose() => ChangeState(new LoseState(this, _signalBus, _uiController));
}


public abstract class GameState
{
    protected readonly GameStateManager manager;
    protected readonly SignalBus signalBus;
    protected readonly UIController _uiController;

    protected GameState(GameStateManager manager, SignalBus signalBus, UIController uiController)
    {
        this.manager = manager;
        this.signalBus = signalBus;
        this._uiController = uiController;
    }

    public abstract void Enter();
    public abstract void Exit();
}


public class StartState : GameState
{
    public StartState(GameStateManager manager, SignalBus signalBus, UIController uiController) 
        : base(manager, signalBus, uiController) { }

    public override void Enter()
    {
        Debug.Log("Game Started");
        Time.timeScale = 1;
        _uiController.HideAllPanels();
        _uiController.ShowPanel(_uiController.PauseButton, true);
        _uiController.SetUIElementVisible(_uiController.MissesLeftText, true);
        signalBus.Fire<StartGameSignal>();
    }
    public override void Exit()
    {
        //Debug.Log("Exit Start State");
    }
}

public class PauseState : GameState
{
    public PauseState(GameStateManager manager, SignalBus signalBus, UIController uiController) 
        : base(manager, signalBus, uiController) { }

    public override void Enter()
    {
        Debug.Log("Game Paused");
        Time.timeScale = 0;
        _uiController.ShowPanel(_uiController.PausePanel, true); 
        _uiController.ShowPanel(_uiController.PauseButton, false); 
        
        signalBus.Fire<PauseGameSignal>();
    }

    public override void Exit()
    {
        _uiController.ShowPanel(_uiController.PausePanel, false); 
        _uiController.ShowPanel(_uiController.PauseButton, true); 
    }
}


public class ResumeState : GameState
{
    public ResumeState(GameStateManager manager, SignalBus signalBus, UIController uiController) : base(manager, signalBus, uiController) { }

    public override void Enter()
    {
        Debug.Log("Game Resumed");
        Time.timeScale = 1;
        signalBus.Fire<ResumeGameSignal>();
    }

    public override void Exit()
    {
       // Debug.Log("Exit Resume State");
        _uiController.HideAllPanels(); 
    }
}

public class WinState : GameState
{
    private readonly SignalBus _signalBus;

    public WinState(GameStateManager manager, SignalBus signalBus, UIController uiController) : base(manager, signalBus, uiController)
    {
        _signalBus = signalBus;
    }

    public override void Enter()
    {
        Debug.Log("You Win!");
        _signalBus.Fire<WinGameSignal>();
        
        _uiController.ShowUIElementWithDelay(_uiController.WinPanel, 2f);

        
        _uiController.ShowPanel(_uiController.PauseButton, false); 
        
        _uiController.SetUIElementVisible(_uiController.MissesLeftText, false);
    }

    public override void Exit()
    {
       // Debug.Log("Exit Win State");
        _uiController.ShowPanel(_uiController.PausePanel, false); 
    }
}


public class LoseState : GameState
{
    public LoseState(GameStateManager manager, SignalBus signalBus, UIController uiController) : base(manager, signalBus, uiController) { }

    public override void Enter()
    {
        Debug.Log("You Lose!");
        signalBus.Fire<LoseGameSignal>();
        
        _uiController.ShowUIElementWithDelay(_uiController.LosePanel, 2f);
        
        _uiController.ShowPanel(_uiController.PauseButton, false); 
        
        _uiController.SetUIElementVisible(_uiController.MissesLeftText, false);
    }

    public override void Exit()
    {
        _uiController.ShowPanel(_uiController.LosePanel, false); 
    }
}
