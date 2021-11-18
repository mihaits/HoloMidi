using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HoloMidiServer
{
    public class Program
    {
        private static UdpClient _udpServer;
        private static TcpListener _tcpListener;
        private static TcpClient _connectedTcpClient;
        private static NetworkStream _networkStream;

        private static readonly CancellationTokenSource Cancellation = new();
        private static bool _shouldExit;

        private static readonly MidiDevice MidiDevice = new();

        public static async Task Main()
        {
            StartTcpListener(Cancellation.Token);
            var tcpClientTask = ConnectTcpClient(_tcpListener, Cancellation.Token);

            while (!_shouldExit)
            {
                var message = Console.ReadLine();

                if (message == "exit")
                {
                    _shouldExit = true;
                    Cancellation.Cancel();
                    _udpServer.Close();
                }
                else if (_connectedTcpClient is {Connected: true})
                    SendMessage(message);
            }

            await tcpClientTask;
        }

        private static async Task ListenForUdpBroadcast()
        {
            _udpServer = new UdpClient(8888);

            Console.WriteLine("waiting for client UDP broadcast...");
            var udpReceiveResult = await _udpServer.ReceiveAsync();

            var clientRequestBytes = udpReceiveResult.Buffer;
            var clientRequest = Encoding.ASCII.GetString(clientRequestBytes);
            var clientEndPoint = udpReceiveResult.RemoteEndPoint;

            if (clientRequest == "HoloMidi?")
            {
                Console.WriteLine($"received UDP broadcast from client at {clientEndPoint.Address}, sending response...");
                var response = Encoding.ASCII.GetBytes("HoloMidi!");
                await _udpServer.SendAsync(response, response.Length, clientEndPoint);
            }

            _udpServer.Close();
        }

        public static void StartTcpListener(CancellationToken cancellationToken)
        {
            _tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"), 8052);
            _tcpListener.Start();

            Console.WriteLine("started TCP listener");
        }

        public static async Task ConnectTcpClient(TcpListener tcpListener, CancellationToken cancellationToken)
        {
            while (!_shouldExit)
            {
                try
                {
                    await ListenForUdpBroadcast();

                    Console.WriteLine("waiting for TCP client...");
                    _connectedTcpClient = await tcpListener.AcceptTcpClientAsync();
                    Console.WriteLine("TCP client connected");

                    await using (_networkStream = _connectedTcpClient.GetStream())
                    {
                        var buffer = new byte[1024];
                        var bufferSize = 0;

                        var receivedBytes = new byte[1024];
                        int receivedBytesSize;

                        while ((receivedBytesSize = await _networkStream.ReadAsync(receivedBytes.AsMemory(0, receivedBytes.Length), cancellationToken)) != 0)
                        {
                            Buffer.BlockCopy(receivedBytes, 0, buffer, bufferSize, receivedBytesSize);
                            bufferSize += receivedBytesSize;

                            while (bufferSize >= 3)
                            {
                                var message = new byte[3];
                                Buffer.BlockCopy(buffer, 0, message, 0, 3);

                                HandleMessage(message);

                                Buffer.BlockCopy(buffer, 3, buffer, 0, bufferSize - 3);
                                bufferSize -= 3;
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("\ndisconnecting TCP client");
                }
                catch (ObjectDisposedException) {}
                catch (Exception ex)
                {
                    Console.WriteLine($"\n{ex.GetType().Name} from {ex.TargetSite}: {ex.Message}");
                }
            }
        }

        private static void HandleMessage(byte[] incomingData) 
        {
            // protocol:
            // incomingData[0] - midi event type + channel (high + low)
            // midi event type
            //      0 - NoteOn 
            //      1 - NoteOff
            //      2 - Control Change
            //      3 - PitchBend
            // incomingData[1], byte[2] - data

            // incomingData[0] == 255 - disconnect

            if (incomingData[0] == 255)
            {
                Cancellation.Cancel();
                return;
            }

            var type = (byte) (incomingData[0] >> 4);
            var channel = (byte) (incomingData[0] & 15);

            switch (type)
            {
                case 0:
                    MidiDevice.NoteOn(channel, incomingData[1], incomingData[2]);
                    break;
                case 1:
                    MidiDevice.NoteOff(channel, incomingData[1]);
                    break;
                case 2:
                    MidiDevice.ControlChange(channel, incomingData[1], incomingData[2]);
                    break;
                case 3:
                    MidiDevice.PitchBend(channel, BitConverter.ToUInt16(incomingData, 1));
                    break;
            }
        }

        private static async void SendMessage(string message)
        {
			try
			{
                if (_networkStream.CanWrite)
                {
                    var serverMessageAsByteArray = Encoding.ASCII.GetBytes(message);
                    await _networkStream.WriteAsync(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                    Console.WriteLine("message sent");
                }
			}
			catch (SocketException socketException)
			{
                Console.WriteLine("SocketException: " + socketException.Message);
			}
		}
	}
}
