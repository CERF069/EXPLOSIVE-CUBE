using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class ClickHandler : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float _clickRadius = 1.0f;
    [SerializeField] private LayerMask _clickableLayer;

    private Camera _mainCamera;
    private Controls _controls;
    private bool _isGameRunning = false;

    [Inject] private SignalBus _signalBus;

    [Inject]
    public void Initialized()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("[ClickHandler] Main Camera not found! Make sure a camera is tagged as 'MainCamera'.");
            return;
        }

        _controls = new Controls();
        _controls.Player.Click.performed += OnClickPerformed;
        _controls.Enable();
        
        _signalBus.Subscribe<StartGameSignal>(OnGameStartOrResume);
        _signalBus.Subscribe<ResumeGameSignal>(OnGameStartOrResume);

        _signalBus.Subscribe<PauseGameSignal>(OnGamePausedOrEnded);
        _signalBus.Subscribe<WinGameSignal>(OnGamePausedOrEnded);
        _signalBus.Subscribe<LoseGameSignal>(OnGamePausedOrEnded);
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (!_isGameRunning) return;

        if (Mouse.current == null)
        {
            Debug.LogWarning("[ClickHandler] Mouse not detected.");
            return;
        }

        Vector2 screenPosition = Mouse.current.position.ReadValue();
        TryHandleClick(screenPosition);
    }

    private void TryHandleClick(Vector2 screenPosition)
    {
        if (_mainCamera == null) return;

        Vector2 worldPoint = _mainCamera.ScreenToWorldPoint(screenPosition);
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPoint, _clickRadius, _clickableLayer);

        if (hits.Length == 0) return;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IClickable>(out var clickable))
            {
                clickable.OnClick();
            }
        }
    }

    private void OnGameStartOrResume()
    {
        _isGameRunning = true;
    }

    private void OnGamePausedOrEnded()
    {
        _isGameRunning = false;
    }

    private void OnDisable()
    {
        if (_controls != null)
        {
            _controls.Player.Click.performed -= OnClickPerformed;
            _controls.Disable();
        }
        
        _signalBus.TryUnsubscribe<StartGameSignal>(OnGameStartOrResume);
        _signalBus.TryUnsubscribe<ResumeGameSignal>(OnGameStartOrResume);
        _signalBus.TryUnsubscribe<PauseGameSignal>(OnGamePausedOrEnded);
        _signalBus.TryUnsubscribe<WinGameSignal>(OnGamePausedOrEnded);
        _signalBus.TryUnsubscribe<LoseGameSignal>(OnGamePausedOrEnded);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;
        if (_mainCamera == null) return;

        Vector2 mousePosition = Mouse.current?.position.ReadValue() ?? Vector2.zero;
        Vector2 worldPoint = _mainCamera.ScreenToWorldPoint(mousePosition);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(worldPoint, _clickRadius);
    }
#endif
}
