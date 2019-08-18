using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        public int Weight;
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
        if (!other.GetComponent<Player>()) 
            return;
        
        if (_wavesIndex < Waves.Count && _instances?.Count == 0 && !_isWaitingWave)
        {
            StartCoroutine(StartNextWave());
        }
    }

    public void Spawn(Wave wave)
    {
        _instances = wave.Points
            .Select(config => Instantiate(GetPrefab(config.Preset)))
            .Select(inst => GetComponent<Enemy>())
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
            StartNextWave();
        }
    }

    private IEnumerator StartNextWave()
    {
        var wave = Waves[_wavesIndex];

        if (wave.WaitForSeconds > 0)
        {
            _isWaitingWave = true;
            yield return new WaitForSeconds(wave.WaitForSeconds);
            _isWaitingWave = false;
        }
        
        Spawn(wave);
        _wavesIndex++;
    }

    private GameObject GetPrefab(List<ChancePair> presets)
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