using UnityEngine;

public class ScrollBGShader : MonoBehaviour
{
    [SerializeField] float speed;
    Renderer render;

    void Start()
    {
        render = GetComponent<Renderer>();    
    }

    void Update()
    {
        // desplazammiento del fondo
        Vector2 offset = speed * Time.deltaTime * Vector2.up;
        
        render.material.mainTextureOffset += offset;
    }
}
