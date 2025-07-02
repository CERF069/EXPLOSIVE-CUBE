using UnityEngine;

public class ExplosiveWaveSystem : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float _radius = 5f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField, Min(1)] private int _maxHits = 32;

    private readonly Collider2D[] _results = new Collider2D[32];
    private Vector2 _lastDebugPosition;
    public void TriggerWave(Vector2 position)
    {
        _lastDebugPosition = position;

        int hitCount = Physics2D.OverlapCircleNonAlloc(position, _radius, _results, _targetLayer);

        for (int i = 0; i < hitCount; i++)
        {
            if (_results[i].TryGetComponent<IClickable>(out var clickable))
            {
                clickable.OnClick();
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_lastDebugPosition, _radius);
    }
#endif
}