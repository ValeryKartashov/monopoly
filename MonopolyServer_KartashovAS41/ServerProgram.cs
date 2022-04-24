using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MonopolyServer
{
    public class ClientObject
    {
        public static byte[] LogToReceive = new byte[256];
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server;
        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }
        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                string message = GetMessage();
                userName = message;
                message = userName + " присоединился к серверу";
                Console.WriteLine(message);
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        if (message.Contains("Результаты хода красного игрока: "))
                        {
                            Console.WriteLine("Красный игрок завершил ход");
                            Console.WriteLine("Ход переходит к синему игроку");
                            server.BroadcastMessage(message, this.Id);
                        }
                        else if (message.Contains("Результаты хода синего игрока: "))
                        {
                            Console.WriteLine("Синий игрок завершил ход");
                            Console.WriteLine("Ход переходит к красному игроку");
                            server.BroadcastMessage(message, this.Id);
                        }
                        else if (/*message.Contains("Результаты") || */message.Contains("Рента"))
                        /*message.Contains("Результаты хода красного игрока") ||
                        message.Contains("Результаты хода синего игрока") ||
                        message.Contains("Рента красному игроку") ||
                        message.Contains("Рента синему игроку")*/
                        {
                            Console.WriteLine(message);
                            server.BroadcastMessage(message, this.Id);
                        }
                        else
                        {
                            message = String.Format("{0}: {1}", userName, message);
                            Console.WriteLine(message);
                            server.BroadcastMessage(message, this.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        message = String.Format("{0} покинул сервер", userName);
                        Console.WriteLine(message);
                        //Console.WriteLine(ex.Message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
    public class ServerObject
    {
        static TcpListener tcpListener;
        List<ClientObject> clients = new List<ClientObject>();
        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id)
                    clients[i].Stream.Write(data, 0, data.Length);
            }
        }
        protected internal void Disconnect()
        {
            tcpListener.Stop();

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            Environment.Exit(0);
        }
    }
    class ServerProgram
    {
        static ServerObject server;
        static Thread listenThread;
        static void Main(string[] args)
        {
            try
            {
                #region Украшение консоли
                Console.Title = "Сервер TCP";
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Выполнил студент группы АС-41 Карташов Валерий Сергеевич.");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("\n");
                #endregion
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}