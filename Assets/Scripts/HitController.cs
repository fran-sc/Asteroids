using UnityEngine;

/*
HitController
Responsabilidad:

- Controlar la vida de la partícula/efecto de impacto de un disparo y destruirla tras un breve retardo.

Estructuras de datos:

- DELAY: tiempo de permanencia en escena antes de ser destruido.
*/
public class HitController : MonoBehaviour
{
    const float DELAY = 0.25f;
    void Start()
    {
        Destroy(gameObject, DELAY);
    }
}
