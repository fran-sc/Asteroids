using UnityEngine;

public class ShootController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speed;
    [SerializeField] float temp;

    void Update()
    {
        // actualizar mi temporizador        
        temp -= Time.deltaTime;
        if (temp < 0) Destroy(gameObject);
        
        // actiualizar la posición
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
