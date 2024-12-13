using UnityEngine;
using UnityEngine.UI;

namespace SolidJump
{
    [RequireComponent(typeof(RawImage))]
    public class RepeatingBackground : MonoBehaviour
    {
        [SerializeField] private float _scrollSpeed = 0.5f;
        private RawImage _rawImage;

        private void Start()
        {
            _rawImage = GetComponent<RawImage>();
        }

        private void Update()
        {
            Vector2 offset = _rawImage.uvRect.position;
            offset.x += _scrollSpeed * Time.deltaTime;
            _rawImage.uvRect = new Rect(offset, _rawImage.uvRect.size);
        }
    }
}