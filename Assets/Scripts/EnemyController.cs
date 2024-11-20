using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    const float DESTROY_HEIGHT = -6f;

    [Header("Settings")]
    [SerializeField] float speed;
    [SerializeField] float shootDelay;
    [SerializeField] float shootProb;

    [Header("References")]
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject shoot;

    void Start()
    {
        StartCoroutine(Shoot());
    }

    void Update()
    {
        // desplazamos la nave hacia abajo
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // si la nave se sale de la pantalla la destruimos
        if (transform.position.y < DESTROY_HEIGHT)
        {
            Destroy(gameObject);
        }     
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        DestroyEnemy();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        DestroyEnemy();
    }

    void DestroyEnemy()
    {
        // incrementamos la puntuación del jugador
        GameManager.GetInstance().AddScore(gameObject.tag);

        // instanciamos la animación de la explosión
        Instantiate(explosion, transform.position, Quaternion.identity);
        
        // destruimos la nave
        Destroy(gameObject);
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            // esperamos un tiempo antes de disparar de nuevo
            yield return new WaitForSeconds(shootDelay);

            // comprobamos si la nave dispara
            if (Random.value < shootProb)
            {
                // buscar la nave del jugador
                GameObject player = GameObject.FindWithTag("Player");

                // instanciamos el disparo si nos encontramos enfrente de la nave del jugador
                if (player != null &&
                    (transform.position.x > player.transform.position.x - 0.5f) &&
                    (transform.position.x < player.transform.position.x + 0.5f))
                {
                    Instantiate(shoot, transform.position, Quaternion.identity);
                }
            }
        }
    }
}
