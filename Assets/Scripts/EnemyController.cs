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
        Instantiate(explosion, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("[Enemy] - Colisión");
        }
    }
}
