using UnityEngine;
using System;
using Game.Core;

namespace Game.Weapons
{
    /// <summary>
    /// Manages weapon switching, ammo tracking, and weapon state.
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        #region Events
        public static event Action<WeaponBase> OnWeaponChanged;
        public static event Action<int, int> OnAmmoChanged;
        #endregion

        #region Weapons
        [Header("Weapons")]
        [SerializeField] private WeaponBase[] _weapons;
        [SerializeField] private int _currentWeaponIndex = 0;

        private WeaponBase _currentWeapon;

        public WeaponBase CurrentWeapon => _currentWeapon;
        #endregion

        #region Weapon Sway
        [Header("Weapon Sway")]
        [SerializeField] private bool _enableSway = true;
        [SerializeField] private float _swayAmount = 0.02f;
        [SerializeField] private float _swaySmooth = 6f;
        [SerializeField] private Transform _weaponHolder;

        private Vector3 _initialWeaponPosition;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            if (_weaponHolder != null)
            {
                _initialWeaponPosition = _weaponHolder.localPosition;
            }

            // Initialize weapons
            if (_weapons != null && _weapons.Length > 0)
            {
                for (int i = 0; i < _weapons.Length; i++)
                {
                    if (_weapons[i] != null)
                    {
                        _weapons[i].Unequip();
                    }
                }

                SwitchWeapon(_currentWeaponIndex);
            }
        }

        private void Update()
        {
            if (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver)
            {
                return;
            }

            HandleWeaponInput();
            HandleWeaponSway();
        }
        #endregion

        #region Weapon Input
        /// <summary>
        /// Handle weapon switching and firing input.
        /// </summary>
        private void HandleWeaponInput()
        {
            if (_currentWeapon == null) return;

            // Weapon switching (1, 2, 3 keys)
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SwitchWeapon(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwitchWeapon(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SwitchWeapon(2);
            }

            // Reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                _currentWeapon.Reload();
            }

            // Aim down sights
            bool isAiming = Input.GetMouseButton(1);
            _currentWeapon.AimDownSights(isAiming);

            // Fire
            if (_currentWeapon.WeaponData.isAutomatic)
            {
                if (Input.GetMouseButton(0))
                {
                    _currentWeapon.Fire();
                    UpdateAmmoUI();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _currentWeapon.Fire();
                    UpdateAmmoUI();
                }
            }
        }
        #endregion

        #region Weapon Switching
        /// <summary>
        /// Switch to a specific weapon.
        /// </summary>
        /// <param name="index">Weapon index</param>
        public void SwitchWeapon(int index)
        {
            if (_weapons == null || index < 0 || index >= _weapons.Length)
            {
                return;
            }

            if (_weapons[index] == null)
            {
                return;
            }

            // Unequip current weapon
            if (_currentWeapon != null)
            {
                _currentWeapon.Unequip();
            }

            // Equip new weapon
            _currentWeaponIndex = index;
            _currentWeapon = _weapons[_currentWeaponIndex];
            _currentWeapon.Equip();

            OnWeaponChanged?.Invoke(_currentWeapon);
            UpdateAmmoUI();
        }

        /// <summary>
        /// Switch to next weapon.
        /// </summary>
        public void NextWeapon()
        {
            int nextIndex = (_currentWeaponIndex + 1) % _weapons.Length;
            SwitchWeapon(nextIndex);
        }

        /// <summary>
        /// Switch to previous weapon.
        /// </summary>
        public void PreviousWeapon()
        {
            int prevIndex = _currentWeaponIndex - 1;
            if (prevIndex < 0)
            {
                prevIndex = _weapons.Length - 1;
            }
            SwitchWeapon(prevIndex);
        }
        #endregion

        #region Weapon Sway
        /// <summary>
        /// Apply weapon sway based on mouse movement.
        /// </summary>
        private void HandleWeaponSway()
        {
            if (!_enableSway || _weaponHolder == null) return;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 targetPosition = _initialWeaponPosition + new Vector3(
                -mouseX * _swayAmount,
                -mouseY * _swayAmount,
                0f
            );

            _weaponHolder.localPosition = Vector3.Lerp(
                _weaponHolder.localPosition,
                targetPosition,
                Time.deltaTime * _swaySmooth
            );
        }
        #endregion

        #region Ammo Management
        /// <summary>
        /// Update ammo UI.
        /// </summary>
        private void UpdateAmmoUI()
        {
            if (_currentWeapon != null)
            {
                OnAmmoChanged?.Invoke(_currentWeapon.CurrentAmmo, _currentWeapon.ReserveAmmo);
            }
        }

        /// <summary>
        /// Add ammo to current weapon.
        /// </summary>
        /// <param name="amount">Amount to add</param>
        public void AddAmmo(int amount)
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.AddAmmo(amount);
                UpdateAmmoUI();
            }
        }

        /// <summary>
        /// Add ammo to specific weapon.
        /// </summary>
        /// <param name="weaponIndex">Weapon index</param>
        /// <param name="amount">Amount to add</param>
        public void AddAmmoToWeapon(int weaponIndex, int amount)
        {
            if (_weapons != null && weaponIndex >= 0 && weaponIndex < _weapons.Length)
            {
                if (_weapons[weaponIndex] != null)
                {
                    _weapons[weaponIndex].AddAmmo(amount);

                    if (weaponIndex == _currentWeaponIndex)
                    {
                        UpdateAmmoUI();
                    }
                }
            }
        }
        #endregion
    }
}
