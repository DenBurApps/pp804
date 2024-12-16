using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject _settingsCanvas;
    [SerializeField] private GameObject _privacyCanvas;
    [SerializeField] private GameObject _termsCanvas;
    [SerializeField] private AudioMixerGroup _audioMixer;

    [SerializeField] private Button _soundButton;
    [SerializeField] private Button _musicButton;

    [SerializeField] private Image _musicButtonImage;
    [SerializeField] private Image _soundButtonImage;

    [SerializeField] private Sprite _toggleOffSprite;
    [SerializeField] private Sprite _toggleOnSprite;

    private bool _musicOn = true;
    private bool _soundOn = true;
    
    private const float MusicOnLevel = -20f;
    private const float MusicOffLevel = -80f;
    private const float SoundOnLevel = -20f;
    private const float SoundOffLevel = -80f;

    private void Awake()
    {
        ResetUI();
    }

    private void OnEnable()
    {
        _soundButton.onClick.AddListener(ProcessSoundToggled);
        _musicButton.onClick.AddListener(ProcessMusicToggled);
    }

    private void OnDisable()
    {
        _soundButton.onClick.RemoveListener(ProcessSoundToggled);
        _musicButton.onClick.RemoveListener(ProcessMusicToggled);
    }

    private void Start()
    {
        // Load user preferences
        _musicOn = PlayerPrefs.GetInt("MusicOff", 0) == 0;
        _soundOn = PlayerPrefs.GetInt("SoundOff", 0) == 0;

        // Apply preferences
        UpdateMusicState();
        UpdateSoundState();
    }

    private void ResetUI()
    {
        _settingsCanvas.SetActive(false);
        _privacyCanvas.SetActive(false);
        _termsCanvas.SetActive(false);
    }

    private void UpdateMusicState()
    {
        if (_musicOn)
        {
            _musicButtonImage.sprite = _toggleOnSprite;
            _audioMixer.audioMixer.SetFloat("Music", MusicOnLevel);
        }
        else
        {
            _musicButtonImage.sprite = _toggleOffSprite;
            _audioMixer.audioMixer.SetFloat("Music", MusicOffLevel);
        }
    }

    private void UpdateSoundState()
    {
        if (_soundOn)
        {
            _soundButtonImage.sprite = _toggleOnSprite;
            _audioMixer.audioMixer.SetFloat("Effects", SoundOnLevel);
        }
        else
        {
            _soundButtonImage.sprite = _toggleOffSprite;
            _audioMixer.audioMixer.SetFloat("Effects", SoundOffLevel);
        }
    }

    private void ProcessSoundToggled()
    {
        _soundOn = !_soundOn;
        PlayerPrefs.SetInt("SoundOff", _soundOn ? 0 : 1);
        PlayerPrefs.Save();
        UpdateSoundState();
    }

    private void ProcessMusicToggled()
    {
        _musicOn = !_musicOn;
        PlayerPrefs.SetInt("MusicOff", _musicOn ? 0 : 1);
        PlayerPrefs.Save();
        UpdateMusicState();
    }

    public void ShowSettings()
    {
        _settingsCanvas.SetActive(true);
    }

    public void RateUs()
    {
#if UNITY_IOS
        Device.RequestStoreReview();
#endif
    }
}
