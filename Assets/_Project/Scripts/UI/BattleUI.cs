using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("End Screen")]
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;

    [Header("Action Counter")]
    [SerializeField] private List<Image> _cursorPosition;

    [Header("Skills Cards")]
    [SerializeField] private List<Image> _skillsSprites;
    [SerializeField] private List<RectTransform> _skillsTransforms;

    private RectTransform _skillCard;

    // Funcoes da Selecao de Skills (cartas) --------------------------------
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
        // Desativa todos os cursores
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

    public void WinScreen()
    {
        _winScreen.SetActive(true);
    }
    public void LoseScreen()
    {
        _loseScreen.SetActive(true);
    }
}
