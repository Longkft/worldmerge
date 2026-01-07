using UnityEngine;
using System.Collections.Generic;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Config")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    // Kéo thả list này ở Inspector
    [SerializeField] private List<SoundItem> soundLibrary;

    // Dictionary để tra cứu nhanh (Tối ưu hiệu năng)
    private Dictionary<SoundType, SoundItem> _soundDict;
    private GameSettings _settings;

    protected override void Awake()
    {
        base.Awake();

        // Chuyển List sang Dictionary để tra cứu cho lẹ
        _soundDict = new Dictionary<SoundType, SoundItem>();
        foreach (var item in soundLibrary)
        {
            if (item.type != SoundType.None && item.clip != null)
            {
                if (!_soundDict.ContainsKey(item.type))
                    _soundDict.Add(item.type, item); 
            }
        }
    }

    private async void Start()
    {
        var data = await DataManager.Instance.GetDataAsync();
        _settings = data.settings;
        ApplySettings();
    }

    // --- PHÁT SFX THEO ENUM ---
    public void PlaySFX(SoundType type)
    {
        if (_settings != null && !_settings.isSfxOn) return;
        if (type == SoundType.None) return;

        if (_soundDict.TryGetValue(type, out SoundItem item))
        {
            // PlayOneShot bắn thẳng clip với volume riêng của nó * volume tổng
            float finalVol = (_settings != null ? _settings.sfxVolume : 1f) * item.volume;
            sfxSource.PlayOneShot(item.clip, finalVol);
        }
        else
        {
            Debug.LogWarning($"Chưa gắn Audio Clip cho loại: {type}");
        }
    }

    // --- PHÁT NHẠC THEO ENUM ---
    public void PlayMusic(SoundType type)
    {
        if (_soundDict.TryGetValue(type, out SoundItem item))
        {
            // Nếu đang phát đúng bài này rồi thì thôi
            if (musicSource.clip == item.clip && musicSource.isPlaying) return;

            musicSource.clip = item.clip;
            musicSource.volume = (_settings != null ? _settings.musicVolume : 1f) * item.volume;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void ApplySettings()
    {
        if (_settings == null) return;
        musicSource.mute = !_settings.isMusicOn;
        sfxSource.mute = !_settings.isSfxOn;
        // Cập nhật lại volume cho nhạc đang phát
        musicSource.volume = _settings.musicVolume;
    }

    public void SetMusicState(bool isOn)
    {
        if (musicSource) musicSource.mute = !isOn;
    }

    public void SetSFXState(bool isOn)
    {
        if (sfxSource) sfxSource.mute = !isOn;
    }
}