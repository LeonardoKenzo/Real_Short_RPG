using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersonagemController : MonoBehaviour
{
    //Status
    protected string _name;
    protected int _hpMax, _hpAtual, _speed;

    //Efeitos
    protected int _roundsStunned, _shield;
    protected Dictionary<int, int> _buffs, _debuffs;

    protected List<HabilidadesSO> _skills;

    public bool IsStunned => _roundsStunned > 0;
    public List<HabilidadesSO> Skills => _skills;

    public Action<int, int> OnHealthChanged;
    public Action<SkillEffects> OnEffectsApplied, OnEffectsRemoved;

    public void InicializarPersonagem(PersonagemSO dados)
    {
        _name = dados.UnitName;
        _hpMax = dados.HpMax;
        _hpAtual = _hpMax;
        _speed = dados.Speed;
        _skills = dados.Skills;

        _roundsStunned = 0;
        _shield = 0;

        _buffs = new Dictionary<int, int>();
        _debuffs = new Dictionary<int, int>();

        OnHealthChanged?.Invoke(_hpAtual, _hpMax);

        EventosGlobais.FimRodada.AddListener(FimRodada);
    }

    private void FimRodada()
    {
        _roundsStunned--;
        if(_roundsStunned < 0)
            _roundsStunned = 0;
        else if (_roundsStunned == 0)
            OnEffectsRemoved?.Invoke(SkillEffects.STUN);

        foreach (var chave in _buffs.Keys.OrderBy(k => k).ToList())
        {
            var valor = _buffs[chave];

            if (chave == 1)
            {
                _buffs.Remove(chave);
                continue;
            }

            _buffs[chave - 1] = valor;
            _buffs.Remove(chave);
        }
        foreach (var chave in _debuffs.Keys.OrderBy(k => k).ToList())
        {
            var valor = _debuffs[chave];

            if (chave == 1)
            {
                _debuffs.Remove(chave);
                continue;
            }

            _debuffs[chave - 1] = valor;
            _debuffs.Remove(chave);
        }
        if (_buffs.Count == 0)
            OnEffectsRemoved?.Invoke(SkillEffects.BUFF);
        if (_debuffs.Count == 0)
            OnEffectsRemoved?.Invoke(SkillEffects.DEBUFF);
    }

    public void ReceberDano(int dano)
    {
        int overdamage = (_shield - dano) * -1;
        _shield = Mathf.Max(_shield - dano, 0);
        if (overdamage < 0)
            return;
        
        _hpAtual -= overdamage;
        if (_hpAtual <= 0)
        {
            EventosGlobais.PersonagemMorreu.Invoke(this.gameObject);
            _hpAtual = 0;
        }

        OnEffectsRemoved?.Invoke(SkillEffects.SHIELD);
        OnHealthChanged?.Invoke(_hpAtual, _hpMax);
    }

    public void Curar(int cura)
    {
        _hpAtual += cura;

        if (_hpAtual > _hpMax)
            _hpAtual = _hpMax;

        OnHealthChanged?.Invoke(_hpAtual, _hpMax);
    }

    public virtual void UsarHabilidade(HabilidadesSO habilidade, List<PersonagemController> aliadosTarget, List<PersonagemController> inimigosTarget)
    {
        //Não deve cair nunca aqui, deve ser tratado externamente
        if (_roundsStunned > 0)
        {
            Debug.LogError("PERSONAGEM " + this._name + " TENTOU REALIZAR UMA AÇÃO STUNNADO!!!");
            return;
        }

        foreach (SkillEffects efeito in habilidade.Effect)
        {
            switch (efeito)
            {
                case SkillEffects.ATTACK:
                    foreach (PersonagemController inimigo in inimigosTarget)
                    {
                        inimigo.ReceberDano(habilidade.Dano + _buffs.Values.Sum() + _debuffs.Values.Sum());
                    }
                    break;

                case SkillEffects.HEAL:
                    foreach (PersonagemController aliado in aliadosTarget)
                        aliado.Curar(habilidade.Cura);
                    break;

                case SkillEffects.SHIELD:
                    foreach (PersonagemController aliado in aliadosTarget)
                    {
                        aliado._shield += habilidade.Shield;
                        aliado.OnEffectsApplied?.Invoke(SkillEffects.SHIELD);
                    }
                    break;

                case SkillEffects.BUFF:
                    foreach (PersonagemController aliado in aliadosTarget)
                    {
                        if (aliado._buffs.ContainsKey(habilidade.BuffTime))
                            aliado._buffs[habilidade.BuffTime] += habilidade.Buff;
                        else
                            aliado._buffs[habilidade.BuffTime] = habilidade.Buff;

                        aliado.OnEffectsApplied?.Invoke(SkillEffects.BUFF);
                    }
                    break;

                case SkillEffects.DEBUFF:
                    foreach (PersonagemController inimigo in inimigosTarget)
                    {
                        if (inimigo._debuffs.ContainsKey(habilidade.DebuffTime))
                            inimigo._debuffs[habilidade.DebuffTime] += -habilidade.Debuff;
                        else
                            inimigo._debuffs[habilidade.DebuffTime] = -habilidade.Debuff;

                        inimigo.OnEffectsApplied?.Invoke(SkillEffects.DEBUFF);
                    }
                    break;

                case SkillEffects.STUN:
                    foreach (PersonagemController inimigo in inimigosTarget)
                    {
                        inimigo._roundsStunned += habilidade.StunTime;
                        inimigo.OnEffectsApplied?.Invoke(SkillEffects.STUN);
                    }
                    break;
            }
        }
    }
}
