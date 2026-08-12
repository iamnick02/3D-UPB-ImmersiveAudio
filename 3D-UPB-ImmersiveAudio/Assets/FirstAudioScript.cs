using UnityEngine;
using UnityEngine.InputSystem;

public class FirstAudioScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public InputActionReference triggerAction;

    private void OnEnable()
    {
        triggerAction.action.Enable();
        triggerAction.action.performed += OnAudioTriggered;
    }

    private void OnDisable()
    {
        triggerAction.action.performed -= OnAudioTriggered;
        triggerAction.action.Disable();
    }

    private void OnAudioTriggered(InputAction.CallbackContext context)
    {
        audioSource.PlayOneShot(audioClip);
    }
}