using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Gerencia toda a UI do cenario de batalha
 * 
 * ------------------------------------------
 * Como usar:
 * 1) Coloque o prefab da BattleUI na cena (o script esta dentro do prefab)
 * 2) Se quiser, mude os GameObject das cenas de fim de jogo ou contador de acoes
 * 
 * -------------------------------------------------------------------------------
 * Dependencias:
 * - GameObject (fim de jogo)
 * - Images (Contador de acoes)
 * - RectTransforms (posicao das cartas)
 * - BattleSystem
 */

public class BattleUI : MonoBehaviour
{
    [Header("End Screen")]
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;

    [Header("Action Counter")]
    [SerializeField] private List<Image> _cursorPosition;
    private BattleSystem _battleSystem;

    [Header("Skills Cards")]
    [SerializeField] private List<Image> _skillsSprites;
    [SerializeField] private List<RectTransform> _skillsTransforms;
    private RectTransform _skillCard;

    public void Initialize(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
        _battleSystem.OnChangeActions += UpdateCursorPosition;
    }

    // Funcoes da Selecao de Skills (cartas) --------------------------------

    // Animacao de selecao das cartas
    public void MoveUpSmooth(int index, float distance, float duration)
    {
        _skillCard = _skillsTransforms[index];
        StartCoroutine(MoveRoutine(distance, duration));
    }

    public void MoveDownSmooth(int index, float distance, float duration)
    {
        _skillCard = _skillsTransforms[index];
        StartCoroutine(MoveRoutine(-distance, duration));
    }

    private IEnumerator MoveRoutine(float distance, float duration)
    {
        Vector2 startPos = _skillCard.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, distance);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _skillCard.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
    }

    public void UpdateSkillCards(List<Sprite> SkillSprites)
    {
        for (int i = 0; i < _skillsSprites.Count; i++)
        {
            _skillsSprites[i].sprite = SkillSprites[i];
        }
    }

    // Funcoes do contador de acoes -----------------------------------------------
    public void UpdateCursorPosition(int currentActions)
    {
        for (int i = 0; i < _cursorPosition.Count; i++)
        {
            _cursorPosition[i].enabled = false;
        }

        // Se ainda há ações, ativa o cursor correspondente (indexado de 0)
        if (currentActions >= 0 && currentActions <= _cursorPosition.Count)
        {
            _cursorPosition[currentActions].enabled = true;
        }
    }

    // Funcoes do fim da batalha ---------------------------------------------------
    public void WinScreen()
    {
        _winScreen.SetActive(true);
        _battleSystem.OnChangeActions -= UpdateCursorPosition;
    }
    public void LoseScreen()
    {
        _loseScreen.SetActive(true);
        _battleSystem.OnChangeActions -= UpdateCursorPosition;
    }
}
