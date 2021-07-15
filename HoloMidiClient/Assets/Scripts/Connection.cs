using System;
using System.Net;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.Events;

public class Connection : MonoBehaviour
{
	private TcpClient _socketConnection;
    private NetworkStream _networkStream;

    private CancellationTokenSource _cancellation;

    private string serverIP;

    public UnityEvent OnConnected;
    public UnityEvent OnServerLookupTimeout;

    public async void ConnectToServer()
    {
        if (await UdpBroadcast())
            _ = ListenForData(_cancellation.Token);
    }

	public void Start()
    {
        _cancellation = new CancellationTokenSource();
        
        ConnectToServer();
    }

    private async Task<bool> UdpBroadcast()
	{
        var serverWasFound = false;

		var udpClient = new UdpClient
        {
            EnableBroadcast = true
        };

        Debug.Log("broadcasting request for server\n");
        var requestBytes = Encoding.ASCII.GetBytes("HoloMidi?");
		await udpClient.SendAsync(requestBytes, requestBytes.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

		Debug.Log("waiting for response\n");
        var udpReceiveTask = udpClient.ReceiveAsync();

        if (await Task.WhenAny(udpReceiveTask, Task.Delay(1000)) == udpReceiveTask)
        {
            var serverResponseBytes = udpReceiveTask.Result.Buffer;
            var serverResponse = Encoding.ASCII.GetString(serverResponseBytes);

			if (serverResponse == "HoloMidi!")
			{
				var serverEndPoint = udpReceiveTask.Result.RemoteEndPoint;
                Debug.Log($"found HoloMidi server at {serverEndPoint.Address}\n");
                serverIP = serverEndPoint.Address.ToString();

                serverWasFound = true;
				OnConnected.Invoke();
            }
		}
        else
        {
            Debug.Log("timeout when searching for server\n");
            OnServerLookupTimeout.Invoke();
        }

        udpClient.Close();

        return serverWasFound;
    }
    
	private async Task ListenForData(CancellationToken cancellationToken)
	{
		try
		{
			_socketConnection = new TcpClient(serverIP, 8052);

            Debug.Log("connected to server");

			var bytes = new byte[1024];
			using (_networkStream = _socketConnection.GetStream())
			{
				int length;
                
				while ((length = await _networkStream.ReadAsync(bytes, 0, bytes.Length, cancellationToken)) != 0)
				{
					var incomingData = new byte[length];
					Array.Copy(bytes, 0, incomingData, 0, length);

					var serverMessage = Encoding.ASCII.GetString(incomingData);
                    Debug.Log("received message from server: " + serverMessage + "\n");
                }
            }
		}
        catch (OperationCanceledException)
        {
            Debug.Log("closing server connection\n");

			_socketConnection.Close();
		}
		catch (Exception e)
		{
            Debug.Log($"{e.GetType().Name}: {e.Message }");
		}
	}

	private void SendMessage(byte[] bytes)
    {
        if (_socketConnection == null) return;

		try
		{
			if (_networkStream.CanWrite)
			{
                _networkStream.Write(bytes, 0, bytes.Length);
			}
		}
		catch (SocketException socketException)
		{
            Debug.Log("Socket exception: " + socketException + "\n");
		}
	}

    public void SendNoteOn(int note)
    {
        SendMessage(new byte[] {0, (byte) note});
    }

    public void SendNoteOff(int note)
    {
        SendMessage(new byte[] { 1, (byte) note });
    }

    public void SendControlChange(SliderEventData sliderEventData)
    {
        SendMessage(new byte[] {2, 10, (byte) (int) (sliderEventData.NewValue * 127)});
    }

    public void OnDestroy()
    {
		_cancellation.Cancel();
    }
}
