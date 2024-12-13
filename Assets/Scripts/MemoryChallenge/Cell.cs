using System;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryChallenge
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Image _emptyImage;

        private CellSpriteProvider _spriteProvider;
        private Image _image;
        private Button _interactButton;
        private bool _isFliped;
        private CellTypes _currentType;

        public event Action<Cell> Clicked;

        public CellTypes CurrentType => _currentType;
        public bool IsFliped => _isFliped;

        private void Awake()
        {
            _interactButton = GetComponent<Button>();
            _image = GetComponent<Image>();

            ReturnToDefault();
        }

        private void Start()
        {
            HideCellImage();
        }

        private void OnEnable()
        {
            _interactButton.onClick.AddListener(ProcessClick);
        }

        private void OnDisable()
        {
            _interactButton.onClick.RemoveListener(ProcessClick);
        }

        public void SetCellSpriteProvider(CellSpriteProvider spriteProvider)
        {
            _spriteProvider = spriteProvider;
        }

        public void SetCellType(CellTypes type)
        {
            _currentType = type;
            _image.sprite = _spriteProvider.GetExactSprite(_currentType);
        }

        public void ShowCellImage()
        {
            _emptyImage.enabled = false;
            _isFliped = true;
            _interactButton.enabled = false;
        }

        public void HideCellImage()
        {
            _emptyImage.enabled = true;
            _isFliped = false;
            _interactButton.enabled = true;
        }

        public void Disable()
        {
            _interactButton.enabled = false;
            _image.enabled = false;
            _emptyImage.enabled = false;
        }

        public void ReturnToDefault()
        {
            HideCellImage();
            _image.enabled = true;
            //_interactButton.onClick.AddListener(ProcessClick);
        }

        private void ProcessClick()
        {
            if (!IsFliped)
                Clicked?.Invoke(this);
        }
    }
}