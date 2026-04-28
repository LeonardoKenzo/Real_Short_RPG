using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PersonagemSO", menuName = "Scriptable Objects/PersonagemSO")]
public class PersonagemSO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private int _hpMax, _speed;
    [SerializeField] private List<HabilidadesSO> _skills;
    [SerializeField] private GameObject _Prefab;

    // Getters --------------------
    public string UnitName => _name;
    public int HpMax => _hpMax;
    public int Speed => _speed;
    public List<HabilidadesSO> Skills => _skills;
    public GameObject Prefab => _Prefab;
}
