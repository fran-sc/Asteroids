using UnityEngine;

/*
ExplosionController
Responsabilidad:

- Reproducir el efecto de sonido de explosión y autodestruir el objeto de la explosión tras un breve retardo.

Estructuras de datos y referencias:

- DELAY: tiempo que permanece la instancia de la explosión antes de destruirse.
- clip (SerializeField): audio clip a reproducir cuando se crea la explosión.

Notas de diseño:

- El sonido se reproduce en la posición de la cámara principal para asegurar audibilidad en 2D.
*/
public class ExplosionController : MonoBehaviour
{
    const float DELAY = 0.25f;

    // Clip de audio asociado a la explosión.
    [SerializeField] AudioClip clip;

    void Start()
    {
        // Reproducir el sonido de explosión en la posición de la cámara principal.
        // Nota: Se usa Camera.main.transform.position en lugar de transform.position
        // para asegurar que el sonido se escuche correctamente en un juego 2D,
        // donde la cámara está en una posición Z diferente a los objetos del juego.
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);

        // Programar autodestrucción del GameObject tras un breve retardo.
        // Esto permite que la animación visual se reproduzca completamente
        // antes de eliminar el objeto de la escena.
        Destroy(gameObject, DELAY);
    }
}
