using UnityEngine;
using UnityEngine.Events;

public class EventosGlobais : MonoBehaviour
{
    public static UnityEvent FimRodada = new UnityEvent();
    public static UnityEvent<CharacterRuntimeData> AliadoMorreu = new UnityEvent<CharacterRuntimeData>();
    public static UnityEvent<CharacterRuntimeData> InimigoMorreu = new UnityEvent<CharacterRuntimeData>();

    public static UnityEvent<bool> AtivarEscolhaAlvo = new UnityEvent<bool>();
    public static UnityEvent<GameObject> AliadoSelecionado = new UnityEvent<GameObject>();
    public static UnityEvent<GameObject> InimigoSelecionado = new UnityEvent<GameObject>();
    public static UnityEvent AcaoConcluida = new UnityEvent();
}
