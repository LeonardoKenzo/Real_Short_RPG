using UnityEngine;

public class ReiniciadorDoJogo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DadosParty.s_ReferenciaParty != null)
        {
            Destroy(DadosParty.s_ReferenciaParty);
        }
    }
}
