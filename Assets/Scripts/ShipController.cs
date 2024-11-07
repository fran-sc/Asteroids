using System.Collections;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3 endPosition;
    [SerializeField] float duration;
    [SerializeField] float force;
    [SerializeField] float blinkNum;

    Rigidbody2D rb;
    Vector2 direction;
    bool active;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    

        StartCoroutine(StarPlayer());
    }

    void FixedUpdate()
    {
        if (active)
        {
            CheckMove();
        }
            
    }

    void CheckMove()
    {
        // dirección de movimiento
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        direction.Normalize();

        // aplicamos la fuerza en esa dirección
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    IEnumerator StarPlayer()
    {
        // desactivamos las colisiones para la nave
        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;

        // posición inicial de la nave
        Vector3 initialPosition = transform.position;

        // referencia al material del sprite
        Material mat = GetComponent<SpriteRenderer>().material;
        Color color = mat.color;
        
        float t = 0, t2 = 0;
        while (t < duration)
        {
            // traslación de la nave
            t += Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp(initialPosition, endPosition, t / duration);
            transform.position = newPosition;

            // parpadeo de la nave
            t2 += Time.deltaTime;
            float newAlpha = blinkNum * (t2 / duration);
            if (newAlpha > 1)
            {
                t2 = 0;
            }
            color.a = newAlpha;
            mat.color = color;

            yield return null;
        }
        // reseteamos el canal alpha
        color.a = 1;
        mat.color = color;

        // activamos las colisiones para la nave
        collider.enabled = true;

        // activamos la nave
        active = true;
    }
}
