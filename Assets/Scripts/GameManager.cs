using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
GameManager
Responsabilidad:

- Coordina el estado global de la partida: puntuación, vidas, pausa, game over y velocidad del tiempo.
- Gestiona UI de puntuación, mensajes y visualización de vidas.
- Persiste y recupera la puntuación máxima (high score) en un archivo JSON.

Estructuras de datos y referencias:

- LIVES, EXTRA_LIFE, SCORE_*: constantes de configuración del juego (vidas iniciales, puntos por tipo, umbral de vida extra).
- DATA_FILE: nombre del archivo de persistencia JSON.
- txtScore, txtHScore, txtMessage (SerializeField): referencias UI para marcador y mensajes.
- imgLives (SerializeField): array de imágenes para representar vidas restantes.
- sfxExtra, sfxGameOver (SerializeField): clips de audio para eventos clave.
- instance: referencia estática para patrón singleton simple.
- score: puntuación actual del jugador; lives: contador de vidas; extra: bandera para otorgar solo una vida extra por partida.
- gameOver, paused: banderas de estado global.
- gameData: contenedor serializable con datos persistentes (actualmente solo hscore).

Notas de diseño:

- Se implementa un singleton básico con Awake(); esta instancia no se marca como DontDestroyOnLoad para reiniciar estado al recargar escena.
- OnGUI se usa para actualizar la UI de forma inmediata; en proyectos más grandes podría migrarse a eventos o binding de UI.
- Se controla el timeScale para pausar/reanudar y para debug (F1/F2/F3 ajustan la velocidad del tiempo).
- AddScore se invoca desde otros controladores (enemigos/asteroides) pasando el tag del objeto destruido.
*/
public class GameManager : MonoBehaviour
{
    const int LIVES = 3;
    const int EXTRA_LIFE = 1500;
    const int SCORE_ENEMY = 50;
    const int SCORE_ASTEROID_BIG = 10;
    const int SCORE_ASTEROID_SMALL = 25;
    const string DATA_FILE = "data.json";

    [Header("GUI")]
    // Texto con la puntuación actual.
    [SerializeField] Text txtScore;
    // Texto con el récord histórico (high score).
    [SerializeField] Text txtHScore;
    // Texto para mensajes de estado (pausa, game over, etc.).
    [SerializeField] Text txtMessage;
    // Iconos que representan las vidas disponibles.
    [SerializeField] Image[] imgLives;

    [Header("Audio Clips")]
    // Sonido al conseguir vida extra.
    [SerializeField] AudioClip sfxExtra;
    // Sonido al terminar la partida.
    [SerializeField] AudioClip sfxGameOver;

    static GameManager instance;

    int score;
    int lives = LIVES;
    bool extra;
    bool gameOver;
    bool paused;
    GameData gameData;

    /*
    IsGameOver
    Consulta pública del estado de fin de partida.
    Otros componentes pueden usar esto para detener acciones cuando el juego ha terminado.
    */
    public bool IsGameOver()
    {
        return gameOver;
    }

    /*
    IsPaused
    Consulta pública del estado de pausa.
    Permite a otros componentes (como ShipController) saber si deben
    ignorar entrada del usuario durante la pausa.
    */
    public bool IsPaused()
    {
        return paused;
    }

    /*
    GetInstance
    Acceso público a la instancia singleton del GameManager.
    Permite a otros scripts acceder a funcionalidades globales del juego
    sin necesidad de referencias SerializeField.
    */
    public static GameManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        // Implementación del patrón singleton:
        // Si no existe instancia, este objeto se convierte en la instancia.
        // Si ya existe otra instancia, este objeto se autodestruye.
        // Esto garantiza que solo exista un GameManager en la escena.
        if (instance == null)
        {
            instance = this;
            // Nota: DontDestroyOnLoad está comentado para que el GameManager
            // se reinicie al cambiar de escena, permitiendo reset completo del estado.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Cargar datos persistentes (high score) desde disco al iniciar.
        gameData = LoadData();
    }

