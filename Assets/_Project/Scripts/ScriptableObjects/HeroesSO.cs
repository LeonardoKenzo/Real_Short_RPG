using UnityEngine;

[CreateAssetMenu(fileName = "HeroesSO", menuName = "Scriptable Objects/HeroesSO")]
public class HeroesSO : ScriptableObject
{
    public string _name;
    public float _hpMax;
    public int _actionsMax;
    public float _speed;

    // Getters ----------------
    public string HeroName{
        get { return _name; }
    }
    public float HpMax {
        get { return _hpMax; }
    }
    public int ActionsMax {
        get { return _actionsMax; }
    }
    public float Speed {
        get { return _speed; }
    }
}
