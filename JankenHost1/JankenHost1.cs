using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace JankenHost1
{
    class S
    {
        public static void Main()
        {
            Console.WriteLine("JankenHost1");
            SocketServer();
            Console.ReadKey();
        }

        public static void SocketServer()
        {
            //ここからIPアドレスやポートの設定
            // 着信データ用のデータバッファー。
            byte[] bytes = new byte[1024];
            string hostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            //ここまでIPアドレスやポートの設定

            //ソケットの作成
            //TCP/IPやUDPといったプロトコルを使ってネットワーク通信を行うためのインターフェース
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //通信の受け入れ準備
            listener.Bind(localEndPoint);
            listener.Listen(10);

            //通信の確立
            Socket handler = listener.Accept();

            // クライアントのIPアドレスを取得して出力
            IPEndPoint? remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
            if (remoteIpEndPoint != null)
            {
                Console.WriteLine($"接続元のIPアドレス: {remoteIpEndPoint.Address}");
            }


            // 任意の処理
            //データの受取をReceiveで行う。
            int bytesRec = handler.Receive(bytes);
            string data1 = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            Console.WriteLine(data1);

            //返すデータを作成する。
            var sendData = "じゃんけんゲーム！\r\n0:ぐう　1:ちょき　2:ぱあ\r\n";

            //クライアントにSendで返す。
            byte[] msg = Encoding.UTF8.GetBytes(sendData);
            handler.Send(msg);
            // じゃんけんの手を受信
            //データの受取をReceiveで行う。
            bytesRec = handler.Receive(bytes);
            data1 = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            Console.WriteLine(data1);

            // ホストの手を出す
            Random random = new Random();
            int hostHand = random.Next(3);

            string hostHandString = hostHand switch
            {
                0 => "ぐう",
                1 => "ちょき",
                2 => "ぱあ",
                _ => "不明"
            };
            var hostHandData = $"ホストの手: {hostHandString}\r\n";
            Console.WriteLine(hostHandData);
            // 勝敗の判定
            string result;
            int clientHand;
            if (int.TryParse(data1.Substring(0, 1), out clientHand))
            {
                if (hostHand == clientHand)
                {
                    result = "あいこ";
                }
                else if ((hostHand + 1) % 3 == clientHand)
                {
                    result = "あなたの負け";
                }
                else
                {
                    result = "あなたの勝ち";
                }
            }
            else
            {
                result = "無効な手が入力されました。";
            }
            msg = Encoding.UTF8.GetBytes($"{hostHandData}\n{result}");
            handler.Send(msg);

            //ソケットの終了
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            //クライアントからの接続を終了する。
        }
    }
}