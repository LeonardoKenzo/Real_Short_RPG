using UnityEngine;

[CreateAssetMenu(fileName = "UnitsSO", menuName = "Scriptable Objects/UnitsSO")]
public class UnitsSO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private int _hpMax;
    [SerializeField] private int _speed;
    [SerializeField] private GameObject _Prefab;
    [SerializeField] private SkillsSO[] _skills;

    // Getters --------------------
    public string UnitName => _name;
    public int HpMax => _hpMax;
    public int Speed => _speed;
    public GameObject Prefab => _Prefab;
    public SkillsSO[] Skills => _skills;
}
