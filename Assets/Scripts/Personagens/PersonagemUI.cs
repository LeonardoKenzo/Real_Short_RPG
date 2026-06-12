using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PersonagemController))]
public class PersonagemUI : MonoBehaviour
{
    [Header("HP Bar")]
    [SerializeField] private Image _hpBar;
    private float _lerpSpeed = 5f, _lerpMin = 0.005f; // Animacao da barra de vida
    private float _targetFill, _currentFill;
    private bool _lancouAcaoFimUpdateVida;

    [Header("Effects")]
    [SerializeField] private Image _stunImage, _buffImage, _debuffImage, _shieldImage, _poisonImage;

    private PersonagemController _unit;

    public Action HealthUpdateEnded;

    private void Awake()
    {
        _unit = GetComponent<PersonagemController>();

        _lancouAcaoFimUpdateVida = true;
    }

    private void Update()
    {
        // Atualiza a barra de vida lentamente
        //2*_lerpMin -> garante que a iteração não vai ultrapassar para o oposto da comparação
        //Ex: objetivo de 0.4, pular de 0.42 para 0.37 (_lerpMin == 0.05), com comparação de > 0.02
        if (Mathf.Abs(_currentFill - _targetFill) > 2*_lerpMin)
        {
            float variacao = Mathf.Lerp(0, _targetFill-_currentFill, Time.deltaTime * _lerpSpeed);
            if (variacao > 0)
                _currentFill += Mathf.Max(variacao, _lerpMin);
            else
                _currentFill += Mathf.Min(variacao, -_lerpMin);
            _hpBar.fillAmount = _currentFill;
        }
        else if (!_lancouAcaoFimUpdateVida)
        {
            _lancouAcaoFimUpdateVida = true;
            HealthUpdateEnded?.Invoke();
        }
    }

    // Funcoes da Barra de Vida ----------------------------------------

    private void UpdateHealth(int current, int max)
    {
        _targetFill = Mathf.Clamp01((float)current / max);
        _lancouAcaoFimUpdateVida = false;
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
            case SkillEffects.POISON:
                _poisonImage.enabled = true;
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
            case SkillEffects.POISON:
                _poisonImage.enabled = false;
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