    /*
    LoadData
    Carga los datos persistentes desde el archivo JSON.
    
    - Si el archivo existe, lo deserializa y devuelve el objeto GameData.
    - Si no existe (primera ejecución), devuelve un objeto GameData nuevo con valores por defecto.
    */
    private GameData LoadData()
    {
        // Verificar si existe el archivo de datos persistentes.
        if (System.IO.File.Exists(DATA_FILE))
        {
            // Leer todo el contenido del archivo como string JSON.
            string fileText = System.IO.File.ReadAllText(DATA_FILE);

            // Deserializar JSON a objeto GameData y devolverlo.
            return JsonUtility.FromJson<GameData>(fileText);
        }

        // Si no existe archivo, devolver datos nuevos (hscore = 0 por defecto).
        return new GameData();
    }

    /*
    SaveData
    Serializa y guarda los datos persistentes en el archivo JSON.
    Se llama automáticamente al terminar la partida (Game Over).
    */
    void SaveData()
    {
        // Serializar el objeto gameData a formato JSON.
        string json = JsonUtility.ToJson(gameData);

        // Escribir el JSON al archivo (sobrescribe si ya existe).
        System.IO.File.WriteAllText(DATA_FILE, json);
    }

    /*
    AddScore
    Método público llamado por otros controladores (asteroides, enemigos)
    para notificar destrucción de objetos y actualizar puntuación.
    
    Parámetro tag: identifica el tipo de objeto destruido ("Enemy", "AsteroidBig", "AsteroidSmall").
    
    Además de sumar puntos:
    - Verifica si se alcanza el umbral para vida extra (solo una vez por partida).
    - Actualiza el high score si la puntuación actual lo supera.
    */
    public void AddScore(string tag)
    {
        int pts = 0;
        
        // Determinar puntos según el tipo de objeto destruido.
        switch (tag)
        {
            case "Enemy":
                pts = SCORE_ENEMY;
                break;
            case "AsteroidBig":
                pts = SCORE_ASTEROID_BIG;
                break;
            case "AsteroidSmall":
                pts = SCORE_ASTEROID_SMALL;
                break;
        }
        
        // Incrementar puntuación acumulada.
        score += pts;

        // Verificar si se alcanza el umbral para vida extra.
        // La bandera 'extra' asegura que solo se otorgue una vida extra por partida.
        if (!extra && score >= EXTRA_LIFE)
        {
            ExtraLife();
        }

        // Actualizar high score si la puntuación actual es superior.
        // Esto se guarda en memoria; se persiste a disco al terminar la partida.
        if (score > gameData.hscore)
        {
            gameData.hscore = score;
        }
    }

    /*
    ExtraLife
    Otorga una vida adicional al jugador cuando alcanza el umbral de puntuación.
    
    - Marca la bandera 'extra' para evitar otorgar más vidas en la misma partida.
    - Incrementa el contador de vidas.
    - Reproduce sonido de recompensa.
    */
    void ExtraLife()
    {
        // Marcar que ya se otorgó la vida extra.
        extra = true;

        // Incrementar contador de vidas.
        lives++;

        // Reproducir sonido de vida extra.
        AudioSource.PlayClipAtPoint(sfxExtra, Camera.main.transform.position, 1);
    }

    /*
    LoseLife
    Método público llamado por ShipController cuando la nave del jugador es destruida.
    
    - Decrementa el contador de vidas.
    - Si las vidas llegan a cero, activa el estado de Game Over.
    */
    public void LoseLife()
    {
        // Decrementar contador de vidas.
        lives--;

        // Si no quedan vidas, terminar la partida.
        if (lives == 0)
        {
            GameOver();
        }
    }

    /*
    GameOver
    Activa el estado de fin de partida.
    
    Secuencia:
    1. Marca la bandera gameOver.
    2. Restaura la escala de tiempo (por si estaba pausado).
    3. Reproduce sonido de Game Over.
    4. Muestra mensaje de Game Over con instrucciones de reinicio.
    5. Persiste los datos (high score) a disco.
    */
    void GameOver()
    {
        // Marcar estado de fin de partida.
        gameOver = true;

        // Restaurar velocidad normal del tiempo (por si estaba en pausa o alterada).
        Time.timeScale = 1;

        // Reproducir efecto de sonido de Game Over.
        AudioSource.PlayClipAtPoint(sfxGameOver, Camera.main.transform.position, 1);

        // Mostrar mensaje de fin de partida con instrucciones.
        txtMessage.text = "GAME OVER\nPRESS <RET> TO RESTART";

        // Guardar datos persistentes (high score) a disco.
        SaveData();
    }

