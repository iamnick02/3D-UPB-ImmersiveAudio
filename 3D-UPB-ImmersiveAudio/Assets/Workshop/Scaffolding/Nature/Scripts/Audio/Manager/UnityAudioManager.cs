using UnityEngine;
using UnityEngine.Audio;
using static Workshop.Scaffolding.Nature.Scripts.Audio.AudioUtils;
using Workshop.Scaffolding.Nature.Scripts.Collectible;

namespace Workshop.Scaffolding.Nature.Scripts.Audio.Manager
{
    public class UnityAudioManager : AudioManager
    {
        [Header("Footsteps")]
        [SerializeField] private AudioSource footstepAudioSource;

        [SerializeField] private AudioClip[] dirtFootsteps;
        [SerializeField] private AudioClip[] stoneFootsteps;
        [SerializeField] private AudioClip[] woodFootsteps;

        [Header("Ambience & Music")]
        [SerializeField] private AudioSource ambienceAudioSource;
        [SerializeField] private AudioSource nightAmbienceAudioSource;
        [SerializeField] private AudioSource musicAudioSource;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Underwater Snapshots")]
        [SerializeField] private AudioMixerSnapshot normalSnapshot;
        [SerializeField] private AudioMixerSnapshot underwaterSnapshot;
        [SerializeField] private float snapshotTransitionTime = 0.5f;

        private void OnEnable()
        {
            fpsController.OnFootstepDetected += HandleFootstep;

            audioOptionsUIController.OnAudioOptionChanged +=
                HandleAudioOptionChanged;

            dayNightCycleController.OnDayNightCycleValueChanged +=
                HandleDayNightCycleChanged;

            waterVolumeDetector.OnUnderwaterStateChanged +=
                HandleUnderwaterStateChanged;

            CollectibleTracker.Instance.OnCollectibleGathered +=
                HandleCollectibleGathered;
        }

        private void OnDisable()
        {
            fpsController.OnFootstepDetected -= HandleFootstep;

            audioOptionsUIController.OnAudioOptionChanged -=
                HandleAudioOptionChanged;

            dayNightCycleController.OnDayNightCycleValueChanged -=
                HandleDayNightCycleChanged;

            waterVolumeDetector.OnUnderwaterStateChanged -=
                HandleUnderwaterStateChanged;

            CollectibleTracker.Instance.OnCollectibleGathered -=
                HandleCollectibleGathered;
        }

        private void Start()
        {
            ambienceAudioSource.volume = 1f;
            nightAmbienceAudioSource.volume = 0f;

            ambienceAudioSource.Play();
            nightAmbienceAudioSource.Play();
            musicAudioSource.Play();

            // Pornim pe sunetul normal.
            if (normalSnapshot != null)
            {
                normalSnapshot.TransitionTo(0f);
            }
        }

        private void HandleFootstep(
            AudioSurfaceType surfaceType,
            float speedPercent)
        {
            AudioClip[] clips = null;

            switch (surfaceType)
            {
                case AudioSurfaceType.Dirt:
                    clips = dirtFootsteps;
                    break;

                case AudioSurfaceType.Stone:
                    clips = stoneFootsteps;
                    break;

                case AudioSurfaceType.Wood:
                    clips = woodFootsteps;
                    break;
            }

            if (clips == null || clips.Length == 0)
                return;

            AudioClip randomClip =
                clips[Random.Range(0, clips.Length)];

            footstepAudioSource.PlayOneShot(randomClip);
        }

        private void HandleDayNightCycleChanged(float value)
        {
            ambienceAudioSource.volume =
                Mathf.Lerp(1f, 0f, value);

            nightAmbienceAudioSource.volume =
                Mathf.Lerp(0f, 1f, value);
        }

        private void HandleUnderwaterStateChanged(bool isUnderwater)
        {
            if (isUnderwater)
            {
                if (underwaterSnapshot != null)
                {
                    underwaterSnapshot.TransitionTo(
                        snapshotTransitionTime);
                }
            }
            else
            {
                if (normalSnapshot != null)
                {
                    normalSnapshot.TransitionTo(
                        snapshotTransitionTime);
                }
            }
        }

        private void HandleAudioOptionChanged(
            AudioOptionType type,
            float value)
        {
            value = Mathf.Clamp(value, 0.0001f, 1f);

            float dbValue =
                Mathf.Log10(value) * 20f;

            switch (type)
            {
                case AudioOptionType.Master:
                    audioMixer.SetFloat(
                        "MasterVolume",
                        dbValue);
                    break;

                case AudioOptionType.SFX:
                    audioMixer.SetFloat(
                        "SFXVolume",
                        dbValue);
                    break;

                case AudioOptionType.Ambience:
                    audioMixer.SetFloat(
                        "AmbienceVolume",
                        dbValue);
                    break;

                case AudioOptionType.Music:
                    audioMixer.SetFloat(
                        "MusicVolume",
                        dbValue);
                    break;
            }
        }

        private void HandleCollectibleGathered(
            CollectibleData data)
        {
            if (data.Clip == null)
                return;

            GameObject audioObject =
                new GameObject("CollectibleAudio");

            audioObject.transform.position =
                data.Position;

            AudioSource audioSource =
                audioObject.AddComponent<AudioSource>();

            audioSource.playOnAwake = false;
            audioSource.loop = false;

            // 1 = sunet complet 3D
            audioSource.spatialBlend = 1f;

            audioSource.clip = data.Clip;

            audioSource.Play();

            Destroy(
                audioObject,
                data.Clip.length);
        }
    }
}