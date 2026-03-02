using UnityEngine;
using System.Collections.Generic;

namespace Game.Weapons
{
    /// <summary>
    /// Manages weapon switching and inventory for weapons.
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        #region Weapon Inventory
        [Header("Weapons")]
        [SerializeField] private List<WeaponBase> _weapons = new List<WeaponBase>();
        [SerializeField] private int _currentWeaponIndex = 0;

        private WeaponBase _currentWeapon;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            InitializeWeapons();
        }

        private void Update()
        {
            HandleWeaponSwitching();
            HandleAiming();
        }
        #endregion

        #region Initialization
        private void InitializeWeapons()
        {
            // Deactivate all weapons except the first
            for (int i = 0; i < _weapons.Count; i++)
            {
                if (_weapons[i] != null)
                {
                    _weapons[i].gameObject.SetActive(i == _currentWeaponIndex);
                }
            }

            if (_weapons.Count > 0 && _weapons[_currentWeaponIndex] != null)
            {
                _currentWeapon = _weapons[_currentWeaponIndex];
            }
        }
        #endregion

        #region Weapon Switching
        private void HandleWeaponSwitching()
        {
            // Number key switching
            for (int i = 0; i < _weapons.Count && i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SwitchWeapon(i);
                }
            }

            // Mouse wheel switching
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                SwitchWeapon(_currentWeaponIndex - 1);
            }
            else if (scroll < 0f)
            {
                SwitchWeapon(_currentWeaponIndex + 1);
            }
        }

        private void SwitchWeapon(int index)
        {
            if (index < 0 || index >= _weapons.Count || index == _currentWeaponIndex)
                return;

            if (_weapons[index] == null)
                return;

            // Deactivate current weapon
            if (_currentWeapon != null)
            {
                _currentWeapon.gameObject.SetActive(false);
            }

            // Activate new weapon
            _currentWeaponIndex = index;
            _currentWeapon = _weapons[_currentWeaponIndex];
            _currentWeapon.gameObject.SetActive(true);
        }
        #endregion

        #region Aiming
        private void HandleAiming()
        {
            if (_currentWeapon == null) return;

            if (Input.GetButtonDown("Fire2"))
            {
                _currentWeapon.AimDownSights(true);
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                _currentWeapon.AimDownSights(false);
            }
        }
        #endregion

        #region Public Methods
        public WeaponBase GetCurrentWeapon()
        {
            return _currentWeapon;
        }

        public void AddWeapon(WeaponBase weapon)
        {
            if (weapon == null) return;

            _weapons.Add(weapon);
            weapon.gameObject.SetActive(false);
        }

        public void RemoveWeapon(WeaponBase weapon)
        {
            if (weapon == null) return;

            _weapons.Remove(weapon);

            if (_currentWeapon == weapon)
            {
                SwitchWeapon(0);
            }
        }
        #endregion
    }
}