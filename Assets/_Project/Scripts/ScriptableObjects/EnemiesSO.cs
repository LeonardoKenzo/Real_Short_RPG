using UnityEngine;

[CreateAssetMenu(fileName = "EnemiesSO", menuName = "Scriptable Objects/EnemiesSO")]
public class EnemiesSO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private float _hpMax;
    [SerializeField] private int _actionsMax;
    [SerializeField] private float _speed;

    // Getters ----------------
    public string EnemyName{
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