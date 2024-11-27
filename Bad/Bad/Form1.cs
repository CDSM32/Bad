using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Bad
{

    public partial class Form1 : Form
    {
      

        private Timer delayTimer;
        
        private PictureBox Volant;
        private PictureBox LCW;
        private PictureBox Axelsen;
        private PictureBox Filet;

        private Timer lcwTimer; // Timer pour animer le déplacement de LCW
        private PictureBox selectedPictureBox1;  // Référence à l'image à déplacer
        private int stepSizeLCW = 20;  // Taille du pas de déplacement
        private Rectangle boundary1; // Limites de l'espace dans lequel l'image peut se déplacer
        private bool lcwMoving = false; // Indicateur si LCW se déplace

        private PictureBox selectedPictureBox;  // Référence à l'image à déplacer
        private int stepSizeaxelsen = 10;  // Taille du pas de déplacement
        private Rectangle boundary; // Limites de l'espace dans lequel l'image peut se déplacer

        private Timer VolantTimer;  // Timer pour simuler le déplacement du volant
        private Random randomvolant = new Random();
        private Rectangle movementZone;
        private int stepSizevolant = 5;  // Taille du pas pour le déplacement du volant

        private Point shuttlecockDestination; // Destination du volant
        private Point shuttlecockDestination2;
        private string lastPlayerTouched = null; // Dernier joueur à avoir touché le volant
        private int numset = 1;
        private int lcwScoreset1 = 0; // Score pour LCW
        private int axelsenScoreset1 = 0; // Score pour Axelsen
        private int lcwScoreset2 = 0; // Score pour LCW
        private int axelsenScoreset2 = 0; // Score pour Axelsen
        private int lcwScoreset3 = 0; // Score pour LCW
        private int axelsenScoreset3 = 0; // Score pour Axelsen
        private int lcwSet = 0; // Score pour LCW
        private int axelsenSet = 0; // Score pour Axelsen

        private Point crossLocation; // Position de la croix
        private bool showCross = false; // Indicateur pour afficher la croix

        private Label temporaryMessageLabel;
        private Timer messageTimer;
        private int selectedLevel;

        public Form1(int level)
        {
          
                InitializeComponent();

              

       
            InitializeComponent();
            this.selectedLevel = level;
            this.Text = $"Match de Badminton - Niveau {selectedLevel}";
            this.Size = new Size(1500, 1000);

            this.Paint += Form1_Paint; // Activer le dessin personnalisé
            StartGame();
        

                // Ajuster les valeurs des timers en fonction du niveau
                switch (level)
                {
                    case 1: // Débutant
                    stepSizevolant = 4;
                    stepSizeLCW = 4;  // Taille du pas de déplacement
                    break;
                    case 2: // Amateur
                    stepSizevolant = 5;
                    stepSizeLCW = 5;
                    break;
                    case 3: // Intermédiaire
                    stepSizevolant = 6;
                    stepSizeLCW = 6;
                    break;
                    case 4: // Professionnel
                    stepSizevolant = 8;
                    stepSizeLCW = 8;
                    break;
                    case 5: // Expert
                    stepSizevolant = 10;
                    stepSizeLCW = 12;
                    break;

                }
            InitializeComponent();

            // Initialiser le Label
            temporaryMessageLabel = new Label();
            temporaryMessageLabel.AutoSize = true;
            temporaryMessageLabel.Location = new Point(810, 400);  // Choisissez la position souhaitée
            temporaryMessageLabel.Font = new Font("Arial", 50);  // Personnalisez la police
            temporaryMessageLabel.ForeColor = Color.Red;
            temporaryMessageLabel.Visible = false;  // Caché par défaut
            this.Controls.Add(temporaryMessageLabel);

            // Initialiser le Timer
            messageTimer = new Timer();
            messageTimer.Interval = 1000;  // 1 seconde (1000 ms)
            messageTimer.Tick += MessageTimer_Tick;

            // Lier l'événement Paint au gestionnaire Form1_Paint
            this.Paint += new PaintEventHandler(Form1_Paint);


            // Initialiser le Timer pour animer le déplacement du volant
            VolantTimer = new Timer();
            VolantTimer.Tick += ShuttlecockTimer_Tick;
            VolantTimer.Interval = 1;  // Intervalle en millisecondes pour chaque mouvement


            lcwTimer = new Timer();
            lcwTimer.Tick += LCWTimer_Tick;
            lcwTimer.Interval = 50;
            

            // Initialiser et configurer le PictureBox
            Volant = new PictureBox();
            Volant.Location = new Point(1000, 600);  // Position dans le formulaire
            Volant.Size = new Size(50, 50);     // Taille du PictureBox
            Volant.SizeMode = PictureBoxSizeMode.StretchImage;  // Ajuste l'image à la taille du PictureBox

            LCW = new PictureBox();
            LCW.Location = new Point(700, 200);  // Position dans le formulaire
            LCW.Size = new Size(150, 150);     // Taille du PictureBox
            LCW.SizeMode = PictureBoxSizeMode.StretchImage;  // Ajuste l'image à la taille du PictureBox

            Axelsen = new PictureBox();
            Axelsen.Location = new Point(1100, 650);  // Position dans le formulaire
            Axelsen.Size = new Size(150, 150);     // Taille du PictureBox
            Axelsen.SizeMode = PictureBoxSizeMode.StretchImage;  // Ajuste l'image à la taille du PictureBox

            Filet = new PictureBox();
            Filet.Location = new Point(500, 475);  // Position dans le formulaire
            Filet.Size = new Size(900, 50);     // Taille du PictureBox
            Filet.SizeMode = PictureBoxSizeMode.StretchImage;  // Ajuste l'image à la taille du PictureBox

            // Charger l'image
            Volant.Image = Image.FromFile("C:\\Users\\marti\\source\\repos\\Bad\\volant.jpg");  // Charger l'image du volant
            LCW.Image = Image.FromFile("C:\\Users\\marti\\source\\repos\\Bad\\Joueur.jpg");  // Charger l'image de LCW
            Axelsen.Image = Image.FromFile("C:\\Users\\marti\\source\\repos\\Bad\\axelsen.jpg");  // Charger l'image d'Axelsen
            Filet.Image = Image.FromFile("C:\\Users\\marti\\source\\repos\\Bad\\filet.jpg");  // Charger l'image du filet

            // Ajouter les PictureBox au formulaire
            this.Controls.Add(Volant);
            this.Controls.Add(LCW);
            this.Controls.Add(Axelsen);
            this.Controls.Add(Filet);

          

          

            
            // Définir les limites dans lesquelles l'image peut se déplacer
            boundary = new Rectangle(500, 500, 900, 450);
            boundary1 = new Rectangle(0, 0, 1500, 1500);

            // Définir l'image à déplacer
            selectedPictureBox = Axelsen;
            selectedPictureBox1 = LCW;

            // Lier l'événement KeyDown pour capturer les touches du clavier
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.KeyPreview = true;

            RotateVolantImage(180);

            
        }
        private void StartGame()
        {
            // Configurez les timers ou règles en fonction du niveau choisi
            
        }
        private void StartDelay()
        {
            delayTimer = new Timer();
            delayTimer.Interval = 1; // 2 secondes
            delayTimer.Tick += OnDelayCompleted;
            delayTimer.Start();
        }
        private void OnDelayCompleted(object sender, EventArgs e)
        {
            // Arrêter le Timer
            delayTimer.Stop();
            StartMovementLCW();
        }
        private void ShowTemporaryMessage(string message)
        {
            temporaryMessageLabel.Text = message;
            temporaryMessageLabel.Visible = true;
            messageTimer.Start();  // Démarrer le Timer pour cacher le message après 1 seconde
        }
        private void MessageTimer_Tick(object sender, EventArgs e)
        {
            temporaryMessageLabel.Visible = false;
            messageTimer.Stop();  // Arrêter le Timer
        }
        private void RotateVolantImage(float angle)
        {
            // Créer un nouveau Bitmap avec la taille de l'image originale
            Bitmap rotatedImage = new Bitmap(Volant.Image.Width, Volant.Image.Height);

            // Utiliser Graphics pour effectuer la rotation
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Définir l'origine de la rotation au centre de l'image
                g.TranslateTransform((float)Volant.Image.Width / 2, (float)Volant.Image.Height / 2);

                // Appliquer la rotation
                g.RotateTransform(angle);

                // Revenir à l'origine pour dessiner l'image après la rotation
                g.TranslateTransform(-(float)Volant.Image.Width / 2, -(float)Volant.Image.Height / 2);

                // Dessiner l'image originale avec la rotation appliquée
                g.DrawImage(Volant.Image, new Point(0, 0));
            }
            // Remplacer l'image dans Volant avec l'image tournée
            Volant.Image = rotatedImage;
        }
        private void Form1_Load(object sender, EventArgs e) { }
        // Méthode pour redessiner le score
        private void UpdateScore()
        {
            this.Invalidate();  // Redessiner le formulaire (cela déclenche l'événement Paint)
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Récupère la position actuelle de l'image sélectionnée
            int x = selectedPictureBox.Location.X;
            int y = selectedPictureBox.Location.Y;

            // Vérifie quelle touche a été pressée et modifie la position en conséquence
            if (e.KeyCode == Keys.Q && x - stepSizeaxelsen >= boundary.Left)
            {
                x -= stepSizeaxelsen;  // Déplacer vers la gauche
            }
            else if (e.KeyCode == Keys.D && x + stepSizeaxelsen + selectedPictureBox.Width <= boundary.Right)
            {
                x += stepSizeaxelsen;  // Déplacer vers la droite
            }
            else if (e.KeyCode == Keys.Z && y - stepSizeaxelsen >= boundary.Top)
            {
                y -= stepSizeaxelsen;  // Déplacer vers le haut
            }
            else if (e.KeyCode == Keys.S && y + stepSizeaxelsen + selectedPictureBox.Height <= boundary.Bottom)
            {
                y += stepSizeaxelsen;  // Déplacer vers le bas
            }

            // Mettre à jour la position du PictureBox sélectionné
            selectedPictureBox.Location = new Point(x, y);

            if (lastPlayerTouched == null)
            {
                if (Axelsen.Bounds.IntersectsWith(Volant.Bounds))
                {
                    ScoreAxelsen();
                    RotateVolantImage(180);
                    lastPlayerTouched = "Axelsen";
                    // Commencer le déplacement du volant et de LCW ensemble
                    showCross = false;
                    // Redessiner uniquement la zone de la croix
                    int crossSize = 10; // Taille de la croix
                    this.Invalidate(new Rectangle(crossLocation.X - crossSize, crossLocation.Y - crossSize, crossSize * 2, crossSize * 2));
                    StartMovementvolant();
                    StartMovementLCW();
                }
            }
            // Vérifier si Axelsen touche le volant
            if (Axelsen.Bounds.IntersectsWith(Volant.Bounds))
            {
                if (lastPlayerTouched == "Axelsen")
                {
                }
                else
                {
                    RotateVolantImage(180);
                    lastPlayerTouched = "Axelsen";
                    // Commencer le déplacement du volant et de LCW ensemble
                    showCross = false;
                    // Redessiner uniquement la zone de la croix
                    int crossSize = 10; // Taille de la croix
                    this.Invalidate(new Rectangle(crossLocation.X - crossSize, crossLocation.Y - crossSize, crossSize * 2, crossSize * 2));
                    StartMovementvolant();
                    StartMovementLCW();

                }
            }
        }
        private void StartMovementAxelsen()
        {
            // Choisir une nouvelle position aléatoire pour le volant
            MoveShuttlecockToRandomPositionAxelsen();
            VolantTimer.Start();  // Démarrer le Timer du volant
            // Mettre à jour la position de la croix
            crossLocation = shuttlecockDestination;
            showCross = true; // Montrer la croix
        }
        // Démarrer le mouvement du volant et de LCW ensemble
        private void StartMovementLCW()
        {

            lcwMoving = true;  // Indiquer que LCW doit se déplacer



            lcwTimer.Start();     // Démarrer le Timer de LCW
        }
        private void StartMovementvolant()
        {
            // Choisir une nouvelle position aléatoire pour le volant
            MoveShuttlecockToRandomPositionLCW();
            VolantTimer.Start();
        }
        // Déplacer le volant de badminton vers une position aléatoire
        private void MoveShuttlecockToRandomPositionLCW()
        {
            if (movementZone.X == 450 && movementZone.Y == 25 && movementZone.Width == 450 && movementZone.Height == 375 || movementZone.X == 950 && movementZone.Y == 25 && movementZone.Width == 450 && movementZone.Height == 375)
            {
            }
            else
            {
                // Définir une nouvelle zone de mouvement aléatoire pour le volant
                movementZone = new Rectangle(500, 25, 900, 475);
            }

            // Choisir des coordonnées aléatoires dans la zone définie
            int randomX1 = randomvolant.Next(movementZone.Left, movementZone.Right);
            int randomY1 = randomvolant.Next(movementZone.Top, movementZone.Bottom);

            // Définir la nouvelle destination du volant
            shuttlecockDestination = new Point(randomX1, randomY1);

            // Redémarrer le Timer du volant pour qu'il se déplace vers la nouvelle destination
            VolantTimer.Start();
        }
        private void MoveShuttlecockToRandomPositionAxelsen()
        {
            if (movementZone.X == 950 && movementZone.Y == 600 && movementZone.Width == 450 && movementZone.Height == 375 || movementZone.X == 450 && movementZone.Y == 600 && movementZone.Width == 450 && movementZone.Height == 375)
            {
            }
            else
            {
                // Définir une nouvelle zone de mouvement aléatoire pour le volant
                movementZone = new Rectangle(500, 500, 900, 475);
            }
            // Choisir des coordonnées aléatoires dans la zone définie
            int randomX1 = randomvolant.Next(movementZone.Left, movementZone.Right);
            int randomY1 = randomvolant.Next(movementZone.Top, movementZone.Bottom);
            // Définir la nouvelle destination du volant
            shuttlecockDestination = new Point(randomX1, randomY1);
            // Redémarrer le Timer du volant pour qu'il se déplace vers la nouvelle destination
            VolantTimer.Start();
        }
        // Timer pour gérer le déplacement de LCW
        private void LCWTimer_Tick(object sender, EventArgs e)
        {
            if (lcwMoving)
            {
                // Déterminer la position actuelle de LCW
                int lcwx = LCW.Location.X;
                int lcwy = LCW.Location.Y;

                // Calculer la direction vers la destination du volant
                int deltaX = shuttlecockDestination.X - lcwx;
                int deltaY = shuttlecockDestination.Y - lcwy;

                // Normaliser le mouvement pour rendre LCW plus fluide
                if (Math.Abs(deltaX) > Math.Abs(deltaY))
                {
                    // Déplacer en X
                    lcwx += stepSizeLCW * Math.Sign(deltaX);
                    // Éviter la division par zéro
                    if (Math.Abs(deltaX) != 0)
                    {
                        lcwy += (stepSizeLCW * deltaY) / Math.Abs(deltaX);
                    }
                }
                else
                {
                    // Déplacer en Y
                    lcwy += stepSizeLCW * Math.Sign(deltaY);
                    // Éviter la division par zéro
                    if (Math.Abs(deltaY) != 0)
                    {
                        lcwx += (stepSizeLCW * deltaX) / Math.Abs(deltaY);
                    }
                }
                // Mettre à jour la position de LCW
                LCW.Location = new Point(lcwx, lcwy);
                // Vérifier si LCW touche le volant
                if (LCW.Bounds.IntersectsWith(Volant.Bounds))
                {
                    RotateVolantImage(180);
                    lastPlayerTouched = "LCW";
                    // Déplacer le volant vers une nouvelle position aléatoire
                    showCross = false;
                    // Redessiner uniquement la zone de la croix
                    int crossSize = 10; // Taille de la croix
                    this.Invalidate(new Rectangle(crossLocation.X - crossSize, crossLocation.Y - crossSize, crossSize * 2, crossSize * 2));
                    StartMovementAxelsen();
                    lcwTimer.Stop();  // Arrêter le déplacement de LCW
                }
            }
        }
        private void ScoreAxelsen()
        {
            if (numset == 3)
            {
                if (axelsenScoreset3 >= 21 && axelsenScoreset3 - lcwScoreset3 >= 2 || axelsenScoreset3 >= 30)
                {
                    lcwTimer.Stop();
                    MessageBox.Show("Axelsen a gagné ! Le match ");
                    Application.Exit();
                }
                else if (axelsenScoreset3 % 2 == 0)
                {
                    LCW.Location = new Point(700, 200);
                    Volant.Location = new Point(1000, 600);
                    Axelsen.Location = new Point(1100, 650);  // Position dans le formulaire
                    movementZone = new Rectangle(450, 25, 450, 375);
                }
                else
                {
                    LCW.Location = new Point(1100, 200);
                    Axelsen.Location = new Point(700, 650);
                    Volant.Location = new Point(900, 600);
                    movementZone = new Rectangle(950, 25, 450, 375);
                }
            }
            if (numset == 2)
            {
                if (axelsenScoreset2 >= 21 && axelsenScoreset2 - lcwScoreset2 >= 2 || axelsenScoreset2 >= 30)
                {
                    lcwTimer.Stop();
                    axelsenSet++;
                    numset++;


                    MessageBox.Show("Axelsen a gagné ! 1 set ");

                    if (axelsenSet == 2)
                    {
                        MessageBox.Show("Axelsen a gagné ! Le match ");
                        Application.Exit();
                    }
                }
                else if (axelsenScoreset2 % 2 == 0)
                {
                    LCW.Location = new Point(700, 200);
                    Volant.Location = new Point(1000, 600);
                    Axelsen.Location = new Point(1100, 650);  // Position dans le formulaire
                    movementZone = new Rectangle(450, 25, 450, 375);
                }
                else
                {
                    LCW.Location = new Point(1100, 200);
                    Axelsen.Location = new Point(700, 650);
                    Volant.Location = new Point(900, 600);
                    movementZone = new Rectangle(950, 25, 450, 375);
                }
            }
            if (numset == 1)
            {
                if (axelsenScoreset1 >= 21 && axelsenScoreset1 - lcwScoreset1 >= 2 || axelsenScoreset1 >= 30)
                {
                    lcwTimer.Stop();
                    axelsenSet++;
                    numset++;


                    MessageBox.Show("Axelsen a gagné ! 1 set ");
                }
                else if (axelsenScoreset1 % 2 == 0)
                {
                    LCW.Location = new Point(700, 200);
                    Volant.Location = new Point(1000, 600);
                    Axelsen.Location = new Point(1100, 650);  // Position dans le formulaire
                    movementZone = new Rectangle(450, 25, 450, 375);
                }
                else
                {
                    LCW.Location = new Point(1100, 200);
                    Axelsen.Location = new Point(700, 650);
                    Volant.Location = new Point(900, 600);
                    movementZone = new Rectangle(950, 25, 450, 375);
                }
            }
        }
        private void ScoreLCW()
        {
            if (numset == 3)
            {
                if (lcwScoreset3 >= 21 && lcwScoreset3 - axelsenScoreset3 >= 2 || lcwScoreset3 >= 30)
                {
                    lcwTimer.Stop();
                    MessageBox.Show("LCW a gagné ! Le match ");
                    Application.Exit();

                }
                else if (lcwScoreset3 % 2 == 0)
                {
                    LCW.Location = new Point(700, 200);
                    Axelsen.Location = new Point(1100, 650);
                    Volant.Location = new Point(900, 350);
                    shuttlecockDestination = new Point(900, 350);
                    StartDelay();
                    movementZone = new Rectangle(950, 600, 450, 375);
                }
                else
                {
                    Axelsen.Location = new Point(700, 650);
                    LCW.Location = new Point(1100, 200);
                    Volant.Location = new Point(1000, 350);
                    shuttlecockDestination = new Point(1000, 350);
                    StartDelay();
                    movementZone = new Rectangle(450, 600, 450, 375);
                }
            }
            if (numset == 2)
            {
                if (lcwScoreset2 >= 21 && lcwScoreset2 - axelsenScoreset2 >= 2 || lcwScoreset2 >= 30)
                {
                    lcwTimer.Stop();
                    lcwSet++;
                    numset++;


                    MessageBox.Show("LCW a gagné ! 1 set ");

                    if (lcwSet == 2)
                    {
                        MessageBox.Show("LCW a gagné ! Le match ");
                        Application.Exit();
                    }
                }
                else if (lcwScoreset2 % 2 == 0)
                {
                    LCW.Location = new Point(700, 200);
                    Axelsen.Location = new Point(1100, 650);
                    Volant.Location = new Point(900, 350);
                    shuttlecockDestination = new Point(900, 350);
                    StartDelay();
                    movementZone = new Rectangle(950, 600, 450, 375);
                }
                else
                {
                    Axelsen.Location = new Point(700, 650);
                    LCW.Location = new Point(1100, 200);
                    Volant.Location = new Point(1000, 350);
                    shuttlecockDestination = new Point(1000, 350);
                    StartDelay();
                    movementZone = new Rectangle(450, 600, 450, 375);
                }
            }
            if (numset == 1)
            {
                if (lcwScoreset1 >= 21 && lcwScoreset1 - axelsenScoreset1 >= 2 || lcwScoreset1 >= 30)
                {
                    lcwTimer.Stop();
                    lcwSet++;
                    numset++;



                    MessageBox.Show("LCW a gagné ! 1 set ");

                }
                else if (lcwScoreset1 % 2 == 0)
                {
                    LCW.Location = new Point(700, 200);
                    Axelsen.Location = new Point(1100, 650);
                    Volant.Location = new Point(900, 350);
                    shuttlecockDestination = new Point(900, 350);
                    StartDelay();
                    movementZone = new Rectangle(950, 600, 450, 375);
                }
                else
                {
                    Axelsen.Location = new Point(700, 650);
                    LCW.Location = new Point(1100, 200);
                    Volant.Location = new Point(1000, 350);
                    shuttlecockDestination = new Point(1000, 350);
                    StartDelay();
                    movementZone = new Rectangle(450, 600, 450, 375);
                }
            }
        }
      
        private void ShuttlecockTimer_Tick(object sender, EventArgs e)
        {
            // Récupérer la position actuelle du volant
            int x = Volant.Location.X;
            int y = Volant.Location.Y;

            // Calculer la direction du mouvement
            int deltaX = shuttlecockDestination.X - x;
            int deltaY = shuttlecockDestination.Y - y;
            // Calculer la direction du mouvement
            int deltaX2 = shuttlecockDestination2.X - x;
            int deltaY2 = shuttlecockDestination2.Y - y;
            // Normaliser les mouvements
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            // Déterminer le mouvement proportionnel
            double moveX = deltaX / distance * stepSizevolant;
            double moveY = deltaY / distance * stepSizevolant;

            // Mettre à jour la position du volant
            x += (int)moveX;
            y += (int)moveY;
            Volant.Location = new Point(x, y);

            // Mettre à jour la position de la croix
            crossLocation = shuttlecockDestination;
            showCross = true; // Montrer la croix

            // Redessiner uniquement la zone de la croix
            int crossSize = 10; // Taille de la croix
            this.Invalidate(new Rectangle(crossLocation.X - crossSize, crossLocation.Y - crossSize, crossSize * 2, crossSize * 2));

            if (lastPlayerTouched == "Axelsen")
            {
                // Définir la zone (par exemple, 500, 500, 200, 200)
                movementZone = new Rectangle(550, 50, 800, 475);
                Rectangle movementZonefilet1 = new Rectangle(500, 475, 900, 50);

                // Vérifier si la destination du volant n'est pas dans cette zone
                if (!movementZone.Contains(shuttlecockDestination) || movementZonefilet1.Contains(shuttlecockDestination))
                {
                    lcwTimer.Stop();
                }
            }
            if (numset == 1)
            {
                // Vérifier si Axelsen touche le filet
                if (Axelsen.Bounds.IntersectsWith(Filet.Bounds))
                {
                    if (lastPlayerTouched == "LCW")
                    {
                        RotateVolantImage(180);
                    }
                    showCross = false;
                    VolantTimer.Stop();
                    lcwScoreset1++;
                    UpdateScore();
                    ScoreLCW();
                    lastPlayerTouched = "Axelsen";
                }
                else if (Math.Abs(x - shuttlecockDestination.X) <= 5 && Math.Abs(y - shuttlecockDestination.Y) <= 5 || Math.Abs(x - shuttlecockDestination2.X) <= 5 && Math.Abs(y - shuttlecockDestination2.Y) <= 5)
                {
                    showCross = false;
                    VolantTimer.Stop();  // Arrêter le mouvement une fois la destination atteinte

                    if (lastPlayerTouched == "LCW")
                    {
                        // Définir la zone (par exemple, 500, 500, 200, 200)
                        movementZone = new Rectangle(550, 500, 800, 450);
                        Rectangle movementZonefilet = new Rectangle(500, 475, 900, 50);

                        // Vérifier si la destination du volant n'est pas dans cette zone
                        if (!movementZone.Contains(shuttlecockDestination) || movementZonefilet.Contains(shuttlecockDestination))
                        {
                            axelsenScoreset1++;
                            UpdateScore();// Incrémenter le score de LCW
                            ScoreAxelsen();
                            // Faire quelque chose si la destination n'est pas dans la zone
                            ShowTemporaryMessage("FAUTE");
                        }
                        else
                        {
                            RotateVolantImage(180);
                            lcwScoreset1++;
                            UpdateScore();
                            ScoreLCW();
                        }
                    }
                    else if (lastPlayerTouched == "Axelsen")
                    {
                        // Définir la zone (par exemple, 500, 500, 200, 200)
                        movementZone = new Rectangle(550, 50, 800, 475);
                        Rectangle movementZonefilet = new Rectangle(500, 475, 900, 50);
                        lastPlayerTouched = "LCW";
                        // Vérifier si la destination du volant n'est pas dans cette zone
                        if (!movementZone.Contains(shuttlecockDestination) || movementZonefilet.Contains(shuttlecockDestination))
                        {
                            // Faire quelque chose si la destination n'est pas dans la zone
                            lcwScoreset1++;
                            lcwTimer.Stop();
                            ShowTemporaryMessage("FAUTE");
                            UpdateScore();// Incrémenter le score d'Axelsen
                            ScoreLCW();
                        }
                        else
                        {
                            RotateVolantImage(180);
                            axelsenScoreset1++;
                            UpdateScore();// Incrémenter le score d'Axelsen
                            lcwTimer.Stop();
                            ScoreAxelsen();
                        }
                    }
                }
            }
            
            if (numset == 2)
            {
                
                // Vérifier si Axelsen touche le filet
                if (Axelsen.Bounds.IntersectsWith(Filet.Bounds))
                {
                    
                    if (lastPlayerTouched == "LCW")
                    {
                        RotateVolantImage(180);
                    }
                    showCross = false;
                    VolantTimer.Stop();  // Arrêter le mouvement une fois la destination atteinte
                    lcwScoreset2++;
                    UpdateScore();
                    ScoreLCW();
                }
                else if (Math.Abs(x - shuttlecockDestination.X) <= 5 && Math.Abs(y - shuttlecockDestination.Y) <= 5 || Math.Abs(x - shuttlecockDestination2.X) <= 5 && Math.Abs(y - shuttlecockDestination2.Y) <= 5)
                {
                    
                    showCross = false;
                    VolantTimer.Stop();  // Arrêter le mouvement une fois la destination atteinte

                    if (lastPlayerTouched == "LCW")
                    {
                        
                        // Définir la zone (par exemple, 500, 500, 200, 200)
                        movementZone = new Rectangle(550, 500, 800, 450);
                        Rectangle movementZonefilet = new Rectangle(500, 475, 900, 50);

                        // Vérifier si la destination du volant n'est pas dans cette zone
                        if (!movementZone.Contains(shuttlecockDestination) || movementZonefilet.Contains(shuttlecockDestination))
                        {
                            
                            axelsenScoreset2++;
                            UpdateScore();
                            ScoreAxelsen();
                            // Faire quelque chose si la destination n'est pas dans la zone
                            ShowTemporaryMessage("FAUTE");
                        }
                        else
                        {
                            RotateVolantImage(180);
                            lcwScoreset2++;
                            UpdateScore();
                            ScoreLCW();
                        }
                    }
                    else if (lastPlayerTouched == "Axelsen")
                    {
                        
                        // Définir la zone (par exemple, 500, 500, 200, 200)
                        movementZone = new Rectangle(550, 50, 800, 475);
                        Rectangle movementZonefilet = new Rectangle(500, 475, 900, 50);
                        lastPlayerTouched = "LCW";
                        // Vérifier si la destination du volant n'est pas dans cette zone
                        if (!movementZone.Contains(shuttlecockDestination) || movementZonefilet.Contains(shuttlecockDestination))
                        {
                            
                            // Faire quelque chose si la destination n'est pas dans la zone
                            lcwScoreset2++;
                            lcwTimer.Stop();
                            ShowTemporaryMessage("FAUTE");
                            UpdateScore();// Incrémenter le score d'Axelsen
                            ScoreLCW();
                        }
                        else
                        {
                            RotateVolantImage(180);
                            axelsenScoreset2++;
                            UpdateScore();// Incrémenter le score d'Axelsen
                            lcwTimer.Stop();
                            ScoreAxelsen();
                        }
                    }
                }
            }
            if (numset == 3)
            {
                // Vérifier si Axelsen touche le filet
                if (Axelsen.Bounds.IntersectsWith(Filet.Bounds))
                {
                    if (lastPlayerTouched == "LCW")
                    {
                        RotateVolantImage(180);
                    }
                    showCross = false;
                    VolantTimer.Stop();  // Arrêter le mouvement une fois la destination atteinte
                    lcwScoreset3++;
                    UpdateScore();
                    ScoreLCW();
                }
                else if (Math.Abs(x - shuttlecockDestination.X) <= 5 && Math.Abs(y - shuttlecockDestination.Y) <= 5 || Math.Abs(x - shuttlecockDestination2.X) <= 5 && Math.Abs(y - shuttlecockDestination2.Y) <= 5)
                {
                    showCross = false;
                    VolantTimer.Stop();  // Arrêter le mouvement une fois la destination atteinte

                    if (lastPlayerTouched == "LCW")
                    {
                        // Définir la zone (par exemple, 500, 500, 200, 200)
                        movementZone = new Rectangle(550, 500, 800, 450);
                        Rectangle movementZonefilet = new Rectangle(500, 475, 900, 50);

                        // Vérifier si la destination du volant n'est pas dans cette zone
                        if (!movementZone.Contains(shuttlecockDestination) || movementZonefilet.Contains(shuttlecockDestination))
                        {
                            axelsenScoreset3++;
                            UpdateScore();// Incrémenter le score de LCW
                            ScoreAxelsen();
                            // Faire quelque chose si la destination n'est pas dans la zone
                            ShowTemporaryMessage("FAUTE");
                        }
                        else
                        {
                            RotateVolantImage(180);
                            lcwScoreset3++;
                            UpdateScore();
                            ScoreLCW();
                        }
                    }
                    else if (lastPlayerTouched == "Axelsen")
                    {
                        // Définir la zone (par exemple, 500, 500, 200, 200)
                        movementZone = new Rectangle(550, 50, 800, 475);
                        Rectangle movementZonefilet = new Rectangle(500, 475, 900, 50);
                        lastPlayerTouched = "LCW";
                        // Vérifier si la destination du volant n'est pas dans cette zone
                        if (!movementZone.Contains(shuttlecockDestination) || movementZonefilet.Contains(shuttlecockDestination))
                        {
                            // Faire quelque chose si la destination n'est pas dans la zone
                            lcwScoreset3++;
                            lcwTimer.Stop();
                            ShowTemporaryMessage("FAUTE");
                            UpdateScore();// Incrémenter le score d'Axelsen
                            ScoreLCW();
                        }
                        else
                        {
                            RotateVolantImage(180);
                            axelsenScoreset3++;
                            UpdateScore();// Incrémenter le score d'Axelsen
                            lcwTimer.Stop();
                            ScoreAxelsen();
                        }
                    }
                }
            }



        }
        // Méthode pour dessiner une ligne
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Dessiner la croix noire si nécessaire
            if (showCross)
            {
                Pen blackPen = new Pen(Color.Black, 5);
                int crossSize = 10; // Taille de la croix

                // Dessiner la croix
                e.Graphics.DrawLine(blackPen, crossLocation.X - crossSize, crossLocation.Y, crossLocation.X + crossSize, crossLocation.Y);
                e.Graphics.DrawLine(blackPen, crossLocation.X, crossLocation.Y - crossSize, crossLocation.X, crossLocation.Y + crossSize);
            }

            // Définir le style du texte
            Font font = new Font("Arial", 16, FontStyle.Bold);
            Brush brush = Brushes.Black;

            // Afficher le score de LCW et Axelsen en haut à droite
            string Nomaxelsen = "Axelsen";
            string NomLCW = "LCW";
            string scoreText1 = $"{lcwScoreset1}";
            string scoreText2 = $"{lcwScoreset2}";
            string scoreText3 = $"{lcwScoreset3}";
            string scoreText4 = $"{axelsenScoreset1}";
            string scoreText5 = $"{axelsenScoreset2}";
            string scoreText6 = $"{axelsenScoreset3}";
            string Niveau = $"Niveau : {selectedLevel}";

            e.Graphics.DrawString(Niveau, font, brush, new Point(265, 50));
            e.Graphics.DrawString(scoreText1, font, brush, new Point(265, 115));
            e.Graphics.DrawString(scoreText2, font, brush, new Point(315, 115));
            e.Graphics.DrawString(scoreText3, font, brush, new Point(365, 115));
            e.Graphics.DrawString(scoreText4, font, brush, new Point(265, 165));
            e.Graphics.DrawString(scoreText5, font, brush, new Point(315, 165));
            e.Graphics.DrawString(scoreText6, font, brush, new Point(365, 165));
            e.Graphics.DrawString(Nomaxelsen, font, brush, new Point( 125, 160));
            e.Graphics.DrawString(NomLCW, font, brush, new Point(145, 115));

            // Crée un pinceau pour dessiner une ligne noire de 2 pixels d'épaisseur
            Pen blackpen = new Pen(Color.Black, 3);
            // Définir les coordonnées de départ et d'arrivée de la ligne
            int startX = 550, startY = 50;
            int endX = 550, endY = 950;
            int startX2 = 500, startY2 = 100;
            int endX2 = 1400, endY2 = 100;
            int startX3 = 500, startY3 = 50;
            int endX3 = 1400, endY3 = 50;
            int startX4 = 1350, startY4 = 50;
            int endX4 = 1350, endY4 = 950;
            int startX5 = 500, startY5 = 950;// ligne fond axelsen
            int endX5 = 1400, endY5 = 950; // ligne fond Axelsen
            int startX6 = 500, startY6 = 900;
            int endX6 = 1400, endY6 = 900;
            int startX7 = 500, startY7 = 400;
            int endX7 = 1400, endY7 = 400;
            int startX8 = 500, startY8 = 600;
            int endX8 = 1400, endY8 = 600;
            int startX9 = 950, startY9 = 400;
            int endX9 = 950, endY9 = 50;
            int startX10 = 950, startY10 = 600;
            int endX10 = 950, endY10 = 950;
            int startX11 = 1400, startY11 = 50;
            int endX11 = 1400, endY11 = 950;
            int startX12 = 500, startY12 = 50;
            int endX12 = 500, endY12 = 950;
            int score1X = 100, score1Y = 100;
            int score2X = 100, score2Y = 200;
            int score3X = 400, score3Y = 100;
            int score4X = 400, score4Y = 200;
            int score5X = 400, score5Y = 100;
            int score6X = 100, score6Y = 150;
            int score7X = 400, score7Y = 150;
            int score8X = 100, score8Y = 200;
            int score9X = 400, score9Y = 200;
            int score10X = 250, score10Y = 100;
            int score11X = 250, score11Y = 200;

            // Dessiner la ligne
            e.Graphics.DrawLine(blackpen, score10X, score10Y, score11X, score11Y);
            e.Graphics.DrawLine(blackpen, score8X, score8Y, score9X, score9Y);
            e.Graphics.DrawLine(blackpen, score6X, score6Y, score7X, score7Y);
            e.Graphics.DrawLine(blackpen, score1X, score1Y, score5X, score5Y);
            e.Graphics.DrawLine(blackpen, score1X, score1Y, score2X, score2Y);
            e.Graphics.DrawLine(blackpen, score3X, score3Y, score4X, score4Y);
            e.Graphics.DrawLine(blackpen, startX, startY, endX, endY) ;
            e.Graphics.DrawLine(blackpen, startX2, startY2, endX2, endY2);
            e.Graphics.DrawLine(blackpen, startX3, startY3, endX3, endY3);
            e.Graphics.DrawLine(blackpen, startX4, startY4, endX4, endY4);
            e.Graphics.DrawLine(blackpen, startX5, startY5, endX5, endY5);
            e.Graphics.DrawLine(blackpen, startX6, startY6, endX6, endY6);
            e.Graphics.DrawLine(blackpen, startX7, startY7, endX7, endY7);
            e.Graphics.DrawLine(blackpen, startX8, startY8, endX8, endY8);
            e.Graphics.DrawLine(blackpen, startX9, startY9, endX9, endY9);
            e.Graphics.DrawLine(blackpen, startX10, startY10, endX10, endY10);
            e.Graphics.DrawLine(blackpen, startX11, startY11, endX11, endY11);
            e.Graphics.DrawLine(blackpen, startX12, startY12, endX12, endY12);

            // Libérer les ressources du pinceau
            blackpen.Dispose();
        }
    }
}
