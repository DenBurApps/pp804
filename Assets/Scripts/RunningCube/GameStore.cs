using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace RunningCube
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class GameStore : MonoBehaviour
    {
        [SerializeField] private TMP_Text _balanceText;
        [SerializeField] private List<StoreElement> _storeElements;
        [SerializeField] private List<StoreElementData> _skinDataList;
        [SerializeField] private PlayerBalance _playerBalance;

        private StoreElement _currentEquippedElement;
        private ScreenVisabilityHandler _screenVisabilityHandler;
        private string _skinDataKey;

        public event Action<ColorType> ColorTypeSelected;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _skinDataKey = Path.Combine(Application.persistentDataPath, "RunningCubeSkinData");
        }

        private void OnEnable()
        {
            foreach (var element in _storeElements)
            {
                element.EquipClicked += EquipSkin;
                element.BuyClicked += BuySkin;
            }
        }

        private void OnDisable()
        {
            foreach (var element in _storeElements)
            {
                element.EquipClicked -= EquipSkin;
                element.BuyClicked -= BuySkin;
            }
        }

        private void Start()
        {
            LoadSkinData();
            UpdateBalanceUI();
            Disable();
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
            UpdateBalanceUI();
        }

        public void Disable()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void BuySkin(StoreElement storeElement)
        {
            if (storeElement.StoreElementData.Price > _playerBalance.CurrentBalance)
            {
                Debug.Log("not enough balance");
                return;
            }

            _playerBalance.DecreaseBalance(storeElement.StoreElementData.Price);
            UpdateBalanceUI();

            if (_currentEquippedElement != null)
                _currentEquippedElement.Unequip();

            _currentEquippedElement = storeElement;
            storeElement.PurchaseEquip();
            ColorTypeSelected?.Invoke(_currentEquippedElement.ColorType);
            SaveSkinData();
        }

        private void EquipSkin(StoreElement storeElement)
        {
            if (_currentEquippedElement != null)
                _currentEquippedElement.Unequip();

            _currentEquippedElement = storeElement;
            _currentEquippedElement.Equip();
            ColorTypeSelected?.Invoke(_currentEquippedElement.ColorType);
            SaveSkinData();
        }

        private void SetDataToElements()
        {
            var skinDataDict = _skinDataList.ToDictionary(data => data.ColorType);

            foreach (var element in _storeElements)
            {
                if (skinDataDict.TryGetValue(element.ColorType, out var skinData))
                {
                    element.SetStoreData(skinData);

                    if (skinData.IsEquipped)
                    {
                        _currentEquippedElement = element;
                        _currentEquippedElement.Equip();
                        ColorTypeSelected?.Invoke(_currentEquippedElement.ColorType);
                    }
                }
            }

            if (_currentEquippedElement == null && _storeElements.Any())
            {
                _currentEquippedElement = _storeElements[0];
                _currentEquippedElement.Equip();
                ColorTypeSelected?.Invoke(_currentEquippedElement.ColorType);
            }
        }

        private void SaveSkinData()
        {
            var dataToSave = new List<StoreElementData>();

            foreach (var data in _storeElements)
            {
                dataToSave.Add(data.StoreElementData);
            }

            SkinDataWrapper wrapper = new SkinDataWrapper(dataToSave);
            string json = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
            File.WriteAllText(_skinDataKey, json);
            Debug.Log("saved");
        }

        private void LoadSkinData()
        {
            if (!File.Exists(_skinDataKey))
            {
                Debug.Log("no saves");
                SetDataToElements();
                return;
            }

            var json = File.ReadAllText(_skinDataKey);
            var statisticsDataWrapper = JsonConvert.DeserializeObject<SkinDataWrapper>(json);

            for (int i = 0; i < statisticsDataWrapper.StoreElementDatas.Count; i++)
            {
                _skinDataList[i] = statisticsDataWrapper.StoreElementDatas[i];
            }

            SetDataToElements();
            Debug.Log("loaded");
        }

        private void UpdateBalanceUI()
        {
            _balanceText.text = "<sprite name=\"Fra1me 8 2\">  " + _playerBalance.CurrentBalance;
        }

        [Serializable]
        private class SkinDataWrapper
        {
            public List<StoreElementData> StoreElementDatas;

            public SkinDataWrapper(List<StoreElementData> storeElementDatas)
            {
                StoreElementDatas = storeElementDatas;
            }
        }
    }
}