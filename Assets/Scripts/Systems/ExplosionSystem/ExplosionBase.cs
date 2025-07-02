using UnityEngine;
using Zenject;
public abstract class ExplosionBase
{
    protected AudioSystem _audioSystem;
    protected ParticlePlaySystem _particleSystem;
    protected ExplosiveWaveSystem _explosiveWaveSystem;
    protected TimeSlowdownSystem _timeSlowdownSystem;
    protected CameraShake _cameraShake;

    [Inject]
    public void Construct(AudioSystem audioSystem, ParticlePlaySystem particleSystem, ExplosiveWaveSystem explosiveWaveSystem, TimeSlowdownSystem timeSlowdownSystem, CameraShake cameraShake)
    {
        _audioSystem = audioSystem;
        
        _particleSystem = particleSystem;
        
        _explosiveWaveSystem = explosiveWaveSystem;
        
        _timeSlowdownSystem = timeSlowdownSystem;
        
        _cameraShake = cameraShake;
    }

    public abstract void Explode(SquareExplodeSO explodeConfig, Transform transform);
}

public interface IExplosionFactory
{
    ExplosionBase Create(ExplosionType type);
}
public class ExplosionFactory : IExplosionFactory
{
    private readonly DiContainer _container;

    public ExplosionFactory(DiContainer container)
    {
        _container = container;
    }

    public ExplosionBase Create(ExplosionType type)
    {
        switch (type)
        {
            case ExplosionType.Fire:
                return _container.Instantiate<FireExplode>();
            case ExplosionType.Ice:
                return _container.Instantiate<IceExplode>();
            case ExplosionType.Default:
                return _container.Instantiate<DefaultExplode>();
            case ExplosionType.Heal:
                return _container.Instantiate<HealExplode>();
            default:
                return _container.Instantiate<DefaultExplode>();
        }
    }

}
public class DefaultExplode : ExplosionBase
{
    public override void Explode(SquareExplodeSO explodeConfig,Transform transform)
    {
       _audioSystem.PlayExplosionSound(explodeConfig.AudioClip);

       _particleSystem.PlayParticleAt(explodeConfig.Particle, transform.position);
       
       _cameraShake.Shake(0.1f, 0.1f);
       
        //Debug.Log("Default explosion!");
    }
}

public class FireExplode : ExplosionBase
{
    public override void Explode(SquareExplodeSO explodeConfig, Transform transform)
    {
        _audioSystem.PlayExplosionSound(explodeConfig.AudioClip);
        
        _particleSystem.PlayParticleAt(explodeConfig.Particle, transform.position);
        
        _explosiveWaveSystem.TriggerWave(transform.position);
        
        _cameraShake.Shake(0.3f, 0.4f);
        
       // Debug.Log("Fire explosion!");
    }
}

public class IceExplode : ExplosionBase
{
    public override void Explode(SquareExplodeSO explodeConfig, Transform transform)
    {
        _audioSystem.PlayExplosionSound(explodeConfig.AudioClip);
        
        _particleSystem.PlayParticleAt(explodeConfig.Particle, transform.position);
        
        _timeSlowdownSystem.SlowTime(1f,0.4f);

        _cameraShake.Shake(0.1f, 0.1f);
        
       // Debug.Log("Ice explosion!");
    }
}

public class HealExplode : ExplosionBase
{
    private GameManager _gameManager;

    [Inject]
    public void InjectManager(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public override void Explode(SquareExplodeSO explodeConfig, Transform transform)
    {
        _audioSystem.PlayExplosionSound(explodeConfig.AudioClip);
        _particleSystem.PlayParticleAt(explodeConfig.Particle, transform.position);
        
        _cameraShake.Shake(0.05f, 0.05f);
        
        var missedCount = _gameManager.GetMissedCount();
        if (missedCount == 0)
        {
            _gameManager.IncreaseMissedLimit(1);
        }
        else
        {
            _gameManager.DecreaseMissedCount(1);
        }
    }
}

