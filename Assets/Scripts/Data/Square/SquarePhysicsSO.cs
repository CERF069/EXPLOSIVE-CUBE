using UnityEngine;

[CreateAssetMenu(fileName = "SquarePhysicsSO", menuName = "Config/SquarePhysics")]
public class SquarePhysicsSO : ScriptableObject
{
    [Header("Сила движения к центру")]
    [SerializeField, Min(0.1f)] private float _forceMagnitude = 5f;

    [Header("Случайное вращение (угловая скорость)")]
    [SerializeField] private float _angularVelocityMin = -90f;
    [SerializeField] private float _angularVelocityMax = 90f;
    
    public float ForceMagnitude => _forceMagnitude;
    public float AngularVelocityMin => _angularVelocityMin;
    public float AngularVelocityMax => _angularVelocityMax;
}