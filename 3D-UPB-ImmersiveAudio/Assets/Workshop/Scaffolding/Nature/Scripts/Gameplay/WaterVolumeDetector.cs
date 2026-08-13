using System;
using NaughtyAttributes;
using UnityEngine;

namespace Workshop.Scaffolding.Nature.Scripts.Gameplay
{
    public class WaterVolumeDetector : MonoBehaviour
    {
        [SerializeField]
        [BoxGroup("Internal components")]
        private Collider waterVolumeCollider;

        [SerializeField]
        [BoxGroup("External components")]
        private Camera playerCamera;

        [SerializeField]
        [BoxGroup("External components")]
        private CharacterController playerController;

        public event Action<bool> OnUnderwaterStateChanged;
        public event Action<bool> OnWaterContactStateChanged;

        private bool isCamUnderwater;
        private bool isPlayerTouchingWater;

        private void Start()
        {
            isCamUnderwater = false;
            isPlayerTouchingWater = false;

            OnUnderwaterStateChanged?.Invoke(false);
            OnWaterContactStateChanged?.Invoke(false);
        }

        private void Update()
        {
            var waterBounds = waterVolumeCollider.bounds;

            // Camera sub apa
            bool cameraUnderwater =
                waterBounds.Contains(playerCamera.transform.position);

            if (cameraUnderwater != isCamUnderwater)
            {
                isCamUnderwater = cameraUnderwater;

                OnUnderwaterStateChanged?.Invoke(
                    isCamUnderwater);
            }

            // Playerul atinge/intersecteaza volumul apei
            bool touchingWater =
                playerController != null &&
                waterBounds.Intersects(playerController.bounds);

            if (touchingWater != isPlayerTouchingWater)
            {
                isPlayerTouchingWater = touchingWater;

                OnWaterContactStateChanged?.Invoke(
                    isPlayerTouchingWater);
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            playerController ??=
                FindAnyObjectByType<CharacterController>();
        }
    }
}