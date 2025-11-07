using System.Collections;
using System.Globalization;
using UnityEngine;

public class CharacterRuntimeData : MonoBehaviour
{
    [SerializeField] private string _name; 
    [SerializeField] private int _hpMax;
    [SerializeField] private int _hpCurrent;
    [SerializeField] private int _actionsMax;
    [SerializeField] private int _actionsCurrent;
    [SerializeField] private int _speed;
    [SerializeField] private UnitsSO stats;
    [SerializeField] private SkillsSO[] skills;

    // Getters ----------------------------------
    public string Name => _name;
    public int HpCurrent => _hpCurrent;
    public int ActionsCurrent => _actionsCurrent;
    public int Speed => _speed;
    public UnitsSO Stats => stats;
    public SkillsSO[] Skills => skills;

    // Initialize Stats ---------------------------
    public void InitializeStats(UnitsSO stats)
    {
        this.stats = stats; 
        _name = stats.name;
        _hpMax = stats.HpMax;
        _hpCurrent = _hpMax;
        _actionsMax = 3;
        _actionsCurrent = _actionsMax;
        _speed = stats.Speed;
    }

    // Functions ---------------------------------------------
    public bool UseSkill(int index, CharacterRuntimeData target)
    {
        if (_actionsCurrent - skills[index].ActionCost < 0)
            return false;

        if (skills[index].Effect == SkillsSO.SkillEffects.Attack)
        {
            target.TakeDamage(skills[index].Power);
        } 
        else if (skills[index].Effect == SkillsSO.SkillEffects.Heal)
        {
            target.Heal(skills[index].Power);
        }

        _actionsCurrent -= skills[index].ActionCost;
        return true;
    }

    public void RecoverActions()
    {
        _actionsCurrent = _actionsMax;
    }

    private void Heal(int cure)
    {
        _hpCurrent += cure;
        if (_hpCurrent >= _hpMax)
        {
            _hpCurrent = _hpMax;
        }
    }

    private void TakeDamage(int damage)
    {
        _hpCurrent -= damage;
        if (_hpCurrent <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        // Faz uma animacao de morte

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }
}
