using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QuickHands
{
    public class SpriteHolder : MonoBehaviour
    {
        [SerializeField] private List<Item> _edibleItems;
        [SerializeField] private List<Item> _unedibleItems;

        private List<Item> _allItems;
        
        private void Awake()
        {
            _allItems = new List<Item>(_edibleItems.Count + _unedibleItems.Count);
            _allItems.AddRange(_edibleItems);
            _allItems.AddRange(_unedibleItems);
        }

        public Item GetRandomItemType()
        {
            if (_allItems == null || _allItems.Count == 0)
            {
                Debug.LogError("Attempting to get a random item from an empty list.");
                return null;
            }
            
            int randomIndex = Random.Range(0, _allItems.Count);
            return _allItems[randomIndex];
        }
    }

    [Serializable]
    public class Item
    {
        public QuickHandsItems Type;
        public Sprite Sprite;
    }

    public enum QuickHandsItems
    {
        Edible,
        Unedible
    }
}