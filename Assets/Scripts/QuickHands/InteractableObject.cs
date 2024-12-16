using System;
using System.Collections;
using UnityEngine;

namespace QuickHands
{
    [RequireComponent(typeof(Collider2D))]
    public class InteractableObject : MonoBehaviour
    {
        [SerializeField] private int _disableInterval = 1;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteRenderer _plusOneSprite;
        [SerializeField] private SpriteRenderer _minusOneSprite;

        private CircleCollider2D _circleCollider2D;
        private bool _isClicked = false;
        public event Action<InteractableObject> ReadyToDisable;
        
        public Item Item { get; private set; }
        
        private void Awake()
        {
            _circleCollider2D = GetComponent<CircleCollider2D>();
        }

        private void OnEnable()
        {
            _minusOneSprite.enabled = false;
            _plusOneSprite.enabled = false;
            _circleCollider2D.enabled = true;
        }

        private void OnDisable()
        {
            IsClicked = false;
            StopCoroutine(StartCountdown());
            StopCoroutine(ShowPlusOne());
        }

        public bool IsClicked
        {
            get => _isClicked;
            private set => _isClicked = value;
        }

        public void MarkAsClicked()
        {
            _isClicked = true;
        }

        public void StartCoroutine()
        {
            StartCoroutine(StartCountdown());
        }

        public void SetItem(Item item)
        {
            Item = item;
            _spriteRenderer.sprite = Item.Sprite;
        }

        public void EnablePlusOneSpriteCoroutine()
        {
            _circleCollider2D.enabled = false;

            StartCoroutine(ShowPlusOne());
        }
        
        public void EnableMinusOneSpriteCoroutine()
        {
            _circleCollider2D.enabled = false;

            StartCoroutine(ShowMinusOne());
        }
        private IEnumerator ShowPlusOne()
        {
            _plusOneSprite.enabled = true;

            yield return new WaitForSeconds(_disableInterval);

            ReadyToDisable?.Invoke(this);
        }
        
        private IEnumerator ShowMinusOne()
        {
            _minusOneSprite.enabled = true;

            yield return new WaitForSeconds(_disableInterval);

            ReadyToDisable?.Invoke(this);
        }

        private IEnumerator StartCountdown()
        {
            yield return new WaitForSeconds(_disableInterval);

            _circleCollider2D.enabled = false;

            ReadyToDisable?.Invoke(this);
        }
    }
}