using UnityEngine;

/*
ShootController
Responsabilidad:

- Controlar el movimiento ascendente del proyectil y su destrucción automática tras un tiempo límite.
- Destruir el proyectil al colisionar con un objetivo e instanciar efecto de impacto.

Estructuras de datos y referencias:

- speed (SerializeField): velocidad de desplazamiento vertical del proyectil.
- temp (SerializeField): tiempo de vida del proyectil antes de autodestruirse.
- hitExplosion (SerializeField): prefab del efecto de impacto al colisionar.

Notas de diseño:

- Usa un temporizador decremental (temp) en Update para autodestrucción.
- OnTriggerEnter2D se usa para detectar colisiones con objetivos y destruir el proyectil.
*/
public class ShootController : MonoBehaviour
{
    [Header("Settings")]
    // Velocidad de movimiento vertical del disparo.
    [SerializeField] float speed;
    // Tiempo de vida del proyectil antes de autodestruirse.
    [SerializeField] float temp;

    [Header("References")]
    // Prefab del efecto visual al impactar.
    [SerializeField] GameObject hitExplosion;

    void Update()
    {
        // Decrementar temporizador de vida del proyectil.
        temp -= Time.deltaTime;
        
        // Si el tiempo se agota, destruir el proyectil.
        // Esto evita acumulación de proyectiles que salieron de la pantalla.
        if (temp < 0) 
            Destroy(gameObject);
        
        // Desplazar proyectil verticalmente hacia arriba a velocidad constante.
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    /*
    OnTriggerEnter2D
    Detecta impacto del proyectil con objetivos (enemigos, asteroides).
    Al impactar, destruye el proyectil e instancia efecto visual de impacto.
    */
    void OnTriggerEnter2D(Collider2D other)
    {
        // Destruir proyectil con efecto visual.
        DestroyHit();
    }

    /*
    DestroyHit
    Secuencia de destrucción del proyectil:
    1. Instancia efecto visual de impacto en la posición actual.
    2. Destruye el GameObject del proyectil.
    
    El efecto visual se maneja por su propio controlador (HitController)
    que lo autodestruye tras un breve retardo.
    */
    void DestroyHit()
    {
        // Crear efecto visual de impacto en la posición del proyectil.
        Instantiate(hitExplosion, transform.position, Quaternion.identity);
        
        // Destruir el proyectil.
        Destroy(gameObject);
    }

}
