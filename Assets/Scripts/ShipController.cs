using System.Collections;
using UnityEngine;

/*
ShipController
Responsabilidad:

- Gestionar el ciclo de vida de la nave del jugador: entrada, movimiento, disparo, destrucción y respawn.
- Implementar una animación de entrada al iniciar la partida o reaparición con parpadeo de invulnerabilidad.
- Notificar al GameManager sobre pérdidas de vida y destruir la nave al colisionar con enemigos o asteroides.

Estructuras de datos y referencias:

- SHOOT_OFFSET: distancia vertical desde la nave a la posición de instanciación del disparo.
- shoot (SerializeField): prefab del disparo del jugador.
- explosion (SerializeField): prefab de explosión al ser destruida la nave.
- endPosition (SerializeField): posición final de la animación de entrada.
- duration (SerializeField): duración de la animación de entrada.
- force (SerializeField): magnitud del impulso aplicado en cada FixedUpdate.
- blinkNum (SerializeField): número de parpadeos durante la animación de entrada.
- rb: Rigidbody2D para aplicar fuerzas.
- direction: dirección normalizada de movimiento basada en la entrada del jugador.
- active: bandera que indica si la nave puede moverse/disparar (desactivada durante animación de entrada).
- posicionInicial: posición desde la que respawna la nave.
- game: referencia al GameManager para consultar estado de pausa/game over.

Notas de diseño:

- El movimiento se controla mediante AddForce en FixedUpdate para física consistente.
- La entrada de disparo usa Input.GetButtonDown("Fire1") para compatibilidad con múltiples dispositivos.
- Durante la animación de entrada (StarPlayer), el collider está desactivado para invulnerabilidad temporal.
- Al destruirse, instancia una nueva nave si el juego no ha terminado; esto permite respawn automático.
*/
public class ShipController : MonoBehaviour
{
    const float SHOOT_OFFSET = 0.5f;

    [Header("References")]
    // Prefab del proyectil que dispara el jugador.
    [SerializeField] GameObject shoot;
    // Prefab de explosión al destruir la nave.
    [SerializeField] GameObject explosion;

    [Header("Settings")]
    // Posición de destino al final de la animación de entrada.
    [SerializeField] Vector3 endPosition;
    // Duración de la animación de entrada de la nave.
    [SerializeField] float duration;
    // Magnitud del impulso de movimiento aplicado en cada FixedUpdate.
    [SerializeField] float force;
    // Número de ciclos de parpadeo durante la entrada.
    [SerializeField] float blinkNum;

    // Física de la nave para aplicar fuerzas de movimiento.
    Rigidbody2D rb;
    // Dirección de movimiento normalizada, calculada desde los ejes de entrada.
    Vector2 direction;
    // Indica si la nave puede moverse y disparar (false durante animación de entrada).
    bool active;
    // Posición de respawn de la nave.
    Vector3 posicionInicial;
    // Referencia al GameManager para consultar estado (pausa/game over).
    GameManager game;

    void Start()
    {
        // Guardar posición inicial para respawn posterior.
        posicionInicial = transform.position;

        // Obtener referencia al Rigidbody2D para aplicar fuerzas.
        rb = GetComponent<Rigidbody2D>();    

        // Obtener referencia al GameManager para consultar estados.
        game = GameManager.GetInstance();

        // Iniciar secuencia de entrada con invulnerabilidad temporal.
        StartCoroutine(StarPlayer());
    }

    void Update()
    {
        // Detectar entrada de disparo solo si:
        // 1. La nave está activa (terminó animación de entrada).
        // 2. El juego no está en pausa.
        // 3. Se presiona el botón de disparo (Fire1).
        // 
        // Fire1 mapea a: botón izquierdo del ratón, Left CTRL, o Joystick Button 0.
        if (active && !game.IsPaused() && Input.GetButtonDown("Fire1")) 
        {
            // Calcular posición del disparo ligeramente por delante de la nave.
            Vector3 position = transform.position;
            Vector3 shootPosition = new Vector3(position.x, position.y + SHOOT_OFFSET, position.z);
            
            // Instanciar proyectil en la posición calculada.
            Instantiate(shoot, shootPosition, Quaternion.identity);
        }
    }

    void FixedUpdate()
    {
        // Solo procesar movimiento si la nave está activa.
        if (active)
        {
            CheckMove();
        }
    }

