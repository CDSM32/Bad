using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bad
{
    public partial class Form2 : Form
    {
        private Panel levelSelectionPanel; // Panneau de sélection du niveau
        public int SelectedLevel { get; private set; } // Niveau sélectionné

        public Form2()
        {
            InitializeComponent();
            this.Text = "Sélection du niveau";
            this.Size = new Size(500, 400);

            InitializeLevelSelection();
        }

        private void InitializeLevelSelection()
        {
            // Panneau de sélection
            levelSelectionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };

            Label titleLabel = new Label
            {
                Text = "Choisissez votre niveau",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, 30)
            };
            levelSelectionPanel.Controls.Add(titleLabel);

            // Liste des niveaux
            string[] levels = { "Débutant (NC)", "Amateur (P12/11/10)", "Intermédiaire (D9/8/7)", "Professionnel (R6/5/4)", "Expert (N3/2/1)" };
            for (int i = 0; i < levels.Length; i++)
            {
                Button levelButton = new Button
                {
                    Text = levels[i],
                    Tag = i + 1, // Stocker le niveau correspondant
                    Size = new Size(400, 50),
                    Location = new Point(50, 80 + (i * 55)),
                    BackColor = Color.CornflowerBlue,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 14, FontStyle.Bold)
                };
                levelButton.Click += LevelButton_Click;
                levelSelectionPanel.Controls.Add(levelButton);
            }

            this.Controls.Add(levelSelectionPanel);
        }
        private void Form2_Load(object sender, EventArgs e) { }
        private void LevelButton_Click(object sender, EventArgs e)
        {
            // Récupérer le niveau choisi
            Button clickedButton = sender as Button;
            SelectedLevel = (int)clickedButton.Tag;

            // Ouvrir Form1 avec le niveau choisi
            this.Hide();
            Form1 form1 = new Form1(SelectedLevel);
            form1.ShowDialog();
            this.Close();
        }
    }
}