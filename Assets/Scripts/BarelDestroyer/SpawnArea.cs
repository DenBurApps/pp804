using UnityEngine;

namespace BarelDestroyer
{
    public class SpawnArea : MonoBehaviour
    {
        [SerializeField] private float _minY;
        [SerializeField] private float _maxY;
        [SerializeField] private float _minX;
        [SerializeField] private float _maxX;

        public Vector2 GetPositionToSpawn()
        {
            return new Vector2(Random.Range(_minX, _maxX), Random.Range(_minY, _maxY));
        }
    }
}