    /*
    CheckMove
    Lee entrada del jugador y aplica fuerza en la dirección indicada.
    
    Usa Input.GetAxis para movimiento analógico suave:
    - "Horizontal": A/D, flechas izq/der, o stick analógico X.
    - "Vertical": W/S, flechas arriba/abajo, o stick analógico Y.
    
    Se normaliza el vector para evitar movimiento más rápido en diagonal.
    */
    void CheckMove()
    {
        // Construir vector de dirección desde entrada horizontal y vertical.
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        // Normalizar para que la magnitud sea 1.0 en todas direcciones.
        // Evita que movimiento diagonal sea más rápido (√2).
        direction.Normalize();

        // Aplicar impulso en la dirección calculada.
        // Modo Impulse aplica la fuerza instantáneamente, afectada por la masa.
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    /*
    OnCollisionEnter2D
    Detecta colisiones físicas con enemigos y asteroides.
    Si colisiona con cualquiera de estos objetos, destruye la nave.
    */
    void OnCollisionEnter2D(Collision2D other)
    {
        string tag = other.gameObject.tag;
        
        // Verificar si colisionó con objetos peligrosos.
        if (tag == "Enemy" || tag == "AsteroidBig" || tag == "AsteroidSmall")
        {
            DestroyShip();
        }
    }

    /*
    OnTriggerEnter2D
    Detecta impactos de proyectiles enemigos (configurados como triggers).
    Si un proyectil enemigo alcanza la nave, esta se destruye.
    */
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            DestroyShip();
        }
    }

    /*
    DestroyShip
    Secuencia de destrucción de la nave del jugador:
    
    1. Desactiva la nave para evitar inputs adicionales.
    2. Crea efecto de explosión.
    3. Notifica al GameManager sobre pérdida de vida.
    4. Si quedan vidas, instancia una nueva nave en la posición inicial.
    5. Destruye el GameObject actual.
    
    El sistema de respawn automático permite continuidad del juego sin
    necesidad de recargar la escena hasta agotar todas las vidas.
    */
    void DestroyShip()
    {
        // Desactivar nave inmediatamente para evitar inputs durante destrucción.
        active = false;

        // Crear efecto visual/sonoro de explosión.
        Instantiate(explosion, transform.position, Quaternion.identity);

        // Notificar al GameManager sobre pérdida de vida.
        GameManager gm = GameManager.GetInstance();
        gm.LoseLife();
        
        // Si el juego no ha terminado, instanciar nueva nave para respawn.
        if (!gm.IsGameOver())
        {
            Instantiate(gameObject, posicionInicial, Quaternion.identity);
        }

        // Destruir el GameObject de la nave actual.
        Destroy(gameObject);
    }

    /*
    StarPlayer
    Corrutina que maneja la animación de entrada de la nave con invulnerabilidad.
    
    Secuencia:
    1. Desactiva collider (invulnerabilidad temporal).
    2. Anima movimiento desde posición inicial hasta endPosition.
    3. Simultáneamente, aplica efecto de parpadeo variando el canal alpha.
    4. Al completar, restaura alpha completo y activa collider.
    5. Marca la nave como activa para permitir control del jugador.
    
    El efecto de parpadeo sirve como indicador visual de invulnerabilidad.
    */
    IEnumerator StarPlayer()
    {
        // Obtener y desactivar collider para invulnerabilidad temporal.
        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;

        // Guardar posición inicial de la animación.
        Vector3 initialPosition = transform.position;

        // Obtener referencia al material para manipular alpha (transparencia).
        Material mat = GetComponent<SpriteRenderer>().material;
        Color color = mat.color;
        
        // Variables de tiempo:
        // t: progreso de traslación (0 a duration).
        // t2: temporizador de ciclos de parpadeo.
        float t = 0, t2 = 0;
        
        // Bucle de animación hasta completar la duración.
        while (t < duration)
        {
            // Actualizar progreso de traslación.
            t += Time.deltaTime;
            
            // Interpolar posición desde inicial hasta final basándose en progreso.
            // Lerp proporciona transición suave.
            Vector3 newPosition = Vector3.Lerp(initialPosition, endPosition, t / duration);
            transform.position = newPosition;

            // Calcular efecto de parpadeo.
            t2 += Time.deltaTime;
            
            // newAlpha oscila entre 0 y 1 según blinkNum ciclos.
            float newAlpha = blinkNum * (t2 / duration);
            
            // Cuando supera 1.0, resetear para iniciar nuevo ciclo de parpadeo.
            if (newAlpha > 1)
            {
                t2 = 0;
            }
            
            // Aplicar alpha al color del material.
            color.a = newAlpha;
            mat.color = color;

            // Esperar hasta el siguiente frame.
            yield return null;
        }
        
        // Al finalizar animación, restaurar alpha completo (totalmente opaco).
        color.a = 1;
        mat.color = color;

        // Activar collider (fin de invulnerabilidad).
        collider.enabled = true;

        // Marcar nave como activa para permitir control del jugador.
        active = true;
    }
}
