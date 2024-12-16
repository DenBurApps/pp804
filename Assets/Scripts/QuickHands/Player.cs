using System;
using System.Collections;
using UnityEngine;

namespace QuickHands
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;

        public event Action<InteractableObject> EdibleCatched;
        public event Action<InteractableObject> UnedibleCatched;

        private Coroutine _touchDetectingCoroutine;

        public void StartDetectingTouch()
        {
            if (_touchDetectingCoroutine == null)
            {
                _touchDetectingCoroutine = StartCoroutine(DetectTouch());
            }
        }

        public void StopDetectingTouch()
        {
            if (_touchDetectingCoroutine != null)
            {
                StopCoroutine(DetectTouch());
                _touchDetectingCoroutine = null;
            }
        }

        private IEnumerator DetectTouch()
        {
            while (true)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                    RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero, Mathf.Infinity, _layerMask);

                    if (hit.collider != null)
                    {
                        if (hit.collider.TryGetComponent(out InteractableObject obj) && !obj.IsClicked)
                        {
                            obj.MarkAsClicked();
                            
                            if (obj.Item.Type == QuickHandsItems.Unedible)
                            {
                                obj.EnableMinusOneSpriteCoroutine();
                                UnedibleCatched?.Invoke(obj);
                            }
                            else if (obj.Item.Type == QuickHandsItems.Edible)
                            {
                                obj.EnablePlusOneSpriteCoroutine();
                                EdibleCatched?.Invoke(obj);
                            }
                        }
                    }
                }

                yield return null;
            }
        }
    }
}