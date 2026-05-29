using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MediapipeUDPClient : MonoBehaviour
{
    [System.Serializable]
    public class MediaPipeData
    {
        public float x;
        public float y;
        public int pizzico;
    }

    // Configurazione Rete
    private UdpClient client;
    private Thread receiveThread;
    public int port = 5005;

    private string lastReceivedString = "";
    private bool hasNewData = false;
    private readonly object lockObject = new object();

    [Header("Riferimenti UI")]
    public RectTransform mirinoTransform; // Oggetto UI che rappresenta il mirino (es. un'immagine)
    public GraphicRaycaster uiRaycaster;  // Canvas principale della GUI
    public EventSystem eventSystem;       // Gestisce gli eventi UI

    public TMP_Text feedbackVisivoAttivazione;

    [Header("Filtri")]
    [Range(0.01f, 1f)] public float smoothness = 0.15f;

    private Vector2 targetScreenPos;
    private Vector2 currentScreenPos;
    private bool lastPizzicoState = false;

    void Start()
    {
        if (eventSystem == null) eventSystem = EventSystem.current;

        currentScreenPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        targetScreenPos = currentScreenPos;

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    public bool Enabled { get; private set; } = false;

    public void ToggleMediapipe()
    {
        Enabled = !Enabled;
        if (feedbackVisivoAttivazione != null)
        {
            feedbackVisivoAttivazione.text = Enabled ? "Mediapipe: ON" : "Mediapipe: OFF";
            feedbackVisivoAttivazione.color = Enabled ? Color.green : Color.red;
        }
    }

    void Update()
    {

        if (!Enabled) return;
        bool currentPizzico = lastPizzicoState;

        if (hasNewData)
        {
            string jsonString;
            lock (lockObject)
            {
                jsonString = lastReceivedString;
                hasNewData = false;
            }

            try
            {
                MediaPipeData data = JsonUtility.FromJson<MediaPipeData>(jsonString);

                // Conversione coordinate normalizzate (0-1) ai pixel dello schermo di gioco
                float targetX = data.x * Screen.width;
                float targetY = (1 - data.y) * Screen.height;
                targetScreenPos = new Vector2(targetX, targetY);

                currentPizzico = data.pizzico == 1;
            }
            catch (Exception e) { Debug.LogWarning("Errore parsing: " + e.Message); }
        }

        // Interpolazione movimento
        currentScreenPos = Vector2.Lerp(currentScreenPos, targetScreenPos, smoothness);

        // Muove lo sprite convertendo le coordinate schermo in coordinate locali della UI
        if (mirinoTransform != null && mirinoTransform.parent != null)
        {
            // Determina la telecamera corretta. Se il Canvas è Overlay, è null. Se è Camera, usa main camera.
            Camera uiCamera = uiRaycaster.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;

            // Converte la posizione dello schermo nella posizione relativa al genitore del mirino
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)mirinoTransform.parent,
                currentScreenPos,
                uiCamera,
                out Vector2 localPoint
            );

            // Applica la nuova posizione
            mirinoTransform.localPosition = localPoint;
        }


        // Rileva il Click (Pizzico appena attivato)
        if (currentPizzico && !lastPizzicoState)
        {
            EseguiUIRaycast(currentScreenPos);
        }

        lastPizzicoState = currentPizzico;
    }

    void EseguiUIRaycast(Vector2 screenPos)
    {
        if (uiRaycaster == null || eventSystem == null) return;

        // Configura i dati del puntatore simulato per la UI
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = screenPos;

        // Lista in cui verranno salvati tutti gli elementi UI colpiti
        List<RaycastResult> risultati = new List<RaycastResult>();

        // Spara il raggio sulla UI
        uiRaycaster.Raycast(pointerData, risultati);

        if (risultati.Count > 0)
        {
            // Il primo elemento della lista [0] è l'oggetto UI più in alto sotto il mirino
            GameObject oggettoColpito = risultati[0].gameObject;
            Debug.Log($"[UI] Colpito elemento: {oggettoColpito.name}");

            // Cerca se l'elemento (o i suoi genitori) ha un bottone o dropdown da cliccare
            Button bottone = oggettoColpito.GetComponentInParent<Button>();
            Dropdown dropdown = oggettoColpito.GetComponentInParent<Dropdown>();
            TMPro.TMP_Dropdown tmproDropdown = oggettoColpito.GetComponentInParent<TMPro.TMP_Dropdown>(); // Per TextMeshPro

            if (bottone != null && bottone.interactable)
            {
                bottone.onClick.Invoke();
            }
            else if (dropdown != null)
            {
                dropdown.Show();
            }
            else if (tmproDropdown != null)
            {
                tmproDropdown.Show();
            }
        }
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);
        while (true)
        {
            try
            {
                byte[] dataBytes = client.Receive(ref anyIP);
                lock (lockObject)
                {
                    lastReceivedString = Encoding.UTF8.GetString(dataBytes);
                    hasNewData = true;
                }
            }
            catch (Exception) { }
        }
    }

    private void OnApplicationQuit()
    {
        if (receiveThread != null && receiveThread.IsAlive) receiveThread.Abort();
        if (client != null) client.Close();
    }
}
