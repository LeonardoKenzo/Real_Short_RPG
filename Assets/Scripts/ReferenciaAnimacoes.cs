using UnityEngine;

public class ReferenciaAnimacoes : MonoBehaviour
{
    public static Transform s_ReferenciaAnimacoes;

    void Awake()
    {
        if (s_ReferenciaAnimacoes != null)
        {
            Debug.LogError("DUPLICATA DO OBJETO DE REFERÊNCIA PRA ANIMAÇÃO!!!!");
            return;
        }

        s_ReferenciaAnimacoes = this.gameObject.transform;
    }
}
