using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Do_An_1
{
    public partial class Form1 : Form
    {

        private Point StartPoint = new Point(6, 19); // groupboxlocation margin + padding
        private Point StopPoint = new Point(536 - 6, 495 - 6); // groupboxsize - margin - padding

        private Size SizeMax = new Size(524, 470); // stoppoint - startpoint
        private Size SizePiece;
        private int NumofRow;
        private int NumofCol;

        private CustomPictureBox PieceSelected = null;
        private List<Control> ArrayPiece;

        private Form HelpForm = new Form();
        private RemoteForm Remote;

        public Form1()
        {
            InitializeComponent();
            button1.Click += Button_Click;

            HelpForm.Text = "Giúp đỡ";
            HelpForm.Width = (int)(SizeMax.Width / 1.4);
            HelpForm.Height = (int)(SizeMax.Height / 1.4); ;
            HelpForm.MaximizeBox = HelpForm.MinimizeBox = false;
            HelpForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            HelpForm.BackgroundImageLayout = ImageLayout.Stretch;
            HelpForm.FormClosing += HelpForm_FormClosing;
        }
        private void HelpForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                checkBox1.Checked = false;
                e.Cancel = true;
                (sender as Form).Hide();
            }
        }
        private void Button_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn hình";
                dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    NewGame();
                    checkBox1.Enabled = checkBox2.Enabled = true;
                    Image MyImage = Image.FromFile(dialog.FileName);
                    HelpForm.BackgroundImage = MyImage;

                    NumofRow = (int)numericUpDown1.Value;
                    NumofCol = (int)numericUpDown2.Value;
                    SizePiece.Width = SizeMax.Width / NumofCol;
                    SizePiece.Height = SizeMax.Height / NumofRow;

                    Bitmap MyImageResize = new Bitmap(MyImage, SizeMax);
                    Random rnd = new Random();

                    for (int row = 0; row < NumofRow; row++)
                    {
                        for (int col = 0; col < NumofCol; col++)
                        {
                            Bitmap PiecePicture = new Bitmap(SizePiece.Width, SizePiece.Height);
                            using (Graphics Graphic = Graphics.FromImage(PiecePicture))
                            {
                                Point Postitive = new Point(col * SizePiece.Width, row * SizePiece.Height);
                                Rectangle PostitivePicturePiece = new Rectangle(Postitive, SizePiece);
                                Graphic.DrawImage(MyImageResize, 0, 0, PostitivePicturePiece, GraphicsUnit.Pixel);
                            }
                            CustomPictureBox Piece = new CustomPictureBox(PiecePicture);
                            Piece.Tag = string.Format("{0},{1}", row, col);

                            Piece.Top = StartPoint.Y + rnd.Next(StopPoint.Y - SizePiece.Height);
                            Piece.Left = StartPoint.X + rnd.Next(StopPoint.X - SizePiece.Width); ;

                            Piece.MouseUpAutoLocation += PieceAutoLocation;
                            Piece.MouseUpCheckWin += PieceCheckWin;

                            groupBox2.Controls.Add(Piece);
                        }
                    }

                    //REMOTE...
                    ArrayPiece = new List<Control>();
                    foreach (Control item in groupBox2.Controls)
                    {
                        ArrayPiece.Add(item);
                    }
                    //random vi tri
                    for (int t = 0; t < ArrayPiece.Count; t++)
                    {
                        Control tmp = ArrayPiece[t];
                        int r = rnd.Next(t, ArrayPiece.Count);
                        ArrayPiece[t] = ArrayPiece[r];
                        ArrayPiece[r] = tmp;
                    }
                }
            }

        }

        private void NewGame()
        {
            Remote?.Close();
            HelpForm.Hide();
            checkBox1.Enabled = checkBox2.Enabled = false;
            checkBox1.Checked = checkBox2.Checked = false;
            groupBox2.Controls.Clear();
        }
        private void PieceAutoLocation(object sender, EventArgs e)
        {
            CustomPictureBox CurrentPiece = sender as CustomPictureBox;
            int realtop = CurrentPiece.Top - StartPoint.Y;
            int newtop = StartPoint.Y + (int)Math.Round((double)realtop / CurrentPiece.Height) * CurrentPiece.Height;
            if (newtop > StopPoint.Y - CurrentPiece.Height)
                newtop = StopPoint.Y - CurrentPiece.Height;
            else if (newtop < StartPoint.Y)
                newtop = StartPoint.Y;
            CurrentPiece.Top = newtop;

            int realleft = CurrentPiece.Left - StartPoint.X;
            int newleft = StartPoint.X + (int)Math.Round((double)realleft / CurrentPiece.Width) * CurrentPiece.Width;
            if (newleft > StopPoint.X - CurrentPiece.Width)
                newleft = StopPoint.X - CurrentPiece.Width;
            else if (newleft < StartPoint.X)
                newleft = StartPoint.X;
            CurrentPiece.Left = newleft;
        }
        private void PieceCheckWin(object sender, EventArgs e)
        {
            foreach (CustomPictureBox Piece in groupBox2.Controls)
            {
                string[] rc = (Piece.Tag.ToString()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int row = int.Parse(rc[0]);
                int col = int.Parse(rc[1]);

                decimal RowofPiece = (decimal)(Piece.Top - StartPoint.Y) / SizePiece.Height;
                decimal ColofPiece = (decimal)(Piece.Left - StartPoint.X) / SizePiece.Width;
                if (row != RowofPiece || col != ColofPiece) return;
            }
            MessageBox.Show("Chúc mừng bạn đã chiến thắng!!!\nTrò chơi mới bắt đầu!!!","WIN GAME");
            NewGame();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                HelpForm.Show();
            }
            else
            {
                HelpForm.Hide();
            }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                Remote = new RemoteForm();
                Remote.MovePiece += Remote_MovePiece;
                Remote.ChangePieceCurrent += Remote_ChangePieceCurrent;
                Remote.FormClosed += (a, b) => { PieceSelected?.SetHighlight(false); };
                Remote.ShowDialog();
                (sender as CheckBox).Checked = false;
            }
        }



        private void Remote_ChangePieceCurrent(ref int number, bool increase)
        {
            number += increase ? 1 : -1;
            if (number < 0)
                number = ArrayPiece.Count - 1;
            else if (number == ArrayPiece.Count)
                number = 0;
            PieceSelected?.SetHighlight(false);
            PieceSelected = ArrayPiece[number] as CustomPictureBox;
            PieceSelected.BringToFront();
            PieceSelected.SetHighlight(true);
        }

        private void Remote_MovePiece(Direction direction)
        {
            int updown = 0,
                leftrigh = 0;
            switch (direction)
            {
                case Direction.up:
                    updown = -1;
                    break;
                case Direction.left:
                    leftrigh = -1;
                    break;
                case Direction.right:
                    leftrigh = 1;
                    break;
                case Direction.down:
                    updown = 1;
                    break;
            }

            int realtop = PieceSelected.Top - StartPoint.Y;
            int newtop = StartPoint.Y + ((int)Math.Round((double)realtop / PieceSelected.Height) + updown) * PieceSelected.Height;
            if (newtop > StopPoint.Y - PieceSelected.Height)
                newtop = StopPoint.Y - PieceSelected.Height;
            else if (newtop < StartPoint.Y)
                newtop = StartPoint.Y;
            PieceSelected.Top = newtop;

            int realleft = PieceSelected.Left - StartPoint.X;
            int newleft = StartPoint.X + (int)Math.Round((double)realleft / PieceSelected.Width + leftrigh) * PieceSelected.Width;
            if (newleft > StopPoint.X - PieceSelected.Width)
                newleft = StopPoint.X - PieceSelected.Width;
            else if (newleft < StartPoint.X)
                newleft = StartPoint.X;
            PieceSelected.Left = newleft;

            PieceCheckWin(null, null);
        }
    }
}
