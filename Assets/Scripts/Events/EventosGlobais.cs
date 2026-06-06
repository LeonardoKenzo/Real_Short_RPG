using UnityEngine;
using UnityEngine.Events;

public class EventosGlobais : MonoBehaviour
{
    public static UnityEvent<GameObject> PersonagemHoverEnter = new UnityEvent<GameObject>();
    public static UnityEvent<GameObject> PersonagemHoverExit = new UnityEvent<GameObject>();
    public static UnityEvent<GameObject> PersonagemSelecionado = new UnityEvent<GameObject>();

    public static UnityEvent<GameObject> CartaHoverEnter = new UnityEvent<GameObject>();
    public static UnityEvent<GameObject> CartaHoverExit = new UnityEvent<GameObject>();
    public static UnityEvent<GameObject> CartaSelecionada = new UnityEvent<GameObject>();

    public static UnityEvent FimRodada = new UnityEvent();
    public static UnityEvent<GameObject> PersonagemMorreu = new UnityEvent<GameObject>();
}
