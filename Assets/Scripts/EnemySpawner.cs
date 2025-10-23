using System.Collections;
using UnityEngine;

/*
EnemySpawner
Responsabilidad:

- Genera naves enemigas de forma periódica a lo largo del eje X dentro de un rango permitido.
- Permite un retardo inicial antes de comenzar las apariciones.

Estructuras de datos y referencias:

- MIN_X / MAX_X: límites de spawn horizontal.
- enemy (SerializeField): prefab de la nave enemiga a instanciar.
- interval (SerializeField): tiempo fijo entre spawns.
- delay (SerializeField): retardo antes del primer spawn.

Notas de diseño:

- La posición Y se toma del transform del spawner; X se genera aleatoriamente en el rango.
- Utiliza una corrutina infinita para instanciar enemigos en intervalos constantes.
*/
public class EnemySpawner : MonoBehaviour
{
    const float MIN_X = -3.5f;
    const float MAX_X = 3.5f;

    [Header("References")]
    // Prefab de la nave enemiga.
    [SerializeField] GameObject enemy;
 
    [Header("Settings")]
    // Tiempo entre spawns consecutivos.
    [SerializeField] float interval;
    // Retardo inicial antes de empezar a generar enemigos.
    [SerializeField] float delay;

    void Start()
    {
        // Iniciar corrutina de generación continua de enemigos.
        StartCoroutine("EnemySpawn");
    }

    /*
    EnemySpawn
    Corrutina que genera naves enemigas de forma periódica.
    
    - Espera un retardo inicial (delay) antes del primer spawn.
    - Genera enemigos en intervalos fijos (interval).
    - La posición X es aleatoria dentro del rango [MIN_X, MAX_X].
    - La posición Y se toma del transform del spawner (normalmente en la parte superior).
    */
    IEnumerator EnemySpawn()
    {
        // Retardo inicial antes de comenzar a generar enemigos.
        yield return new WaitForSeconds(delay);

        // Bucle infinito de generación de enemigos.
        while (true)
        {
            // Calcular posición de spawn con X aleatorio y Y fijo del spawner.
            // Esto crea apariciones impredecibles a lo ancho de la pantalla.
            Vector3 position = new Vector3(Random.Range(MIN_X, MAX_X), transform.position.y, 0);
            
            // Instanciar nave enemiga en la posición calculada.
            Instantiate(enemy, position, Quaternion.identity);

            // Esperar intervalo fijo antes del siguiente spawn.
            yield return new WaitForSeconds(interval);
        }
    }
}
