using System.Collections.Generic;
using Residents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class BackgroundManagerInMainMenu : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private List<BackgroundObjectMover> residentsBackgroundPrefabs;

        #endregion

        #region Private Variables

        private float _timer;
        private float _timeBetweenSpawns;
        private Camera _mainCamera;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _mainCamera = Camera.main;
            _timer = 0;
            _timeBetweenSpawns = Random.Range(1, 4);
        }

        private void Update()
        {
            _timer += Time.unscaledDeltaTime;
            if (!(_timer >= _timeBetweenSpawns)) return;
            _timer = 0;
            _timeBetweenSpawns = Random.Range(0.3f, 0.5f);
            SpawnNewPrefab();
        }

        #endregion

        #region Private Methods

        private void SpawnNewPrefab()
        {
            // Choose a random side (0 = top, 1 = right, 2 = bottom, 3 = left)
            var side = Random.Range(0, 4);

            // Start position just outside the camera view
            var spawnPosition = Vector2.zero;
            var baseDirection = Vector2.zero;
        
            switch (side)
            {
                case 0: // Top
                    spawnPosition = new Vector2(Random.Range(0f, 1f), 1.1f);
                    baseDirection = Vector2.down; // Base direction toward the screen
                    break;
                case 1: // Right
                    spawnPosition = new Vector2(1.1f, Random.Range(0f, 1f));
                    baseDirection = Vector2.left; // Base direction toward the screen
                    break;
                case 2: // Bottom
                    spawnPosition = new Vector2(Random.Range(0f, 1f), -0.1f);
                    baseDirection = Vector2.up; // Base direction toward the screen
                    break;
                case 3: // Left
                    spawnPosition = new Vector2(-0.1f, Random.Range(0f, 1f));
                    baseDirection = Vector2.right; // Base direction toward the screen
                    break;
            }

            // Convert viewport position to world position
            var worldSpawnPosition = _mainCamera.ViewportToWorldPoint(new Vector3(spawnPosition.x, spawnPosition.y, Random.Range(12,19)));
        
            // Generate a direction that's guaranteed to move toward the screen
            // by combining the base inward direction with some randomness
            var randomOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            var movementDirection = (baseDirection + randomOffset).normalized;
        
            // Create random rotation
            var rotX = Random.Range(0f, 360f);
            var rotY = Random.Range(140f, 220f);
            var rotZ = Random.Range(0f, 360f);
            var randomRotation = Quaternion.Euler(rotX, rotY, rotZ);

            // Spawn object
            var randomMover = residentsBackgroundPrefabs[Random.Range(0, residentsBackgroundPrefabs.Count)];
            var gb = Instantiate(randomMover.gameObject, transform);
            gb.transform.SetLocalPositionAndRotation(worldSpawnPosition, randomRotation);

            // Add movement component to the spawned object
            gb.GetComponent<BackgroundObjectMover>().Direction = movementDirection;
            gb.gameObject.SetActive(true);
        }

        #endregion
    }
}