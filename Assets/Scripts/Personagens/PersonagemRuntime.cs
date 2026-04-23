using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PersonagemRuntime : MonoBehaviour
{
    private string _name;
    private int _hpMax, _hpCurrent, _shield, _damageBuff, _stunTime;
    private bool _isStunned;
    private UnitsSO _stats;
    private SkillsSO[] _skills;
    private GameObject _prefab;

    //UI
    public event Action<int, int> OnHealthChanged; // (current, max)
    public event Action<SkillsSO.SkillEffects> OnEffectsApplied;
    public event Action<SkillsSO.SkillEffects> OnEffectsRemove;

    // Getters ----------------------------------
    public string Name => _name;
    public int HpCurrent => _hpCurrent;
    public int Shield => _shield;
    public int DamageBuff => _damageBuff;
    public int StunTime => _stunTime;
    public bool IsStunned => _isStunned;
    public UnitsSO Stats => _stats;
    public SkillsSO[] Skills => _skills;
    public GameObject Prefab => _prefab;

    // Functions ---------------------------------------------

    public PersonagemRuntime(UnitsSO stats)
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
        _prefab = stats.Prefab;

        OnHealthChanged?.Invoke(_hpCurrent, _hpMax);
    }

    public void CartaSelecionada(SkillsSO skill, List<PersonagemRuntime> aliados, List<PersonagemRuntime> oponentes)
    {
        if (skill.IsAOE)
        {
            foreach (PersonagemRuntime aliado in aliados)
                UseSkill(skill, aliado, true);

            foreach (PersonagemRuntime inimigo in oponentes)
                UseSkill(skill, inimigo, false);
        }
        else
        {
            StartCoroutine(SkillEmAlvo(skill));
        }
    }

    private IEnumerator SkillEmAlvo(SkillsSO skill)
    {
        EventosGlobais.AtivarEscolhaAlvo.Invoke(true);
        UnityAction<GameObject> lambda;
        bool selecionado = false;

        switch(skill.Effect[0])
        {
            case SkillsSO.SkillEffects.ATTACK:
            case SkillsSO.SkillEffects.STUN:
                lambda = (GameObject inimigo) =>
                    {
                        UseSkill(skill, inimigo.GetComponent<PersonagemRuntime>(), false);
                        selecionado = true;
                    };
                EventosGlobais.InimigoSelecionado.AddListener(lambda);

                yield return new WaitUntil(() => selecionado);

                EventosGlobais.InimigoSelecionado.RemoveListener(lambda);

                break;
            default:
                lambda = (GameObject aliado) =>
                    {
                        UseSkill(skill, aliado.GetComponent<PersonagemRuntime>(), true);
                        selecionado = true;
                    };
                EventosGlobais.AliadoSelecionado.AddListener(lambda);

                yield return new WaitUntil(() => selecionado);

                EventosGlobais.AliadoSelecionado.RemoveListener(lambda);

                break;
        }
        EventosGlobais.AcaoConcluida.Invoke();
    }

    public void UseSkill(SkillsSO skill, PersonagemRuntime target, bool ehAliado)
    {
        foreach (var effect in skill.Effect)
        {
            if (ehAliado)
            {
                switch (effect)
                {
                    case SkillsSO.SkillEffects.HEAL:
                        target.Heal(skill.Power);
                        break;

                    case SkillsSO.SkillEffects.BUFF:
                        target._damageBuff += skill.Power;
                        break;

                    case SkillsSO.SkillEffects.SHIELD:
                        target._shield += skill.Power;
                        break;
                }
            }
            else
            {
                switch (effect)
                {
                    case SkillsSO.SkillEffects.ATTACK:
                        if (!_isStunned)
                        {
                            target.TakeDamage(skill.Power + _damageBuff);
                            UseBuff();
                        }
                        break;
                        
                    case SkillsSO.SkillEffects.STUN:
                        target._isStunned = true;
                        target._stunTime += skill.Power;
                        break;
                }
            }
            target.OnEffectsApplied?.Invoke(effect);
        }
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
        _stunTime--;
        if (_stunTime <= 0)
        {
            _stunTime = 0;
            _isStunned = false;
            OnEffectsRemove?.Invoke(SkillsSO.SkillEffects.STUN);
        }
    }
    private void UseBuff()
    {
        _damageBuff = 0;
        OnEffectsRemove?.Invoke(SkillsSO.SkillEffects.BUFF);
    }

    private void Heal(int cure)
    {
        _hpCurrent = Mathf.Min(_hpCurrent + cure, _hpMax);
        OnHealthChanged?.Invoke(_hpCurrent, _hpMax);
    }

    private void TakeDamage(int damage)
    {
        int overdamage = (_shield - damage) * -1;
        _shield = Math.Max(_shield - damage, 0);
        if (overdamage < 0)
            return;
        
        _hpCurrent -= overdamage;
        OnEffectsRemove?.Invoke(SkillsSO.SkillEffects.SHIELD);
        OnHealthChanged?.Invoke(_hpCurrent, _hpMax);
        /*if (_hpCurrent <= 0)
            StartCoroutine(Die());*/
    }


    /*private IEnumerator Die()
    {
        _battleSystem.OnPassTurn -= StunDecrease;

        yield return new WaitForSeconds(2f);
        OnDeath?.Invoke();
        Destroy(gameObject);
    }*/
}
