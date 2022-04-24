using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Monopoly_KartashovAS41
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }
        private void OpenRules_button_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.mosigra.ru/monopoliy_rossiy/rules/");
        }

        private void returnBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}