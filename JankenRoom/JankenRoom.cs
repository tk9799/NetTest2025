using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace JankenRoom
{
    class S
    {
        public static void Main()
        {
            Console.WriteLine("JankenRoom");
            SocketServer();
            Console.ReadKey();
        }

        public static void SocketServer()
        {
            // IPアドレスやポートの設定
            byte[] bytes = new byte[1024];
            string hostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // ソケットの作成
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);

            Console.WriteLine("クライアントの接続を待っています...");

            // クライアント2つの接続を待つ
            Socket client1 = listener.Accept();
            Console.WriteLine("クライアント1が接続しました。");
            int bytesRec1 = client1.Receive(bytes);
            Console.WriteLine($"{Encoding.UTF8.GetString(bytes, 0, bytesRec1)}");
            Socket client2 = listener.Accept();
            Console.WriteLine("クライアント2が接続しました。");
            int bytesRec2 = client2.Receive(bytes);
            Console.WriteLine($"{Encoding.UTF8.GetString(bytes, 0, bytesRec2)}");

            // クライアントにじゃんけんのメッセージを送信
            string sendData = "じゃんけんゲーム！\r\n0:ぐう　1:ちょき　2:ぱあ\r\n";
            byte[] msg = Encoding.UTF8.GetBytes(sendData);
            client1.Send(msg);
            client2.Send(msg);

            // クライアント1の手を受信
            bytesRec1 = client1.Receive(bytes);
            string client1HandStr = Encoding.UTF8.GetString(bytes, 0, bytesRec1);
            Console.WriteLine($"クライアント1の手: {client1HandStr}");

            // クライアント2の手を受信
            bytesRec2 = client2.Receive(bytes);
            string client2HandStr = Encoding.UTF8.GetString(bytes, 0, bytesRec2);
            Console.WriteLine($"クライアント2の手: {client2HandStr}");

            // 勝敗の判定
            string result1, result2;
            if (int.TryParse(client1HandStr.Substring(0, 1), out int client1Hand) &&
                int.TryParse(client2HandStr.Substring(0, 1), out int client2Hand))
            {
                if (client1Hand == client2Hand)
                {
                    result1 = result2 = "あいこ";
                }
                else if ((client1Hand + 1) % 3 == client2Hand)
                {
                    result1 = "クライアント1の勝ち";
                    result2 = "クライアント2の負け";
                }
                else
                {
                    result1 = "クライアント1の負け";
                    result2 = "クライアント2の勝ち";
                }
            }
            else
            {
                result1 = result2 = "無効な手が入力されました。";
            }

            // 結果をクライアントに送信
            client1.Send(Encoding.UTF8.GetBytes($"結果: {result1}\r\n"));
            client2.Send(Encoding.UTF8.GetBytes($"結果: {result2}\r\n"));

            // ソケットの終了
            client1.Shutdown(SocketShutdown.Both);
            client1.Close();
            client2.Shutdown(SocketShutdown.Both);
            client2.Close();
            listener.Close();
        }
    }
}
