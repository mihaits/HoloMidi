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
        private static TcpClient _connectedTcpClient;
        private static NetworkStream _networkStream;

        private static readonly MidiDevice MidiDevice = new();

        public static async Task Main()
        {
            var cancellation = new CancellationTokenSource();

            var tcpListenerTask = ListenForTcpClient(cancellation.Token);

            _ = ListenForUdpBroadcast();

            while (!cancellation.Token.IsCancellationRequested)
            {
                var message = Console.ReadLine();

                if (message == "exit")
                    cancellation.Cancel();
                else if (_connectedTcpClient is {Connected: true})
                    SendMessage(message);
            }

            await tcpListenerTask;
        }

        private static async Task ListenForUdpBroadcast()
        {
            var udpServer = new UdpClient(8888);

            Console.WriteLine("waiting for client UDP broadcast...");
            var udpReceiveResult = await udpServer.ReceiveAsync();

            var clientRequestBytes = udpReceiveResult.Buffer;
            var clientRequest = Encoding.ASCII.GetString(clientRequestBytes);
            var clientEndPoint = udpReceiveResult.RemoteEndPoint;

            Console.WriteLine($"received {clientRequest} from {clientEndPoint.Address}");

            if (clientRequest == "HoloMidi?")
            {
                Console.WriteLine("sending response");
                var response = Encoding.ASCII.GetBytes("HoloMidi!");
                await udpServer.SendAsync(response, response.Length, clientEndPoint);
            }
        }

        public static async Task ListenForTcpClient(CancellationToken cancellationToken)
        {
            var listener = new TcpListener(IPAddress.Parse("0.0.0.0"), 8052);
            listener.Start();
            cancellationToken.Register(listener.Stop);

            try
            {
                Console.WriteLine("starting TCP listener");

                _connectedTcpClient = await listener.AcceptTcpClientAsync();
                
                Console.WriteLine("client connected");

                await using (_networkStream = _connectedTcpClient.GetStream())
                {
                    var bytes = new byte[1024];
                    while (await _networkStream.ReadAsync(bytes.AsMemory(0, bytes.Length), cancellationToken)
                           != 0)
                    {
                        HandleMessage(bytes);
                        // var clientMessage = Encoding.ASCII.GetString(bytes);
                        // Console.WriteLine("received message from client: " + clientMessage);
                    }
                }
                
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("disconnecting client and stopping TCP listener");
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.SocketErrorCode == SocketError.OperationAborted
                    ? "stopping TCP listener"
                    : $"SocketException: {e.SocketErrorCode}. Message: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().Name} from {ex.TargetSite}: {ex.Message}");
            }
        }

        private static void HandleMessage(byte[] incomingData) 
        {
            // protocol:
            // byte[0] - midi event type + channel (high + low)
            // midi event type
            //      0 - NoteOn 
            //      1 - NoteOff
            //      2 - Control Change
            //      3 - PitchBend
            // byte[1], byte[2], ... - data

            // Console.WriteLine("received: " + incomingData[0]);

            var type = (byte) (incomingData[0] >> 4);
            var channel = (byte) (incomingData[0] & 15);

            switch (type)
            {
                case 0:
                    MidiDevice.NoteOn(channel, incomingData[1], incomingData[2]);
                    Console.WriteLine("received note on");
                    break;
                case 1:
                    MidiDevice.NoteOff(channel, incomingData[1]);
                    Console.WriteLine("received note off");
                    break;
                case 2:
                    MidiDevice.ControlChange(channel, incomingData[1], incomingData[2]);
                    Console.WriteLine("received control change");
                    break;
                case 3:
                    MidiDevice.PitchBend(channel, BitConverter.ToUInt16(incomingData, 1));
                    Console.WriteLine($"received pitch bend: {BitConverter.ToUInt16(incomingData, 1)}");
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
