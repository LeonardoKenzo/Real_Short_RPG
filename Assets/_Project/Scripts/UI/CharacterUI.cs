using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

/*
 * Gerencia toda a UI dos personagens
 * 
 * ------------------------------------
 * Como usar:
 * 1) Anexe o script ao filho responavel pela UI do prefab do personagem
 * 2) Coloque as imagens da barra de vida e do selecionador do personagem
 * 
 * -----------------------------------------------------------------------
 * Dependencias:
 * - CharacterRuntimeData do pai
 * - Images
 */
public class CharacterUI : MonoBehaviour
{
    [SerializeField] private CharacterRuntimeData _unit;

    [Header("Cursor")]
    [SerializeField] private Image _selectedUnit;

    [Header("HP Bar")]
    [SerializeField] private Image _hpBar;
    [SerializeField] private float _lerpSpeed = 5f; // Animacao da barra de vida
    [SerializeField] private float _targetLerpValue;
    private float _targetFill;
    private float _currentFill;

    [Header("Effects")]
    [SerializeField] private Image _stunImage;
    [SerializeField] private Image _buffImage;
    [SerializeField] private Image _shieldImage;

    private void Awake()
    {
        if (_unit == null)
            _unit = GetComponentInParent<CharacterRuntimeData>();
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

    // Funcoes do cursor de selecao de personagem ---------------------
    
    private void SelectHero(bool isEnabled)
    {
        _selectedUnit.enabled = isEnabled;
    }

    private void EffectUIApply(SkillEffects effects)
    {
        switch (effects)
        {
            case SkillEffects.BUFF:
                _buffImage.enabled = true;
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
            _unit.OnSelected += SelectHero;
            _unit.OnEffectsApplied += EffectUIApply;
            _unit.OnEffectsRemove += EffectUIRemove;
        }
    }

    private void OnDisable()
    {
        if (_unit != null)
        {
            _unit.OnHealthChanged -= UpdateHealth;
            _unit.OnSelected -= SelectHero;
            _unit.OnEffectsApplied -= EffectUIApply;
            _unit.OnEffectsRemove -= EffectUIRemove;
        }
    }

}
