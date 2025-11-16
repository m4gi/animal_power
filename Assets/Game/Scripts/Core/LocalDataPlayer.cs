using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.GameData;
using Magi.Scripts.GameData;
using Magi.Scripts.Utils;
using Tuns.Base;

public class LocalDataPlayer : Singleton<LocalDataPlayer>
{
    public bool isFistTime = true;
    public AnimalDatabaseLocal[] AnimalDataLocal;

    private PlayerData playerData;
    private readonly object _lock = new();
    private List<LobbyPlayerData> lobbyPlayerData = new List<LobbyPlayerData>();

    public List<LobbyPlayerData> LobbyPlayerData => lobbyPlayerData;

    public PlayerData PlayerData
    {
        get
        {
            if (playerData != null) return playerData;

            lock (_lock)
            {
                if (playerData != null) return playerData;
                playerData = SaveLoadUtils.LoadData<PlayerData>() ?? InitDefaultPlayerData();
                SaveLoadUtils.SaveData(playerData);
            }

            return playerData;
        }
    }

    public Action<int> OnCoinChanged;
    public Action<bool, bool> OnSoundChangeStatus;
    public Action<float, float> OnSoundChangeVolume;

    private PlayerData InitDefaultPlayerData()
    {
        string defaultSkin = "fox";
        var data = new PlayerData
        {
            Coin = 500000,
            MusicStatus = true,
            SfxStatus = true,
            SfxVolume = 1f,
            MusicVolume = 1f,
            Skins = new()
            {
                defaultSkin,
            },
            SelectedSkin = defaultSkin,
        };
        return data;
    }

    public void AddSkin(string skinID)
    {
        if (PlayerData.Skins.Contains(skinID)) return;
        PlayerData.Skins.Add(skinID);
        SaveData();
    }

    public bool HasSkin(string skinID)
    {
        return PlayerData.Skins.Contains(skinID);
    }

    public void SetCurrentSkin(string skinID)
    {
        PlayerData.SelectedSkin = skinID;
        SaveData();
    }

    public string GetCurrentSkin()
    {
        return PlayerData.SelectedSkin;
    }

    public void AddCoin(int amount)
    {
        PlayerData.Coin += amount;
        SaveData();
        OnCoinChanged?.Invoke(PlayerData.Coin);
    }

    public bool TrySpendCoin(int amount)
    {
        if (PlayerData.Coin < amount) return false;
        PlayerData.Coin -= amount;
        SaveData();
        OnCoinChanged?.Invoke(PlayerData.Coin);
        return true;
    }

    public bool CanSpendCoin(int amount)
    {
        return PlayerData.Coin >= amount;
    }

    public void SaveData()
    {
        SaveLoadUtils.SaveData(playerData);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerData.MusicVolume = volume;
        OnSoundChangeVolume?.Invoke(PlayerData.MusicVolume, PlayerData.SfxVolume);
    }

    public void SetSfxVolume(float volume)
    {
        PlayerData.SfxVolume = volume;
        OnSoundChangeVolume?.Invoke(PlayerData.MusicVolume, PlayerData.SfxVolume);
    }

    public void SetMusicStatus(bool status)
    {
        PlayerData.MusicStatus = status;
        SaveData();
        OnSoundChangeStatus?.Invoke(PlayerData.MusicStatus, PlayerData.SfxStatus);
    }

    public void SetSfxStatus(bool status)
    {
        PlayerData.SfxStatus = status;
        SaveData();
        OnSoundChangeStatus?.Invoke(PlayerData.MusicStatus, PlayerData.SfxStatus);
    }

    public AnimalData GetAnimalData(string animalID)
    {
        var data = AnimalDataLocal.FirstOrDefault(x => x.animalName == animalID);
        if (data != null)
        {
            return data.animalData;
        }
        return null;
    }

    public AnimalDatabaseLocal GetCurrentAnimalData()
    {
        string skinId = PlayerData.SelectedSkin;
        return AnimalDataLocal.FirstOrDefault(x => x.animalID == skinId);
    }
}