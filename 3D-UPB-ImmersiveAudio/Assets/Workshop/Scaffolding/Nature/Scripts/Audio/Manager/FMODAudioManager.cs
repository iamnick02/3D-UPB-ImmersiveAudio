#if FMOD_INSTALLED

using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

using static Workshop.Scaffolding.Nature.Scripts.Audio.AudioUtils;

namespace Workshop.Scaffolding.Nature.Scripts.Audio.Manager
{
    public class FMODAudioManager : AudioManager
    {
        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference footstepEvent;

        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference jumpEvent;

        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference ambientEvent;

        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference collectiblePickupEvent;

        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference musicEvent;

        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference underwaterSnapshot;


        [SerializeField, BoxGroup("FMOD VCAs")]
        private string vcaMaster = "vca:/VCA_Master";

        [SerializeField, BoxGroup("FMOD VCAs")]
        private string vcaSFX = "vca:/VCA_SFX";

        [SerializeField, BoxGroup("FMOD VCAs")]
        private string vcaAmbience = "vca:/VCA_Ambience";

        [SerializeField, BoxGroup("FMOD VCAs")]
        private string vcaMusic = "vca:/VCA_Music";


        private EventInstance ambientInstance;

        private bool isTouchingWater;


        private void OnEnable()
        {
            fpsController.OnFootstepDetected += HandleFootstep;
            fpsController.OnJump += HandleJump;

            dayNightCycleController.OnDayNightCycleValueChanged +=
                HandleDayNightCycleChanged;

            waterVolumeDetector.OnWaterContactStateChanged +=
                HandleWaterContactStateChanged;
        }


        private void OnDisable()
        {
            fpsController.OnFootstepDetected -= HandleFootstep;
            fpsController.OnJump -= HandleJump;

            dayNightCycleController.OnDayNightCycleValueChanged -=
                HandleDayNightCycleChanged;

            waterVolumeDetector.OnWaterContactStateChanged -=
                HandleWaterContactStateChanged;
        }


        private void Start()
        {
            ambientInstance =
                RuntimeManager.CreateInstance(ambientEvent);

            ambientInstance.start();
        }


        private void OnDestroy()
        {
            if (ambientInstance.isValid())
            {
                ambientInstance.stop(
                    FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                ambientInstance.release();
            }
        }


        private void HandleFootstep(
            AudioSurfaceType surfaceType,
            float speedPercent)
        {
            EventInstance footstepInstance =
                RuntimeManager.CreateInstance(footstepEvent);

            string materialLabel;

            if (isTouchingWater)
            {
                materialLabel = "Water";
            }
            else
            {
                materialLabel = surfaceType.ToString();
            }

            // Dirt / Stone / Wood / Water
            footstepInstance.setParameterByNameWithLabel(
                "MaterialType",
                materialLabel);

            // 0 = walk, 1 = run
            footstepInstance.setParameterByName(
                "SpeedBlend",
                Mathf.Clamp01(speedPercent));

            footstepInstance.start();
            footstepInstance.release();
        }


        private void HandleWaterContactStateChanged(
            bool touchingWater)
        {
            isTouchingWater = touchingWater;
        }


        private void HandleJump()
        {
            EventInstance jumpInstance =
                RuntimeManager.CreateInstance(jumpEvent);

            jumpInstance.start();
            jumpInstance.release();
        }


        private void HandleDayNightCycleChanged(float value)
        {
            ambientInstance.setParameterByName(
                "AmbientBlend",
                value);
        }
    }
}

#endif