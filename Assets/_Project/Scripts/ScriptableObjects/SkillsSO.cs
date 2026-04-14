using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillsSO", menuName = "Scriptable Objects/SkillsSO")]
public class SkillsSO : ScriptableObject
{
    public enum SkillEffects { ATTACK, HEAL, BUFF, DEBUFF, STUN, SHIELD };

    [SerializeField] private string _skillName;
    [SerializeField] private string _skillDescription;
    [SerializeField] private int _actionCost;
    [SerializeField] private int _power;
    [SerializeField] private int _cooldown;
    [SerializeField] private bool _isAOE;
    [SerializeField] private Sprite _skillImage;
    [SerializeField] private List<SkillEffects> _skillEffect;

    // Getters ------------------------------------
    public string SkillName => _skillName;
    public string Description => _skillDescription;
    public int ActionCost => _actionCost;
    public int Power => _power;
    public bool IsAOE => _isAOE;
    public int Cooldown => _cooldown;
    public Sprite SkillImage => _skillImage;
    public List<SkillEffects> Effect => _skillEffect;
}
