using System.Collections.Generic;
using UnityEngine;

public enum SkillEffects { ATTACK, HEAL, BUFF, DEBUFF, STUN, SHIELD };

[CreateAssetMenu(fileName = "HabilidadesSO", menuName = "Scriptable Objects/HabilidadesSO")]
public class HabilidadesSO : ScriptableObject
{
    [SerializeField] private string _skillName, _skillDescription;
    [SerializeField] private int _actionCost, _cooldown, _qtdAlvosAliados, _qtdAlvosInimigos;
    [SerializeField] private int _dano, _cura, _shield, _forcaBuff, _roundsBuff, _roundsStun;
    [SerializeField] private Sprite _skillImage;
    [SerializeField] private List<SkillEffects> _skillEffect;

    // Getters ------------------------------------
    public string SkillName => _skillName;
    public string Description => _skillDescription;
    public int ActionCost => _actionCost;
    public int Dano => _dano;
    public int Cura => _cura;
    public int Shield => _shield;
    public int Buff => _forcaBuff;
    public int BuffTime => _roundsBuff;
    public int StunTime => _roundsStun;
    public int QtdAlvosAliados => _qtdAlvosAliados;
    public int QtdAlvosInimigos => _qtdAlvosInimigos;
    public int Cooldown => _cooldown;
    public Sprite SkillImage => _skillImage;
    public List<SkillEffects> Effect => _skillEffect;
}
