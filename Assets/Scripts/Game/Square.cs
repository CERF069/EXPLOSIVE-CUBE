using Zenject;
using UnityEngine;

public class Square : MonoBehaviour, IClickable, ISpawner
{
    [SerializeField] private SquareExplodeSO _explodeConfig;
    [SerializeField] private SquarePhysicsSO _physicsConfig;
    
    [SerializeField] private Rigidbody2D _rigidbody2D;
    private Camera _camera;

    private IExplosionFactory _explosionFactory;
    private ExplosionBase _explosionInstance;

    [Inject]
    public void Initialized(IExplosionFactory explosionFactory)
    {
        if (_explodeConfig == null)
            throw new System.ArgumentNullException(nameof(_explodeConfig), "Explode config is not assigned!");

        _explosionFactory = explosionFactory;
        _explosionInstance = _explosionFactory.Create(_explodeConfig.Type);

        if (_explosionInstance == null)
            throw new System.Exception($"Factory returned null for type {_explodeConfig.Type}");
        
        _camera = Camera.main;
    }
    private void OnValidate()
    {
        if (_explodeConfig == null)
            Debug.LogWarning($"[Square] Explode config is not assigned on {gameObject.name}");
    }
    
    public void OnSpawn(Vector3 position)
    {
        transform.position = position;
        SetActive(true);
        ApplySpawnForces();
    }

    private void ApplySpawnForces()
    {
        if (_physicsConfig == null)
        {
            Debug.LogWarning($"[Square] Physics config is not assigned for {gameObject.name}");
            return;
        }

        if (_rigidbody2D == null)
        {
            Debug.LogWarning($"[Square] Rigidbody2D is not assigned for {gameObject.name}");
            return;
        }

        if (_camera == null)
            _camera = Camera.main;
        
        Vector3 centerWorld = _camera.ViewportToWorldPoint(
            new Vector3(0.5f, 0.5f, transform.position.z - _camera.transform.position.z)
        );
        
        Vector2 randomOffset = Random.insideUnitCircle * 3f;
        Vector2 randomTarget = (Vector2)centerWorld + randomOffset;

        // Направление от текущей позиции к этой рандомной точке
        Vector2 direction = (randomTarget - (Vector2)transform.position).normalized;

        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0f;

        _rigidbody2D.AddForce(direction * _physicsConfig.ForceMagnitude, ForceMode2D.Impulse);

        _rigidbody2D.angularVelocity = Random.Range(_physicsConfig.AngularVelocityMin, _physicsConfig.AngularVelocityMax);
    }


    public void OnClick()
    {
        SetActive(false);
        if (_explosionInstance == null)
        {
            Debug.LogError("[Square] _explosionInstance is NULL! Ensure it was properly injected.");
            return;
        }
        _explosionInstance.Explode(_explodeConfig, gameObject.transform);
       
    }
    

    private void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
