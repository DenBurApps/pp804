using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolidJump
{
    public class PlayerSpriteProvider : MonoBehaviour
    {
        [SerializeField] private List<SpriteHolder> _spriteHolders;
        [SerializeField] private StoreScreen _storeScreen;

        public event Action NewSpriteSet;
        
        public Sprite CurrentDefaultSprite { get; private set; }
        public Sprite CurrentJumpingSprite { get; private set; }
        
        private void OnEnable()
        {
            _storeScreen.TypeSelected += SetSprite;
        }

        private void OnDisable()
        {
            _storeScreen.TypeSelected -= SetSprite;
        }

        private void SetSprite(SpriteType type)
        {
            foreach (var spriteHolder in _spriteHolders)
            {
                if (type == spriteHolder.SpriteType)
                {
                    CurrentDefaultSprite = spriteHolder.DefaultSprite;
                    CurrentJumpingSprite = spriteHolder.JumpingSprite;
                    NewSpriteSet?.Invoke();
                    
                }
            }
        }
    }

    [Serializable]
    public class SpriteHolder
    {
        public Sprite DefaultSprite;
        public Sprite JumpingSprite;
        public SpriteType SpriteType;
    }
}