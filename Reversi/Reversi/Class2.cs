using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Reversi
{
    class Steen
    {
        Color color;

        double posX, posY, xPos, yPos;
        int size = 46;
        int grootte = 50;

        public Steen(double posX, double posY, bool red)
        {
            this.posX = posX * grootte + 2;
            this.posY = posY * grootte + 2;

            if (red)
                color = Color.Red;
            else
                color = Color.Blue;
        }

        public void DrawSteen(object o, PaintEventArgs pea)
        {
            Brush brush = new SolidBrush(color);
            pea.Graphics.FillEllipse(brush, (float)posX, (float)posY, size, size);
        }

        public void LegeSteen(double xPos, double yPos, bool red)
        {
            this.xPos = xPos * grootte + 7;
            this.yPos = yPos * grootte + 7;
            size -= 10;
        }

        public void DrawLegeSteen(object o, PaintEventArgs pea)
        {
            pea.Graphics.DrawEllipse(Pens.Black, (float)xPos,(float)yPos, size, size);
        }
    }
}
