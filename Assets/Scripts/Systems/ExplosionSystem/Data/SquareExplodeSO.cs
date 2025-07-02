using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SquareExplodeSO", menuName = "Explosions/SquareExplodeSO")]
 public class SquareExplodeSO : ScriptableObject
 {
     [FormerlySerializedAs("type")] [FormerlySerializedAs("explosionType")] [SerializeField] private ExplosionType _type;
     public ExplosionType Type => _type;
     
     [SerializeField] private AudioClip _audioClip;

     public AudioClip AudioClip => _audioClip;
     
     
     [SerializeField] private ParticleSystem _particle;
     
     public ParticleSystem Particle => _particle;

 }
 public enum ExplosionType
 {
     Default,
     Fire,
     Ice,
     Heal
 }
