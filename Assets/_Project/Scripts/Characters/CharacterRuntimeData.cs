using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

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

    // Events -----------------------------------

    public event Action<int, int> OnHealthChanged; // (current, max)
    public event Action<bool> OnSelected; // (isSelected)
    public event Action<int> OnCastSkill; // (actionCost)
    public event Action OnDeath;
 
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
        _actionsMax = 5;
        _actionsCurrent = _actionsMax;
        _speed = stats.Speed;

        OnHealthChanged?.Invoke(_hpCurrent, _hpMax);
        OnCastSkill?.Invoke(_actionsCurrent);
    }

    // Functions ---------------------------------------------
    public bool UseSkill(int index, CharacterRuntimeData target)
    {
        if (_actionsCurrent - skills[index].ActionCost < 0)
            return false;

        if (skills[index].Effect == SkillsSO.SkillEffects.ATTACK)
        {
            target.TakeDamage(skills[index].Power);
        } 
        if (skills[index].Effect == SkillsSO.SkillEffects.HEAL)
        {
            target.Heal(skills[index].Power);
        }

        _actionsCurrent -= skills[index].ActionCost;
        OnCastSkill?.Invoke(_actionsCurrent);
        return true;
    }

    public void RecoverActions()
    {
        _actionsCurrent = _actionsMax;
    }

    public UnitsSO GetStatsSO()
    {
        return stats;
    }

    private void Heal(int cure)
    {
        _hpCurrent = Mathf.Min(_hpCurrent + cure, _hpMax);
        OnHealthChanged?.Invoke(_hpCurrent, _hpMax);
    }

    private void TakeDamage(int damage)
    {
        _hpCurrent -= damage;
        OnHealthChanged?.Invoke(_hpCurrent, _hpMax);

        if (_hpCurrent <= 0)
            StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(2f);
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public void IsSelected(bool isSelected)
    {
        OnSelected?.Invoke(isSelected);
    }

    public List<Sprite> GetSkillsImages()
    {
        List<Sprite> skillImages = new List<Sprite>();
        foreach (SkillsSO skillsSO in skills)
            skillImages.Add(skillsSO.SkillImage);

        return skillImages;
    }
}
