using System.Collections.Generic;
using BarelDestroyer;
using UnityEngine;

namespace QuickHands
{
    public class ObjectSpawner : ObjectPool<InteractableObject>
    {
        [SerializeField] private InteractableObject _prefab;
        [SerializeField] private SpawnArea _spawnArea;
        [SerializeField] private int _poolCapacity;
        [SerializeField] private SpriteHolder _spriteHolder;

        private List<InteractableObject> _clickableObjects = new List<InteractableObject>();

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
            
            if (TryGetObject(out InteractableObject @object, _prefab))
            {
                @object.transform.position = _spawnArea.GetPositionToSpawn();
                @object.SetItem(_spriteHolder.GetRandomItemType());
                @object.ReadyToDisable += ReturnToPool;
                @object.StartCoroutine();
                _clickableObjects.Add(@object);
            }
        }

        public void ReturnToPool(InteractableObject @object)
        {
            if (@object == null)
                return;

            @object.ReadyToDisable -= ReturnToPool;
            PutObject(@object);

            if (_clickableObjects.Contains(@object))
                _clickableObjects.Remove(@object);
        }

        public void ReturnAllObjectsToPool()
        {
            if (_clickableObjects.Count <= 0)
                return;

            List<InteractableObject> objectsToReturn = new List<InteractableObject>(_clickableObjects);
            foreach (var @object in objectsToReturn)
            {
                ReturnToPool(@object);
            }
        }
    }
}