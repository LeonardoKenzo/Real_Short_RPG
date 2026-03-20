using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/*
 * Script que controla todos os _stats dos personagens, 
 * usa eventos para atualizar a UI dos personagens.
 * 
 * --------------------------------------------------------------
 * Como usar:
 * 1) Anexar esse Script ao prefab dos personagens
 * 2) Definir os SOs pela Unity para as variaveis _stats e _skills
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
    [SerializeField] private int _stunTime;
    [SerializeField] private bool _isStunned;
    [SerializeField] private UnitsSO _stats;
    private SkillsSO[] _skills;
    private BattleSystem _battleSystem;

    // Events -----------------------------------

    public event Action<int, int> OnHealthChanged; // (current, max)
    public event Action<bool> OnSelected; // (isSelected)
    public event Action OnDeath;

    // Getters ----------------------------------
    public string Name => _name;
    public int HpCurrent => _hpCurrent;
    public int Shield => _shield;
    public int DamageBuff => _damageBuff;
    public int StunTime => _stunTime;
    public bool IsStunned => _isStunned;
    public UnitsSO Stats => _stats;
    public SkillsSO[] Skills => _skills;

    // Functions ---------------------------------------------
    public void InitializeStats(UnitsSO stats, BattleSystem battleSystem)
    {
        this._stats = stats;
        _name = stats.name;
        _hpMax = stats.HpMax;
        _hpCurrent = _hpMax;
        _shield = 0;
        _damageBuff = 0;
        _stunTime = 0;
        _isStunned = false;
        _skills = stats.Skills;
        _battleSystem = battleSystem;

        OnHealthChanged?.Invoke(_hpCurrent, _hpMax);
        _battleSystem.OnPassTurn += StunDecrease;
        _battleSystem.OnPassTurn += UseBuff;
    }

    public bool UseSkill(SkillsSO skill, CharacterRuntimeData target)
    {
        if (skill.Effect.Contains(SkillsSO.SkillEffects.ATTACK) && _isStunned == false)
        {
            target.TakeDamage(skill.Power + _damageBuff);
        }
        if (skill.Effect.Contains(SkillsSO.SkillEffects.HEAL))
        {
            target.Heal(skill.Power);
        }
        if (skill.Effect.Contains(SkillsSO.SkillEffects.BUFF))
        {
            target._damageBuff += skill.Power;
        }
        if (skill.Effect.Contains(SkillsSO.SkillEffects.SHIELD))
        {
            target._shield += skill.Power;
        }
        if (skill.Effect.Contains(SkillsSO.SkillEffects.STUN))
        {
            target._isStunned = true;
            target._stunTime += skill.Power;
        }

        return true;
    }

    public void IsSelected(bool isSelected)
    {
        OnSelected?.Invoke(isSelected);
    }

    public List<Sprite> GetSkillsImages()
    {
        List<Sprite> skillImages = new List<Sprite>();
        foreach (SkillsSO skillsSO in _skills)
            skillImages.Add(skillsSO.SkillImage);

        return skillImages;
    }

    // Private Functions ----------------------------------------
    private void StunDecrease()
    {
        _stunTime = Mathf.Max(_stunTime - 1, 0);
        if (_stunTime <= 0)
        {
            _stunTime = 0;
            _isStunned = false;
        }
    }
    private void UseBuff()
    {
        _damageBuff = 0;
    }

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
        _battleSystem.OnPassTurn -= StunDecrease;
        _battleSystem.OnPassTurn -= UseBuff;

        yield return new WaitForSeconds(2f);
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
