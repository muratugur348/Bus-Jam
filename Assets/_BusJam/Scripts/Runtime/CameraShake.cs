using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    private Vector3 originalPosition;
    public float duration;
    public float magnitude;

    private void Awake()
    {
        MakeSingleton();
    }
    private void MakeSingleton()
    {
        if (!Instance)
            Instance = this;
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        // Save the original position of the camera
        originalPosition = transform.position;
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y , originalPosition.z + y);

            elapsed += Time.deltaTime;

            yield return null;
        }

        // Return the camera to its original position
        transform.position = originalPosition;
    }
}
