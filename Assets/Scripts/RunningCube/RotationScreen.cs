using System;
using System.Collections;
using UnityEngine;

namespace RunningCube
{
    public class RotationScreen : MonoBehaviour
    {
        public event Action ScreenClosed;

        private void Start()
        {
            StartCoroutine(StartDisabling());
        }

        private IEnumerator StartDisabling()
        {
            yield return new WaitForSeconds(3);
            
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            ScreenClosed?.Invoke();
            gameObject.SetActive(false);
        }
    }
}