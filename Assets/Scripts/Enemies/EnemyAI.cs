using UnityEngine;
using UnityEngine.AI;
using Game.Core;

namespace Game.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyBase))]
    public class EnemyAI : MonoBehaviour
    {
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

        [Header("Detection")]
        [SerializeField] private float _sightRange = 20f;
        [SerializeField] private float _fieldOfView = 90f;
        [SerializeField] private float _hearingRange = 15f;
        [SerializeField] private LayerMask _detectionMask;

        private Transform _player;
        private bool _playerDetected;

        [Header("Patrol")]
        [SerializeField] private Transform[] _patrolPoints;
        [SerializeField] private float _patrolWaitTime = 2f;

        private int _currentPatrolIndex = 0;
        private float _patrolWaitTimer;

        [Header("Chase")]
        [SerializeField] private float _chaseSpeed = 5f;
        [SerializeField] private float _attackDistance = 2f;

        private NavMeshAgent _agent;
        private EnemyBase _enemyBase;

        public EnemyState GetCurrentState() => _currentState;

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

            CheckPlayerDetection();
        }

        private void IdleState()
        {
            _agent.isStopped = true;

            if (_playerDetected)
            {
                _currentState = EnemyState.Alert;
            }
        }

        private void PatrolState()
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0)
            {
                _currentState = EnemyState.Idle;
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
                _currentState = EnemyState.Alert;
            }
        }

        private void AlertState()
        {
            _agent.isStopped = true;
            _currentState = EnemyState.Chase;
        }

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

            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            if (distanceToPlayer <= _attackDistance)
            {
                _currentState = EnemyState.Attack;
            }

            if (!_playerDetected && distanceToPlayer > _sightRange * 1.5f)
            {
                _currentState = EnemyState.Patrol;
            }
        }

        private void AttackState()
        {
            if (_player == null)
            {
                _currentState = EnemyState.Patrol;
                return;
            }

            _agent.isStopped = true;

            Vector3 direction = (_player.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            }

            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            if (distanceToPlayer > _attackDistance)
            {
                _currentState = EnemyState.Chase;
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
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                if (angleToPlayer <= _fieldOfView / 2f)
                {
                    Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, _sightRange, _detectionMask))
                    {
                        if (hit.collider.CompareTag("Player"))
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
    }
}