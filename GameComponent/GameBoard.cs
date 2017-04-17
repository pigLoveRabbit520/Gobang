using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GameComponent
{
    public partial class GameBoard: Panel
    {

        private enum GameMode
        {
            PersonToPerson,
            PersonToComputer
        }
        #region 字段数据和存储数组
        private static Bitmap blackChessImg = GameComponent.Properties.Resources.black;
        private static Bitmap whiteChessImg = GameComponent.Properties.Resources.white;
        //size*size个点的游戏，横向size个点，纵向size个点
        private const int gameSize = 15;
        //一个格子的尺寸
        private const int blockSize = 30;
        //棋子图片尺寸
        private int chessmanSize = 24;
        //坐标偏移量
        private static int delta = 42;
        //有效距离的平方
        private static int eDistance = 100;
        //坐标点文字大小
        private static int textSize = 10;
        //坐标点文字颜色
        private static Color textColor = Color.FromArgb(38, 38, 38);
        //判断游戏是否开始
        private bool isStart = false;
        //是否电脑先行
        private bool isComputerfirst = false;
        //判断棋子颜色,1表示黑棋，2表示白棋
        private int color = 1;
        //游戏落子记录表，0表示空，1表示黑棋，2表示白棋
        private int[,] map = new int[gameSize, gameSize];
        //横向计数
        private int hSum;
        //纵向计数
        private int vSum;
        //左斜线计数
        private int lSum;
        //右斜线计数
        private int rSum;
        // 计算后的控件尺寸
        private int boradSize;

        //定义棋子数改变事件
        public event EventHandler<ChessmenNumChangeEventArgs> OnChessmenNumChange;
        //定义游戏结束事件
        public event EventHandler<GameEndEventArgs> OnGameEnd;
        #endregion

        public GameBoard()
        {
            SetStyle(ControlStyles.UserPaint, true);
            // 禁止擦除背景.
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            // 双缓冲
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            boradSize = GetGameBoardSize();
            this.Size = new Size(boradSize, boradSize);
            Createbackgroudimage();
        }

        /// <summary>
        /// 获得游戏面板的大小
        /// </summary>
        /// <returns></returns>
        private int GetGameBoardSize()
        {
            return (gameSize - 1) * blockSize + delta * 2;
        }

        /// <summary>
        /// 工具方法，把内部坐标变成屏幕上坐标
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <returns></returns>
        private Point IndexToScreen(int x, int y)
        {
            return new Point(x * blockSize + delta, y * blockSize + delta);
        }

        /// <summary>
        /// 工具方法，把屏幕坐标转化为内部坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Point ScreenToIndex(int x, int y)
        {
            return new Point((x - delta) / blockSize, (y - delta) / blockSize);
        }

        /// <summary>
        /// 画gameSize*gameSize的棋盘的背景图
        /// </summary>
        private void Createbackgroudimage()
        {
            int boardSize = GetGameBoardSize();
            Bitmap img = new Bitmap(boardSize, boardSize);
            Graphics g = Graphics.FromImage(img);
            drawLinesAndCircles(boardSize, g);
            drawCoordinateText(g);
            g.Dispose();
            this.BackgroundImage = img;
        }

        /// <summary>
        /// 绘制横竖线和小原点
        /// </summary>
        /// <param name="boardSize"></param>
        /// <param name="g"></param>
        private void drawLinesAndCircles(int boardSize, Graphics g)
        {
            g.FillRectangle(Brushes.BurlyWood, new Rectangle(0, 0, boardSize, boardSize));
            //创建Pen对象
            Pen mypen = new Pen(Color.Black, 1);
            SolidBrush brush = new SolidBrush(Color.Black);
            //(delta, delta)为初始点，每个格子宽度为blockSize
            for (int i = 0; i < gameSize; i++)
            {
                //画横线
                Point pHorizontal1 = IndexToScreen(0, i);
                Point pHorizontal2 = IndexToScreen(gameSize - 1, i);
                g.DrawLine(mypen, pHorizontal1, pHorizontal2);
                //画竖线
                Point pVertical1 = IndexToScreen(i, 0);
                Point pVertical2 = IndexToScreen(i, gameSize - 1);
                g.DrawLine(mypen, pVertical1, pVertical2);
            }
            //画圆圈
            Point p1 = IndexToScreen(3, 3);
            Point p2 = IndexToScreen(12, 3);
            Point p3 = IndexToScreen(3, 12);
            Point p4 = IndexToScreen(12, 12);
            g.FillEllipse(brush, p1.X - 3, p1.Y - 3, 6, 6);
            g.FillEllipse(brush, p2.X - 3, p2.Y - 3, 6, 6);
            g.FillEllipse(brush, p3.X - 3, p3.Y - 3, 6, 6);
            g.FillEllipse(brush, p4.X - 3, p4.Y - 3, 6, 6);
        }

       /// <summary>
        /// 绘制坐标文字
       /// </summary>
       /// <param name="g"></param>
        private void drawCoordinateText(Graphics g)
        {
            Font myFont = new Font("宋体", textSize, FontStyle.Bold);
            Brush brush = new SolidBrush(textColor);
            StringFormat format = new StringFormat();
            //垂直居中
            format.LineAlignment = StringAlignment.Center;  
            //水平居中
            format.Alignment = StringAlignment.Center;
            for (int i = 0; i < gameSize; i++)
            {
                Point pHorizontal   = IndexToScreen(i, 0);
                Point pVertical     = IndexToScreen(0, i);
                Rectangle rHorizontal = new Rectangle(pHorizontal.X - blockSize / 2, pHorizontal.Y - blockSize - chessmanSize / 2, blockSize, blockSize);
                Rectangle rVertical = new Rectangle(pVertical.X - blockSize - chessmanSize / 2, pVertical.Y - blockSize / 2, blockSize, blockSize);
                g.DrawString((i + 1).ToString(), myFont, brush, rHorizontal, format);
                g.DrawString((i + 1).ToString(), myFont, brush, rVertical, format);
            }
        }

        /// <summary>
        /// 是否为有效的点击点，是则返回内部坐标
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <param name="p">内部坐标</param>
        /// <returns></returns>
        private bool IsEffectiveArea(int x, int y, ref Point p)
        {
            for (int i = 0; i <= gameSize; i++)//二重的循环
            {
                for (int j = 0; j <= gameSize; j++)
                {
                    Point pMain = IndexToScreen(i, j);
                    Point pCompare = new Point(x, y);
                    int distance = CalDistance(pMain, pCompare);
                    if (distance <= eDistance)
                    {
                        if(map[j, i] != 0)
                        {
                            return false;
                        }
                        else
                        {
                            p.X = i;
                            p.Y = j;
                            return true;
                        }
                       
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 计算两个点之间的距离的平方
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private int CalDistance(Point p1, Point p2)
        {
            int deltaX = p1.X - p2.X;
            int deltaY = p1.Y - p2.Y;
            int distance = deltaX * deltaX + deltaY * deltaY;
            return distance;
        }

        /// <summary>
        /// 各个方向上计数
        /// </summary>
        /// <param name="xpos">横坐标</param>
        /// <param name="ypos">纵坐标</param>
        /// <param name="color">棋子颜色</param>
        private void AllDirectionsCount(int xpos, int ypos, int color)
        {
            //水平方向的计数
            hSum = 1;
            for (int i = xpos - 1; i >= 0; i--)
            {
                if (!ExistSameColor(i, ypos, color))
                {
                    break;
                }
                hSum++;
            }
            for (int i = xpos + 1; i <= gameSize; i++)
            {
                if (!ExistSameColor(i, ypos, color))
                {
                    break;
                }
                hSum++;
            }

            //纵向的计数
            vSum = 1;
            for (int i = ypos - 1; i >= 0; i--)
            {
                if (!ExistSameColor(xpos, i, color))
                {
                    break;
                }
                vSum++;
            }
            for (int i = ypos + 1; i <= gameSize; i++)
            {
                if (!ExistSameColor(xpos, i, color))
                {
                    break;
                }
                vSum++;
            }

            //左斜线计数
            lSum = 1;
            for (int i = xpos - 1, j = ypos - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (!ExistSameColor(i, j, color))
                {
                    break;
                }
                lSum++;
            }
            for (int i = xpos + 1, j = ypos + 1; i <= gameSize && j <= gameSize; i++, j++)
            {
                if (!ExistSameColor(i, j, color))
                {
                    break;
                }
                lSum++;
            }
            //右斜线的判断
            rSum = 1;
            for (int i = xpos - 1, j = ypos + 1; i >= 0 && j <= gameSize; i--, j++)
            {
                if (!ExistSameColor(i, j, color))
                {
                    break;
                }
                rSum++;
            }
            for (int i = xpos + 1, j = ypos - 1; i <= gameSize && j >= 0; i++, j--)
            {
                if (!ExistSameColor(i, j, color))
                {
                    break;
                }
                rSum++;//横向的计数
            }
        }


        /// <summary>
        /// 判断游戏是否结束
        /// </summary>
        /// <param name="p">内部坐标点</param>
        /// <returns></returns>
        private bool IsGameEnd(Point p)
        {
            AllDirectionsCount(p.X, p.Y, color);
            if (hSum >= 5 || vSum >= 5 || lSum >= 5 || rSum >= 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 添加新的棋子
        /// </summary>
        /// <param name="p">屏幕上的点</param>
        /// <param name="color"></param>
        private void AddChessman(Point p, int color)
        {
            Bitmap img = color == 1 ? blackChessImg : whiteChessImg;
            Graphics g = Graphics.FromImage(this.BackgroundImage);
            p.X = p.X - chessmanSize / 2;
            p.Y = p.Y - chessmanSize / 2;
            g.DrawImage(img, p.X, p.Y, chessmanSize, chessmanSize);
            g.Dispose();
            this.Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (isStart == true)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Point p = new Point(0, 0);
                    if(IsEffectiveArea(e.X, e.Y, ref p))
                    {
                        map[p.Y, p.X] = color;
                        AddChessman(IndexToScreen(p.X, p.Y), color);
                        if (IsGameEnd(p))
                        {
                            OnGameEnd(this, new GameEndEventArgs(color));
                            isStart = false;
                        }
                        else
                        {
                            color = 3 - color;
                            Point pCom = ComputerNextStep(color);
                            AddChessman(IndexToScreen(pCom.X, pCom.Y), color);
                            map[pCom.Y, pCom.X] = color;
                            if (IsGameEnd(pCom))
                            {
                                OnGameEnd(this, new GameEndEventArgs(color));
                                isStart = false;
                            }
                            else
                            {
                                color = 3 - color;
                            }
                        }
                    }
                }
            }
            base.OnMouseClick(e);
        }



        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            isStart = true;
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        public void ReStartGame()
        {
            Createbackgroudimage();
            map = new int[gameSize + 1, gameSize + 1];
            isStart = true;
        }
    }
}
