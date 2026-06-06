# AirBound 🎮🪽

**AirBound** è una piattaforma di mini-giochi interattivi sviluppata in **Unity**, progettata per sfidare l'utente contro intelligenze artificiali avanzate e sperimentare sistemi di input innovativi basati sulla **Computer Vision**.

Attualmente il progetto include il classico gioco del **Tris (Tic-Tac-Toe)**, ma l'architettura è nativamente predisposta per integrare una serie di altri videogiochi separati in futuro.

---

## 🚀 Caratteristiche Principali

- **Hub Multi-gioco:** Una struttura software flessibile che ospiterà una collezione di giochi in un'unica applicazione.
- **AI Scalabile per Difficoltà:** Gli avversari virtuali non seguono semplici script algoritmici, ma sono guidati da modelli di Deep Learning ottimizzati per diversi livelli di sfida.
- **Input tramite Computer Vision (Hand Tracking):** Un sistema di controllo rivoluzionario che permette di muovere il cursore di gioco e cliccare (tramite il gesto del "pizzico") semplicemente muovendo la mano davanti alla webcam, senza toccare il mouse.

---

## 🧠 L'Architettura dell'Intelligenza Artificiale

L'AI di AirBound è stata interamente progettata, addestrata ed esportata per funzionare nativamente all'interno di Unity a frame-rate elevati.

1. **Addestramento Locale:** I modelli sono stati strutturati e allenati in locale utilizzando **TensorFlow (Python)**.
2. **Esportazione in ONNX:** Attraverso la libreria `tf2onnx`, i modelli TensorFlow sono stati convertiti nel formato standard universale `.onnx`.
3. **Inference in Unity:** Unity esegue l'inferenza del modello in tempo reale sul gioco tramite i file ONNX, garantendo zero latenza e avversari reattivi in base alla difficoltà selezionata.

> 🛠️ **Repository dedicata al Machine Learning:** Per consultare il codice sorgente dell'addestramento, i dataset o i notebook di configurazione dei modelli, visita il [AirBound-ModelTrainingCamp](https://github.com/smattia-rizzo/AirBound-ModelTrainingCamp).

---

## 🖐️ Sistema di Controllo Real-Time (Hand Tracking & UDP)

Per abilitare il controllo tramite tracciamento della mano, il gioco si interfaccia con un'architettura distribuita basata su socket di rete:

```text
 [ Webcam ] ──> [ Python Server (OpenCV + MediaPipe) ]
                                   │
                                   ▼ (Pacchetti UDP)
                            [ Unity Game ]

```

* **Rilevamento:** Un server locale in **Python** cattura il flusso video della webcam, elabora le costellazioni della mano tramite **MediaPipe** e calcola le coordinate spaziali del cursore e lo stato del click (gesto del *pinch* / pizzico).
* **Comunicazione:** I dati elaborati vengono trasmessi istantaneamente al motore grafico Unity attraverso il protocollo **UDP**, minimizzando l'overhead di rete e garantendo un input fluido.

> 🖥️ **Repository dedicata al Server di Input:** Per scaricare, configurare ed avviare il server di tracciamento della mano, visita il [AirBound-pythonServer](https://github.com/smattia-rizzo/AirBound-pythonServer).

---

## 🛠️ Requisiti e Installazione

### 1. Scarica la release

Scarica il file .zip dell'ultima [release](https://github.com/smattia-rizzo/AirBound/releases) del progetto.


Estrai la cartella ed esegui il .exe del gioco

### 2. Configura il Server di Input (Opzionale, richiesto per Hand Tracking)

Se desideri giocare utilizzando i movimenti della mano:

1. Clona e avvia il server seguendo le istruzioni presenti nella repository [AirBound-pythonServer](https://github.com/smattia-rizzo/AirBound-pythonServer).
2. Assicurati che il server sia in esecuzione sulla stessa macchina (`localhost`) sulla porta UDP configurata nel gioco.

*Nota: Se il server Python non è attivo, il gioco rimarrà perfettamente fruibile utilizzando il mouse tradizionale.*

---

## 🎮 Come Giocare

1. **Modalità Classica:** Avvia il gioco, segui le istruzioni della gui, scegli una difficoltà e divertiti!
2. **Modalità Hand-Tracking:** - Avvia il server Python.
* Una volta avviato il gioco, cliccare sul pulsante in basso a sinistra per abilitare l'integrazione
* Posizionati davanti alla webcam.
* Muovi la mano per spostare il mirino sullo schermo.
* **Fai un "pizzico" (unisci pollice e indice)** per simulare il click del tasto sinistro del mouse



---

## 🗺️ Roadmap del Progetto

* [x] Sviluppo del gioco base (Tris/Tic-Tac-Toe).
* [x] Integrazione dell'inferenza dei modelli ONNX in Unity.
* [x] Sviluppo del sistema di tracciamento UDP con Python/MediaPipe.
* [x] Implementazione di nuovi algoritmi e modelli IA per difficoltà estreme.
* [x] Ottimizzazione della stabilità del filtro di movimento del cursore UDP.
* [ ] Salvataggio delle statistiche delle partite in un database
* [ ] Introduzione di nuovi mini-giochi nell'Hub di AirBound (es. Forza 4, Scacchi o giochi Arcade).


---

## 🔗 Repository Correlate

Il progetto AirBound è diviso in tre moduli principali:

1. **[AirBound (Questo Repository)](https://github.com/smattia-rizzo/AirBound)** - Il client di gioco principale sviluppato in Unity.
2. **[AirBound-pythonServer](https://github.com/smattia-rizzo/AirBound-pythonServer)** - Il server di Computer Vision in Python per il tracciamento della mano.
3. **[AirBound-ModelTrainingCamp](https://github.com/smattia-rizzo/AirBound-ModelTrainingCamp)** - L'ambiente di sviluppo e addestramento delle IA in TensorFlow.
