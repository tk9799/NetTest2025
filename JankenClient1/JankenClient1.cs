using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace JankenClient1
{
    class C
    {
        public static void Main()
        {
            //今回送るHello World!
            string st = "じゃんけんしたい！";
            Console.WriteLine("JankenClient1");
            SocketClient(st);
            Console.ReadKey();
        }


        public static void SocketClient(string st)
        {
            //IPアドレスやポートを設定(自PC、ポート:11000）
            string hostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            //外部を指定する場合
            // IPAddress ipAddress = IPAddress.Parse("172.25.91.135");
            // IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            //ソケットを作成
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //接続する。失敗するとエラーで落ちる。
            try
            {
                socket.Connect(remoteEP);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Connect Faild{e.ToString()}");
                return;
            }
            //Sendで送信している。
            byte[] msg = Encoding.UTF8.GetBytes(st + "<EOF>");
            socket.Send(msg);

            //Receiveで受信している。
            byte[] bytes = new byte[1024];
            int bytesRec = socket.Receive(bytes);
            string data1 = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            Console.WriteLine(data1);

            // 入力した文字列を送信する
            string userInput = Console.ReadLine();
            // 入力した文字列を送信
            msg = Encoding.UTF8.GetBytes(userInput + "<EOF>");
            socket.Send(msg);

            //Receiveで受信している。
            bytes = new byte[1024];
            bytesRec = socket.Receive(bytes);
            data1 = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            Console.WriteLine(data1);



            //ソケットを終了している。
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}

