using System;
using System.Threading;
using System.Windows.Forms;

namespace MonopolyServer
{
    public partial class ServerForm : Form
    {
        private static ServerObject server;
        private static Thread listenThread;
        public ServerForm()
        {
            InitializeComponent();
            Program.f = this;
            Text = "Сервер. Состояние: выключен";
            btnTurnOff.Text = "Закрыть программу";
        }
        private void btnTurnOff_Click(object sender, EventArgs e)
        {
            switch (Text)
            {
                case "Сервер. Состояние: выключен":
                    switch (MessageBox.Show("Закрыть окно работы с сервером?", 
                                "Закрытие", MessageBoxButtons.YesNo))
                    {
                        case DialogResult.Yes:
                            Close();
                            break;
                        case DialogResult.No:
                            break;
                    }
                    break;
                case "Сервер. Состояние: включен":
                    switch (MessageBox.Show("Выключить сервер?" + Environment.NewLine + "Текущая игра будет прервана.", 
                                "Выключение", MessageBoxButtons.YesNo))
                    {
                        case DialogResult.Yes:
                            server?.CloseAndExit();
                            break;
                        case DialogResult.No:
                            break;
                    }
                    break;
            }
        }
        private void btnTurnOn_Click(object sender, EventArgs e)
        {
            try
            {
                btnTurnOn.Enabled = false;
                btnTurnOff.Enabled = true;
                server = new ServerObject();
                listenThread = new Thread(server.Listen);
                listenThread.Start();
                Text = "Сервер. Состояние: включен";
                btnTurnOff.Text = "Выключить сервер";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Включить сервер не удалось:" + ex.Message);
                server?.CloseAndExit();
            }
        }
    }
}