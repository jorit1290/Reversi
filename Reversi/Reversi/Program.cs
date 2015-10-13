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
        int xkolommen = 7;
        int yrijen = 7;
        int x, y;
        int n = 0;

        Button nieuwspel, help;
        Label nummerblauw, nummerrood, zet;
        PictureBox velden;

        Steen[,] stenen;

        public Scherm()
        {
            x = xkolommen * 50;
            y = yrijen * 50;

            stenen = new Steen[xkolommen, yrijen];

            int centerX = xkolommen / 2;
            int centerY = yrijen / 2;
            stenen[centerX - 1, centerY - 1] = new Steen(centerX - 1, centerY - 1, true);
            stenen[centerX, centerY] = new Steen(centerX, centerY, true);
            stenen[centerX - 1, centerY] = new Steen(centerX - 1, centerY, false);
            stenen[centerX, centerY - 1] = new Steen(centerX, centerY - 1, false);

            SetupGUI();
            
            this.velden.Paint += Veldentekener;
            this.Paint += Teken;
            this.velden.MouseClick += Veldenklikker;
            nieuwspel.Click += spelnieuw;
        }

        private void SetupGUI()
        {
            this.Text = "Reversi";
            this.Size = new Size(75 + x, 250 + y);
            this.MinimumSize = new Size(250, 250 + y);
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
            velden.Size = new Size(x + 1, y + 1);
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

        }

        private void Teken(object o, PaintEventArgs pea)
        {
            //Stenen in de GUI
            System.Drawing.Brush rood, blauw; 
            rood = new SolidBrush(System.Drawing.Color.Red);
            blauw = new SolidBrush(System.Drawing.Color.Blue);
            pea.Graphics.FillEllipse(rood, 25, 50, 50, 50);
            pea.Graphics.FillEllipse(blauw, 25, 105, 50, 50);
        }

        private void Veldentekener(object o, PaintEventArgs pea)
        {
            Pen pen = new Pen(Color.Black);

            for (int z = 0; z <= xkolommen; z++)
                pea.Graphics.DrawLine(pen, z * 50, 0, z * 50, y);

            for(int k = 0; k <= yrijen; k++)
                pea.Graphics.DrawLine(pen, 0, k * 50, x, k * 50);

            foreach (Steen s in stenen)
                if (s != null)
                    s.Draw(o, pea);
        }

        private void Veldenklikker(object o, MouseEventArgs mea)
        {
            //Step 1: Get mouseposition
            int locatieMuisX = mea.X;
            int locatieMuisY = mea.Y;
            int a = 0, b = 0;
            
            Point locatieSteen = new Point(a,b);

            for (int positieX = xkolommen; locatieMuisX < xkolommen * 50 && locatieMuisX > 0; positieX -= 1)
            {
                locatieMuisX += 50;
                a = positieX-1;
            }
            for (int positieY = yrijen; locatieMuisY < xkolommen * 50 && locatieMuisY > 0; positieY -= 1)
            {
                locatieMuisY += 50;
                b = positieY-1;
            }

            //Step 2: Create new Steen at position (if there isn't one already)
            if (n%2 == 0)
            { stenen[a, b] = new Steen(a, b, true);
                n = n + 1;
            }
            else
            {
                stenen[a, b] = new Steen(a, b, false);
                n = n + 1;
            }
            velden.Invalidate();

            //Step 3: Check around for other stones, and change surrounding stones when necessary.

        }

        private void spelnieuw(object o, EventArgs ea)
        {
            n = 0;
            Array.Clear(stenen, 0, stenen.Length);
            int centerX = xkolommen / 2;
            int centerY = yrijen / 2;
            stenen[centerX - 1, centerY - 1] = new Steen(centerX - 1, centerY - 1, true);
            stenen[centerX, centerY] = new Steen(centerX, centerY, true);
            stenen[centerX - 1, centerY] = new Steen(centerX - 1, centerY, false);
            stenen[centerX, centerY - 1] = new Steen(centerX, centerY - 1, false);
            velden.Invalidate();


        }
    }
}

