using UnityEngine;

public class EnemyRuntimeStats : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private float _hpMax;
    [SerializeField] private float _hpCurrent;
    [SerializeField] private int _actionsMax;
    [SerializeField] private float _actionsCurrent;
    [SerializeField] private float _speed;

    // Getters ------------------------------------
    public string EnemyName => _name;
    public float HPMax => _hpMax;
    public float HPCurrent
    {
        get { return _hpCurrent; }
        set { _hpCurrent = Mathf.Clamp(value, 0f, _hpMax); }
    }
    public float ActionsMax => _actionsMax;
    public float ActionsCurrent
    {
        get { return _actionsCurrent; }
        set { _actionsCurrent = Mathf.Clamp(value, 0, _actionsCurrent); }
    }
    public float Speed => _speed;

    // Constructor ---------------------------------
    public EnemyRuntimeStats(EnemiesSO enemiesStats)
    {
        _name = enemiesStats.EnemyName;
        _hpMax = enemiesStats.HpMax;
        _actionsMax = enemiesStats.ActionsMax;
        _speed = enemiesStats.Speed;
        _hpCurrent = _hpMax;
        _actionsCurrent = _actionsMax;
    }
}
