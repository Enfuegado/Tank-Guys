using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class NetworkClient : INetworkClient
{
    private TcpClient client;
    private NetworkStream stream;

    public bool IsConnected { get; private set; }

    public Action<string> OnMessageReceived { get; set; }

    public async Task Connect(string ip, int port)
    {
        client = new TcpClient();
        await client.ConnectAsync(ip, port);

        stream = client.GetStream();
        IsConnected = true;

        _ = ReceiveLoop();
    }

    private async Task ReceiveLoop()
    {
        byte[] buffer = new byte[1024];
        StringBuilder data = new StringBuilder();

        try
        {
            while (IsConnected)
            {
                int bytes = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytes <= 0)
                    break;

                data.Append(Encoding.UTF8.GetString(buffer, 0, bytes));

                while (data.ToString().Contains("\n"))
                {
                    int index = data.ToString().IndexOf("\n");

                    string msg = data.ToString().Substring(0, index);
                    data.Remove(0, index + 1);

                    OnMessageReceived?.Invoke(msg);
                }
            }
        }
        catch
        {

        }

        Disconnect();
    }

    public async Task Send(string message)
    {
        if (!IsConnected) return;

        byte[] data = Encoding.UTF8.GetBytes(message + "\n");
        await stream.WriteAsync(data, 0, data.Length);
    }

    public void Disconnect()
    {
        IsConnected = false;
        stream?.Close();
        client?.Close();
    }
}