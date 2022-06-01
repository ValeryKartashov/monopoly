using System;
using System.Windows.Forms;

namespace MonopolyClient
{
    public partial class ColorChoosing : Form
    {
public ColorChoosing()
{
Program.colorChoosing = this;
InitializeComponent();
tbColor.Text = "Не выбран";
if (ConnectionOptions.NameRedIsTaken) chooseRedPlayerBtn.Enabled = false;
if (ConnectionOptions.NameBlueIsTaken) chooseBluePlayerBtn.Enabled = false;
}
        private void connect_button_Click(object sender, EventArgs e)
        {
            switch (tbColor.Text)
            {
                case "Красный":
                    ConnectionOptions.PlayerName = "Red";
                    Close();
                    DialogResult = DialogResult.OK;
                    break;
                case "Синий":
                    ConnectionOptions.PlayerName = "Blue";
                    Close();
                    DialogResult = DialogResult.OK;
                    break;
                case "Не выбран":
                    MessageBox.Show("Выберите цвет");
                    break;
            }
        }
        private void returnBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void chooseRedPlayerBtn_Click(object sender, EventArgs e)
        {
            tbColor.Text = "Красный";
        }
        private void chooseBluePlayerBtn_Click(object sender, EventArgs e)
        {
            tbColor.Text = "Синий";
        }
    }
}
