using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{

    [Header("Animator")]
    public Animator dropdownAnimator;

    [Header("Pannelli della UI")]
    public GameObject panelMainMenu;
    public GameObject panelSelectGame;


    [Header("Pannelli dei giochi")]
    public GameObject panelTris;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Mostra il menu principale all'avvio
        BackToMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToMainMenu()
    {
        panelMainMenu.SetActive(true);
        panelSelectGame.SetActive(false);
        panelTris.SetActive(false);
    }

    public void SelectGame()
    {
        panelMainMenu.SetActive(false);
        panelSelectGame.SetActive(true);
        panelTris.SetActive(false);
    }


    // Metodo per avviare il gioco del Tris
    public void StartTrisGame()
    {
        if (DifficultyManager.CurrentDifficultyTris == DifficultyManager.DifficultyLevelTris.None)
        {
            Debug.LogWarning("Animazione errore avviata!");
            dropdownAnimator.SetTrigger("TriggerError");
            return;
        }
        panelMainMenu.SetActive(false);
        panelSelectGame.SetActive(false);
        panelTris.SetActive(true);
    }


    public void QuitGame()
    {
        // Chiude l'applicazione se il gioco è avviato (build finale)
        Application.Quit();

        // Ferma la riproduzione se sei dentro l'editor di Unity
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
