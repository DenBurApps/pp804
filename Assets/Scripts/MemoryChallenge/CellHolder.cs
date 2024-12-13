using System.Collections.Generic;
using UnityEngine;

namespace MemoryChallenge
{
    public class CellHolder : MonoBehaviour
    {
        [SerializeField] private List<Cell> _cells;

        public List<Cell> Cells => _cells;

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
