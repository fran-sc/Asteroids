using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    const int LIVES = 3;
    const int EXTRA_LIFE = 1500;
    const int SCORE_ENEMY = 50;
    const int SCORE_ASTEROID_BIG = 10;
    const int SCORE_ASTEROID_SMALL = 25;
    const string DATA_FILE = "data.json";

    [Header("GUI")]
    [SerializeField] Text txtScore;
    [SerializeField] Text txtHScore;
    [SerializeField] Text txtMessage;
    [SerializeField] Image[] imgLives;

    [Header("Audio Clips")]
    [SerializeField] AudioClip sfxExtra;
    [SerializeField] AudioClip sfxGameOver;

    static GameManager instance;

    int score;
    int lives = LIVES;
    bool extra;
    bool gameOver;
    bool paused;
    GameData gameData;

    public bool IsGameOver()
    {
        return gameOver;
    }

    public bool IsPaused()
    {
        return paused;
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameData = LoadData();
    }

    private GameData LoadData()
    {
        if (System.IO.File.Exists(DATA_FILE))
        {
            string fileText = System.IO.File.ReadAllText(DATA_FILE);

            return JsonUtility.FromJson<GameData>(fileText);
        }

        return new GameData();
    }

    void SaveData()
    {
        // creamos la repressentaicón en JSON de los datos del juego
        string json = JsonUtility.ToJson(gameData);

        // volcar los datos en el archivo
        System.IO.File.WriteAllText(DATA_FILE, json);
    }

    public void AddScore(string tag)
    {
        int pts = 0;
        
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
        
        // incrementamos la puntuación del jugador
        score += pts;

        // check extra life
        if (!extra && score >= EXTRA_LIFE)
        {
            ExtraLife();
        }

        // comprobar si la puntuación actual supera la máxima
        if (score > gameData.hscore)
        {
            gameData.hscore = score;
        }
    }

    void ExtraLife()
    {
        extra = true;

        lives++;

        AudioSource.PlayClipAtPoint(sfxExtra, Camera.main.transform.position, 1);
    }

    public void LoseLife()
    {
        lives--;

        if (lives == 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gameOver = true;

        // restauramos la escala de tiempo
        Time.timeScale = 1;

        // reproducimos el audio de game over
        AudioSource.PlayClipAtPoint(sfxGameOver, Camera.main.transform.position, 1);

        // establecemos el mensaje de game over
        txtMessage.text = "GAME OVER\nPRESS <RET> TO RESTART";

        // guardamos los datos del juego
        SaveData();
    }

    void OnGUI()
    {
        // activar los iconos de las vidas
        for (int i = 0; i < imgLives.Length; i++)
        {
            imgLives[i].enabled = i < lives - 1;
        }

        // mostrar la puntuación del jugador
        //txtScore.text = string.Format("{0,4:D4}", score);
        txtScore.text = $"{score:D4}";

        // mostrar la puntuación máxima
        //txtHScore.text = string.Format("{0,4:D4}", gameData.hscore);
        txtHScore.text = $"{gameData.hscore:D4}";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (!gameOver)
        {
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
            else if (Input.GetKeyDown(KeyCode.F1))
            {
                Time.timeScale /= 1.25f;
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                Time.timeScale *= 1.25f;
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                Time.timeScale = 1;
            }
        } 
        else if (gameOver && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void PauseGame()
    {
        paused = true;

        // detenemos la música de fondo
        Camera.main.GetComponent<AudioSource>().Pause();

        // establecemos el mensaje de pausa
        txtMessage.text = "PAUSED\nPRESS <P> TO RESUME";

        // pausamos el juego
        Time.timeScale = 0;
    }

    private void ResumeGame()
    {
        paused = false;

        // reanudamos la música de fondo
        Camera.main.GetComponent<AudioSource>().UnPause();

        // eliminamos el mensaje de pausa
        txtMessage.text = "";

        // reanudamos el juego
        Time.timeScale = 1;
    }    
}
