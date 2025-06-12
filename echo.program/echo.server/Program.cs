using System.Net;
using System.Net.Sockets;
using System.Text;

namespace echo.server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. 소켓을 생성한다
            // 파일을 열고 닫는 권한을 OS에 요청해야함
            // IDisposable 사용 : socket.close(); . 자원을 관리할수 있게 되지만 
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // IDisposable 을 사용한 뒤 using 을 사용하면 자동 해제됨
            // using문 사용해서 안전하게 프로그래밍. 

            // 2. 서버 주소(소켓 주소 (ex:192.168.0.0 : 8080)
            // 를 소켓에 바인딩한다
                                                        //와일드카드 번호 : 0,0,0,0     / 포트번호 : 12345 (known 번호 x )
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 12345);  //0.0.0.0 :12345 라는 소켓주소가 생김
            socket.Bind(serverEndPoint);

            // 3 . 클라이언트의 접속을 대기.
            socket.Listen();

            Console.WriteLine("Server started."); //서버를 시작
            try
            {
                // 4. 클라이언트의 연결을 수락한다.
                using Socket session = socket.Accept(); // 3way Handshake가 끝나면 Accept 종료. //실제 클라이언트와 통신하는 부분을 만듬
                if (IsBlackList(session.RemoteEndPoint as IPEndPoint))
                {
                    Console.WriteLine($"{session.RemoteEndPoint} is blocked.");
                }
                Console.WriteLine("Connection established"); //연결 성립.
                // 5. 클라이언트로부터 데이터를 받는다.
                // 5-1 : 수신받을 byte 버퍼를 생성.
                byte[] buffer = new byte[1_024];

                while (true)
                {
                    int receviedBytes = session.Receive(buffer);

                    if (receviedBytes == 0)
                    {
                        break;
                    }
                    //buffer 저장된 데이터는 UTF-8 문자열
                    string message = Encoding.UTF8.GetString(buffer, 0, receviedBytes);
                  
                    Console.WriteLine($"[Received] : {message}");

                    // 6. 클라이언트에게 에코잉한다.
                    // Span = 연속된 메모리의 부분. 읽기 전용.
                    session.Send(new ReadOnlySpan<byte>(buffer, 0, receviedBytes));
                    //고급 =. Span : 연속된 메모리의 부분. 읽기 전용
                    //가변 객체보다 불변 객체를 선호하라. => 버그 발생하면 가변 객체에서 발생한 것임.

                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Error : {e.ToString()}");
            }
            

        }
        static List<IPAddress> s_blackList = new List<IPAddress>();
        
        static bool IsBlackList(IPEndPoint remoteEndPoint)
        {
            foreach (IPAddress ip in s_blackList)
            {
                if (ip.Equals(remoteEndPoint.Address))
                    return true;
            }
            return false;    
        }
    }
}
