using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Player Player;
    public List<Enemy> Enemies;

    private void Start()
    {
        foreach (var enemy in Enemies)
        {
            enemy.SubscribeOnDeath(en =>
            {
                Enemies.Remove(en);
                SetEnemies();
            });
        }
        SetEnemies();
    }

    private void SetEnemies()
    {
        Player.Enemies = Enemies;
    }
}
