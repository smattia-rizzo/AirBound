using TMPro;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    //Enum per i livelli di difficoltà
    public enum DifficultyLevelTris
    {
        None,
        Easy,
        Medium,
        Hard
    }

    public static DifficultyLevelTris CurrentDifficultyTris { get; private set; } = DifficultyLevelTris.None;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeTrisDifficulty(int index)
    {
        // Converte l'indice numerico della dropdown nell'enum corrispondente
        CurrentDifficultyTris = (DifficultyLevelTris)index;

        Debug.Log("Difficoltà cambiata in: " + CurrentDifficultyTris);

    }
}
