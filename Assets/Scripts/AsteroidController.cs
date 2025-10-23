using Unity.VisualScripting;
using UnityEngine;

/*
AsteroidController
-----------------
Responsabilidad:
- Gestiona el ciclo de vida de un asteroide (grande o pequeño): lanzamiento inicial, conteo de impactos, fragmentación y destrucción.
- Notifica al GameManager para sumar puntos cuando el asteroide es destruido.
- Elimina el asteroide cuando sale del área visible por la parte inferior.

Estructuras de datos y referencias:
- DESTROY_HEIGHT: límite vertical inferior a partir del cual el asteroide se destruye automáticamente.
- HITS_TO_DESTROY: número de impactos requeridos para que un asteroide grande se fragmente.
- force (SerializeField): impulso inicial aplicado a los asteroides grandes.
- explosion (SerializeField): prefab que se instancia al destruir el asteroide.
- asteroid (SerializeField): prefab usado para generar asteroides pequeños al fragmentar uno grande.
- hitCount: contador interno de impactos recibidos por un asteroide grande.
- rb: referencia al Rigidbody2D del asteroide para aplicar fuerzas/velocidades.

Notas de diseño:
- Se utiliza la propiedad tag para diferenciar entre "AsteroidBig" y "AsteroidSmall".
- OnCollisionEnter2D: si impacta con el jugador, se destruye de inmediato.
- OnTriggerEnter2D: se interpreta como impacto de disparo. Los grandes acumulan impactos hasta fragmentarse; los pequeños se destruyen directamente.
- Al fragmentar, se instancian dos asteroides pequeños con velocidades heredadas y simétricas en el eje X.
*/
public class AsteroidController : MonoBehaviour
{
    const float DESTROY_HEIGHT = -6f;
    const int HITS_TO_DESTROY = 4;

    // Impulso inicial aplicado al asteroide grande para ponerlo en movimiento.
    [SerializeField] float force;
    // Prefab de explosión visual/sonora al destruir el asteroide.
    [SerializeField] GameObject explosion;
    // Prefab del asteroide pequeño usado al fragmentar uno grande.
    [SerializeField] GameObject asteroid;
    
    // Número de impactos acumulados (solo relevante para asteroides grandes).
    int hitCount;
    // Física 2D del asteroide para aplicar fuerzas y consultar velocidades.
    Rigidbody2D rb;

    void Start()
    {
        // Obtener referencia al componente de física 2D del asteroide.
        rb = GetComponent<Rigidbody2D>();

        // Si es un asteroide grande, aplicar impulso inicial para ponerlo en movimiento.
        if (tag == "AsteroidBig")
        {
            LaunchBigAsteroid();
        }
    }

    void Update()
    {
        // Destruir el asteroide si sale por la parte inferior de la pantalla
        // para evitar acumulación de objetos fuera del área de juego.
        if (transform.position.y < DESTROY_HEIGHT)
        {
            Destroy(gameObject);
        }
    }

    /*
    OnCollisionEnter2D
    Detecta colisiones físicas (con Collider2D no configurado como trigger).
    Si el asteroide colisiona con el jugador, se destruye inmediatamente,
    causando daño a la nave del jugador.
    */
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DestroyAsteroid();
        }
    }

    /*
    OnTriggerEnter2D
    Detecta impactos de proyectiles (configurados como triggers).
    
    - Asteroides grandes: acumulan impactos hasta alcanzar HITS_TO_DESTROY,
      momento en el que se fragmentan en dos asteroides pequeños.
    - Asteroides pequeños: se destruyen con un solo impacto.
    */
    void OnTriggerEnter2D(Collider2D other)
    {
        if (tag == "AsteroidBig")
        {
            // Incrementar contador de impactos recibidos.
            hitCount++;
            
            // Si alcanza el número requerido, fragmentar el asteroide.
            if (hitCount == HITS_TO_DESTROY)
            {
                // Generar dos asteroides pequeños heredando la velocidad actual.
                LaunchSmallAsteroid();

                // Destruir el asteroide grande tras fragmentarlo.
                DestroyAsteroid();
            }
        }
        else
        {
            // Los asteroides pequeños se destruyen con un solo impacto.
            DestroyAsteroid();
        }
    }

    /*
    DestroyAsteroid
    Secuencia de destrucción del asteroide:
    1. Notifica al GameManager para sumar puntos según el tipo (tag).
    2. Instancia efecto visual/sonoro de explosión.
    3. Elimina el GameObject del asteroide de la escena.
    */
    void DestroyAsteroid()
    {
        // Notificar al gestor de juego para actualizar la puntuación.
        // El tag identifica si es AsteroidBig o AsteroidSmall.
        GameManager.GetInstance().AddScore(gameObject.tag);

        // Crear efecto de explosión en la posición actual del asteroide.
        Instantiate(explosion, transform.position, Quaternion.identity);
        
        // Eliminar el asteroide de la escena.
        Destroy(gameObject);
    }

    /*
    LaunchBigAsteroid
    Aplica el impulso inicial a un asteroide grande para ponerlo en movimiento.
    Se llama automáticamente en Start() si el tag es "AsteroidBig".
    
    La dirección combina movimiento horizontal (derecha) con descenso leve,
    creando una trayectoria diagonal característica.
    */
    void LaunchBigAsteroid()
    {
        // Construir vector de dirección: principalmente horizontal con descenso leve.
        // (1, -0.25) significa: avanza hacia la derecha descendiendo suavemente.
        Vector2 direction = new Vector2(1, -0.25f);
        direction.Normalize();

        // Aplicar impulso en la dirección calculada para movimiento lineal.
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        
        // Aplicar torque negativo para rotación antihoraria lenta.
        rb.AddTorque(-.1f, ForceMode2D.Impulse);
    }

    /*
    LaunchSmallAsteroid
    Genera dos asteroides pequeños al fragmentar uno grande.
    
    Cada fragmento hereda la velocidad lineal y angular del padre,
    pero con la componente X invertida para separarlos en direcciones opuestas.
    Esto crea el efecto visual de fragmentación en dos piezas divergentes.
    */
    void LaunchSmallAsteroid()
    {
        // Capturar la posición actual del asteroide grande antes de destruirlo.
        Vector3 position = transform.position;
        
        // Capturar la velocidad lineal y angular del asteroide padre.
        Vector2 linearVelocity = rb.linearVelocity;
        float angularVelocity = rb.angularVelocity;

        // Crear dos asteroides pequeños con velocidades simétricas en X.
        // s alterna entre +1 y -1 para invertir la componente horizontal.
        for (int i=0, s=1; i<2; i++, s*=-1)
        {
            // Instanciar asteroide pequeño en la posición del padre.
            GameObject smallAsteroid = Instantiate(asteroid, position, Quaternion.identity);
            
            // Obtener el Rigidbody2D del nuevo asteroide pequeño.
            Rigidbody2D rbSmall = smallAsteroid.GetComponent<Rigidbody2D>();

            // Asignar velocidad lineal: heredar Y, pero invertir X según s (+1 o -1).
            // Esto hace que ambos fragmentos se separen en direcciones opuestas.
            rbSmall.linearVelocity = new Vector2(s * linearVelocity.x, linearVelocity.y);
            
            // Heredar la velocidad angular para mantener rotación similar.
            rbSmall.angularVelocity = angularVelocity;
        }
    }
}

