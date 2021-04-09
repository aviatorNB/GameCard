using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCard
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
            Panel1.AutoSize = true;
            WindowState = FormWindowState.Maximized;

            string imgPath = @"images\TwoClubs.jpg";

           var aPic = new PictureBox()
            {
                Image = Image.FromFile(imgPath),
                Text = "Two",
                Width = 71,
                Height = 100,
                Left = 41 * 2
            };

           Panel1.Controls.Add(aPic);
           aPic.BringToFront();
        }

        

        private Deck aDeck;

        private void SetUp()
        {
            try
            {
                aDeck = new Deck();
                aDeck.Shuffle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetUp();

        }

        private void btnNewDeck_Click(object sender, EventArgs e)
        {
            SetUp();
        }

        private void btnDeal_Click(object sender, EventArgs e)
        {
            try
            {
                Hand aHand = aDeck.DealHand(6);
                ShowHand(aHand);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowHand(Hand aHand)
        {
            Panel1.Controls.Clear();

            Card aCard;
            PictureBox aPic;

            for (int i = 0; i < aHand.Count; i++)
            {
                aCard = aHand[i];
                string imgPath = $@"images/{aCard.FaceValue}{aCard.Suit}.jpg";

                aPic = new PictureBox()
                {
                    Image = Image.FromFile(imgPath),
                    Text = aCard.FaceValue.ToString(),
                    Width = 71,
                    Height = 100,
                    Left = 141 * i
                };

                Panel1.Controls.Add(aPic);
                aPic.BringToFront();
            }
        }
    }
}
