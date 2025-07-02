using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private ClickHandler _clickHandler;
    [SerializeField] private AudioSystem _audioSystem;
    [SerializeField] private ParticlePlaySystem _particlePlaySystem;
    [SerializeField] private ExplosiveWaveSystem _explosiveWaveSystem;
    [SerializeField] private TimeSlowdownSystem _timeSlowdownSystem;
    [SerializeField] private CameraShake _cameraShake;
    [SerializeField] private Spawner _spawner;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        
        
        Container.DeclareSignal<ObjectMissedSignal>();
        
        Container.DeclareSignal<StartGameSignal>();
        Container.DeclareSignal<PauseGameSignal>();
        Container.DeclareSignal<ResumeGameSignal>();
        Container.DeclareSignal<WinGameSignal>();
        Container.DeclareSignal<LoseGameSignal>();
        
        
        Container.DeclareSignal<WaveProgressSignal>();
        Container.DeclareSignal<ObjectMissedSignal>();
        Container.DeclareSignal<AllWavesCompletedSignal>();
       

        
        Container.Bind<ClickHandler>().FromInstance(_clickHandler).AsSingle();
        Container.Bind<Spawner>().FromInstance(_spawner).AsSingle();
        Container.Bind<AudioSystem>().FromInstance(_audioSystem).AsSingle();
        Container.Bind<ParticlePlaySystem>().FromInstance(_particlePlaySystem).AsSingle();
        Container.Bind<ExplosiveWaveSystem>().FromInstance(_explosiveWaveSystem).AsSingle();
        Container.Bind<TimeSlowdownSystem>().FromInstance(_timeSlowdownSystem).AsSingle();
        Container.Bind<CameraShake>().FromInstance(_cameraShake).AsSingle();
        
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameStateManager>().AsSingle();

        Container.Bind<DefaultExplode>().AsTransient();
        Container.Bind<FireExplode>().AsTransient();
        Container.Bind<IceExplode>().AsTransient();
        
        Container.Bind<UIController>().FromComponentInHierarchy().AsSingle();

        Container.Bind<IExplosionFactory>().To<ExplosionFactory>().AsSingle();
        
        
        
    }
}
