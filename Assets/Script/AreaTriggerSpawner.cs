using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class AreaTriggerSpawner: MonoBehaviour, IEnemiesGenerator
{
    [Serializable]
    public class Wave
    {
        public float WaitForSeconds;
        public List<PointConfig> Points;
    }
    
    [Serializable]
    public class PointConfig
    {
        public Transform Point;
        public List<ChancePair> Preset;
    }
    
    [Serializable]
    public class ChancePair
    {
        public int Weight = 1;
        public GameObject Prefab;
    }


    public List<Wave> Waves;
    
    private int _wavesIndex;
    private List<Enemy> _instances;
    private bool _isWaitingWave;

    private event Action<IEnumerable<Enemy>> _onAdd;
    
    public void SubscribeOnAdd(Action<IEnumerable<Enemy>> enemies)
    {
        _onAdd += enemies;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // exec if player entered
        if (!other.GetComponent<Player>()) 
            return;
        
        Debug.Log("Spawn area triggered");
        var hasWaves = _wavesIndex < Waves.Count;
        var allDead = _instances == null || _instances?.Count == 0;
        if (hasWaves && allDead && !_isWaitingWave)
        {
            StartCoroutine(StartNextWave());
        }
    }

    private IEnumerator StartNextWave()
    {
        var wave = Waves[_wavesIndex];
        Debug.Log("Wait before wave " + wave.WaitForSeconds + " seconds");

        // wait cooldown
        if (wave.WaitForSeconds > 0)
        {
            _isWaitingWave = true;
            yield return new WaitForSeconds(wave.WaitForSeconds);
            _isWaitingWave = false;
        }
        
        Spawn(wave);
        _wavesIndex++;
    }

    private void Spawn(Wave wave)
    {
        Debug.Log("Spawn wave");
        _instances = wave.Points
            .Select(config => Instantiate(GetRandomPrefab(config.Preset), config.Point.position, Quaternion.identity))
            .Select(inst => inst.GetComponent<Enemy>())
            .ToList();
        
        foreach (var enemy in _instances)
        {
            enemy.SubscribeOnDeath(OnDeath);
        }
    }

    private void OnDeath(Enemy enemy)
    {
        _instances.Remove(enemy);
        if (_instances.Count == 0)
        {
            StartCoroutine(StartNextWave());
        }
    }

    [ContextMenu("Test")]
    public void TestRandomGenerator()
    {
        foreach (var wave in Waves)
        {
            foreach (var point in wave.Points)
            {
                var randomPrefab = GetRandomPrefab(point.Preset);
                Debug.LogError(randomPrefab.name);
            }
        }
    }

    private GameObject GetRandomPrefab(List<ChancePair> presets)
    {
        var sumWeight = presets.Sum(pair => pair.Weight);
        var val = Random.Range(0, sumWeight);
        var weightBuffer = 0;
        for (var i = 0; i < presets.Count; i++)
        {
            var preset = presets[i];
            weightBuffer += preset.Weight;
            if (val < weightBuffer)
            {
                return preset.Prefab;
            }
        }

        Debug.LogError("Wrong chance calculation");
        return null;
    }

    private void OnDestroy()
    {
        _onAdd = null;
    }
}