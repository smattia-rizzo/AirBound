using UnityEngine;
using Unity.InferenceEngine; // La nuova API ufficiale per Unity 6+ (Sentis 2.x)

public class TrisSentisAIWorker : System.IDisposable
{
    private Model modelloInizializzato;
    private Worker worker; // Diventa una classe concreta instanziabile (addio IWorker)

    public TrisSentisAIWorker(ModelAsset modelAsset)
    {
        // Caricamento del modello (rimasto invariato)
        modelloInizializzato = ModelLoader.Load(modelAsset);

        // NUOVA API: Si crea il worker direttamente tramite costruttore
        worker = new Worker(modelloInizializzato, BackendType.CPU);
    }

    public int CalcolaMossaModello(Tris gioco, int numAi)
    {
        int[] tavoloInt = gioco.VisualizzaTavoliere(numAi);

        float[] tavoloFloat = new float[9];
        for (int i = 0; i < 9; i++) tavoloFloat[i] = (float)tavoloInt[i];

        // NUOVA API: Uso del Tensor generico di tipo float al posto di TensorFloat
        using Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 9), tavoloFloat);

        // NUOVA API: .Execute() diventa .Schedule()
        worker.Schedule(inputTensor);

        // NUOVA API: Cast dell'output sul nuovo formato generico Tensor<float>
        Tensor<float> outputTensor = worker.PeekOutput() as Tensor<float>;

        // NUOVA API: Estrazione dei dati con un'unica chiamata sincrona e pulita
        float[] logitUscita = outputTensor.DownloadToArray();

        // --- Logica di mascheramento mosse illegittime (identica a prima) ---
        int mossaMigliore = -1;
        float punteggioMassimo = float.MinValue;
        int[] tavoliereReale = gioco.Tavoliere;

        for (int i = 0; i < logitUscita.Length; i++)
        {
            if (tavoliereReale[i] == 0) // Casella libera
            {
                if (logitUscita[i] > punteggioMassimo)
                {
                    punteggioMassimo = logitUscita[i];
                    mossaMigliore = i;
                }
            }
        }

        return mossaMigliore;
    }

    public void Dispose()
    {
        // Ricordati sempre di fare il Dispose del worker per liberare la memoria nativa
        worker?.Dispose();
    }
}