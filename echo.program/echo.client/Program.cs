using System.Net;
using System.Net.Sockets;
using System.Text;

namespace echo.client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. 소켓 생성
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 2. connect() 

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.8"), 12345);
            socket.Connect(serverEndPoint);

            Console.WriteLine($"Server[{serverEndPoint.Address}] connected");

            byte[] buffer = new byte [1_024];
            // 
            while (true)
            {
                //콘솔 입력으로 메세지 받기
                Console.Write("> ");
                string message = Console.ReadLine();

                if (message == "<END>")
                {
                    //socket.Shutdown(SocketShutdown.Both); //Fin 세그먼트를 보내지만 소켓 파일을 받지 않는것.
                    socket.Close();   //Shutdown() 후 정상적으로 연결이 해지되면 소켓 파일도 닫음.
                    break; 
                }
                buffer = Encoding.UTF8.GetBytes(message);
                socket.Send(buffer);

                //서버로부터 에코잉 된 메세지를 수신한다.
                int receviedBytes = socket.Receive(buffer);
                //buffer 저장된 데이터는 UTF-8 문자열
                message = Encoding.UTF8.GetString(buffer, 0, receviedBytes);
                Console.WriteLine($"From server : {message}");
            }
        }
    }
}
