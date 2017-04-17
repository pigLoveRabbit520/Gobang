using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Chess
{
    class Chessman//棋子类
    {
        static Bitmap blackchess = new Bitmap(Chess.Properties.Resources.black);
        static Bitmap whitechess = new Bitmap(Chess.Properties.Resources.white);
        public int X { get; set; }
        public int Y { get; set; }
        public int Color { get; set; }

        public void AddedToChessBoard(PictureBox[,] Chesspics)
        {
            Chesspics[X, Y].Visible = true;
            if (Color == 0)
            {
                Chesspics[X, Y].Image = blackchess;
            }
            else
            {
                Chesspics[X, Y].Image = whitechess;
            }
        }
    }
}
