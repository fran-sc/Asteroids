using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    [SerializeField] float speed;

    float height;

    void Start()
    {
        height = GetComponent<SpriteRenderer>().bounds.size.y;        
    }

    void Update()
    {
        // scroll del fondo
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // si el fondo está fuera de la pantalla, reseteamos su posición
        if (transform.position.y < -height)
        {
            transform.Translate(Vector3.up * height * 2);
        }
    }
}