    /*
    OnGUI
    Se ejecuta cada frame para actualizar elementos de UI.
    
    Actualiza:
    - Visibilidad de iconos de vidas (muestra lives - 1 iconos).
    - Texto de puntuación actual formateado a 4 dígitos.
    - Texto de high score formateado a 4 dígitos.
    
    Nota: OnGUI es un método legacy de Unity, pero funcional para UI simple.
    En proyectos más complejos se recomienda usar eventos o data binding.
    */
    void OnGUI()
    {
        // Actualizar visibilidad de iconos de vidas.
        // Se muestran lives-1 iconos (la vida actual se representa con la nave en pantalla).
        for (int i = 0; i < imgLives.Length; i++)
        {
            imgLives[i].enabled = i < lives - 1;
        }

        // Mostrar puntuación actual formateada a 4 dígitos con ceros a la izquierda.
        txtScore.text = $"{score:D4}";

        // Mostrar high score formateado a 4 dígitos.
        txtHScore.text = $"{gameData.hscore:D4}";
    }

    /*
    Update
    Maneja entrada de teclado para funciones globales del juego.
    
    Controles:
    - ESC: Salir de la aplicación.
    - P: Alternar pausa (solo durante partida activa).
    - F1/F2/F3: Ajustar velocidad del tiempo (debug, solo durante partida activa).
    - ENTER: Reiniciar partida (solo en estado Game Over).
    */
    void Update()
    {
        // ESC siempre disponible para salir del juego.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        // Controles durante partida activa (no Game Over).
        else if (!gameOver)
        {
            // P: Alternar pausa.
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (paused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
            // F1: Reducir velocidad del tiempo (debug, ralentizar juego).
            else if (Input.GetKeyDown(KeyCode.F1))
            {
                Time.timeScale /= 1.25f;
            }
            // F2: Aumentar velocidad del tiempo (debug, acelerar juego).
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                Time.timeScale *= 1.25f;
            }
            // F3: Restaurar velocidad normal del tiempo.
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                Time.timeScale = 1;
            }
        } 
        // Control en estado Game Over.
        else if (gameOver && Input.GetKeyDown(KeyCode.Return))
        {
            // ENTER: Recargar la escena para reiniciar la partida.
            SceneManager.LoadScene(0);
        }
    }

    /*
    PauseGame
    Activa el estado de pausa del juego.
    
    Acciones:
    1. Marca bandera de pausa.
    2. Pausa la música de fondo.
    3. Muestra mensaje de pausa.
    4. Congela el tiempo (Time.timeScale = 0) para detener física y animaciones.
    */
    private void PauseGame()
    {
        // Marcar estado de pausa.
        paused = true;

        // Pausar música de fondo (asumiendo que está en la cámara principal).
        Camera.main.GetComponent<AudioSource>().Pause();

        // Mostrar mensaje de pausa con instrucciones.
        txtMessage.text = "PAUSED\nPRESS <P> TO RESUME";

        // Congelar el tiempo del juego (detiene física, animaciones, corrutinas con WaitForSeconds).
        Time.timeScale = 0;
    }

    /*
    ResumeGame
    Desactiva el estado de pausa y reanuda el juego.
    
    Acciones:
    1. Desmarca bandera de pausa.
    2. Reanuda la música de fondo.
    3. Oculta mensaje de pausa.
    4. Restaura velocidad normal del tiempo.
    */
    private void ResumeGame()
    {
        // Desmarcar estado de pausa.
        paused = false;

        // Reanudar música de fondo.
        Camera.main.GetComponent<AudioSource>().UnPause();

        // Ocultar mensaje de pausa.
        txtMessage.text = "";

        // Restaurar velocidad normal del tiempo.
        Time.timeScale = 1;
    }    
}
