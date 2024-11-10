using UnityEngine;

public class EnemyController : MonoBehaviour
{
    const float DESTROY_HEIGHT = -6f;
    [SerializeField] float speed;

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
}
