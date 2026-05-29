using UnityEngine;
using Unity.InferenceEngine;

public class TrisSentisAIWorker : System.IDisposable
{
    private Model modelloInizializzato;
    private Worker worker;

    public TrisSentisAIWorker(ModelAsset modelAsset)
    {
        // Caricamento del modello
        modelloInizializzato = ModelLoader.Load(modelAsset);

        // Creazione del worker per l'inferenza
        worker = new Worker(modelloInizializzato, BackendType.CPU);
    }

    public int CalcolaMossaModello(Tris gioco, int numAi)
    {
        int[] tavoloInt = gioco.VisualizzaTavoliere(numAi);

        float[] tavoloFloat = new float[9];
        for (int i = 0; i < 9; i++) tavoloFloat[i] = (float)tavoloInt[i];

        // Generazione del tensor di input
        using Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 9), tavoloFloat);

        worker.Schedule(inputTensor);

        // Generazione del tensor di output
        Tensor<float> outputTensor = worker.PeekOutput() as Tensor<float>;

        // Download dei dati di output in un array
        float[] logitUscita = outputTensor.DownloadToArray();

        // Controlli mosse illegali
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
        // Liberiamo le risorse del worker
        worker?.Dispose();
    }
}