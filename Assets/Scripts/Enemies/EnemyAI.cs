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
        public enum AIState
        {
            Idle,
            Patrol,
            Alert,
            Chase,
            Attack,
            Death
        }

        [Header("State")]
        [SerializeField] private AIState _currentState = AIState.Patrol;

        public AIState CurrentState => _currentState;
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
                _currentState = AIState.Idle;
            }
        }

        private void Update()
        {
            if (!_enemyBase.IsAlive)
            {
                _currentState = AIState.Death;
                return;
            }

            UpdateState();
        }
        #endregion

        #region State Machine
        private void UpdateState()
        {
            switch (_currentState)
            {
                case AIState.Idle:
                    IdleState();
                    break;
                case AIState.Patrol:
                    PatrolState();
                    break;
                case AIState.Alert:
                    AlertState();
                    break;
                case AIState.Chase:
                    ChaseState();
                    break;
                case AIState.Attack:
                    AttackState();
                    break;
                case AIState.Death:
                    DeathState();
                    break;
            }

            CheckPlayerDetection();
        }

        private void IdleState()
        {
            _agent.isStopped = true;

            if (_playerDetected)
            {
                _currentState = AIState.Alert;
            }
        }

        private void PatrolState()
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0)
            {
                _currentState = AIState.Idle;
                return;
            }

            _agent.isStopped = false;
            _agent.speed = _chaseSpeed * 0.5f;

            if (_patrolPoints[_currentPatrolIndex] != null)
            {
                _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);

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
                _currentState = AIState.Alert;
            }
        }

        private void AlertState()
        {
            _agent.isStopped = true;
            _currentState = AIState.Chase;
        }

        private void ChaseState()
        {
            if (_player == null)
            {
                _currentState = AIState.Patrol;
                return;
            }

            _agent.isStopped = false;
            _agent.speed = _chaseSpeed;
            _agent.SetDestination(_player.position);

            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            if (distanceToPlayer <= _attackDistance)
            {
                _currentState = AIState.Attack;
            }

            if (!_playerDetected && distanceToPlayer > _sightRange * 1.5f)
            {
                _currentState = AIState.Patrol;
            }
        }

        private void AttackState()
        {
            if (_player == null)
            {
                _currentState = AIState.Patrol;
                return;
            }

            _agent.isStopped = true;

            Vector3 direction = (_player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            if (distanceToPlayer > _attackDistance)
            {
                _currentState = AIState.Chase;
            }
        }

        private void DeathState()
        {
            _agent.isStopped = true;
            enabled = false;
        }

        private void CheckPlayerDetection()
        {
            if (_player == null)
            {
                _playerDetected = false;
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

            if (distanceToPlayer <= _sightRange)
            {
                Vector3 directionToPlayer = (_player.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);

                if (angle <= _fieldOfView / 2f)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, directionToPlayer, out hit, _sightRange, _detectionMask))
                    {
                        if (hit.transform == _player)
                        {
                            _playerDetected = true;
                            return;
                        }
                    }
                }
            }

            if (distanceToPlayer <= _hearingRange)
            {
                _playerDetected = true;
                return;
            }

            _playerDetected = false;
        }

        public AIState GetCurrentState()
        {
            return _currentState;
        }
        #endregion
    }
}