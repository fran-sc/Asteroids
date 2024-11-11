using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    const float DELAY = 0.25f;

    [SerializeField] AudioClip clip;

    void Start()
    {
        // reproducir el sonido
        //AudioSource.PlayClipAtPoint(clip, transform.position);
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);

        // destruir la explosión
        Destroy(gameObject, DELAY);
    }
}
