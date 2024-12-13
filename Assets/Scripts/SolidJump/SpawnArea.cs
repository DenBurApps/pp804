using UnityEngine;
using Random = UnityEngine.Random;

namespace SolidJump
{
    public class SpawnArea : MonoBehaviour
    {
        [SerializeField] private float _minY;
        [SerializeField] private float _maxY;

        private float _xPosition;
        
        private void Awake()
        {
            _xPosition = transform.position.x;
        }

        public Vector2 GetPositionToSpawn()
        {
            return new Vector2(_xPosition, Random.Range(_minY, _maxY));
        }
    }
}