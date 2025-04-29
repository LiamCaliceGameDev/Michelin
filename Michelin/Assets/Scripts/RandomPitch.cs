using UnityEngine;

public class RandomPitch : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the AudioSource
    public float minPitch = 0.8f;  // Minimum pitch value
    public float maxPitch = 1.2f;  // Maximum pitch value

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();  // Get the AudioSource if not assigned in the Inspector
        }

        // Set a random pitch at the start
        audioSource.pitch = Random.Range(minPitch, maxPitch);
    }
}
