using UnityEngine;
using UnityEngine.Audio;
using static Workshop.Scaffolding.Nature.Scripts.Audio.AudioUtils;
using Workshop.Scaffolding.Nature.Scripts.Collectible;

namespace Workshop.Scaffolding.Nature.Scripts.Audio.Manager
{
    public class UnityAudioManager : AudioManager
    {
        [SerializeField] private AudioSource footstepAudioSource;

        [SerializeField] private AudioClip[] dirtFootsteps;
        [SerializeField] private AudioClip[] stoneFootsteps;
        [SerializeField] private AudioClip[] woodFootsteps;

        [SerializeField] private AudioSource ambienceAudioSource;
        [SerializeField] private AudioSource nightAmbienceAudioSource;
        [SerializeField] private AudioSource musicAudioSource;

        [SerializeField] private AudioMixer audioMixer;

        private void OnEnable()
        {
            fpsController.OnFootstepDetected += HandleFootstep;

            audioOptionsUIController.OnAudioOptionChanged +=
                HandleAudioOptionChanged;

            dayNightCycleController.OnDayNightCycleValueChanged +=
                HandleDayNightCycleChanged;

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