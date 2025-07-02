using Zenject;
public class StartGameSignal : Signal<StartGameSignal>{ }
public class PauseGameSignal : Signal<PauseGameSignal> { }
public class ResumeGameSignal : Signal<ResumeGameSignal> { }
public class WinGameSignal : Signal<WinGameSignal> { }
public class LoseGameSignal : Signal<LoseGameSignal> { }