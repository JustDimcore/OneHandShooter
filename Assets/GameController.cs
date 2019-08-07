using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public Player Player;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Game controller already exists");
        }
        Instance = this;
    }
}
