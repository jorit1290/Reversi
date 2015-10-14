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
        int xkolommen = 6;
        int yrijen = 6;
        int beurt = 0;
        int size = 46;
        int grootte = 50;
        
        int x, y;

        Button nieuwspel, help;

        Label nummerblauw, nummerrood, zet;
        PictureBox velden;


        Steen[,] stenen;

        public Scherm()
        {
            x = xkolommen * grootte;
            y = yrijen * grootte;

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
            nieuwspel.Click += Spelnieuw;
            //help.Click += Legaal;
        }

        private void SetupGUI()
        {
            this.Text = "Reversi";
            this.Size = new Size(75 + x, 250 + y);
            this.MinimumSize = new Size(250, 250 + y);
            this.BackColor = System.Drawing.Color.LightBlue;

            nieuwspel = new Button();
            nieuwspel.Location = new Point(70, 20);
            nieuwspel.Size = new Size(90, 30);
            nieuwspel.Text = "nieuw spel";
            nieuwspel.Font = new Font("Ariel", 10);
            nieuwspel.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(nieuwspel);

            help = new Button();
            help.Location = new Point(200, 20);
            help.Size = new Size(60, 30);
            help.Text = "help";
            help.Font = new Font("Ariel", 10);
            help.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(help);

            velden = new PictureBox();
            velden.Location = new Point(25, 175);
            velden.Size = new Size(x + 1, y + 1);
            velden.BackColor = System.Drawing.Color.White;
            this.Controls.Add(velden);

            //nu staat er nog x en y, dat moet nog aangepast worden.

            nummerrood = new Label();
            nummerrood.Size = new Size(70, 30);
            nummerrood.Location = new Point(85, 85);
            nummerrood.Text = "X stenen";
            nummerrood.Font = new Font("Arial", 11);
            this.Controls.Add(nummerrood);

            nummerblauw = new Label();
            nummerblauw.Size = new Size(70, 30);
            nummerblauw.Location = new Point(85, 130);
            nummerblauw.Text = "Y stenen";
            nummerblauw.Font = new Font("Arial", 11);
            this.Controls.Add(nummerblauw);

            zet = new Label();
            zet.Size = new Size(100, 30);
            zet.Location = new Point(200, 110);
            zet.Text = Uitkomst();
            zet.Font = new Font("Ariel", 11);
            this.Controls.Add(zet);
        }

        private void Teken(object o, PaintEventArgs pea)
        {
            //Stenen in de GUI
            int diameter = 30;
            Brush rood, blauw;

            rood = new SolidBrush(System.Drawing.Color.Red);
            blauw = new SolidBrush(System.Drawing.Color.Blue);
            pea.Graphics.FillEllipse(rood, 25, 75, diameter, diameter);
            pea.Graphics.DrawEllipse(Pens.Black, 25, 75, diameter, diameter);
            pea.Graphics.FillEllipse(blauw, 25, 120, diameter, diameter);
            pea.Graphics.DrawEllipse(Pens.Black, 25, 120, diameter, diameter);
        }

        private void Veldentekener(object o, PaintEventArgs pea)
        {
            Pen pen = new Pen(Color.Black);

            for (int z = 0; z <= xkolommen; z++)
                pea.Graphics.DrawLine(pen, z * grootte, 0, z * grootte, y);

            for(int k = 0; k <= yrijen; k++)
                pea.Graphics.DrawLine(pen, 0, k * grootte, x, k * grootte);

            foreach (Steen s in stenen)
                if (s != null)
                    s.DrawSteen(o, pea);
        }

        private void Veldenklikker(object o, MouseEventArgs mea)
        {
            //Step 1: Get mouseposition
            int locatieMuisX = mea.X;
            int locatieMuisY = mea.Y;
            int a = 0, b = 0;
            bool legaal = false;

            Point locatieSteen = new Point(a,b);

            for (int positieX = xkolommen; locatieMuisX < xkolommen * grootte && locatieMuisX > 0; positieX -= 1)
            {
                locatieMuisX += grootte;
                a = positieX-1;
            }
            for (int positieY = yrijen; locatieMuisY < xkolommen * grootte && locatieMuisY > 0; positieY -= 1)
            {
                locatieMuisY += grootte;
                b = positieY-1;
            }

            //Step 2: Is this position a legal option?

            if(stenen[a,b] == stenen[0,0])
            {
                legaal = true;
            }


            //Step 3: Create new Steen at position (if there isn't one already)
            if(legaal == true)
            {
                if (beurt % 2 == 0)
                {
                    stenen[a, b] = new Steen(a, b, true);
                    beurt = beurt + 1;
                }
                else
                {
                stenen[a, b] = new Steen(a, b, false);
                beurt = beurt + 1;
                }

            zet.Text = Uitkomst();
            legaal = false;
            velden.Invalidate();
            }
            //Step 4: Check around for other stones, and change surrounding stones when necessary.

        }

        private void Spelnieuw(object o, EventArgs ea)
        {
            beurt = 0;
            Array.Clear(stenen, 0, stenen.Length);
            int centerX = xkolommen / 2;
            int centerY = yrijen / 2;
            stenen[centerX - 1, centerY - 1] = new Steen(centerX - 1, centerY - 1, true);
            stenen[centerX, centerY] = new Steen(centerX, centerY, true);
            stenen[centerX - 1, centerY] = new Steen(centerX - 1, centerY, false);
            stenen[centerX, centerY - 1] = new Steen(centerX, centerY - 1, false);
            zet.Text = Uitkomst();
            velden.Invalidate();
            
        }



        private string Uitkomst()
        {
            if (beurt % 2 == 0)
                return "rood aan zet";
            else if (beurt % 2 == 1)
                return "blauw aan zet";
           // else if (?)
             //   return "rood heeft gewonnen!";
           // else if (?)
             //   return "blauw heeft gewonen!";
            else
                return "remise";
        }

        /*
        //klopt helemaal niets van, waarden achter return hebben ook geen betekenis, gewoon random iets neergezet
        /*private int Legaal()
        {
            foreach (Steen s in stenen)
            {
                if (beurt % 2 == 0 && s == null)
                    return 0;
                else if (beurt % 2 != 0 && s == null)
                    return 1;
                else
                    return 2;
            }
        }*/
    }
}

