using System.Globalization;
using UnityEngine;

public class CharacterRuntimeData : MonoBehaviour
{
    [SerializeField] private string _name; 
    [SerializeField] private int _hpMax;
    [SerializeField] private int _hpCurrent;
    [SerializeField] private int _actionsMax;
    [SerializeField] private int _actionsCurrent;
    [SerializeField] private float _speed;

    // Getters ----------------------------------
    public string Name => _name;
    public int HpMax => _hpMax;
    public int HpCurrent
    {
        get { return _hpCurrent; }
        set { _hpCurrent = Mathf.Clamp(value, 0, HpMax); }
    }
    public int ActionsMax => _actionsMax;
    public int ActionsCurrent
    {
        get { return _actionsCurrent; }
        set { _actionsCurrent = Mathf.Clamp(value, 0, _actionsMax); }
    }
    public float speed => _speed;

    // Initialize Stats ---------------------------
    public CharacterRuntimeData(UnitsSO stats)
    {
        _name = stats.UnitName;
        _hpMax = stats.HpMax;
        _hpCurrent = _hpMax;
        _actionsMax = 3;
        _actionsCurrent = _actionsMax;
        _speed = stats.Speed;
    }
}
