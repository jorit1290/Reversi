using System;
using System.Windows.Forms;

namespace Reversi
{
    enum stukje
    {
        leeg = 0,
        blauw = 1,
        rood = 2
    }

    class ReversiBord
    {
        private stukje[,] bord;
        public stukje SpelerAanZet;
        public bool spelActief, beurtOvergeslagen, Robot;
        public int Breedte, Hoogte;

        delegate bool rico(int x, int y, int i); //RIchtings COnditie
        delegate int ritr(int x, int y, int i);  //RIchtings TRansformatie
        static ritr[] riXen = new ritr[8]
        {
            (x,y,i) => x-i,
            (x,y,i) => x,
            (x,y,i) => x+i,
            (x,y,i) => x-i,
            (x,y,i) => x+i,
            (x,y,i) => x-i,
            (x,y,i) => x,
            (x,y,i) => x+i
        };
        static ritr[] riYen = new ritr[8]
        {
            (x,y,i) => y-i,
            (x,y,i) => y-i,
            (x,y,i) => y-i,
            (x,y,i) => y,
            (x,y,i) => y,
            (x,y,i) => y+i,
            (x,y,i) => y+i,
            (x,y,i) => y+i
        };

        rico[] richtingen;

        /// <summary>
        /// Deze mini-methodes zorgen ervoor dat het bord direct kan worden benaderd via this[x,y].
        /// </summary>
        /// <param name="x">Kolom van het stuk</param>
        /// <param name="y">Rij van het stuk</param>
        /// <returns></returns>
        public stukje this[int x, int y]
        {
            get
            {
                return bord[x, y];
            }
            set
            {
                bord[x, y] = value;
            }
        }
        public stukje SpelerNietAanZet
        {
            get
            {
                if (this.SpelerAanZet == stukje.blauw)
                    return stukje.rood;
                else return stukje.blauw;
            }
        }

        // Score-properties
        public int BlauweStukken
        {
            get
            {
                return this.telStukken().Item1;
            }
        }
        public int RodeStukken
        {
            get
            {
                return this.telStukken().Item2;
            }
        }
        public int LegeVelden
        {
            get
            {
                return this.telStukken().Item3;
            }
        }
        public Tuple<int, int, int> Score
        {
            get
            {
                return telStukken();
            }
        }
        private Tuple<int, int, int> telStukken()
        {
            int legeVelden = 0, blauweStukken = 0, rodeStukken = 0;
            foreach (stukje stuk in this.bord)
            {
                switch (stuk)
                {
                    case stukje.blauw:
                        blauweStukken++;
                        break;
                    case stukje.rood:
                        rodeStukken++;
                        break;
                    default:
                        legeVelden++;
                        break;
                }
            }
            return new Tuple<int, int, int>(blauweStukken, rodeStukken, legeVelden);
        }
        // Constructor
        public ReversiBord(int breedte, int hoogte)
        {
            // Bouw het bord op
            this.Breedte = breedte;
            this.Hoogte = hoogte;
            this.bord = new stukje[breedte, hoogte];

            // Zet de startsituatie op: vier stukken in het midden
            int midx = (breedte / 2) - 1;
            int midy = (hoogte / 2) - 1;
            bord[midx, midy] = stukje.blauw;
            bord[midx, midy + 1] = stukje.rood;
            bord[midx + 1, midy] = stukje.rood;
            bord[midx + 1, midy + 1] = stukje.blauw;

            // Spelstatus: blauw begint
            this.SpelerAanZet = stukje.blauw;
            this.beurtOvergeslagen = false;
            this.spelActief = true;

            // Bouw richtingenarray voor zettencontrole
            /* Door de array richtingen kunnen we gemakkelijk checken in alle 8 richtingen of een zet geldig is.
             * Een richting is een combinatie van een of twee condities, die samen bepalen wanneer de rand van het bord is bereikt in ControleerZet.
             * De richtingen worden via de volgende tabel opgebouwd
             *      
             *      | x>=0 |    | x < B
             * y>=0 |  LB  | B  | RB
             *      |  L   |    | R
             * y<H  |  LO  | O  | RO
             *
             * Waarbij B = this.Breedte en H = this.Hoogte
             */
            richtingen = new rico[8]
            {
                (x,y,i) => (x-i >= 0 && y-i >= 0),                      // LB
                (x,y,i) => (y-i >= 0),                                  // B
                (x,y,i) => (x+i < this.Breedte && y-i >= 0),            // RB
                (x,y,i) => (x-i >= 0),                                  // L
                (x,y,i) => (x+i < this.Breedte),                        // R
                (x,y,i) => (x-i >= 0 && y+i < this.Hoogte),             // LO
                (x,y,i) => (y+i < this.Hoogte),                         // O
                (x,y,i) => (x+i < this.Breedte && y+i < this.Hoogte)    // RO
            };
        }

