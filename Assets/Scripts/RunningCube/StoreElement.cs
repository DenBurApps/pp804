using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RunningCube
{
    public class StoreElement : MonoBehaviour
    {
        [SerializeField] private ColorType _colorType;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _equippedButton;
        [SerializeField] private Button _equipButton;
        [SerializeField] private TMP_Text _priceText;

        public ColorType ColorType => _colorType;
        public StoreElementData StoreElementData { get; private set; }

        public event Action<StoreElement> BuyClicked;
        public event Action<StoreElement> EquipClicked;

        private void OnEnable()
        {
            _buyButton.onClick.AddListener(OnBuyButtonClicked);
            _equipButton.onClick.AddListener(OnEquipClicked);
        }

        private void OnDisable()
        {
            _buyButton.onClick.RemoveListener(OnBuyButtonClicked);
            _equipButton.onClick.RemoveListener(OnEquipClicked);
        }

        public void SetStoreData(StoreElementData data)
        {
            StoreElementData = data;
            UpdateButtonsStatus();
            _priceText.text = "<sprite name=\"Fra1me 8 2\">  " + StoreElementData.Price.ToString();
        }

        public void PurchaseEquip()
        {
            StoreElementData.IsPurchased = true;
            StoreElementData.IsEquipped = true;
            UpdateButtonsStatus();
        }

        public void Equip()
        {
            if(!StoreElementData.IsPurchased)
                return;
            
            StoreElementData.IsEquipped = true;
            UpdateButtonsStatus();
        }

        public void Unequip()
        {
            StoreElementData.IsEquipped = false;
            UpdateButtonsStatus();
        }

        private void UpdateButtonsStatus()
        {
            if (StoreElementData.IsEquipped && StoreElementData.IsPurchased)
            {
                _equippedButton.gameObject.SetActive(true);
                _equipButton.gameObject.SetActive(false);
                _buyButton.gameObject.SetActive(false);
            }
            else if (StoreElementData.IsPurchased && !StoreElementData.IsEquipped)
            {
                _equippedButton.gameObject.SetActive(false);
                _equipButton.gameObject.SetActive(true);
                _buyButton.gameObject.SetActive(false);
            }
            else if (!StoreElementData.IsPurchased)
            {
                _equippedButton.gameObject.SetActive(false);
                _equipButton.gameObject.SetActive(false);
                _buyButton.gameObject.SetActive(true);
            }
        }

        private void OnBuyButtonClicked()
        {
            BuyClicked?.Invoke(this);
        }

        private void OnEquipClicked()
        {
            EquipClicked?.Invoke(this);
        }
    }

    [Serializable]
    public class StoreElementData
    {
        public int Price;
        public bool IsEquipped;
        public bool IsPurchased;
        public ColorType ColorType;
    }

    public enum ColorType
    {
        Orange,
        Yellow,
        Blue,
        Green,
        LightBlue
    }
}