using System;
using UnityEngine;

public class AnimacaoHabilidade : MonoBehaviour
{
    public event Action<GameObject> AnimacaoDestruida;

    private Animator _controladorAnimacao;

    private void Awake()
    {
        _controladorAnimacao = GetComponent<Animator>();
    }

    void Start()
    {
        var clip = _controladorAnimacao.runtimeAnimatorController.animationClips[0];

        //Destrói ao fim da animação
        Destroy(gameObject, clip.length);
    }

    private void OnDestroy()
    {
        AnimacaoDestruida?.Invoke(this.gameObject);
    }
}