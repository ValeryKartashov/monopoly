using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Monopoly_KartashovAS41
{
    public partial class Connection : Form
    {
        public Connection()
        {
            InitializeComponent();
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectionOptions.Port = Convert.ToInt32(port_textbox.Text);
            ConnectionOptions.IP = ip_textbox.Text;
            if (redPlayer_radioButton.Checked)
                ConnectionOptions.PlayerName = "Red";
            else
                ConnectionOptions.PlayerName = "Blue";
            this.Hide();
            Game game = new Game();
            game.ShowDialog();
        }
        private void Connection_Load(object sender, EventArgs e)
        {
            try
            {
                redPlayer_radioButton.Checked = true;
                string firstLine = File.ReadLines("C:\\Users\\socdd\\Desktop\\Monopoly_KartashovAS41\\config.ini").First();
                string secondLine = File.ReadLines("C:\\Users\\socdd\\Desktop\\Monopoly_KartashovAS41\\config.ini").Skip(1).First();
                ip_textbox.Text = firstLine;
                port_textbox.Text = secondLine;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void redPlayer_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (redPlayer_radioButton.Checked)
                redPlayer_radioButton.Checked = true; 
            else
                redPlayer_radioButton.Checked = false; 
        }
        private void returnBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void chooseRedPlayerBtn_Click(object sender, EventArgs e)
        {
            redPlayer_radioButton.Checked = true;
        }

        private void chooseBluePlayerBtn_Click(object sender, EventArgs e)
        {
            bluePlayer_radioButton.Checked = true;
        }
    }
}