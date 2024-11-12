using Unity.VisualScripting;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    const float DESTROY_HEIGHT = -6f;
    const int HITS_TO_DESTROY = 4;

    [SerializeField] float force;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject asteroid;
    
    int hitCount;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (tag == "AsteroidBig")
        {
            LaunchBigAsteroid();
        }
    }

    void Update()
    {
        if (transform.position.y < DESTROY_HEIGHT)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DestroyAsteroid();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (tag == "AsteroidBig")
        {
            hitCount++;
            if (hitCount == HITS_TO_DESTROY)
            {
                LaunchSmallAsteroid();

                DestroyAsteroid();
            }
        }
        else
        {
            DestroyAsteroid();
        }
    }

    void DestroyAsteroid()
    {
        // instanciamos la animación de la explosión
        Instantiate(explosion, transform.position, Quaternion.identity);
        
        // destruimos el asteroide
        Destroy(gameObject);
    }

    void LaunchBigAsteroid()
    {
        Vector2 direction = new Vector2(1, -0.25f);
        direction.Normalize();

        rb.AddForce(direction * force, ForceMode2D.Impulse);
        rb.AddTorque(-.1f, ForceMode2D.Impulse);
    }

    void LaunchSmallAsteroid()
    {
        // posición y velocidad del padre
        Vector3 position = transform.position;
        Vector2 linearVelocity = rb.linearVelocity;
        float angularVelocity = rb.angularVelocity;

        // instanciamos los dos asteroides pequeños
        for (int i=0, s=1; i<2; i++, s*=-1)
        {
            GameObject smallAsteroid = Instantiate(asteroid, position, Quaternion.identity);
            
            Rigidbody2D rbSmall = smallAsteroid.GetComponent<Rigidbody2D>();

            rbSmall.linearVelocity = new Vector2(s * linearVelocity.x, linearVelocity.y);
            rbSmall.angularVelocity = angularVelocity;
        }
    }
}

