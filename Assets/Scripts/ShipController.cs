using System.Collections;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    const float SHOOT_OFFSET = 0.5f;

    [Header("References")]
    [SerializeField] GameObject shoot;
    [SerializeField] GameObject explosion;

    [Header("Settings")]
    [SerializeField] Vector3 endPosition;
    [SerializeField] float duration;
    [SerializeField] float force;
    [SerializeField] float blinkNum;

    Rigidbody2D rb;
    Vector2 direction;
    bool active;
    Vector3 posicionInicial;
    GameManager game;

    void Start()
    {
        posicionInicial = transform.position;

        rb = GetComponent<Rigidbody2D>();    

        game = GameManager.GetInstance();

        StartCoroutine(StarPlayer());
    }

    void Update()
    {
        // if (active && Input.GetKeyDown(KeyCode.Space))
        // Fire1 es el botón izquierdo del ratón, Left CTRL y Joystick Button 0
        if (active && !game.IsPaused() && Input.GetButtonDown("Fire1")) 
        {
            // obtenemos la posición de la nave
            Vector3 position = transform.position;
            // establecemos la posición del disparo
            Vector3 shootPosition = new Vector3(position.x, position.y + SHOOT_OFFSET, position.z);
            // instanciamos el disparo
            Instantiate(shoot, shootPosition, Quaternion.identity);
        }
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

    void OnCollisionEnter2D(Collision2D other)
    {
        string tag = other.gameObject.tag;
        if (tag == "Enemy" || tag == "AsteroidBig" || tag == "AsteroidSmall")
        {
            DestroyShip();
        }
    }

    void DestroyShip()
    {
        active = false;

        // instanciamos la animación de la explosión
        Instantiate(explosion, transform.position, Quaternion.identity);

        // actualizamos las vidas del jugador
        GameManager gm = GameManager.GetInstance();
        gm.LoseLife();
        
        // instanciamos una nueva nave
        if (!gm.IsGameOver())
        {
            Instantiate(gameObject, posicionInicial, Quaternion.identity);
        }

        // destruimos la nave actual
        Destroy(gameObject);

        // reiniciamos la nave
        //rb.linearVelocity = Vector2.zero;
        //transform.position = posicionInicial;
        //StartCoroutine(StarPlayer());
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
