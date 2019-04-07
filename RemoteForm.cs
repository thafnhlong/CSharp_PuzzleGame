using System;
using System.Windows.Forms;

namespace Do_An_1
{
    public enum Direction
    {
        up, left, right, down
    }
    public partial class RemoteForm : Form
    {
        public delegate void DirectionHandle(Direction direction);
        public event DirectionHandle MovePiece;
        public delegate void ChangePieceCurrentHandle(ref int number, bool increase);
        public event ChangePieceCurrentHandle ChangePieceCurrent;


        public RemoteForm()
        {
            InitializeComponent();
            button1.Click += Left_Click;
            button2.Click += Up_Click;
            button3.Click += Right_Click;
            button4.Click += Down_Click;
            button5.Click += Decrease_Click;
            button6.Click += Increase_Click;
            this.Load += RemoteForm_Load;
        }

        private void RemoteForm_Load(object sender, EventArgs e)
        {
            Decrease_Click(null, null);
        }

        private void Increase_Click(object sender, EventArgs e)
        {
            int Count = int.Parse(label1.Text) - 1;
            ChangePieceCurrent?.Invoke(ref Count, true);
            label1.Text = (Count + 1).ToString();
        }

        private void Decrease_Click(object sender, EventArgs e)
        {
            int Count = int.Parse(label1.Text) - 1;
            ChangePieceCurrent?.Invoke(ref Count, false);
            label1.Text = (Count + 1).ToString();
        }

        private void Down_Click(object sender, EventArgs e)
        {
            MovePiece?.Invoke(Direction.down);
        }

        private void Right_Click(object sender, EventArgs e)
        {
            MovePiece?.Invoke(Direction.right);
        }

        private void Up_Click(object sender, EventArgs e)
        {
            MovePiece?.Invoke(Direction.up);
        }

        private void Left_Click(object sender, EventArgs e)
        {
            MovePiece?.Invoke(Direction.left);
        }
    }
}
