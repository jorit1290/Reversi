using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Reversi
{
    class Program
    {
        static void Main()
        {
            Application.Run(new Scherm());
        }
    }

    class Scherm : Form
    {
        Panel velden;
        Button nieuwspel, help;
        Label nummerblauw, nummerrood, zet;


        public Scherm()
        {
            this.Text = "Reversi";
            this.Size = new Size(500, 650);
            this.BackColor = System.Drawing.Color.LightBlue;

            nieuwspel = new Button();
            nieuwspel.Location = new Point(160, 10);
            nieuwspel.Size = new Size(75, 30);
            nieuwspel.Text = "nieuw spel";
            nieuwspel.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(nieuwspel);

            help = new Button();
            help.Location = new Point(240, 10);
            help.Size = new Size(50, 30);
            help.Text = "help";
            help.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(help);

            velden = new Panel();
            velden.Location = new Point(25, 175);
            velden.Size = new Size(425, 400);
            velden.BackColor = System.Drawing.Color.White;
            this.Controls.Add(velden);

            //nu staat er nog x en y, dat moet nog aangepast worden.

            nummerrood = new Label();
            nummerrood.Location = new Point(100, 65);
            nummerrood.Text = "X stenen";
            nummerrood.Font = new Font("Arial", 12);
            this.Controls.Add(nummerrood);

            nummerblauw = new Label();
            nummerblauw.Location = new Point(100, 120);
            nummerblauw.Text = "Y stenen";
            nummerblauw.Font = new Font("Arial", 12);
            this.Controls.Add(nummerblauw);



            this.Paint += teken;



        }
        private void teken(object o, PaintEventArgs pea)
        {
            System.Drawing.Brush rood, blauw; 
            rood = new SolidBrush(System.Drawing.Color.Red);
            blauw = new SolidBrush(System.Drawing.Color.Blue);
            pea.Graphics.FillEllipse(rood, 25, 50, 50, 50);
            pea.Graphics.FillEllipse(blauw, 25, 105, 50, 50);

        }
    }
}

