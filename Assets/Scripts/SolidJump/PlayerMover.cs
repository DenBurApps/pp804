using System;
using System.Collections;
using UnityEngine;

namespace SolidJump
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _inputCooldown = 0.2f;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private PlayerSpriteProvider _playerSpriteProvider;

        private Rigidbody2D _rigidbody2D;

        private bool _canJump = true;
        private Coroutine _touchCoroutine;
        private Vector2 _defaultPosition;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _defaultPosition = transform.position;
        }

        private void OnEnable()
        {
            _playerSpriteProvider.NewSpriteSet += SetNewSprite;
        }

        private void OnDisable()
        {
            _playerSpriteProvider.NewSpriteSet -= SetNewSprite;
        }

        public void ReturnToDefaultPosition()
        {
            transform.position = _defaultPosition;
        }
        
        public void EnableInput()
        {
            _touchCoroutine = StartCoroutine(HandleInput());
        }

        public void DisableInput()
        {
            if (_touchCoroutine != null)
            {
                StopCoroutine(_touchCoroutine);
                _touchCoroutine = null;
            }
        }

        private void SetNewSprite()
        {
            _spriteRenderer.sprite = _playerSpriteProvider.CurrentDefaultSprite;
        }

        private IEnumerator HandleInput()
        {
            while (true)
            {
                if (_canJump && Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        Jump();
                        _canJump = false;
                        yield return new WaitForSeconds(_inputCooldown);
                        _canJump = true;
                    }
                }

                yield return null;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            _spriteRenderer.sprite = _playerSpriteProvider.CurrentDefaultSprite;
        }

        private void Jump()
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0f);
            _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _spriteRenderer.sprite = _playerSpriteProvider.CurrentJumpingSprite;
        }
    }
}
