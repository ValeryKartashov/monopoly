using System;
using System.Windows.Forms;

namespace Monopoly_KartashovAS41
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }
        private void StartSingleplayerGameBtn_Click(object sender, EventArgs e)
        {
            Gamemodes.Singleplayer = true;
            Gamemodes.Multiplayer = false;
            this.Hide();
            Game game = new Game();
            game.ShowDialog();
        }
        private void StartMultiplayerGameBtn_Click(object sender, EventArgs e)
        {
            Gamemodes.Singleplayer = false;
            Gamemodes.Multiplayer = true;
            this.Hide();
            Connection connection = new Connection();
            connection.ShowDialog();
            this.Show();
        }
        private void HelpBtn_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            this.Hide();
            help.ShowDialog();
            this.Show();
        }
        private void QuitBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Завершить работу?", "Выход", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
                Application.Exit();
            else if (dialog == DialogResult.No)
                return;
        }
    }
}