using UnityEngine;

public class ScrollBGShader : MonoBehaviour
{
    [SerializeField] float speed;
    Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();    
    }

    void Update()
    {
        // desplazammiento del fondo
        Vector2 offset = speed * Time.deltaTime * Vector2.up;
        
        renderer.material.mainTextureOffset += offset;
    }
}
