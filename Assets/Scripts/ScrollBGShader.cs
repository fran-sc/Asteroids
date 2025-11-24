using UnityEngine;

/*
ScrollBGShader
Responsabilidad:

- Desplazar el offset de textura del material del fondo para simular scroll vertical sin mover el transform del objeto.

Estructuras de datos y referencias:

- speed (SerializeField): velocidad de desplazamiento de la textura.
- render: componente Renderer del GameObject para acceder al material.

Notas de diseño:

- Modifica mainTextureOffset del material en cada frame, lo que permite un scroll continuo usando shaders.
- Más eficiente que mover múltiples objetos cuando se usa tiling de texturas.
*/
public class ScrollBGShader : MonoBehaviour
{
    // Velocidad de desplazamiento del offset de textura.
    [SerializeField] float speed;
    Renderer render;

    void Start()
    {
        // Obtener referencia al componente Renderer para acceder al material.
        render = GetComponent<Renderer>();    
    }

    void Update()
    {
        // Calcular desplazamiento del offset de textura basado en velocidad y tiempo.
        // Vector2.up (0, 1) desplaza verticalmente hacia arriba.
        Vector2 offset = speed * Time.deltaTime * Vector2.up;
        
        // Aplicar el offset acumulativo al material.
        // Esto crea el efecto de scroll sin mover el GameObject.
        // Unity maneja automáticamente el wrapping cuando el offset supera 1.0.
        // La imagen de textura debe estar configurada para repetir (Wrap Mode: Repeat).
        render.material.mainTextureOffset += offset;
    }
}
