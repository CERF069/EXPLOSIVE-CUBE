using UnityEngine;
using System.Collections;
using Zenject;

public class CameraShake : MonoBehaviour
{
    private Transform _cameraTransform;
    private Vector3 _originalPosition;
    private Coroutine _shakeCoroutine;

    [Inject]
    private void Construct()
    {
        var mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("[CameraShake] Camera.main не найдена!");
            return;
        }

        _cameraTransform = mainCamera.transform;
        _originalPosition = _cameraTransform.localPosition;
    }
    public void Shake(float magnitude, float duration)
    {
        if (_cameraTransform == null)
        {
            Debug.LogWarning("[CameraShake] Камера не найдена, тряска невозможна.");
            return;
        }

        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);

        _shakeCoroutine = StartCoroutine(ShakeCoroutine(magnitude, duration));
    }

    private IEnumerator ShakeCoroutine(float magnitude, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * magnitude;
            _cameraTransform.localPosition = _originalPosition + new Vector3(shakeOffset.x, shakeOffset.y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _cameraTransform.localPosition = _originalPosition;
        _shakeCoroutine = null;
    }
}