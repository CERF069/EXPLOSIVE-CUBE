using UnityEngine;
using Zenject;
public class AudioSystem : MonoBehaviour
{
    private AudioSource _audioSource;

    [Inject]
    private void Initialized()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayExplosionSound(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound {clip} not found!");
        }
    }
}
