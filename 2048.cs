namespace _2048_new
{
    public partial class Form1 : Form
    {
        int[,] grid = new int[4, 4];
        Label[,] labels = new Label[4, 4];
        Random random = new Random();
        Button restartButton = new Button();
        Button startButton = new Button(); // Tombol Start
        Label scoreLabel = new Label();
        int score = 0;

        public Form1()
        {
            InitializeComponent();
            this.Text = "2048 Game";
            this.BackColor = ColorTranslator.FromHtml("#FAF8EF");
            InitScoreLabel();
            InitGrid();
            InitRestartButton();
            InitStartButton(); // Inisialisasi tombol Start
            this.KeyDown += Form1_KeyDown;
        }

        private void InitScoreLabel()
        {
            scoreLabel.Text = "Score: 0";
            scoreLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            scoreLabel.Location = new Point(10, 370);
            scoreLabel.Size = new Size(200, 40);
            scoreLabel.BackColor = Color.Transparent;
            this.Controls.Add(scoreLabel);
        }

        private void InitGrid()
        {
            this.ClientSize = new Size(400, 420);
            int tileSize = 80;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Label lbl = new Label();
                    lbl.Size = new Size(tileSize, tileSize);
                    lbl.Location = new Point(j * (tileSize + 10) + 10, i * (tileSize + 10) + 10);
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.Font = new Font("Arial", 20, FontStyle.Bold);
                    lbl.BackColor = Color.LightGray;
                    lbl.BorderStyle = BorderStyle.FixedSingle;
                    this.Controls.Add(lbl);
                    labels[i, j] = lbl;
                }
            }
        }

        private void InitRestartButton()
        {
            restartButton.Text = "Restart";
            restartButton.Font = new Font("Arial", 10, FontStyle.Bold);
            restartButton.Size = new Size(80, 30);
            restartButton.Location = new Point(300, 370);
            restartButton.Click += (s, e) => StartGame();
            this.Controls.Add(restartButton);
        }

        private void InitStartButton()
        {
            startButton.Text = "Start";
            startButton.Font = new Font("Arial", 10, FontStyle.Bold);
            startButton.Size = new Size(80, 30);
            startButton.Location = new Point(300, 330); // Posisi tombol start
            startButton.Click += (s, e) => StartGame(); // Event handler untuk Start
            this.Controls.Add(startButton);
        }

        private void StartGame()
        {
            Array.Clear(grid, 0, grid.Length);
            score = 0;
            UpdateScore();
            AddRandomTile();
            AddRandomTile();
            DrawGrid();
            startButton.Enabled = false; // Disable tombol start setelah game dimulai
        }

        private void UpdateScore()
        {
            scoreLabel.Text = $"Score: {score}";
        }

        private void DrawGrid()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    int val = grid[i, j];
                    labels[i, j].Text = val == 0 ? "" : val.ToString();
                    labels[i, j].BackColor = GetTileColor(val);
                    labels[i, j].ForeColor = GetTextColor(val);
                }
        }

        private void AddRandomTile()
        {
            List<Point> empty = new List<Point>();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (grid[i, j] == 0) empty.Add(new Point(i, j));

            if (empty.Count > 0)
            {
                var point = empty[random.Next(empty.Count)];
                grid[point.X, point.Y] = random.Next(1, 3) * 2;
            }
        }

        private void Move(Func<int, int, (int, int)> get, Action<int, int, int> set)
        {
            bool moved = false;

            for (int i = 0; i < 4; i++)
            {
                int[] line = new int[4];
                int idx = 0;

                for (int j = 0; j < 4; j++)
                {
                    var (x, y) = get(i, j);
                    int val = grid[x, y];
                    if (val != 0) line[idx++] = val;
                }

                for (int j = 0; j < 3; j++)
                {
                    if (line[j] != 0 && line[j] == line[j + 1])
                    {
                        line[j] *= 2;
                        score += line[j];
                        line[j + 1] = 0;
                        moved = true;
                    }
                }

                int[] newLine = new int[4];
                idx = 0;
                for (int j = 0; j < 4; j++)
                    if (line[j] != 0) newLine[idx++] = line[j];

                for (int j = 0; j < 4; j++)
                {
                    var (x, y) = get(i, j);
                    if (grid[x, y] != newLine[j])
                    {
                        grid[x, y] = newLine[j];
                        moved = true;
                    }
                }
            }

            if (moved)
            {
                AddRandomTile();
                DrawGrid();
                UpdateScore();
                if (IsGameOver())
                {
                    MessageBox.Show("Game Over!", "2048", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void MoveLeft() =>
            Move((i, j) => (i, j), (i, j, v) => grid[i, j] = v);

        private void MoveRight() =>
            Move((i, j) => (i, 3 - j), (i, j, v) => grid[i, 3 - j] = v);

        private void MoveUp() =>
            Move((i, j) => (j, i), (i, j, v) => grid[j, i] = v);

        private void MoveDown() =>
            Move((i, j) => (3 - j, i), (i, j, v) => grid[3 - j, i] = v);

        private bool IsGameOver()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (grid[i, j] == 0)
                        return false;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                    if (grid[i, j] == grid[i, j + 1]) return false;

                for (int j = 0; j < 3; j++)
                    if (grid[j, i] == grid[j + 1, i]) return false;
            }

            return true;
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) MoveLeft();
            else if (e.KeyCode == Keys.Right) MoveRight();
            else if (e.KeyCode == Keys.Up) MoveUp();
            else if (e.KeyCode == Keys.Down) MoveDown();
        }

        private Color GetTileColor(int value)
        {
            return value switch
            {
                0 => ColorTranslator.FromHtml("#CDC1B4"),
                2 => ColorTranslator.FromHtml("#EEE4DA"),
                4 => ColorTranslator.FromHtml("#EDE0C8"),
                8 => ColorTranslator.FromHtml("#F2B179"),
                16 => ColorTranslator.FromHtml("#F59563"),
                32 => ColorTranslator.FromHtml("#F67C5F"),
                64 => ColorTranslator.FromHtml("#F65E3B"),
                128 => ColorTranslator.FromHtml("#EDCF72"),
                256 => ColorTranslator.FromHtml("#EDCC61"),
                512 => ColorTranslator.FromHtml("#EDC850"),
                1024 => ColorTranslator.FromHtml("#EDC53F"),
                2048 => ColorTranslator.FromHtml("#EDC22E"),
                _ => Color.Black,
            };
        }

        private Color GetTextColor(int value)
        {
            return value <= 4 ? ColorTranslator.FromHtml("#776E65") : ColorTranslator.FromHtml("#F9F6F2");
        }

        private void Form1_Load(object sender, EventArgs e) { }
    }
}
