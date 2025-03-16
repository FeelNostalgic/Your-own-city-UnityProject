using UnityEngine;
using Random = UnityEngine.Random;

namespace Residents
{ 
	public class BackgroundObjectMover : MonoBehaviour
	{
		#region Private Variables
		
		private Vector2 _direction;
		private float _speed;
		private Camera _mainCamera;
		private float _rotationSpeedX;
		private float _rotationSpeedZ;
		private bool _canMove;
		
		#endregion

		#region Public Variables

		public Vector2 Direction
		{
			set => _direction = value;
		}

		#endregion
		
		#region Unity Methods

		private void Start()
		{
			_speed = Random.Range(2,7);
			_mainCamera = Camera.main;
			_rotationSpeedX = Random.Range(-90f, 90f);
			_rotationSpeedZ = Random.Range(-90f, 90f);
			
			_canMove = true;
		}

		private void Update()
		{
			if (!_canMove) return;
			// Move object in world space
			var movement = new Vector3(_direction.x, _direction.y, 0) * (_speed * Time.deltaTime);
			transform.position += movement;
        
			// Rotate object around X and Z axes (visual only)
			transform.Rotate(_rotationSpeedX * Time.deltaTime, 0f, _rotationSpeedZ * Time.deltaTime);
        
			// Check if object is outside the camera view with some buffer
			var viewportPosition = _mainCamera.WorldToViewportPoint(transform.position);
			if (viewportPosition.x < -1f || viewportPosition.x > 2f || 
			    viewportPosition.y < -1f || viewportPosition.y > 2f)
			{
				Destroy(gameObject);
			}
		}
		
		#endregion
		}
}