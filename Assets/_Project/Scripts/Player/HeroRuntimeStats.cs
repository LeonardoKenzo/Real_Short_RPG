using System;
using UnityEngine;

[System.Serializable]
public class HeroRuntimeStats
{
    [SerializeField] private string _heroName;
    [SerializeField] private float _hpMax;
    [SerializeField] private float _hpCurrent;
    [SerializeField] private int _actionsMax;
    [SerializeField] private int _actionsCurrent;
    [SerializeField] private float _speed;

    // Getters ---------------------------------
    public string HeroName => _heroName;
    public float HPMax => _hpMax;
    public float HpCurrent {
        get { return  _hpCurrent; }
        set { _hpCurrent = Math.Clamp(value, 0f, _hpMax); }
    }
    public int  ActionsMax => _actionsMax;
    public int ActionsCurrent
    {
        get { return _actionsCurrent; }
        set { _actionsCurrent = Math.Clamp(value, 0, _actionsMax); }
    }
    public float Speed => _speed;

    // Constructor ------------------------------
    public HeroRuntimeStats(HeroesSO heroesStats)
    {
        _heroName = heroesStats.HeroName;
        _hpMax = heroesStats.HpMax;
        _actionsMax = heroesStats.ActionsMax;
        _speed = heroesStats.Speed;
        _hpCurrent = _hpMax;
        _actionsCurrent = _actionsMax;
    }
}
