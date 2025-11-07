using UnityEngine;

[CreateAssetMenu(fileName = "SkillsSO", menuName = "Scriptable Objects/SkillsSO")]
public class SkillsSO : ScriptableObject
{
    public enum SkillEffects { Attack, Heal }; // Adicionar outros depois

    [SerializeField] private string _skillName;
    [SerializeField] private string _skillDescription;
    [SerializeField] private int _actionCost;
    [SerializeField] private int _power;
    [SerializeField] private int _cooldown;
    [SerializeField] private SkillEffects _skillEffect;

    // Getters ----------------
    public string UnitName => _skillName;
    public string Description => _skillDescription;
    public int ActionCost => _actionCost;
    public int Power => _power;
    public int Cooldown => _cooldown;
    public SkillEffects Effect => _skillEffect;
}
