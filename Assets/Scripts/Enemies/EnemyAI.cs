using UnityEngine;
using UnityEngine.AI;
using Game.Core;

namespace Game.Enemies
{
    /// <summary>
    /// AI state machine for enemy behavior.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyBase))]
    public class EnemyAI : MonoBehaviour
    {
        #region States
        public enum EnemyState
        {
            Idle,
            Patrol,
            Alert,
            Chase,
            Attack,
            Death
        }

        [Header("State")]
        [SerializeField] private EnemyState _currentState = EnemyState.Patrol;

        public EnemyState CurrentState => _currentState;
        #endregion

        #region Detection
        [Header("Detection")]
        [SerializeField] private float _sightRange = 20f;
        [SerializeField] private float _fieldOfView = 90f;
        [SerializeField] private float _hearingRange = 15f;
        [SerializeField] private LayerMask _detectionMask;

        private Transform _player;
        private bool _playerDetected;
        #endregion

        #region Patrol
        [Header("Patrol")]
        [SerializeField] private Transform[] _patrolPoints;
        [SerializeField] private float _patrolWaitTime = 2f;

        private int _currentPatrolIndex = 0;
        private float _patrolWaitTimer;
        #endregion

        #region Chase
        [Header("Chase")]
        [SerializeField] private float _chaseSpeed = 5f;
        [SerializeField] private float _attackDistance = 2f;
        #endregion

        #region Components
        private NavMeshAgent _agent;
        private EnemyBase _enemyBase;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _enemyBase = GetComponent<EnemyBase>();

            // Find player
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
            }
        }

        private void Start()
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0)
            {
                _currentState = EnemyState.Idle;
            }
        }

        private void Update()
        {
            if (!_enemyBase.IsAlive)
            {
                _currentState = EnemyState.Death;
                return;
            }

            UpdateState();
        }
        #endregion

        #region State Machine
        /// <summary>
        /// Update current state behavior.
        /// </summary>
        private void UpdateState()
        {
            switch (_currentState)
            {
                case EnemyState.Idle:
                    IdleState();
                    break;
                case EnemyState.Patrol:
                    PatrolState();
                    break;
                case EnemyState.Alert:
                    AlertState();
                    break;
                case EnemyState.Chase:
                    ChaseState();
                    break;
                case EnemyState.Attack:
                    AttackState();
                    break;
                case EnemyState.Death:
                    DeathState();
                    break;
            }

            // Check for player detection
            CheckPlayerDetection();
        }

        /// <summary>
        /// Idle state - stand still.
        /// </summary>
        private void IdleState()
        {
            _agent.isStopped = true;

            if (_playerDetected)
            {
                _currentState = EnemyState.Alert;
            }
        }

        /// <summary>
        /// Patrol state - move between waypoints.
        /// </summary>
        private void PatrolState()
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0)
            {
                _currentState = EnemyState.Idle;
                return;
            }

            _agent.isStopped = false;
            _agent.speed = _chaseSpeed * 0.5f;

            // Move to current patrol point
            if (_patrolPoints[_currentPatrolIndex] != null)
            {
                _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);

                // Check if reached patrol point
                if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
                {
                    _patrolWaitTimer += Time.deltaTime;

                    if (_patrolWaitTimer >= _patrolWaitTime)
                    {
                        _patrolWaitTimer = 0f;
                        _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
                    }
                }
            }

            if (_playerDetected)
            {
                _currentState = EnemyState.Alert;
            }
        }

        /// <summary>
        /// Alert state - transition to chase.
        /// </summary>
        private void AlertState()
        {
            _agent.isStopped = true;
            _currentState = EnemyState.Chase;
        }

        /// <summary>
        /// Chase state - pursue player.
        /// </summary>
        private void ChaseState()
        {
            if (_player == null)
            {
                _currentState = EnemyState.Patrol;
                return;
            }

            _agent.isStopped = false;
            _agent.speed = _chaseSpeed;
            _agent.SetDestination(_player.position);

            // Check if in attack range
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            if (distanceToPlayer <= _attackDistance)
            {
                _currentState = EnemyState.Attack;
            }

            // Lose player if out of range
            if (!_playerDetected && distanceToPlayer > _sightRange * 1.5f)
            {
                _currentState = EnemyState.Patrol;
            }
        }

        /// <summary>
        /// Attack state - attack player.
        /// </summary>
        private void AttackState()
        {
            if (_player == null)
            {
                _currentState = EnemyState.Patrol;
                return;
            }

            _agent.isStopped = true;

            // Face player
            Vector3 direction = (_player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // Attack
            _enemyBase.AttackPlayer(_player);

            // Check if player moved out of range
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            if (distanceToPlayer > _attackDistance)
            {
                _currentState = EnemyState.Chase;
            }
        }

        /// <summary>
        /// Death state - disabled.
        /// </summary>
        private void DeathState()
        {
            _agent.isStopped = true;
            enabled = false;
        }
        #endregion

        #region Detection
        /// <summary>
        /// Check if player is detected.
        /// </summary>
        private void CheckPlayerDetection()
        {
            if (_player == null)
            {
                _playerDetected = false;
                return;
            }

            Vector3 directionToPlayer = (_player.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

            // Check sight range and FOV
            if (distanceToPlayer <= _sightRange)
            {
                float angle = Vector3.Angle(transform.forward, directionToPlayer);

                if (angle <= _fieldOfView / 2f)
                {
                    // Raycast to check line of sight
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, _sightRange, _detectionMask))
                    {
                        if (hit.transform == _player || hit.transform.IsChildOf(_player))
                        {
                            _playerDetected = true;
                            return;
                        }
                    }
                }
            }

            _playerDetected = false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Called when enemy takes damage.
        /// </summary>
        /// <param name="hitPoint">Hit position</param>
        /// <param name="hitDirection">Hit direction</param>
        public void OnDamageTaken(Vector3 hitPoint, Vector3 hitDirection)
        {
            // Alert nearby enemies
            Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, _hearingRange);
            foreach (var col in nearbyEnemies)
            {
                EnemyAI otherAI = col.GetComponent<EnemyAI>();
                if (otherAI != null && otherAI != this)
                {
                    otherAI.AlertToPosition(hitPoint);
                }
            }

            // Immediately chase player
            if (_currentState != EnemyState.Attack && _currentState != EnemyState.Chase)
            {
                _currentState = EnemyState.Alert;
            }
        }

        /// <summary>
        /// Alert enemy to a position.
        /// </summary>
        /// <param name="position">Alert position</param>
        public void AlertToPosition(Vector3 position)
        {
            if (_currentState == EnemyState.Idle || _currentState == EnemyState.Patrol)
            {
                _currentState = EnemyState.Alert;
            }
        }
        #endregion
    }
}
