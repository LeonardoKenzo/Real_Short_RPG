using System;
using UnityEngine;

[System.Serializable]
public class HeroRuntimeStats
{
    [SerializeField] private string _heroName;
    [SerializeField] private float _hpMax;
    [SerializeField] private float _hpCurrent;
    [SerializeField] private int _actionsMax;
    [SerializeField] private float _speed;

    // Getters ---------------------------------
    public string HeroName => _heroName;
    public float HPMax => _hpMax;
    public float HpCurrent {
        get { return  _hpCurrent; }
        set { _hpCurrent = Math.Clamp(value, 0f, _hpMax); }
    }
    public int  ActionsMax => _actionsMax;
    public float Speed => _speed;

    // Constructor ------------------------------
    public HeroRuntimeStats(HeroesSO heroesStats)
    {
        _heroName = heroesStats.HeroName;
        _hpMax = heroesStats._hpMax;
        _actionsMax = heroesStats._actionsMax;
        _speed = heroesStats._speed;
    }
}
