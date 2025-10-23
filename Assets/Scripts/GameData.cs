using System;

/*
GameData
Responsabilidad:

- Contener datos persistentes del juego que puedan serializarse a/desde JSON.

Estructuras de datos:

- hscore: puntuación máxima alcanzada en sesiones previas.

Notas de diseño:

- Marcada como [Serializable] para permitir su serialización con JsonUtility.
- El almacenamiento y carga de este objeto lo gestiona GameManager (archivo "data.json").
*/
[Serializable]
public class GameData
{
    public int hscore;
}
