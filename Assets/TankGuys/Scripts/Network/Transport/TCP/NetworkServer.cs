using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

public class NetworkServer : INetworkServer
{
    private TcpListener listener;
    private List<TcpClient> clients = new List<TcpClient>();

    private bool isRunning = false;

    public Action<string, TcpClient> OnMessageReceived { get; set; }
    public Action<TcpClient> OnClientDisconnected { get; set; }

    public async Task Start(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        isRunning = true;

        while (isRunning)
        {
            TcpClient client;

            try
            {
                client = await listener.AcceptTcpClientAsync();
            }
            catch
            {
                break;
            }

            clients.Add(client);
            _ = HandleClient(client);
        }
    }

    private async Task HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();

        byte[] buffer = new byte[1024];
        StringBuilder data = new StringBuilder();

        try
        {
            while (isRunning)
            {
                int bytes = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytes == 0)
                {
                    break;
                }

                data.Append(Encoding.UTF8.GetString(buffer, 0, bytes));

                while (data.ToString().Contains("\n"))
                {
                    int index = data.ToString().IndexOf("\n");

                    string msg = data.ToString().Substring(0, index);
                    data.Remove(0, index + 1);

                    OnMessageReceived?.Invoke(msg, client);
                }
            }
        }
        catch
        {
        }

        OnClientDisconnected?.Invoke(client);

        clients.Remove(client);

        try { client.Close(); } catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}\n{ex.StackTrace}");
        }
    }

    public async Task Send(TcpClient client, string message)
    {
        if (client == null || !client.Connected) return;

        byte[] data = Encoding.UTF8.GetBytes(message + "\n");

        try
        {
            await client.GetStream().WriteAsync(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}\n{ex.StackTrace}");
        }
    }

    public async Task Broadcast(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\n");

        var clientsCopy = new List<TcpClient>(clients);

        foreach (var client in clientsCopy)
        {
            try
            {
                await client.GetStream().WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    public void Stop()
    {
        isRunning = false;

        try { listener?.Stop(); } catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}\n{ex.StackTrace}");
        }

        foreach (var client in clients)
        {
            try { client.Close(); } catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}\n{ex.StackTrace}");
        }
        }

        clients.Clear();
    }
}