        // Zetten checken maken en speleinde
        public int ControleerZet(int x, int y, bool snel = true)
        {
            int aantal = 0;
            if (this[x, y] != stukje.leeg) return 0; // Veld moet leeg zijn


            int testX = 0;
            int testY = 0;
            stukje spelernietaanzet = this.SpelerNietAanZet;

            int tel;
            for (int r = 0; r < 8; r++)
            {
                tel = 0;
                for (int i = 1; richtingen[r](x, y, i); i++)
                {
                    testX = riXen[r](x, y, i);
                    testY = riYen[r](x, y, i);
                    stukje veld = this[testX, testY];
                    if (veld == spelernietaanzet) tel++;
                    else
                    {
                        if (veld == stukje.leeg) tel = 0;       // moet wel een eigen stuk zitten he
                        aantal += tel;
                        break;
                    }
                }
                if (snel && aantal > 0) return aantal;
            }
            return aantal;
        }
        public void MaakZet(int x, int y)
        {
            stukje spelernietaanzet = this.SpelerNietAanZet;
            this[x, y] = SpelerAanZet;
            int tel;
            for (int r = 0; r < 8; r++)
            {
                tel = 0;
                for (int i = 1; richtingen[r](x, y, i); i++)
                {
                    stukje veld = this[riXen[r](x, y, i), riYen[r](x, y, i)];
                    if (veld == spelernietaanzet)
                        tel++;
                    else
                    {
                        if (veld == stukje.leeg) tel = 0;
                        for (; tel > 0; tel--)
                        {
                            this[riXen[r](x, y, tel), riYen[r](x, y, tel)] = SpelerAanZet;
                        }
                        break;
                    }
                }
            }
            this.SpelerAanZet = spelernietaanzet;
        }
        public void BeeindigSpel()
        {
            Tuple<int, int, int> score = this.Score;
            if (score.Item1 == score.Item2)
                MessageBox.Show("Gelijkspel! " + score.Item1 + "-" + score.Item2);
            else if (score.Item1 > score.Item2)
                MessageBox.Show("Blauw wint! " + score.Item1 + "-" + score.Item2);
            else
                MessageBox.Show("Rood wint! " + score.Item1 + "-" + score.Item2);
            this.spelActief = false;
            this.beurtOvergeslagen = false;
        }
        public Tuple<int, int> BepaalBesteZet()
        {
            int besteX = -1, besteY = -1; // waarde komt er alleen uit bij onmogelijke zet
            int maxgeslagen = 0; int zetwinst;
            for (int x = 0; x < this.Breedte; x++)
            {
                for (int y = 0; y < this.Hoogte; y++)
                {
                    zetwinst = this.ControleerZet(x, y, false);
                    if (zetwinst > maxgeslagen)
                    {
                        maxgeslagen = zetwinst;
                        besteX = x;
                        besteY = y;
                    }
                }
            }
            return new Tuple<int, int>(besteX, besteY);
        }
    }

}