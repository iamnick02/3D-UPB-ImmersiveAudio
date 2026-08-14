#if FMOD_INSTALLED

using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

using Workshop.Scaffolding.Nature.Scripts.Audio;
using Workshop.Scaffolding.Nature.Scripts.Collectible;

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
        private EventReference collectibleRemovedEvent;

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
        private EventInstance musicInstance;
        private EventInstance underwaterSnapshotInstance;

        private bool isTouchingWater;


        private void OnEnable()
        {
            fpsController.OnFootstepDetected += HandleFootstep;
            fpsController.OnJump += HandleJump;

            dayNightCycleController.OnDayNightCycleValueChanged +=
                HandleDayNightCycleChanged;

            waterVolumeDetector.OnWaterContactStateChanged +=
                HandleWaterContactStateChanged;

            waterVolumeDetector.OnUnderwaterStateChanged +=
                HandleUnderwaterStateChanged;

            CollectibleTracker.Instance.OnCollectibleGathered +=
                HandleCollectibleGathered;

            CollectibleTracker.Instance.OnCollectibleRemoved +=
                HandleCollectibleRemoved;

            audioOptionsUIController.OnAudioOptionChanged +=
                HandleAudioOptionChanged;
        }


        private void OnDisable()
        {
            fpsController.OnFootstepDetected -= HandleFootstep;
            fpsController.OnJump -= HandleJump;

            dayNightCycleController.OnDayNightCycleValueChanged -=
                HandleDayNightCycleChanged;

            waterVolumeDetector.OnWaterContactStateChanged -=
                HandleWaterContactStateChanged;

            waterVolumeDetector.OnUnderwaterStateChanged -=
                HandleUnderwaterStateChanged;

            CollectibleTracker.Instance.OnCollectibleGathered -=
                HandleCollectibleGathered;

            CollectibleTracker.Instance.OnCollectibleRemoved -=
                HandleCollectibleRemoved;

            audioOptionsUIController.OnAudioOptionChanged -=
                HandleAudioOptionChanged;
        }


        private void Start()
        {
            ambientInstance =
                RuntimeManager.CreateInstance(ambientEvent);

            ambientInstance.start();


            musicInstance =
                RuntimeManager.CreateInstance(musicEvent);

            musicInstance.setParameterByName(
                "MusicState",
                0f);

            musicInstance.start();
        }


        private void OnDestroy()
        {
            if (ambientInstance.isValid())
            {
                ambientInstance.stop(
                    FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                ambientInstance.release();
            }

            if (musicInstance.isValid())
            {
                musicInstance.stop(
                    FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                musicInstance.release();
            }

            if (underwaterSnapshotInstance.isValid())
            {
                underwaterSnapshotInstance.stop(
                    FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                underwaterSnapshotInstance.release();
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

            footstepInstance.setParameterByNameWithLabel(
                "MaterialType",
                materialLabel);

            footstepInstance.setParameterByName(
                "SpeedBlend",
                speedPercent);

            footstepInstance.start();
            footstepInstance.release();
        }


        private void HandleWaterContactStateChanged(
            bool touchingWater)
        {
            isTouchingWater = touchingWater;
        }


        private void HandleUnderwaterStateChanged(
            bool isUnderwater)
        {
            if (isUnderwater)
            {
                underwaterSnapshotInstance =
                    RuntimeManager.CreateInstance(
                        underwaterSnapshot);

                underwaterSnapshotInstance.start();
            }
            else
            {
                if (underwaterSnapshotInstance.isValid())
                {
                    underwaterSnapshotInstance.stop(
                        FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                    underwaterSnapshotInstance.release();
                }
            }
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


        private void HandleCollectibleGathered(
            CollectibleData data)
        {
            if (data.Clip != null)
            {
                EventInstance collectibleInstance =
                    RuntimeManager.CreateInstance(
                        collectiblePickupEvent);

                collectibleInstance.set3DAttributes(
                    data.Position.To3DAttributes());

                ProgrammerInstrumentService
                    .SetupAndStartWithAudioClip(
                        collectibleInstance,
                        data.Clip);
            }


            int musicState;

            if (data.Count <= 0)
            {
                musicState = 0;
            }
            else if (data.Count <= 2)
            {
                musicState = 1;
            }
            else if (data.Count <= 4)
            {
                musicState = 2;
            }
            else
            {
                musicState = 3;
            }

            musicInstance.setParameterByName(
                "MusicState",
                musicState);
        }


        private void HandleCollectibleRemoved(int count)
        {
            int musicState;

            if (count <= 0)
            {
                musicState = 0;
            }
            else if (count <= 2)
            {
                musicState = 1;
            }
            else if (count <= 4)
            {
                musicState = 2;
            }
            else
            {
                musicState = 3;
            }

            musicInstance.setParameterByName(
                "MusicState",
                musicState);


            EventInstance removedInstance =
                RuntimeManager.CreateInstance(
                    collectibleRemovedEvent);

            removedInstance.start();
            removedInstance.release();
        }


        private void HandleAudioOptionChanged(
            AudioOptionType type,
            float value)
        {
            string vcaPath;

            switch (type)
            {
                case AudioOptionType.Master:
                    vcaPath = vcaMaster;
                    break;

                case AudioOptionType.SFX:
                    vcaPath = vcaSFX;
                    break;

                case AudioOptionType.Ambience:
                    vcaPath = vcaAmbience;
                    break;

                case AudioOptionType.Music:
                    vcaPath = vcaMusic;
                    break;

                default:
                    return;
            }

            VCA vca =
                RuntimeManager.GetVCA(vcaPath);

            vca.setVolume(value);
        }
    }
}

#endif