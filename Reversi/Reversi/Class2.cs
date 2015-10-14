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

        public Steen(double posX, double posY, bool green)
        {
            this.posX = posX * grootte + 2;
            this.posY = posY * grootte + 2;

            if (green)
                color = Color.FromArgb(178, 255, 102);
            else
                color = Color.FromArgb(153, 255, 255);
        }

        public void DrawSteen(object o, PaintEventArgs pea)
        {
            Brush brush = new SolidBrush(color);
            pea.Graphics.FillEllipse(brush, (float)posX, (float)posY, size, size);
            pea.Graphics.DrawEllipse(Pens.Black,(float)posX, (float)posY, size, size);
        }

        public void LegeSteen(double xPos, double yPos)
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
