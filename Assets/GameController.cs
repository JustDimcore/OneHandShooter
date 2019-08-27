using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Player Player;
    public List<Enemy> Enemies;

    private void Start()
    {
        SetEnemies();
    }

    private void SetEnemies()
    {
        Player.Enemies = Enemies;
    }
}
