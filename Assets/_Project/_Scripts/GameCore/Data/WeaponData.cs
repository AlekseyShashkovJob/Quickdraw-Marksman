using UnityEngine;

namespace GameCore.Data
{
    [CreateAssetMenu(fileName = "Weapon_", menuName = "Game/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Info")]
        public string id;
        public Sprite icon;
        public int price;

        [Header("Prefab")]
        public GameObject weaponPrefab;
    }
}