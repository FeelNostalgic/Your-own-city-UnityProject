using Proyecto.Building;
using UnityEngine;
using UnityEngine.AI;

namespace Proyecto.Habitantes
{
    public class HabitanteIA : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private float speed;

        #endregion

        #region Public Variables

        [HideInInspector] public GameObject Target; //El target es la casa

        #endregion

        #region Private Variables

        private NavMeshAgent _navAgent;
        private Rigidbody _rb;
        private Vector3 _startPosition;
        private bool _targetNotFound;
        private CasaFunctionality _targetFunctionalite;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _navAgent = GetComponent<NavMeshAgent>();
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _startPosition = transform.position;
            transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
            _navAgent.speed = speed;
            _targetNotFound = false;
        }

        private void Update()
        {
            if (_navAgent.destination != null) IsTargetReached();
        }

        private void FixedUpdate()
        {
            MoveToTarget();
        }

        #endregion

        #region Public Methods

        public void BuildPath(GameObject target)
        {
            Target = target;
            _targetFunctionalite = Target.GetComponent<CasaFunctionality>();
            _navAgent.SetDestination(target.transform.position);
            _navAgent.isStopped = false;
        }

        #endregion

        #region Private Methods

        private void IsTargetReached()
        {
            if (Target == null && !_targetNotFound) ReturnToStartPoint(); //Si se destruye la casa
            if (Target != null && !_targetNotFound &&
                _targetFunctionalite.MaxHabitantes <= _targetFunctionalite.Habitantes)
                ReturnToStartPoint(); //No hay hueco para el nuevo habitante

            if (!_targetNotFound)
            {
                //Destino alcanzable
                if (_navAgent.enabled && _navAgent.remainingDistance < _navAgent.stoppingDistance)
                    _navAgent.enabled = false;
            }
            else
            {
                if (Vector3.Distance(transform.position, _startPosition) <
                    _navAgent.stoppingDistance) //StartPointAlcanzado -> destruir
                {
                    Destroy(gameObject);
                }
            }
        }

        private void MoveToTarget()
        {
            if (!_navAgent.enabled)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation((Target.transform.parent.position + (Vector3.up * transform.position.y)) -
                                            transform.position), Time.deltaTime * 180);
                Vector3 dir = transform.position + (transform.forward * speed * Time.deltaTime);
                _rb.MovePosition(dir);
            }
        }

        private void ReturnToStartPoint()
        {
            _navAgent.SetDestination(_startPosition);
            _targetNotFound = true;
            transform.tag = "Untagged";
        }

        #endregion
    }
}