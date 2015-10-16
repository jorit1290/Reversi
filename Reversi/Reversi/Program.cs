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
        //Belangrijke variabelen
        int xkolommen = 6;
        int yrijen = 6;
        int beurt = 0;
        int grootte = 50;

        bool legaal = false;
        bool hulp = false;

        int x, y;

        Button nieuwspel, help;
        Label aantalblauw, aantalgroen, zet;
        PictureBox velden;
        Steen[,] stenen;


        //In de constructormethode worden methodes aangeroepen die de GUI maken en zorgen dat het spel werkt.
        public Scherm()
        {
            x = xkolommen * grootte;
            y = yrijen * grootte;

            stenen = new Steen[xkolommen, yrijen];

            BeginStand();
            SetupGUI();

            this.velden.Paint += Veldentekener;
            this.Paint += Legenda;
            this.velden.MouseClick += Veldenklikker;
            nieuwspel.Click += Spelnieuw;
            help.Click += Helper;
        }


        //De onderstaande methode maakt de GUI
        private void SetupGUI()
        {
            this.Text = "Reversi";
            this.Size = new Size(75 + x, 250 + y);
            this.MinimumSize = new Size(250, 250 + y);
            this.BackColor = System.Drawing.Color.FromArgb(246, 255, 248);

            //Buttons nieuwspel en help
            nieuwspel = new Button();
            nieuwspel.Location = new Point(70, 20);
            nieuwspel.Size = new Size(90, 30);
            nieuwspel.Text = "nieuw spel";
            nieuwspel.Font = new Font("Ariel", 10);
            nieuwspel.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.Controls.Add(nieuwspel);

            help = new Button();
            help.Location = new Point(200, 20);
            help.Size = new Size(60, 30);
            help.Text = "help";
            help.Font = new Font("Ariel", 10);
            help.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.Controls.Add(help);

            //Labels aantalgroen, aantalblauw en zet
            aantalgroen = new Label();
            aantalgroen.Size = new Size(80, 30);
            aantalgroen.Location = new Point(85, 85);
            aantalgroen.Text = Aantalgroen() + " stenen";
            aantalgroen.Font = new Font("Arial", 11);
            this.Controls.Add(aantalgroen);

            aantalblauw = new Label();
            aantalblauw.Size = new Size(80, 30);
            aantalblauw.Location = new Point(85, 130);
            aantalblauw.Text = Aantalblauw() + " stenen";
            aantalblauw.Font = new Font("Arial", 11);
            this.Controls.Add(aantalblauw);

            zet = new Label();
            zet.Size = new Size(100, 30);
            zet.Location = new Point(200, 110);
            zet.Text = Uitkomst();
            zet.Font = new Font("Ariel", 11);
            this.Controls.Add(zet);

            //Picturebox velden
            velden = new PictureBox();
            velden.Location = new Point(25, 175);
            velden.Size = new Size(x + 1, y + 1);
            velden.BackColor = System.Drawing.Color.White;
            this.Controls.Add(velden);
        }


        //BeginStand tekent de vier stenen die al op het bord liggen aan het begin van het spel.
        private void BeginStand()
        {
            int centerX = xkolommen / 2;
            int centerY = yrijen / 2;
            stenen[centerX - 1, centerY - 1] = new Steen(centerX - 1, centerY - 1, true);
            stenen[centerX, centerY] = new Steen(centerX, centerY, true);
            stenen[centerX - 1, centerY] = new Steen(centerX - 1, centerY, false);
            stenen[centerX, centerY - 1] = new Steen(centerX, centerY - 1, false);
        }


        //De methode Legenda tekent de rode en blauwe steen die in de GUI staan.
        private void Legenda(object o, PaintEventArgs pea)
        {
            int diameter = 30;
            Brush groen, blauw;

            groen = new SolidBrush(System.Drawing.Color.FromArgb(178, 255, 102));
            blauw = new SolidBrush(System.Drawing.Color.FromArgb(153, 255, 255));
            pea.Graphics.FillEllipse(groen, 25, 75, diameter, diameter);
            pea.Graphics.DrawEllipse(Pens.Black, 25, 75, diameter, diameter);
            pea.Graphics.FillEllipse(blauw, 25, 120, diameter, diameter);
            pea.Graphics.DrawEllipse(Pens.Black, 25, 120, diameter, diameter);
        }


        //Als de onderstaande methode wordt aangeroepen, worden de lijnen van het veld getekend.
        private void Veldentekener(object o, PaintEventArgs pea)
        {
            Pen pen = new Pen(Color.Black);

            for (int z = 0; z <= xkolommen; z++)
                pea.Graphics.DrawLine(pen, z * grootte, 0, z * grootte, y);

            for (int k = 0; k <= yrijen; k++)
                pea.Graphics.DrawLine(pen, 0, k * grootte, x, k * grootte);

            foreach (Steen s in stenen)
                if (s != null)
                    s.DrawSteen(o, pea);

            for (int x = 0; x < xkolommen; x++)
            {
                for (int y = 0; y < yrijen; y++)
                {
                    if (hulp && stenen[x, y] == null && insluit(x, y) == true) pea.Graphics.DrawEllipse(Pens.Black, x * grootte + 2, y * grootte + 2, 46, 46);
                }
            }
        }


        // Als er op de Picturebox geklikt wordt, wordt Veldenklikker aangeroepen. Deze methode kijkt of het legaal
        // is om op de desbetreffende plek een steen te plaatsen. Zo ja, dan wordt er een steen getekend.
        private void Veldenklikker(object o, MouseEventArgs mea)
        {
            int locatieMuisX = mea.X;
            int locatieMuisY = mea.Y;
            int a = 0, b = 0;


            Point locatieSteen = new Point(a, b);

            //Stap 1: Transleer de muispositie naar de positie in de array stenen.
            for (int positieX = xkolommen; locatieMuisX < xkolommen * grootte && locatieMuisX > 0; positieX -= 1)
            {
                locatieMuisX += grootte;
                a = positieX - 1;
            }
            for (int positieY = yrijen; locatieMuisY < xkolommen * grootte && locatieMuisY > 0; positieY -= 1)
            {
                locatieMuisY += grootte;
                b = positieY - 1;
            }

            //Stap 2: Ligt er al een steen op de desbetreffende positie?
            if (stenen[a, b] == null && insluit(a, b) == true)
            {
                legaal = true;
            }
            //Stap 3: Worden er wel stenen ingesloten door een steen te plaatsen op de desbetreffende positie?


            //Stap 4: Creeer een nieuwe Steen op de positie indien deze legaal is.
            if (legaal == true)
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

                legaal = false;
                velden.Invalidate();
                zet.Text = Uitkomst();
                aantalgroen.Text = Aantalgroen() + " stenen";
                aantalblauw.Text = Aantalblauw() + " stenen";


                //Stap 5: Verander omliggende stenen van kleur, als dat nodig is.
            }
        }

        //Deze methode wordt aangeroepen zodra er op de nieuwspelbutton geklikt wordt.
        //Hij zorgt ervoor dat je weer het beginscherm krijgt en opnieuw kunt beginnen.
        private void Spelnieuw(object o, EventArgs ea)
        {
            beurt = 0;
            Array.Clear(stenen, 0, stenen.Length);
            BeginStand();
            zet.Text = Uitkomst();
            velden.Invalidate();

        }


        //De methode Uitkomst bepaald welke tekst het label zet laat zien.
        private string Uitkomst()
        {
            if (beurt % 2 == 0)
                return "groen aan zet";
            else if (beurt % 2 == 1)
                return "blauw aan zet";
            // else if (geen zetten meer mogelijk && Aantalgroen>Aantalblauw)
            //   return "groen heeft gewonnen!";
            // else if (geen zetten meer mogelijk && Aantalblauw>Aantalgroen)
            //   return "blauw heeft gewonen!";
            else
                return "Gelijkspel";
        }

        //Aantalgroenbepaalt hoeveel groene stenen er op het bord staan.
        private string Aantalgroen()
        {
            int aantalgr = 0;
            foreach (Steen s in stenen)
            {
                if (s == null) continue;
                if (s.green == true)
                    aantalgr++;
            }
            return aantalgr.ToString();
        }


        //Aantalblauw bepaalt hoeveel blauwe stenen er op het bord staan.
        private string Aantalblauw()
        {
            int aantalbl = 0;
            foreach (Steen s in stenen)
            {
                if (s == null) continue;
                if (s.green == false)
                    aantalbl++;
            }
            return aantalbl.ToString();
        }


        public bool insluit(int a, int b)
        {
            bool nietaanzet;
            if (beurt % 2 == 0)
                nietaanzet = false;
            else nietaanzet = true;

            
            var ding = ingesloot();
            foreach (int bleh in ding.Item1)
            {
                foreach(int blah in ding.Item2) {
                    int stappengedaan = 1;
                    int teller = 0;
                    for (int x = a + bleh * stappengedaan, y = b + blah * stappengedaan; 
                        x >= 0 && y >= 0 && x < xkolommen && y < yrijen; stappengedaan++, x = a + bleh * stappengedaan, y = b + blah * stappengedaan)
                    {

                        Steen huidigesteen = stenen[x, y];
                        if (huidigesteen == null)
                            break;

                        if (huidigesteen.green == nietaanzet)
                        {
                            // steen is van de vijand
                            teller++;
                        }
                        else
                        {
                            
                            if (teller > 0) return true;
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public Tuple<int[], int[]> ingesloot()
        {
            int[] poot = new int[3];
            int[] pootato = new int[3];

            poot[0] = -1;
            poot[1] = 0;
            poot[2] = 1;

            pootato[0] = -1;
            pootato[1] = 0;
            pootato[2] = 1;

            return new Tuple<int[], int[]>(poot, pootato);
        }

        public void Helper(object o, EventArgs ea)
        {
            hulp = !hulp;
            velden.Invalidate();
        }
    }
}

