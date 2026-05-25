using UnityEngine;
using UnityEngine.UI;
using TMPro; // Rimuovi se usi il testo standard di Unity al posto di TextMeshPro

public class GameManager1 : MonoBehaviour
{
    private Tris partita;
    private int giocatoreCorrente;
    private int turno = 1;

    // Trascina i tuoi 9 bottoni qui nell'Ispettore (l'ordine nell'array sarà l'indice 0-8)
    public Button[] bottoni;

    void Start()
    {
        partita = new Tris();
        giocatoreCorrente = Random.Range(1, 3); // Sceglie casualmente tra 1 e 2 chi inizia




        //Event Listeners
        // Assegniamo la funzione a tutti i bottoni dinamicamente
        for (int i = 0; i < bottoni.Length; i++)
        {
            // CRITICO: Devi creare una variabile locale temporanea.
            // Se usassi direttamente 'i', tutti i bottoni registrerebbero il valore finale del ciclo (9).
            int indiceMossa = i;

            // Collega il click del bottone alla funzione passando l'indice corretto
            bottoni[i].onClick.AddListener(() => OnGrigliaClick(indiceMossa));
        }

        
    }

    //
    private void Update()
    {
        // Qui potresti aggiornare un testo UI per mostrare il turno corrente o altre info
        // Es: turnoText.text = $"Turno: {turno} - Giocatore {giocatoreCorrente}";
    }



    private void OnDestroy()
    {
        // Rimuovi i listener per evitare memory leak
        for (int i = 0; i < bottoni.Length; i++)
        {
            bottoni[i].onClick.RemoveAllListeners();
        }
    }



    // La funzione UNICA richiamata da tutti i bottoni
    private void OnGrigliaClick(int indice)
    {
        Debug.Log($"Premuto il bottone in posizione: {indice}");
    }

}