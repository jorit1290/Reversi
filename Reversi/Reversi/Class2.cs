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

        int posX, posY;
        int size = 46;

        public Steen(int posX, int posY, bool red)
        {
            this.posX = posX * 50 + 2;
            this.posY = posY * 50 + 2;

            if (red)
                color = Color.Red;
            else
                color = Color.Blue;
        }

        public void Draw(object o, PaintEventArgs pea)
        {
            Brush brush = new SolidBrush(color);
            pea.Graphics.FillEllipse(brush, posX, posY, size, size);
        }
    }
}
