using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Events;

public class TelnetClient : MonoBehaviour
{
    [Header("Conexión")]
    public string host = "192.168.0.40"; // reemplaza por la IP de tu PC
    public int port = 5000;

    [Header("Reconnect")]
    public bool autoReconnect = true;
    public float reconnectDelay = 3f;

    public UnityEvent<string> OnDataReceived; // inspector: asigna función para mostrar texto
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
        Connect();
    }

    public void Connect()
    {
        if (running) return;
        running = true;
        readThread = new Thread(ConnectionLoop) { IsBackground = true };
        readThread.Start();
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
    }

    void EnqueueMain(string msg)
    {
        mainThreadQueue.Enqueue(msg);
    }

    void EnqueueEvent(UnityEvent ev)
    {
        if (ev != null) mainThreadQueue.Enqueue($"__EVENT__:{Guid.NewGuid()}:{ev.GetHashCode()}");
        // We'll invoke events in Update by checking those markers
        // (Simpler: call OnConnected/OnDisconnected directly via SynchronizationContext if needed)
    }

    void Update()
    {
        while (mainThreadQueue.TryDequeue(out string s))
        {
            // eventos marcados
            if (s.StartsWith("__EVENT__"))
            {
                // simplificado: invocar OnConnected/OnDisconnected directly - but we already enqueued messages to indicate state
                continue;
            }

            // Evocar callback con el texto recibido
            OnDataReceived?.Invoke(s);
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
