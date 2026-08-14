using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

#if FMOD_INSTALLED
using FMODUnity;
using Workshop.Scaffolding.Nature.Scripts.Audio;
#endif

namespace Workshop.Scaffolding.Nature.Scripts.Staff
{
    public class StaffController : MonoBehaviour
    {
        [SerializeField]
        [BoxGroup("Staff components")]
        private Light staffLight;

        [SerializeField]
        [BoxGroup("Staff components")]
        private Transform staffCrystal;

        [SerializeField]
        [BoxGroup("Settings")]
        private float lightDecayDuration = 0.3f;

        [SerializeField]
        [BoxGroup("Settings")]
        private float maxScaleAnimation = 1f;

        [SerializeField]
        [BoxGroup("Settings")]
        private float scalePunchDuration = 0.4f;

#if FMOD_INSTALLED
        [SerializeField]
        [BoxGroup("FMOD")]
        private EventReference beatEvent;
#endif

        private Vector3 _initialScale;
        private Tween _lightTween;
        private Tween _scaleTween;


        private void Awake()
        {
            _initialScale = staffCrystal.localScale;
            staffLight.intensity = 0f;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

#if FMOD_INSTALLED
            TimelineBeatService.OnBeat += HandleBeat;
            TimelineBeatService.OnMarker += HandleMarker;

            TimelineBeatService.Start(
                beatEvent,
                transform.position);
#endif
        }


        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

#if FMOD_INSTALLED
            TimelineBeatService.OnBeat -= HandleBeat;
            TimelineBeatService.OnMarker -= HandleMarker;

            TimelineBeatService.Stop();
#endif

            // Don't leave a pulse mid-flight once the player walks away.
            _lightTween?.Kill();
            _scaleTween?.Kill();
            staffLight.intensity = 0f;
            staffCrystal.localScale = _initialScale;
        }


        private void HandleBeat(int bar, int beat)
        {
            if (bar == 0 && beat == 0) return;

            var maxIntensity = beat == 1 ? 15f : 5f;

            _lightTween?.Kill();
            staffLight.intensity = maxIntensity;

            _lightTween =
                staffLight
                    .DOIntensity(0f, lightDecayDuration)
                    .SetEase(Ease.OutQuad);

            _scaleTween?.Kill();
            staffCrystal.localScale = _initialScale;

            _scaleTween =
                staffCrystal.DOPunchScale(
                    Vector3.one * maxScaleAnimation,
                    scalePunchDuration);
        }


#if FMOD_INSTALLED
        private void HandleMarker(string markerName)
        {
            switch (markerName)
            {
                case "Green":
                    staffLight.color = Color.green;
                    break;

                case "Blue":
                    staffLight.color = Color.blue;
                    break;

                case "Red":
                    staffLight.color = Color.red;
                    break;
            }
        }
#endif
    }
}