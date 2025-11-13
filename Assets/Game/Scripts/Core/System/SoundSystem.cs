using System.Linq;
using Magi.Scripts.GameData;
using Tuns.Base;
using UnityEngine;

namespace Game.Scripts
{
    public class SoundSystem : Singleton<SoundSystem>
    {
        [SerializeField]
        private AudioSource musicAudioSource;
        
        [SerializeField]
        private AudioSource[] sfxAudioSource;
        
        public SoundData[] musicData;
        public SoundData[] sfxData;

        private LocalDataPlayer LocaData => LocalDataPlayer.Instance;
        
        private string currentMusicId;
        private bool _musicStatus;
        private bool _sfxStatus;
        private float _musicVolume;
        private float _sfxVolume;
        
        private float _initMusicVolume = 1f;
        private void Start()
        {
            LocaData.OnSoundChangeStatus += OnSoundChangeStatus;
            LocaData.OnSoundChangeVolume += OnSoundChangeVolume;
            
            _musicStatus = LocaData.PlayerData.MusicStatus;
            _sfxStatus = LocaData.PlayerData.SfxStatus;
            
            _musicVolume = LocaData.PlayerData.MusicVolume;
            _sfxVolume = LocaData.PlayerData.SfxVolume;
            
            OnSoundChangeStatus(_musicStatus, _sfxStatus);
            OnSoundChangeVolume(_musicVolume, _sfxVolume);
        }

        private void OnDestroy()
        {
            if (LocaData != null)
            {
                LocaData.OnSoundChangeStatus -= OnSoundChangeStatus;
                LocaData.OnSoundChangeVolume -= OnSoundChangeVolume;
            }
        }
        
        private void OnSoundChangeStatus(bool musicStatus, bool sfxStatus)
        {
            musicAudioSource.mute = !musicStatus;
            foreach (var sfx in sfxAudioSource)
            {
                sfx.mute = !sfx.mute;
            }
            _musicStatus = musicStatus;
            _sfxStatus = sfxStatus;
        }
        
        private void OnSoundChangeVolume(float musicVolume, float sfxVolume)
        {
            musicAudioSource.volume = _initMusicVolume * musicVolume;
            foreach (var sfx in sfxAudioSource)
            {
                sfx.volume = sfxVolume;
            }
            _musicVolume = musicVolume;
            _sfxVolume = sfxVolume;
        }
        
        public void PlayMusic(string musicId)
        {
            if (currentMusicId == musicId) return;

            var data = musicData.FirstOrDefault(x => x.soundId == musicId);
            if (data == null)
            {
                Debug.LogWarning($"Music ID '{musicId}' not found!");
                return;
            }

            _initMusicVolume = data.volume;
            currentMusicId = musicId;
            musicAudioSource.clip = data.clip;
            musicAudioSource.volume = data.volume * _musicVolume;
            musicAudioSource.loop = true;
            musicAudioSource.mute = !_musicStatus;
            musicAudioSource.Play();
        }
        
        // ===================================================
        // =============== SFX CONTROL =======================
        // ===================================================
        public void PlaySFX(string sfxId)
        {
            var data = sfxData.FirstOrDefault(x => x.soundId == sfxId);
            if (data == null)
            {
                Debug.LogWarning($"SFX ID '{sfxId}' not found!");
                return;
            }

            var freeSource = GetFreeSFXSource();
            if (freeSource == null)
            {
                Debug.LogWarning("No available SFX AudioSource in pool!");
                return;
            }

            freeSource.clip = data.clip;
            freeSource.volume = data.volume * _sfxVolume;
            freeSource.loop = false;
            freeSource.mute = !_sfxStatus;
            freeSource.Play();
        }
        
        private AudioSource GetFreeSFXSource()
        {
            foreach (var source in sfxAudioSource)
            {
                if (!source.isPlaying)
                    return source;
            }
            
            return sfxAudioSource[0];
        }

       
    }
}