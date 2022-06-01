using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MonopolyServer
{
    public class ClientObject
    {
        private readonly TcpClient Client;
        private readonly ServerObject server;
        private string userName;
        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            Client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }
        protected internal string Id { get; }
        protected internal NetworkStream Stream { get; private set; }
        public void Process()
        {
            try
            {
                Stream = Client.GetStream();
                while (true)
                {
                    var message = GetMessage();
                    switch (message)
                    {
                        case "Оба подключились":
                        {
                            server.SendMessageToEveryone(message, Id);
                            Program.f.tbLog.Invoke((MethodInvoker)delegate
                            {
                                Program.f.tbLog.Text += "[" + DateTime.Now + "] " + message + Environment.NewLine;
                            });
                            break;
                        }
                        case "Red":
                        {
                            Taken.Red = true;
                            userName = message;
                            Program.f.tbLog.Invoke((MethodInvoker)delegate
                            {
                                Program.f.tbLog.Text += "[" + DateTime.Now + "] " + userName + " присоединился к серверу" + Environment.NewLine;
                            });
                            server.SendMessageToOpponentClient(userName + " присоединился к серверу", Id);
                            break;
                        }
                        case "Blue":
                        {
                            Taken.Blue = true;
                            userName = message;
                            Program.f.tbLog.Invoke((MethodInvoker)delegate
                            {
                                Program.f.tbLog.Text += "[" + DateTime.Now + "] " + userName + " присоединился к серверу" + Environment.NewLine;
                            });
                            server.SendMessageToOpponentClient(userName + " присоединился к серверу", Id);
                            break;
                        }
                        case "Кто-то присоединился к серверу":
                        {
                            Program.f.tbLog.Invoke((MethodInvoker)delegate
                            {
                                Program.f.tbLog.Text += "[" + DateTime.Now + "] " + message + Environment.NewLine;
                            });
                            if (Taken.Red) server.SendMessageToSender("Красный занят", Id);
                            if (Taken.Blue) server.SendMessageToSender("Синий занят", Id);
                            break;
                        }
                        case "Красный занят":
                        {
                            server.SendMessageToOpponentClient(message, Id);
                            Program.f.tbLog.Invoke((MethodInvoker)delegate
                            {
                                Program.f.tbLog.Text += "[" + DateTime.Now + "] " + message + Environment.NewLine;
                            });
                            break;
                        }
                        case "Синий занят":
                        {
                            server.SendMessageToOpponentClient(message, Id);
                            Program.f.tbLog.Invoke((MethodInvoker)delegate
                            {
                                Program.f.tbLog.Text += "[" + DateTime.Now + "] " + message + Environment.NewLine;
                            });
                            break;
                        }
                        case "Red покидает сервер" when userName is "Red":
                        {
                            Program.f.tbLog.Invoke((MethodInvoker)delegate
                            {
                                Program.f.tbLog.Text += "[" + DateTime.Now + "] " + message + Environment.NewLine;
                            });
                            server.RemoveConnection(this.Id);
                            break;
                        }
                        case "Blue покидает сервер" when userName is "Blue":
                        {
                            Program.f.tbLog.Invoke((MethodInvoker)delegate
                            {
                                Program.f.tbLog.Text += "[" + DateTime.Now + "] " + message + Environment.NewLine;
                            });
                            server.RemoveConnection(this.Id);
                            break;
                        }
                    }
                    if (message.Contains("Результаты хода красного"))
                    {
                        Program.f.tbLog.Invoke((MethodInvoker)delegate
                        {
                            Program.f.tbLog.Text += "[" + DateTime.Now + "] " + "Красный игрок завершил ход" + Environment.NewLine;
                            Program.f.tbLog.Text += "[" + DateTime.Now + "] " + "Ход переходит к синему игроку" + Environment.NewLine;
                        });
                        server.SendMessageToOpponentClient(message, Id);
                    }
                    if (message.Contains("Результаты хода синего"))
                    {
                        Program.f.tbLog.Invoke((MethodInvoker)delegate
                        {
                            Program.f.tbLog.Text += "[" + DateTime.Now + "] " + "Синий игрок завершил ход" + Environment.NewLine;
                            Program.f.tbLog.Text += "[" + DateTime.Now + "] " + "Ход переходит к красному игроку" + Environment.NewLine;
                        });
                        server.SendMessageToOpponentClient(message, Id);
                    }
                    if (message.Contains("Рента"))
                    {
                        Program.f.tbLog.Invoke((MethodInvoker)delegate
                        {
                            Program.f.tbLog.Text += "[" + DateTime.Now + "] " + message + Environment.NewLine;
                        });
                        server.SendMessageToOpponentClient(message, Id);
                    }
                }
            }
            catch (Exception e)
            {
                Program.f.tbLog.Invoke((MethodInvoker)delegate
                {
                    Program.f.tbLog.Text += "[" + DateTime.Now + "] " + e.Message + Environment.NewLine;
                });
            }
        }
        private string GetMessage()
        {
            var data = new byte[256];
            var builder = new StringBuilder();
            do
            {
                builder.Append(Encoding.Unicode.GetString(data, 0, 
                    Stream.Read(data, 0, data.Length)));
            } while (Stream.DataAvailable);
            return builder.ToString();
        }
        protected internal void Close()
        {
            Stream.Close();
            Client.Close();
        }
    }
}



