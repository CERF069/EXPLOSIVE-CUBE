using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Collections;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _pauseButton;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;

    [SerializeField] private TextMeshProUGUI _missesLeftText;
    [SerializeField] private Slider _waveProgressSlider;

    public GameObject StartPanel => _startPanel;
    public GameObject PausePanel => _pausePanel;
    public GameObject PauseButton => _pauseButton;
    public GameObject WinPanel => _winPanel;
    public GameObject LosePanel => _losePanel;
    public TextMeshProUGUI MissesLeftText => _missesLeftText;

    public void SetWaveProgressSlider(float normalizedValue)
    {
        _waveProgressSlider.value = Mathf.Clamp01(normalizedValue);
    }

    public void HideAllPanels()
    {
        _startPanel?.SetActive(false);
        _pausePanel?.SetActive(false);
        _winPanel?.SetActive(false);
        _losePanel?.SetActive(false);
        _pauseButton?.SetActive(false);
    }

    public void SetUIElementVisible(Object uiElement, bool visible)
    {
        if (uiElement == null) return;

        if (uiElement is GameObject go)
        {
            go.SetActive(visible);
        }
        else if (uiElement is Behaviour behaviour)
        {
            behaviour.enabled = visible;
        }
        else if (uiElement is Renderer renderer)
        {
            renderer.enabled = visible;
        }
        else if (uiElement is CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;
        }
        else
        {
            Debug.LogWarning($"UIController: Unsupported UI element type: {uiElement.GetType()}");
        }
    }

    public void ShowPanel(GameObject panel, bool show)
    {
        if (panel != null)
        {
            panel.SetActive(show);
        }
    }

    public void UpdateMissesLeft(int remaining)
    {
        if (_missesLeftText != null)
            _missesLeftText.text = remaining.ToString();
    }
    
    public void ShowUIElementWithDelay(GameObject uiElement, float delaySeconds)
    {
        StartCoroutine(ShowAfterDelayCoroutine(uiElement, delaySeconds));
    }

    private IEnumerator ShowAfterDelayCoroutine(GameObject uiElement, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (uiElement != null)
        {
            uiElement.SetActive(true);
        }
    }
}
