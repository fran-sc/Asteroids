using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    const float MIN_X = -3.5f;
    const float MAX_X = 3.5f;

    [Header("References")]
    [SerializeField] GameObject enemy;
 
    [Header("Settings")]
    [SerializeField] float interval;
    [SerializeField] float delay;

    void Start()
    {
        StartCoroutine("EnemySpawn");
    }

    IEnumerator EnemySpawn()
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            Vector3 position = new Vector3(Random.Range(MIN_X, MAX_X), transform.position.y, 0);
            Instantiate(enemy, position, Quaternion.identity);

            yield return new WaitForSeconds(interval);
        }
    }
}
