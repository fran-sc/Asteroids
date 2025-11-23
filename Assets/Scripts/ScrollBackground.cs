using UnityEngine;

/*
ScrollBackground
Responsabilidad:

- Realizar el desplazamiento vertical continuo de un sprite de fondo y reciclar su posición cuando sale por la parte inferior.

Estructuras de datos y referencias:

- speed (SerializeField): velocidad del scroll vertical del fondo.
- height: altura del sprite calculada a partir de SpriteRenderer.bounds.

Notas de diseño:

- Cuando el fondo se desplaza una altura completa hacia abajo, se traslada hacia arriba el doble de su altura para crear un bucle con dos paneles.
*/
public class ScrollBackground : MonoBehaviour
{
    // Velocidad de desplazamiento vertical del fondo.
    [SerializeField] float speed;

    float height;

    void Start()
    {
        // Calcular y cachear la altura del sprite para usarla en el reposicionado.
        // bounds.size.y proporciona la altura en unidades del mundo.
        height = GetComponent<SpriteRenderer>().bounds.size.y;        
    }

    void Update()
    {
        // Desplazar el fondo hacia abajo a velocidad constante.
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Cuando el fondo se desplaza completamente fuera de la vista por abajo,
        // reposicionarlo hacia arriba el doble de su altura.
        // Esto crea un efecto de scroll infinito con dos paneles que se reciclan.
        if (transform.position.y < -height)
        {
            // Saltar hacia arriba el doble de la altura (asumiendo que hay dos paneles).
            transform.Translate(Vector3.up * height * 2);
        }
    }
}
