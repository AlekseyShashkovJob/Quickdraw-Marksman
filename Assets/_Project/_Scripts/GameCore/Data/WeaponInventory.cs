using UnityEngine;

namespace GameCore.Data
{
    public static class WeaponInventory
    {
        private const string OWNED_PREFIX = "weapon_owned_";
        private const string SELECTED_KEY = "selected_weapon";

        public static bool IsOwned(string id)
        {
            return PlayerPrefs.GetInt(OWNED_PREFIX + id, 0) == 1;
        }

        public static void Purchase(string id)
        {
            PlayerPrefs.SetInt(OWNED_PREFIX + id, 1);
            PlayerPrefs.Save();
        }

        public static string GetSelectedId()
        {
            return PlayerPrefs.GetString(SELECTED_KEY, "");
        }

        public static void SetSelected(string id)
        {
            PlayerPrefs.SetString(SELECTED_KEY, id);
            PlayerPrefs.Save();
        }
    }
}