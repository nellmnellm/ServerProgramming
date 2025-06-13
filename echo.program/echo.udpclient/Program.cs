using System.Net;
using System.Net.Sockets;
using System.Text;

namespace echo.udpclient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //사용자로부터 메세지를 입력받아 브로드캐스트
            // 1. 소켓 생성
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Parse("192.168.0.255"), 12345);
            byte[] buffer = new byte[1_024];

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            /*using UdpClient udpClient = new UdpClient(AddressFamily.InterNetwork);
            udpClient.EnableBroadcast = true;*/ // 소켓 대신 udpClient 를 써도 된다 

            while (true)
            {
                //콘솔 입력으로 메세지 받기
                Console.Write("> ");
                string message = Console.ReadLine();

                buffer = Encoding.UTF8.GetBytes(message);
                socket.SendTo(buffer, remoteEndpoint); // TCP와 다르게 endpoint 설정해줘야함. socket은 sendto, udpClient는 send로 가능
            }
        }
    }
}
