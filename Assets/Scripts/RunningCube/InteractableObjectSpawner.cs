using System.Collections.Generic;
using RunningCube;
using SpaceMission;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceMission
{
    public class InteractableObjectSpawner : ObjectPool<MovingObject>
    {
        [SerializeField] private MovingObject[] _prefabs;
        [SerializeField] private SpawnArea _spawnArea;
        [SerializeField] private int _poolCapacity;
        [SerializeField] private float _objMovingSpeed;

        private List<MovingObject> _spawnedObjects = new List<MovingObject>();

        private void Awake()
        {
            for (int i = 0; i <= _poolCapacity; i++)
            {
                ShuffleArray();

                foreach (var prefab in _prefabs)
                {
                    prefab.SetSpeed(_objMovingSpeed);
                    Initalize(prefab);
                }
            }
        }

        public void Spawn()
        {
            if (ActiveObjects.Count >= _poolCapacity)
                return;

            int randomIndex = Random.Range(0, _prefabs.Length);
            MovingObject prefabToSpawn = _prefabs[randomIndex];

            if (TryGetObject(out MovingObject @object, prefabToSpawn))
            {
                if (@object is Spikes)
                {
                    @object.transform.position = _spawnArea.GetSpikePositionToSpawn();
                }
                else
                {
                    @object.transform.position = _spawnArea.GetPlatformPositionToSpawn();
                }

                _spawnedObjects.Add(@object);
                @object.EnableMovement();
                @object.SetSpeed(_objMovingSpeed);
            }
        }

        public void ReturnToPool(MovingObject @object)
        {
            if (@object == null)
                return;

            @object.DisableMovement();
            PutObject(@object);

            if (_spawnedObjects.Contains(@object))
                _spawnedObjects.Remove(@object);
        }

        public void ReturnAllObjectsToPool()
        {
            if (_spawnedObjects.Count <= 0)
                return;

            List<MovingObject> objectsToReturn = new List<MovingObject>(_spawnedObjects);
            foreach (var @object in objectsToReturn)
            {
                @object.DisableMovement();
                ReturnToPool(@object);
            }
        }

        private void ShuffleArray()
        {
            for (int i = 0; i < _prefabs.Length - 1; i++)
            {
                MovingObject temp = _prefabs[i];
                int randomIndex = Random.Range(0, _prefabs.Length);
                _prefabs[i] = _prefabs[randomIndex];
                _prefabs[randomIndex] = temp;
            }
        }
    }
}