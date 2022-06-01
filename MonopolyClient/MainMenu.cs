using System;
using System.Windows.Forms;

namespace MonopolyClient
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
            Hide();
            var game = new Game();
            game.ShowDialog();
            Show();
        }
        private void StartMultiplayerGameBtn_Click(object sender, EventArgs e)
        {
            Gamemodes.Singleplayer = false;
            Gamemodes.Multiplayer = true;
            Hide();
            var game = new Game();
            game.ShowDialog();
            Show();
        }
        private void HelpBtn_Click(object sender, EventArgs e)
        {
            var help = new Help();
            Hide();
            help.ShowDialog();
            Show();
        }
        private void QuitBtn_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show("Завершить работу?", "Выход", MessageBoxButtons.YesNo))
            {
                case DialogResult.Yes:
                    Application.Exit();
                    break;
                case DialogResult.No:
                    return;
            }
        }
    }
}

