using UnityEngine;

public class EnemyController : MonoBehaviour
{
    const float DESTROY_HEIGHT = -6f;
    [SerializeField] float speed;
    [SerializeField] GameObject explosion;

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
        // instanciamos la animación de la explosión
        Instantiate(explosion, transform.position, Quaternion.identity);
        
        // destruimos la nave
        Destroy(gameObject);
    }
}
