using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Events; 
using TMPro;

public class TelnetClient : MonoBehaviour
{
    [Header("Conexión")]
    public string host = "127.0.0.1"; // reemplaza por la IP de tu PC
    public int port = 5000; // reemplaza por el puerto de tu PC

    public TMP_Dropdown portDropdown;

    ConcurrentQueue<UnityEvent> eventQueue = new ConcurrentQueue<UnityEvent>();

    [Header("Reconnect")]
    public bool autoReconnect = true;
    public float reconnectDelay = 3f;

    public UnityEvent<string> OnDataReceived;
    
    public UnityEvent OnDeviceChanged; // inspector: asigna función para mostrar texto
    public UnityEvent OnConnected;
    public UnityEvent OnDisconnected;

    TcpClient client;
    NetworkStream stream;
    Thread readThread;
    volatile bool running = false;

    // Cola de mensajes que se procesan desde el hilo principal
    ConcurrentQueue<string> mainThreadQueue = new ConcurrentQueue<string>();

    void Start()
    {
        // Start connection attempt
        Debug.Log("Ruta: " + Application.persistentDataPath);
        portDropdown.onValueChanged.AddListener(OnPortChanged);
        host = ConfigManager.Instance.Config.ip;
        Connect();
    }

    public void Connect()
    {
        if (running) return;
        running = true;
        readThread = new Thread(ConnectionLoop) { IsBackground = true };
        readThread.Start();
    }

    public void ChangePort(int newPort)
    {
        port = newPort;

        // Solo forzar reconexión
        SafeClose();
    }

    void OnPortChanged(int index)
    {
        int selectedText = 0;
        switch (portDropdown.options[index].text)
        {
            case "Router-1":
                selectedText = ConfigManager.Instance.Config.puerto1;
                break;
            case "PC1":
                selectedText = ConfigManager.Instance.Config.puerto2;
                break;
            case "PC2":
                selectedText = ConfigManager.Instance.Config.puerto3;
                break;
            case "PC3":
                selectedText = ConfigManager.Instance.Config.puerto4;
                break;
            // Agrega más casos según tus opciones
        }
        int newPort = selectedText;

        ChangePort(newPort);
    }

    void ConnectionLoop()
    {
        while (running)
        {
            try
            {
                client = new TcpClient();
                var result = client.BeginConnect(host, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));
                if (!success)
                {
                    EnqueueMain($"[Telnet] No se pudo conectar a {host}:{port}");
                    client.Close();
                    if (!autoReconnect) break;
                    Thread.Sleep((int)(reconnectDelay * 1000));
                    continue;
                }

                client.EndConnect(result);
                stream = client.GetStream();
                EnqueueMain($"[Telnet] Conectado a {host}:{port}");
                EnqueueEvent(OnConnected);

                // lectura
                byte[] buffer = new byte[4096];
                while (client != null && client.Connected && running)
                {
                    if (!stream.DataAvailable)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read == 0) break;
                    string s = Encoding.ASCII.GetString(buffer, 0, read);
                    EnqueueMain(s);
                }
            }
            catch (Exception ex)
            {
                EnqueueMain($"[Telnet] Excepción: {ex.Message}");
            }
            finally
            {
                SafeClose();
                EnqueueEvent(OnDisconnected);
            }

            if (!autoReconnect) break;
            Thread.Sleep((int)(reconnectDelay * 1000));
        } // while
    }

    public void Send(string text)
    {
        try
        {
            if (client == null || !client.Connected)
            {
                EnqueueMain("[Telnet] No conectado.");
                return;
            }
            byte[] data = Encoding.ASCII.GetBytes(text + "\r\n");
            stream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            EnqueueMain($"[Telnet] Error enviando: {ex.Message}");
    }
    }



    void SafeClose()
    {
        try { stream?.Close(); } catch { }
        try { client?.Close(); } catch { }
        client = null;
        stream = null;
        EnqueueEvent(OnDeviceChanged);
    }

    void EnqueueMain(string msg)
    {
        mainThreadQueue.Enqueue(msg);
    }

    void EnqueueEvent(UnityEvent ev)
    {
        if (ev != null)
            eventQueue.Enqueue(ev);
    }

    void Update()
    {
        // Procesar texto
        while (mainThreadQueue.TryDequeue(out string s))
        {
            OnDataReceived?.Invoke(s);
        }

        // 🔥 Procesar eventos correctamente
        while (eventQueue.TryDequeue(out UnityEvent ev))
        {
            ev?.Invoke();
        }
    }

    void OnApplicationQuit()
    {
        running = false;
        try
        {
            readThread?.Join(500);
        }
        catch { }
        SafeClose();
    }
}
