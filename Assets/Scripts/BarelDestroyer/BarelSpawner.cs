using System;
using System.Collections.Generic;
using RunningCube;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BarelDestroyer
{
    public class BarelSpawner : ObjectPool<Barel>
    {
        [SerializeField] private Barel _prefab;
        [SerializeField] private SpawnArea _spawnArea;
        [SerializeField] private int _poolCapacity;

        private List<Barel> _spawnedObjects = new List<Barel>();

        public event Action BarrelDestroyed;

        private void Awake()
        {
            for (int i = 0; i <= _poolCapacity; i++)
            {
                Initalize(_prefab);
            }
        }
        
        public void Spawn()
        {
            if (ActiveObjects.Count >= _poolCapacity)
                return;

            Barel prefabToSpawn = _prefab;

            if (TryGetObject(out Barel @object, prefabToSpawn))
            {
                @object.transform.position = _spawnArea.GetPositionToSpawn();
                @object.Destroyed += OnDestroyed;
                @object.Diactivated += PutObject;
                @object.StartDisabling();
                _spawnedObjects.Add(@object);
            }
        }
        
        public void ReturnToPool(Barel @object)
        {
            if (@object == null)
                return;
            
            @object.Destroyed -= OnDestroyed;
            @object.Diactivated -= PutObject;
            PutObject(@object);

            if (_spawnedObjects.Contains(@object))
                _spawnedObjects.Remove(@object);
        }

        public void ReturnAllObjectsToPool()
        {
            if (_spawnedObjects.Count <= 0)
                return;

            List<Barel> objectsToReturn = new List<Barel>(_spawnedObjects);
            foreach (var @object in objectsToReturn)
            {
                @object.Destroyed -= OnDestroyed;
                ReturnToPool(@object);
            }
        }
        
        private void OnDestroyed(Barel barel)
        {
            BarrelDestroyed?.Invoke();
            PutObject(barel);
        }
    }
}