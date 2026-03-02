using UnityEngine;

namespace Game.Weapons
{
    /// <summary>
    /// Base class for all weapons in the game.
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        #region Weapon Data
        [Header("Weapon Configuration")]
        [SerializeField] protected WeaponData _weaponData;

        public WeaponData WeaponData => _weaponData;
        #endregion

        #region State
        protected int _currentAmmo;
        protected bool _isReloading;
        protected float _nextFireTime;

        public int CurrentAmmo => _currentAmmo;
        public bool IsReloading => _isReloading;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            if (_weaponData != null)
            {
                _currentAmmo = _weaponData.magazineSize;
            }
        }

        protected virtual void Update()
        {
            // Base implementation - can be overridden by subclasses
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Fire the weapon.
        /// </summary>
        public abstract void Fire();

        /// <summary>
        /// Reload the weapon.
        /// </summary>
        public abstract void Reload();
        #endregion

        #region Public Methods
        /// <summary>
        /// Check if weapon can fire.
        /// </summary>
        public virtual bool CanFire()
        {
            return !_isReloading && _currentAmmo > 0 && Time.time >= _nextFireTime;
        }

        /// <summary>
        /// Add ammo to the weapon.
        /// </summary>
        public virtual void AddAmmo(int amount)
        {
            _currentAmmo = Mathf.Min(_currentAmmo + amount, _weaponData.magazineSize);
        }
        #endregion
    }
}
