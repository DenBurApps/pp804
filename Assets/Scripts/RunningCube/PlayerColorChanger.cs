using System;
using System.Collections.Generic;
using UnityEngine;

namespace RunningCube
{
    public class PlayerColorChanger : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private List<ColorHolder> _colorHolders;
        [SerializeField] private GameStore _gameStore;

        private void OnEnable()
        {
            _gameStore.ColorTypeSelected += SetColor;
        }

        private void OnDisable()
        {
            _gameStore.ColorTypeSelected -= SetColor;
        }

        public void SetColor(ColorType type)
        {
            foreach (var colorHolder in _colorHolders)
            {
                if (type == colorHolder.ColorType)
                {
                    _spriteRenderer.color = colorHolder.Color;
                }
            }
        }
    }

    [Serializable]
    public class ColorHolder
    {
        public Color Color;
        public ColorType ColorType;
    }
}