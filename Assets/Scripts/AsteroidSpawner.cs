using System.Collections;
using UnityEngine;

/*
AsteroidSpawner
Responsabilidad:

- Genera asteroides a intervalos aleatorios dentro de un rango de tiempo configurable.
- Permite establecer un retardo inicial antes de comenzar el ciclo de aparición.

Estructuras de datos y referencias:

- asteroid (SerializeField): prefab del asteroide a instanciar.
- minInterval / maxInterval (SerializeField): rango de espera aleatoria entre spawns consecutivos.
- delay (SerializeField): tiempo de espera antes de iniciar el bucle de spawns.
- initialPosition: posición desde la que se instancian los asteroides (cacheada desde el transform).

Notas de diseño:

- Usa una corrutina infinita para el spawn, con pausas controladas por WaitForSeconds.
- La posición es fija (la del propio spawner), lo que simplifica el patrón de aparición vertical.
*/
public class AsteroidSpawner : MonoBehaviour
{
    // Prefab del asteroide a crear periódicamente.
    [SerializeField] GameObject asteroid;
    // Intervalo mínimo entre apariciones.
    [SerializeField] float minInterval;
    // Intervalo máximo entre apariciones.
    [SerializeField] float maxInterval;
    // Retardo inicial antes de iniciar el bucle de spawns.
    [SerializeField] float delay;

    Vector3 initialPosition;
    
    void Start()
    {
        // Cachear la posición del spawner para usarla en cada instanciación.
        initialPosition = transform.position;

        // Iniciar corrutina de generación continua de asteroides.
        StartCoroutine(AsteroidSpawn());        
    }

    /*
    AsteroidSpawn
    Corrutina que genera asteroides de forma continua con intervalos aleatorios.
    
    - Espera un retardo inicial (delay) antes de comenzar.
    - Luego entra en un bucle infinito instanciando asteroides.
    - Entre cada spawn espera un tiempo aleatorio entre minInterval y maxInterval.
    */
    IEnumerator AsteroidSpawn()
    {
        // Retardo inicial antes del primer spawn (permite que el jugador se prepare).
        yield return new WaitForSeconds(delay);

        // Bucle infinito de generación de asteroides.
        while (true)
        {
            // Instanciar asteroide en la posición del spawner sin rotación.
            Instantiate(asteroid, initialPosition, Quaternion.identity);

            // Esperar un intervalo aleatorio antes del siguiente spawn.
            // Esto crea un patrón de aparición impredecible.
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        }
    }
}
