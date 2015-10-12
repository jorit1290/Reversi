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
        int xkolommen = 25;
        int yrijen = 13;
        

        Button nieuwspel, help;
        Label nummerblauw, nummerrood, zet;
        PictureBox velden; 

        public Scherm()
        {
            int x = xkolommen * 50;
            int y = yrijen * 50;
            this.Text = "Reversi";
            this.Size = new Size(75+x, 250+y);
            this.MinimumSize = new Size(250, 250+y);
            this.BackColor = System.Drawing.Color.LightBlue;

            nieuwspel = new Button();
            nieuwspel.Location = new Point(60, 10);
            nieuwspel.Size = new Size(75, 30);
            nieuwspel.Text = "nieuw spel";
            nieuwspel.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(nieuwspel);

            help = new Button();
            help.Location = new Point(140, 10);
            help.Size = new Size(50, 30);
            help.Text = "help";
            help.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(help);

            velden = new PictureBox();
            velden.Location = new Point(25, 175);
            velden.Size = new Size(x+1, y+1);
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
            this.velden.Paint += veldentekener;
            this.velden.MouseClick += veldenklikker;

        }
        private void teken(object o, PaintEventArgs pea)
        {
            System.Drawing.Brush rood, blauw; 
            rood = new SolidBrush(System.Drawing.Color.Red);
            blauw = new SolidBrush(System.Drawing.Color.Blue);
            pea.Graphics.FillEllipse(rood, 25, 50, 50, 50);
            pea.Graphics.FillEllipse(blauw, 25, 105, 50, 50);

        }

        private void veldentekener(object o, PaintEventArgs pea)
        {
            Pen pen = new Pen(Color.Black);
            int x = xkolommen * 50;
            int y = yrijen * 50;
            int z = 0;
            int k = 0;
            while (z <= x)
            {
                pea.Graphics.DrawLine(pen, z, 0, z, y);
                z = z + 50;
            }
            while (k <= y)
            {
                pea.Graphics.DrawLine(pen, 0, k, x, k);
                k = k + 50;
            }

        }
        private void veldenklikker(object o, MouseEventArgs mea)
        {

        }
    }
}

