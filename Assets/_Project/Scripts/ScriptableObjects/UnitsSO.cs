using UnityEngine;

[CreateAssetMenu(fileName = "UnitsSO", menuName = "Scriptable Objects/UnitsSO")]
public class UnitsSO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private int _hpMax;
    [SerializeField] private float _speed;

    // Getters ----------------
    public string UnitName{
        get { return _name; }
    }
    public int HpMax {
        get { return _hpMax; }
    }
    public float Speed {
        get { return _speed; }
    }
}
