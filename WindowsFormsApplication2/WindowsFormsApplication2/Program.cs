using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Reversi
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ReversiForm reF = new ReversiForm();
            Application.Run(reF);
        }
    }
    class ReversiForm : Form
    {
        InstellingenForm instellingenForm;
        ReversiBord speelBord;
        bool hulp = false;
        bool activeerBot;

        PictureBox pbSpeelBord;
        ToolStripStatusLabel statusLabel, blauwScoreLabel, roodScoreLabel;

        public ReversiForm()
        {
            this.pbSpeelBord = new PictureBox();
            this.pbSpeelBord.Location = new Point(20, 45);
            this.pbSpeelBord.Size = new Size(400, 400);
            this.pbSpeelBord.MouseClick += klikOpBord;
            this.pbSpeelBord.Paint += speelBordTekenWrapper;

            MenuStrip ms = new MenuStrip();
            this.bouwMenu(ms);

            StatusStrip sts = new StatusStrip();
            this.bouwStatusStrip(sts);

            this.ClientSize = new Size(440, 485);
            this.Resize += ResizeHandler;
            this.Text = "Reversi";

            this.Controls.Add(pbSpeelBord);
            this.Controls.Add(ms);
            this.Controls.Add(sts);

            this.instellingenForm = new InstellingenForm();

            this.klikNieuwSpel(null, null);
        }

        private void ResizeHandler(object o, EventArgs ea)
        {
            if (speelBord == null) return;
            int beschikbareHoogte = this.ClientSize.Height - 85; // Marge + statusbar + menu
            int beschikbareBreedte = this.ClientSize.Width - 40; // 20 px marge aan beide kanten
            int vakGrootte;
            if (beschikbareBreedte < beschikbareHoogte)
            {
                vakGrootte = beschikbareBreedte / speelBord.Breedte;
            }
            else
            {
                vakGrootte = beschikbareHoogte / speelBord.Hoogte;
            }
            Size nieuweGrootte = new Size(speelBord.Breedte * vakGrootte,
                speelBord.Hoogte * vakGrootte);
            pbSpeelBord.Size = nieuweGrootte;

            int nieuweX, nieuweY;
            nieuweX = (beschikbareBreedte - nieuweGrootte.Width) / 2 + 20;
            nieuweY = (beschikbareHoogte - nieuweGrootte.Height) / 2 + 45;
            pbSpeelBord.Location = new Point(nieuweX, nieuweY);

            pbSpeelBord.Invalidate();
        }

        private void bouwMenu(MenuStrip ms)
        {
            ToolStripMenuItem spelMenu;
            spelMenu = new ToolStripMenuItem("Spel");
            spelMenu.DropDownItems.Add("Nieuw spel tegen andere speler",
                null, this.klikNieuwSpel);
            spelMenu.DropDownItems.Add("Nieuw spel tegen computer",
                null, this.klikNieuwSpelMetBot);
            spelMenu.DropDownItems.Add("Bordgrootte instellen...",
                null, this.toonInstellingen);

            ToolStripMenuItem hulpMenu;
            hulpMenu = new ToolStripMenuItem("Hulp");
            hulpMenu.DropDownItems.Add("Toon/verberg mogelijke zetten",
                null, this.klikMetHulp);

            ms.Items.Add(spelMenu);
            ms.Items.Add(hulpMenu);

        }

        private void bouwStatusStrip(StatusStrip sts)
        {
            statusLabel = new ToolStripStatusLabel();
            statusLabel.Text = "Start een nieuw spel via het menu Spel.";
            statusLabel.Spring = true;
            blauwScoreLabel = new ToolStripStatusLabel();
            blauwScoreLabel.ForeColor = Color.Blue;
            roodScoreLabel = new ToolStripStatusLabel();
            roodScoreLabel.ForeColor = Color.Red;
            sts.Items.Add(blauwScoreLabel);
            sts.Items.Add(statusLabel);
            sts.Items.Add(roodScoreLabel);
        }

        private void tekenBord(Graphics gr)
        {
            if (speelBord == null) return;
            int vakBreedte = pbSpeelBord.Width / speelBord.Breedte;
            int vakHoogte = pbSpeelBord.Height / speelBord.Hoogte;
            Pen pen = new Pen(Color.Black);
            gr.FillRectangle(Brushes.White, 0, 0, pbSpeelBord.Width, pbSpeelBord.Height);
            gr.DrawRectangle(pen, 0, 0, pbSpeelBord.Width - 1, pbSpeelBord.Height - 1);
            for (int x = 0; x < speelBord.Breedte; x++)
                gr.DrawLine(pen, x * vakBreedte, 0, x * vakBreedte, pbSpeelBord.Height);
            for (int y = 0; y < speelBord.Hoogte; y++)
                gr.DrawLine(pen, 0, y * vakHoogte, pbSpeelBord.Width, y * vakHoogte);
            Tuple<int, int, int> score = speelBord.Score;
            bool mogelijkeZetten = false;

            for (int kolom = 0; kolom < speelBord.Breedte; kolom++)
            {
                for (int rij = 0; rij < speelBord.Hoogte; rij++)
                {
                    //gr.FillRectangle(Brushes.White, kolom * vakBreedte, rij * vakHoogte, vakBreedte, vakHoogte);

                    //gr.DrawRectangle(pen, kolom * vakBreedte, rij * vakHoogte, vakBreedte, vakHoogte);

                    if (speelBord[kolom, rij] == stukje.blauw)
                    {
                        gr.FillEllipse(Brushes.Blue, kolom * vakBreedte + 3, rij * vakHoogte + 3, vakBreedte - 6, vakHoogte - 6);
                    }
                    else if (speelBord[kolom, rij] == stukje.rood)
                    {
                        gr.FillEllipse(Brushes.Red, kolom * vakBreedte + 3, rij * vakHoogte + 3, vakBreedte - 6, vakHoogte - 6);
                    }
                    else
                    {
                        if (speelBord.ControleerZet(kolom, rij) > 0)
                        {
                            mogelijkeZetten = true;
                            if (hulp)
                                gr.DrawEllipse(pen, kolom * vakBreedte + 3, rij * vakHoogte + 3, vakBreedte - 6, vakHoogte - 6);
                        }
                    }

                    this.blauwScoreLabel.Text = "Blauw: " + score.Item1;
                    this.roodScoreLabel.Text = "Rood: " + score.Item2;
                }
            }
            if (!speelBord.spelActief) return;

            if (!mogelijkeZetten)
            {
                if (speelBord.beurtOvergeslagen) // is vorige ook al overgeslagen
                {
                    speelBord.BeeindigSpel();
                    statusLabel.ForeColor = Color.Black;
                    statusLabel.Text = "Start een nieuw spel via het menu Spel.";
                    return;
                }
                speelBord.beurtOvergeslagen = true;
                speelBord.SpelerAanZet = speelBord.SpelerNietAanZet;
                this.pbSpeelBord.Invalidate();
                return;
            }
            else speelBord.beurtOvergeslagen = false;

            if (speelBord.SpelerAanZet == stukje.blauw)
            {
                statusLabel.ForeColor = Color.Blue;
                statusLabel.Text = "Blauw aan zet";
            }
            else
            {
                statusLabel.ForeColor = Color.Red;
                statusLabel.Text = "Rood aan zet";
            }

            if (speelBord.Robot && speelBord.SpelerAanZet == stukje.rood)
            {
                Tuple<int, int> zet = speelBord.BepaalBesteZet();
                speelBord.MaakZet(zet.Item1, zet.Item2);
                this.tekenBord(gr);
            }

        }
        private void beurtTeken(object o, PaintEventArgs pea)
        {
            Graphics gr = pea.Graphics;
            if (speelBord == null || !speelBord.spelActief) return;
            if (speelBord.SpelerAanZet == stukje.blauw)
                gr.FillEllipse(new SolidBrush(Color.Blue), 0, 0, 40 - 3, 40 - 3);
            else gr.FillEllipse(new SolidBrush(Color.Red), 0, 0, 40 - 3, 40 - 3);

        }

        private void speelBordTekenWrapper(object o, PaintEventArgs pea)
        {
            tekenBord(pea.Graphics);
        }

        public void beeindigSpel()
        {
            // Verzamel score, bepaal wie wint en stop nieuwe kliks
            Tuple<int, int, int> score = speelBord.Score;
            if (score.Item1 == score.Item2)
                MessageBox.Show("Gelijkspel! " + score.Item1 + "-" + score.Item2);
            else if (score.Item1 > score.Item2)
                MessageBox.Show("Blauw wint! " + score.Item1 + "-" + score.Item2);
            else
                MessageBox.Show("Rood wint! " + score.Item1 + "-" + score.Item2);
        }

        private void klikOpBord(object o, MouseEventArgs mea)
        {
            if (this.speelBord != null && this.speelBord.spelActief)
            {
                int rij = mea.X / (pbSpeelBord.Width / speelBord.Breedte);
                int kolom = mea.Y / (pbSpeelBord.Height / speelBord.Hoogte);

                if (speelBord.ControleerZet(rij, kolom) > 0)
                {
                    speelBord.MaakZet(rij, kolom);
                    this.pbSpeelBord.Invalidate();
                }
            }
        }

        private void klikNieuwSpel(object o, EventArgs ea)
        {
            int bordBreedte = instellingenForm.Breedte;
            int bordHoogte = instellingenForm.Hoogte;
            this.speelBord = new ReversiBord(bordBreedte, bordHoogte);
            if (this.activeerBot) this.speelBord.Robot = true;
            else this.speelBord.Robot = false;
            this.OnResize(null);
            this.pbSpeelBord.Invalidate();
        }
        private void klikNieuwSpelMetBot(object o, EventArgs ea)
        {
            this.activeerBot = true;
            this.klikNieuwSpel(o, ea);
            this.activeerBot = false;
        }

        private void klikMetHulp(object o, EventArgs ea)
        {
            hulp = !hulp;
            this.pbSpeelBord.Invalidate();
        }
        private void toonInstellingen(object o, EventArgs ea)
        {
            this.instellingenForm.ShowDialog();
        }

    }

    class InstellingenForm : Form
    {
        Label lblBordBreedte, lblBordHoogte;
        NumericUpDown aantalBreedKeuze, aantalHoogKeuze;
        Button okKnop, cancelKnop;
        public InstellingenForm()
        {
            this.Text = "Bordgrootte";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.lblBordBreedte = new Label();
            this.lblBordBreedte.AutoSize = true;
            this.lblBordBreedte.Location = new Point(10, 10);
            this.lblBordBreedte.Text = "Aantal breed:";

            this.lblBordHoogte = new Label();
            this.lblBordHoogte.AutoSize = true;
            this.lblBordHoogte.Location = new Point(10, lblBordBreedte.Bottom + 10);
            this.lblBordHoogte.Text = "Aantal hoog:";

            this.aantalBreedKeuze = new NumericUpDown();
            this.aantalBreedKeuze.Location = new Point(lblBordBreedte.Right, lblBordBreedte.Top);
            this.aantalBreedKeuze.Size = new Size(50, 15);
            this.aantalBreedKeuze.Minimum = 3;
            this.aantalBreedKeuze.Maximum = 10;
            this.aantalBreedKeuze.Value = 6;

            this.aantalHoogKeuze = new NumericUpDown();
            this.aantalHoogKeuze.Location = new Point(aantalBreedKeuze.Left, aantalBreedKeuze.Bottom + 10);
            this.aantalHoogKeuze.Size = new Size(50, 15);
            this.aantalHoogKeuze.Minimum = 3;
            this.aantalHoogKeuze.Maximum = 10;
            this.aantalHoogKeuze.Value = 6;

            this.okKnop = new Button();
            okKnop.Text = "OK";
            okKnop.Location = new Point(aantalBreedKeuze.Left, this.aantalHoogKeuze.Bottom + 20);
            this.okKnop.DialogResult = DialogResult.OK;
            this.AcceptButton = okKnop;

            this.Size = new Size(okKnop.Right + 30, okKnop.Bottom + 50);

            this.Controls.Add(lblBordBreedte);
            this.Controls.Add(lblBordHoogte);
            this.Controls.Add(aantalBreedKeuze);
            this.Controls.Add(aantalHoogKeuze);
            this.Controls.Add(okKnop);
            this.Controls.Add(cancelKnop);
        }
        public int Hoogte
        {
            get
            {
                return (int)this.aantalHoogKeuze.Value;
            }
        }
        public int Breedte
        {
            get
            {
                return (int)this.aantalBreedKeuze.Value;
            }
        }
    }
}