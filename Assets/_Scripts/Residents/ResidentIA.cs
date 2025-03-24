using System;
using Buildings;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Residents
{
    public class ResidentIA : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private float speed;

        #endregion

        #region Public Variables

        //

        #endregion

        #region Private Variables

        private NavMeshAgent _navAgent;
        private Rigidbody _rb;
        private Vector3 _startPosition;
        private bool _targetUnreachable;
        private HouseFunctionality _houseTarget;

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
        }

        private void Update()
        {
            if (_navAgent.hasPath) IsTargetReached();
        }

        private void FixedUpdate()
        {
            MoveToTarget();
        }

        private void OnDestroy()
        {
            if (_houseTarget.IsNull()) return;
            _houseTarget.OnDemolish -= OnHouseDemolish;
        }

        #endregion

        #region Public Methods

        public void BuildPath(HouseFunctionality target)
        {
            _houseTarget = target;
            _houseTarget.OnDemolish += OnHouseDemolish;
            _houseTarget.OnResidentUpdate += OnHouseResidentUpdate;
            _navAgent.SetDestination(target.transform.position);
        }

        #endregion

        #region Private Methods

        private void OnHouseDemolish()
        {
            // If house is unreachable return to start point
            ReturnToStartPoint();
        }

        private void OnHouseResidentUpdate()
        {
            // If there is no space for the new resident, then return to start point
            ReturnToStartPoint();
        }

        private void IsTargetReached()
        {
            if (!_targetUnreachable)
            {
                // Destination reached
                if (!_navAgent.enabled || !(_navAgent.remainingDistance < _navAgent.stoppingDistance)) return;
                _houseTarget.OnResidentUpdate -= OnHouseResidentUpdate;
                _navAgent.enabled = false;
            }
            else // No space or house demolished
            {
                if (!(Vector3.Distance(transform.position, _startPosition) < _navAgent.stoppingDistance)) return; //Start point reached -> Destroy Resident
                // TODO: vanish before destroy
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Move last meters to the target
        /// </summary>
        private void MoveToTarget()
        {
            if (_navAgent.enabled) return;
            if (_houseTarget.IsNull()) return;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_houseTarget.transform.parent.position + Vector3.up * transform.position.y - transform.position),
                Time.deltaTime * 180);
            var dir = transform.position + (transform.forward * (speed * Time.deltaTime));
            _rb.MovePosition(dir);
        }

        private void ReturnToStartPoint()
        {
            _navAgent.enabled = true;
            _houseTarget.OnDemolish -= OnHouseDemolish;
            _houseTarget.OnResidentUpdate -= OnHouseResidentUpdate;
            _houseTarget = null;
            _navAgent.SetDestination(_startPosition);
            _targetUnreachable = true;
            transform.tag = "Untagged";
        }

        #endregion
    }
}