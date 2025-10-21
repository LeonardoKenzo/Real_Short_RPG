using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public TurnManager TurnManager { get; private set; }

    // Singleton Pattern ----------------
    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Inicializacao dos managers --------------
    void Start()
    {
        TurnManager = new TurnManager();


    }
}
