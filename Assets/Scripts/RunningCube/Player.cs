using System;
using System.Collections;
using UnityEngine;

namespace RunningCube
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _inputCooldown = 0.2f;

        private Rigidbody2D _rigidbody2D;

        private bool _canJump = true;
        private Coroutine _touchCoroutine;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void EnableInput()
        {
            _touchCoroutine = StartCoroutine(HandleInput());
            _canJump = true;
        }

        public void DisableInput()
        {
            if (_touchCoroutine != null)
            {
                StopCoroutine(_touchCoroutine);
                _touchCoroutine = null;
            }
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

        private void Jump()
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0f);
            _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }
}