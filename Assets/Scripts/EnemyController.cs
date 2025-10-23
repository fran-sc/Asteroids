using System.Collections;
using UnityEngine;

/*
EnemyController
Responsabilidad:

- Controla el movimiento descendente de la nave enemiga, su ciclo de disparo y su destrucción.
- Notifica al GameManager para sumar puntos cuando es destruida.

Estructuras de datos y referencias:

- speed (SerializeField): velocidad de descenso constante.
- shootDelay (SerializeField): tiempo entre decisiones de disparo.
- shootProb (SerializeField): probabilidad de disparar en cada ciclo tras el delay.
- explosion (SerializeField): prefab de explosión al destruirse.
- shoot (SerializeField): prefab del proyectil enemigo.

Notas de diseño:

- El enemigo se destruye si sale por la parte inferior de la pantalla (DESTROY_HEIGHT).
- Dispara solo si se encuentra alineado horizontalmente con el jugador dentro de un margen.
- Usa corrutina infinita para temporizar disparos con probabilidad.
*/
public class EnemyController : MonoBehaviour
{
    const float DESTROY_HEIGHT = -6f;

    [Header("Settings")]
    // Velocidad de descenso de la nave enemiga.
    [SerializeField] float speed;
    // Tiempo de espera entre cada oportunidad de disparo.
    [SerializeField] float shootDelay;
    // Probabilidad de que se dispare en cada oportunidad tras el delay.
    [SerializeField] float shootProb;

    [Header("References")]
    // Prefab de explosión al ser destruido.
    [SerializeField] GameObject explosion;
    // Prefab del disparo enemigo.
    [SerializeField] GameObject shoot;

    void Start()
    {
        // Iniciar corrutina de disparo automático.
        StartCoroutine(Shoot());
    }

    void Update()
    {
        // Desplazar la nave enemiga verticalmente hacia abajo a velocidad constante.
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Destruir la nave si sale de la pantalla por la parte inferior
        // para liberar recursos de objetos fuera del área visible.
        if (transform.position.y < DESTROY_HEIGHT)
        {
            Destroy(gameObject);
        }     
    }

    /*
    OnTriggerEnter2D
    Detecta impactos de proyectiles del jugador (configurados como triggers).
    Al ser alcanzado, el enemigo se destruye inmediatamente.
    */
    void OnTriggerEnter2D(Collider2D other)
    {
        DestroyEnemy();
    }

    /*
    OnCollisionEnter2D
    Detecta colisiones físicas, típicamente con la nave del jugador.
    Al colisionar, el enemigo se destruye causando daño mutuo.
    */
    void OnCollisionEnter2D(Collision2D other)
    {
        DestroyEnemy();
    }

    /*
    DestroyEnemy
    Secuencia de destrucción de la nave enemiga:
    1. Notifica al GameManager para sumar puntos (identificado por tag "Enemy").
    2. Instancia efecto visual/sonoro de explosión.
    3. Elimina el GameObject de la escena.
    */
    void DestroyEnemy()
    {
        // Notificar al gestor de juego para actualizar puntuación.
        GameManager.GetInstance().AddScore(gameObject.tag);

        // Crear efecto de explosión en la posición actual.
        Instantiate(explosion, transform.position, Quaternion.identity);
        
        // Eliminar la nave enemiga de la escena.
        Destroy(gameObject);
    }

    /*
    Shoot
    Corrutina que gestiona el sistema de disparo automático del enemigo.
    
    Comportamiento:
    - Espera shootDelay segundos entre cada oportunidad de disparo.
    - Usa shootProb como probabilidad de disparar en cada oportunidad.
    - Solo dispara si está alineado horizontalmente con el jugador (±0.5 unidades).
    
    Esta combinación crea un patrón de disparo predecible en timing pero
    probabilístico en ejecución, y solo cuando representa amenaza real.
    */
    IEnumerator Shoot()
    {
        while (true)
        {
            // Intervalo fijo entre decisiones de disparo.
            yield return new WaitForSeconds(shootDelay);

            // Decidir si disparar basándose en probabilidad configurada.
            // Random.value devuelve un float entre 0.0 y 1.0.
            if (Random.value < shootProb)
            {
                // Buscar la nave del jugador en la escena.
                GameObject player = GameObject.FindWithTag("Player");

                // Disparar solo si:
                // 1. El jugador existe (no ha sido destruido).
                // 2. El enemigo está alineado horizontalmente con el jugador.
                //    Se usa un margen de ±0.5 unidades para definir "alineado".
                if (player != null &&
                    (transform.position.x > player.transform.position.x - 0.5f) &&
                    (transform.position.x < player.transform.position.x + 0.5f))
                {
                    // Instanciar proyectil enemigo en la posición actual.
                    Instantiate(shoot, transform.position, Quaternion.identity);
                }
            }
        }
    }
}
