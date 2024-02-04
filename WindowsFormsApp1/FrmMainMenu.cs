using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FrmMainMenu : Form
    {
        private ScoreLoader scoreLoader = new ScoreLoader();
        public FrmMainMenu()
        {
            InitializeComponent();
            lblKeyInfo.Location = new Point(this.Width - lblKeyInfo.Width - 10, 10);
            lblKeyInfo.Text = "W, A, S, D: Hareket\n  P: Duraklat";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!IsValidPlayerName(txtOyuncuAdi.Text))
            {
                MessageBox.Show("Oyuncu Adı Girilmesi Zorunludur!");
                return;
            }

            GameForm frm = new GameForm
            {
                PlayerName = txtOyuncuAdi.Text
            };
            frm.Show();
            this.Hide();

        }

        private bool IsValidPlayerName(string playerName)
        {
            return !String.IsNullOrWhiteSpace(playerName);
        }

        private void btnViewScoreBoard_Click(object sender, EventArgs e)
        {
            string filePath = "skor.txt"; // Txt dosyasının yolu
            var topScores = scoreLoader.LoadTopScores(filePath);
            lstViewRates.Items.Clear();
            lstViewRates.Visible = true;

            foreach (var score in topScores)
            {
                lstViewRates.Items.Add($"{score.PlayerName} - {score.PlayerScore}");
            }
        }
    }
}
