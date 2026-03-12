using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script que controla todos os stats dos personagens, usa eventos para atualizar a UI dos personagens.
 * 
 * ----------------------------------------------------------------------------------------------------
 * Como usar:
 * 1) Anexar esse Script ao prefab dos personagens
 * 2) Definir os SOs pela Unity para as variaveis stats e skills
 * 
 * --------------------------------------------------------------
 * Dependencias:
 * - UnitSO
 * - SkillsSO
*/

public class CharacterRuntimeData : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private int _hpMax;
    [SerializeField] private int _hpCurrent;
    [SerializeField] private int _shield;
    [SerializeField] private int _damageBuff;
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
    public int Shield => _shield;
    public int DamageBuff => _damageBuff;
    public int ActionsCurrent => _actionsCurrent;
    public int Speed => _speed;
    public UnitsSO Stats => stats;
    public SkillsSO[] Skills => skills;

    // Functions ---------------------------------------------
    public void InitializeStats(UnitsSO stats)
    {
        this.stats = stats;
        _name = stats.name;
        _hpMax = stats.HpMax;
        _hpCurrent = _hpMax;
        _shield = 0;
        _damageBuff = 0;
        _actionsMax = 5;
        _actionsCurrent = _actionsMax;
        _speed = stats.Speed;

        OnHealthChanged?.Invoke(_hpCurrent, _hpMax);
        OnCastSkill?.Invoke(_actionsCurrent);
    }

    public bool UseSkill(int index, CharacterRuntimeData target)
    {
        if (_actionsCurrent - skills[index].ActionCost < 0)
            return false;

        if (skills[index].Effect.Contains(SkillsSO.SkillEffects.ATTACK))
        {
            if (_damageBuff >= 0)
            {
                target.TakeDamage(skills[index].Power + _damageBuff);
                _damageBuff = 0;
            }
            else
                target.TakeDamage(skills[index].Power);
        }
        if (skills[index].Effect.Contains(SkillsSO.SkillEffects.HEAL))
        {
            target.Heal(skills[index].Power);
        }
        if (skills[index].Effect.Contains(SkillsSO.SkillEffects.BUFF))
        {
            target._damageBuff += skills[index].Power;
        }
        if (skills[index].Effect.Contains(SkillsSO.SkillEffects.SHIELD))
        {
            target._shield += skills[index].Power;
        }

        _actionsCurrent -= skills[index].ActionCost;
        OnCastSkill?.Invoke(_actionsCurrent);
        return true;
    }

    public void RecoverActions()
    {
        _actionsCurrent = _actionsMax;
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

    // Private Functions ----------------------------------------

    private void Heal(int cure)
    {
        _hpCurrent = Mathf.Min(_hpCurrent + cure, _hpMax);
        OnHealthChanged?.Invoke(_hpCurrent, _hpMax);
    }

    private void TakeDamage(int damage)
    {
        int overdamage = (_shield - damage) * -1;
        _shield -= damage;
        if (overdamage <= 0)
            return;

        _shield = 0;
        _hpCurrent -= overdamage;
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
}
