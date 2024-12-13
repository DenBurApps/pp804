using System;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryChallenge
{
    public class CellSpriteProvider : MonoBehaviour
    {
        [SerializeField] private List<CellSpriteHolder> _spriteHolders;

        [SerializeField] private Sprite _emptySprite;

        public Sprite GetExactSprite(CellTypes type)
        {
            foreach (var spriteHolder in _spriteHolders)
            {
                if (spriteHolder.CellType == type)
                {
                    return spriteHolder.Sprite;
                }
            }

            return _emptySprite;
        }
    }

    [Serializable]
    public class CellSpriteHolder
    {
        public CellTypes CellType;
        public Sprite Sprite;
    }
}