using UnityEngine;

public class ShootController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speed;
    [SerializeField] float temp;

    [Header("References")]
    [SerializeField] GameObject hitExplosion;

    void Update()
    {
        // actualizar mi temporizador        
        temp -= Time.deltaTime;
        if (temp < 0) Destroy(gameObject);
        
        // actualizar la posición
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // destruir el disparo
        DestroyHit();
    }

    void DestroyHit()
    {
        // instanciar la animación de la explosión
        Instantiate(hitExplosion, transform.position, Quaternion.identity);
        
        // destruir el disparo
        Destroy(gameObject);
    }

}
