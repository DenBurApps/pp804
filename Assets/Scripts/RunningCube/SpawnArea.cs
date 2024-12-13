using UnityEngine;
using Random = UnityEngine.Random;

namespace RunningCube
{
    public class SpawnArea : MonoBehaviour
    {
        [SerializeField] private float _platformSpawnPosition;
        [SerializeField] private float _spikesSpawnPosition;
        private float _xPosition;

        private void Awake()
        {
            _xPosition = transform.position.x;
        }

        public Vector2 GetPlatformPositionToSpawn()
        {
            return new Vector2(_xPosition, _platformSpawnPosition);
        }
        
        public Vector2 GetSpikePositionToSpawn()
        {
            return new Vector2(_xPosition, _spikesSpawnPosition);
        }
    }
}