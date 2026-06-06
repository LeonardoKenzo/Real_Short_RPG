using System.Collections.Generic;
using UnityEngine;

public enum SkillEffects { ATTACK, HEAL, SHIELD, BUFF, DEBUFF, STUN, POISON, SUMMON };

[CreateAssetMenu(fileName = "HabilidadesSO", menuName = "Scriptable Objects/HabilidadesSO")]
public class HabilidadesSO : ScriptableObject
{
    [SerializeField] private string _skillName, _skillDescription;
    [SerializeField] private int _actionCost, _cooldown, _qtdAlvosAliados, _qtdAlvosInimigos;
    [SerializeField] private int _dano, _cura, _shield;
    [SerializeField] private int _forcaBuff, _duracaoBuff, _forcaDebuff, _duracaoDebuff, _roundsStun, _forcaVeneno, _duracaoVeneno;
    [SerializeField] private int _qtdSummons;
    [SerializeField] private List<PersonagemSO> _summons;
    [SerializeField] private Sprite _skillImage;
    [SerializeField] private List<SkillEffects> _skillEffect;
    [SerializeField] private GameObject _prefabAnimacao;

    // Getters ------------------------------------
    public string SkillName => _skillName;
    public string Description => _skillDescription;
    public int ActionCost => _actionCost;
    public int Cooldown => _cooldown;
    public int Dano => _dano;
    public int Cura => _cura;
    public int Shield => _shield;
    public int Buff => _forcaBuff;
    public int BuffTime => _duracaoBuff;
    public int Debuff => _forcaDebuff;
    public int DebuffTime => _duracaoDebuff;
    public int StunTime => _roundsStun;
    public int Veneno => _forcaVeneno;
    public int VenenoTime => _duracaoVeneno;
    public int QtdAlvosAliados => _qtdAlvosAliados;
    public int QtdAlvosInimigos => _qtdAlvosInimigos;
    public int QtdSummons => _qtdSummons;
    public List<PersonagemSO> Summons => _summons;
    public Sprite SkillImage => _skillImage;
    public List<SkillEffects> Effects => _skillEffect;
    public GameObject PrefabAnimacao => _prefabAnimacao;

    public HabilidadesSO Copiar()
    {
        HabilidadesSO copia = CreateInstance<HabilidadesSO>();

        copia._skillName = _skillName;
        copia._skillDescription = _skillDescription;

        copia._actionCost = _actionCost;
        copia._cooldown = _cooldown;

        copia._qtdAlvosAliados = _qtdAlvosAliados;
        copia._qtdAlvosInimigos = _qtdAlvosInimigos;

        copia._dano = _dano;
        copia._cura = _cura;
        copia._shield = _shield;

        copia._forcaBuff = _forcaBuff;
        copia._duracaoBuff = _duracaoBuff;

        copia._forcaDebuff = _forcaDebuff;
        copia._duracaoDebuff = _duracaoDebuff;

        copia._roundsStun = _roundsStun;

        copia._qtdSummons = _qtdSummons;

        copia._skillImage = _skillImage;

        copia._summons = new List<PersonagemSO>(_summons);
        copia._skillEffect = new List<SkillEffects>(_skillEffect);

        copia._prefabAnimacao = _prefabAnimacao;

        return copia;
    }
}
