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

public class BatalhaUI : MonoBehaviour
{
    [Header("End Screen")]
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;

    [Header("Action Counter")]
    [SerializeField] private List<Image> _cursorPosition;
    private SistemaBatalha _battleSystem;

    [Header("Skills Cards")]
    [SerializeField] private List<Image> _skillsSprites;
    [SerializeField] private List<RectTransform> _skillsTransforms;
    [SerializeField] private float _distanciaSubida = 112f, _duracaoMovimento = 0.5f;
    private float _alturaInicialCartas;

    void Start()
    {
        _alturaInicialCartas = _skillsTransforms[0].anchoredPosition.y;
    }

    public void Initialize(SistemaBatalha battleSystem)
    {
        _battleSystem = battleSystem;
        _battleSystem.OnChangeActions += UpdateCursorPosition;
    }

    // Funcoes da Selecao de Skills (cartas) --------------------------------

    // Animacao de selecao das cartas
    public void MoveUpSmooth(int index)
    {
        RectTransform skillCard = _skillsTransforms[index];
        StartCoroutine(MoveRoutine(skillCard, _distanciaSubida, _duracaoMovimento));
    }

    public void MoveDownSmooth(int index)
    {
        RectTransform skillCard = _skillsTransforms[index];
        StartCoroutine(MoveRoutine(skillCard, 0, _duracaoMovimento));
    }

    private IEnumerator MoveRoutine(RectTransform skillCard, float distance, float duration)
    {
        Vector2 startPos = skillCard.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, _alturaInicialCartas) + new Vector2(0, distance);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            skillCard.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
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

        // Se ainda h� a��es, ativa o cursor correspondente (indexado de 0)
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
