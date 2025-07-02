using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlaySystem : MonoBehaviour
{
    private Dictionary<ParticleSystem, List<ParticleSystem>> _pool = new Dictionary<ParticleSystem, List<ParticleSystem>>();

    public void PlayParticleAt(ParticleSystem prefab, Vector3 position)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Prefab is null. Cannot play particle.");
            return;
        }

        if (!_pool.TryGetValue(prefab, out var list))
        {
            list = new List<ParticleSystem>();
            _pool[prefab] = list;
        }

        ParticleSystem psToPlay = null;
        for (int i = 0; i < list.Count; i++)
        {
            var ps = list[i];
            if (ps == null || ps.gameObject == null)
            {
                list.RemoveAt(i);
                i--;
                continue;
            }

            if (!ps.gameObject.activeInHierarchy)
            {
                psToPlay = ps;
                break;
            }
        }

        if (psToPlay == null)
        {
            psToPlay = Instantiate(prefab, transform);
            list.Add(psToPlay);
            StartCoroutine(DisableAfterPlaying(psToPlay));
        }

        psToPlay.transform.position = position;
        psToPlay.gameObject.SetActive(true);
        psToPlay.Play(true);
    }

    private IEnumerator DisableAfterPlaying(ParticleSystem ps)
    {
        while (ps != null && ps.gameObject != null && ps.isPlaying)
        {
            yield return null;
        }

        if (ps != null && ps.gameObject != null)
        {
            ps.gameObject.SetActive(false);
        }
    }
}