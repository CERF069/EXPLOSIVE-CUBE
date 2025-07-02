using UnityEngine;
using System.Collections;

public class TimeSlowdownSystem : MonoBehaviour
{
    private Coroutine _slowdownCoroutine;
    public void SlowTime(float duration, float timeScale)
    {
        if (_slowdownCoroutine != null)
            StopCoroutine(_slowdownCoroutine);

        _slowdownCoroutine = StartCoroutine(SlowdownRoutine(duration, timeScale));
    }

    private IEnumerator SlowdownRoutine(float duration, float targetTimeScale)
    {
        float originalFixedDeltaTime = Time.fixedDeltaTime;
        Time.timeScale = targetTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime * targetTimeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
    }
}