using System.Collections;
using TMPro;
using Unity.InferenceEngine; // Necessario per passare il ModelAsset dall'Ispettore
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private Tris partita;
    private TrisSentisAIWorker aiSentisWorker; // Il nostro nuovo worker AI neurale
    private Button[] bottoni; // Array dei 9 bottoni della griglia

    [Header("Sentis AI Asset")]
    public ModelAsset easyTrisModel; // Trascina qui il file .onnx / .sentis dall'Ispettore di Unity
    public ModelAsset mediumTrisModel; // Trascina qui il file .onnx / .sentis dall'Ispettore di Unity
    public ModelAsset hardTrisModel; // Trascina qui il file .onnx / .sentis dall'Ispettore di Unity

    [Header("UI Elementi")]
    public GameObject buttonPrefab;       // Il Prefab del pulsante TMP
    public Transform gridParent;          // L'oggetto "GrigliaPulsanti" con il Grid Layout Group
    public TMP_Text gameStateText;
    public TMP_Text gameTimerText;

    [Header("Bottoni di fine partita")]
    public Button restartButton;
    public Button menuButton;

    //Timer
    private float gameTimer;

    private int giocatoreCorrente;   // 1 = Player, 2 = AI
    private int conteggioTurni = 0;
    private bool giocoAttivo = false;
    private bool aiInAttesa = false;


    // Event Handlers Unity
    void Start()
    {

    }

    void OnEnable()
    {
        // Impostazione dei bottoni di fine partita
        restartButton.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);

        // Inizializzazione della griglia dei bottoni
        bottoni = new Button[9];
        for (int i = 0; i < 9; i++)
        {
            GameObject nuovoBottone = Instantiate(buttonPrefab, gridParent);
            bottoni[i] = nuovoBottone.GetComponent<Button>();
        }


        partita = new Tris();

        // Inizializziamo il worker passandogli l'asset caricato nell'Ispettore
        switch (DifficultyManager.CurrentDifficultyTris)
        {
            case DifficultyManager.DifficultyLevelTris.Easy:
                if (easyTrisModel == null)
                {
                    Debug.LogError("Manca il file del modello AI easy (ModelAsset) nel GameManager!");
                    return;
                }
                aiSentisWorker = new TrisSentisAIWorker(easyTrisModel);
                break;
            case DifficultyManager.DifficultyLevelTris.Medium:
                if (mediumTrisModel == null)
                {
                    Debug.LogError("Manca il file del modello AI medium (ModelAsset) nel GameManager!");
                    return;
                }
                aiSentisWorker = new TrisSentisAIWorker(mediumTrisModel);
                break;
            case DifficultyManager.DifficultyLevelTris.Hard:
                if (mediumTrisModel == null)
                {
                    Debug.LogError("Manca il file del modello AI hard (ModelAsset) nel GameManager!");
                    return;
                }
                aiSentisWorker = new TrisSentisAIWorker(hardTrisModel);
                break;
        }

        for (int i = 0; i < bottoni.Length; i++)
        {
            int indiceMossa = i;
            bottoni[i].onClick.AddListener(() => OnBottonePremuto(indiceMossa));
        }

        AvviaPartita();
    }

    private void OnDisable()
    {
        DestroyButtons();
        // Liberiamo la memoria del worker AI quando il GameManager viene disabilitato
        aiSentisWorker?.Dispose();
    }

    void AvviaPartita()
    {
        // Impostazione dei bottoni di fine partita
        restartButton.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        gameTimer = 0f;

        giocoAttivo = true;
        conteggioTurni = 1;
        ResetGraficaBottoni();

        // Scelta randomica di chi inizia
        giocatoreCorrente = Random.Range(1, 3);
        GestisciInizioTurno();
    }

    void Update()
    {
        if (!giocoAttivo) return;

        //Aumento timer
        gameTimer += Time.deltaTime;
        gameTimerText.text = $"Tempo partita: {Mathf.CeilToInt(gameTimer)}s";

    }


    // Game Logic
    void GestisciInizioTurno()
    {
        if (giocatoreCorrente == 1)
        {
            gameStateText.text = $"Turno {conteggioTurni}: Tocca a te!";
        }
        else
        {
            gameStateText.text = $"Turno {conteggioTurni}: L'IA sta calcolando, attendere prego...";
            StartCoroutine(EseguiMossaAI());
        }
    }

    void OnBottonePremuto(int indice)
    {
        if (!giocoAttivo || giocatoreCorrente != 1 || aiInAttesa) return;
        RisolviMossa(indice);
    }

    IEnumerator EseguiMossaAI()
    {
        aiInAttesa = true;
        yield return new WaitForSeconds(1.6f); // Pausa per dare naturalezza all'azione

        // Chiediamo la mossa alla Rete Neurale!
        int mossaScelta = aiSentisWorker.CalcolaMossaModello(partita, 2);

        if (mossaScelta != -1)
        {
            RisolviMossa(mossaScelta);
        }
        aiInAttesa = false;
    }

    void RisolviMossa(int indice)
    {
        if (partita.Play(giocatoreCorrente, indice))
        {
            bottoni[indice].GetComponentInChildren<TMP_Text>().text = (giocatoreCorrente == 1) ? "X" : "O";
            bottoni[indice].interactable = false;

            bool? statoPartita = partita.CheckVittoria(giocatoreCorrente);

            if (statoPartita == true)
            {
                gameStateText.text = (giocatoreCorrente == 1) ? "Hai VINTO!" : "La Rete Neurale ha VINTO!";
                TerminaPartita();
            }
            else if (statoPartita == null)
            {
                gameStateText.text = "Partita finita in PAREGGIO!";
                TerminaPartita();
            }
            else
            {
                giocatoreCorrente = (giocatoreCorrente == 1) ? 2 : 1;
                conteggioTurni++;
                GestisciInizioTurno();
            }
        }
    }

    // Utils
    void TerminaPartita()
    {
        giocoAttivo = false;
        gameTimerText.text = "Fine";
        foreach (var b in bottoni) b.interactable = false;

        // Mostriamo i bottoni di fine partita
        restartButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
    }

    public void RestartPartita()
    {
        if (!giocoAttivo)
        {
            partita = new Tris();
            AvviaPartita();
        }
        
    }

    void ResetGraficaBottoni()
    {
        foreach (var b in bottoni)
        {
            b.GetComponentInChildren<TMP_Text>().text = "";
            b.interactable = true;
        }
    }

    public void DestroyButtons()
    {
        foreach (var b in bottoni)
        {
            Destroy(b.gameObject);
        }
        bottoni = null;
    }

    // IMPRESCINDIBILE: Quando distruggiamo il GameManager o cambiamo scena, 
    // dobbiamo liberare la memoria di Sentis per evitare Memory Leak gravissimi in Unity.
    private void OnDestroy()
    {
        aiSentisWorker?.Dispose();
    }
}