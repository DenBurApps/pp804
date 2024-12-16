using System;
using System.Collections;
using UnityEngine;

namespace BarelDestroyer
{
    public class Barel : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _plusoneSprite;

        private Transform _transform;
        private Coroutine _disablingCoroutine;
        private BoxCollider2D _boxCollider2D;

        public event Action<Barel> Destroyed;
        public event Action<Barel> Diactivated; 

        private void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        private void OnEnable()
        {
            _plusoneSprite.enabled = false;
            _boxCollider2D.enabled = true;
        }

        private void OnDisable()
        {
            if (_disablingCoroutine != null)
            {
                StopCoroutine(_disablingCoroutine);
                _disablingCoroutine = null;
            }
        }

        public void StartDisabling()
        {
            _disablingCoroutine = StartCoroutine(DisablingCoroutine());
        }

        private IEnumerator HitCoroutine()
        {
            _plusoneSprite.enabled = true;
            _boxCollider2D.enabled = false;

            yield return new WaitForSeconds(0.5f);
            Destroyed?.Invoke(this);
        }

        private IEnumerator DisablingCoroutine()
        {
            if (_disablingCoroutine != null)
            {
                StopCoroutine(_disablingCoroutine);
                _disablingCoroutine = null;
            }
            
            yield return new WaitForSeconds(3);
            Diactivated?.Invoke(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Bullet bullet))
            {
                OnGotPlayer();
                bullet.Collided();
            }
        }

        private void OnGotPlayer()
        {
            StartCoroutine(HitCoroutine());
        }
    }
}