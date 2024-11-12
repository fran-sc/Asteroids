using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] GameObject asteroid;
    [SerializeField] float minInterval;
    [SerializeField] float maxInterval;
    [SerializeField] float delay;

    Vector3 initialPosition;
    
    void Start()
    {
        initialPosition = transform.position;

        StartCoroutine(AsteroidSpawn());        
    }

    IEnumerator AsteroidSpawn()
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            // instanciamos el asteroide
            Instantiate(asteroid, initialPosition, Quaternion.identity);

            // hacemos una pausa aleatoria
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        }
    }
}
