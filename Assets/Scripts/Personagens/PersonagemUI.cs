using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PersonagemController))]
public class PersonagemUI : MonoBehaviour
{
    [Header("HP Bar")]
    [SerializeField] private Image _hpBar;
    [SerializeField] private float _lerpSpeed = 5f; // Animacao da barra de vida
    private float _targetFill;
    private float _currentFill;

    [Header("Effects")]
    [SerializeField] private Image _stunImage, _buffImage, _debuffImage, _shieldImage;

    private PersonagemController _unit;

    private void Awake()
    {
        _unit = GetComponent<PersonagemController>();
    }

    private void Update()
    {
        // Atualiza a barra de vida lentamente
        if (Mathf.Abs(_currentFill - _targetFill) > 0.001f)
        {
            _currentFill = Mathf.Lerp(_currentFill, _targetFill, Time.deltaTime * _lerpSpeed);
            _hpBar.fillAmount = _currentFill;
        }
    }

    // Funcoes da Barra de Vida ----------------------------------------

    private void UpdateHealth(int current, int max)
    {
        _targetFill = Mathf.Clamp01((float)current / max);
    }

    private void EffectUIApply(SkillEffects effects)
    {
        switch (effects)
        {
            case SkillEffects.BUFF:
                _buffImage.enabled = true;
                break;
            case SkillEffects.DEBUFF:
                _debuffImage.enabled = true;
                break;
            case SkillEffects.SHIELD:
                _shieldImage.enabled = true;
                break;
            case SkillEffects.STUN:
                _stunImage.enabled = true;
                break;
        }
    }

    private void EffectUIRemove(SkillEffects effects)
    {
        switch (effects)
        {
            case SkillEffects.BUFF:
                _buffImage.enabled = false;
                break;
            case SkillEffects.DEBUFF:
                _debuffImage.enabled = false;
                break;
            case SkillEffects.SHIELD:
                _shieldImage.enabled = false;
                break;
            case SkillEffects.STUN:
                _stunImage.enabled = false;
                break;
        }
    }

    // Liga as funcoes aos events ------------------------------------
    private void OnEnable()
    {
        if (_unit != null)
        {
            _unit.OnHealthChanged += UpdateHealth;
            _unit.OnEffectsApplied += EffectUIApply;
            _unit.OnEffectsRemoved += EffectUIRemove;
        }
    }

    private void OnDisable()
    {
        if (_unit != null)
        {
            _unit.OnHealthChanged -= UpdateHealth;
            _unit.OnEffectsApplied -= EffectUIApply;
            _unit.OnEffectsRemoved -= EffectUIRemove;
        }
    }
}
