using Newtonsoft.Json;
using UnityEngine;

namespace Magi.Scripts.Utils
{
    public class SaveLoadUtils
    {
        public static void SaveData<T>(T data)
        {
            string key = typeof(T).Name;
            string json = JsonConvert.SerializeObject(data);
            string encrypted = CryptoUtils.Encrypt(json);
            PlayerPrefs.SetString(key, encrypted);
            PlayerPrefs.Save();
        }

        public static T LoadData<T>() where T : class
        {
            string key = typeof(T).Name;
            if (PlayerPrefs.HasKey(key))
            {
                string encrypted = PlayerPrefs.GetString(key);
                string decrypted = CryptoUtils.Decrypt(encrypted);
                return JsonConvert.DeserializeObject<T>(decrypted);
            }

            return null;
        }
    }
}