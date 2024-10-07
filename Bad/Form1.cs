﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bad
{
    public partial class Form1 : Form
    {
        private bool isAtDestination = false;

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
        private int lcwScore = 0; // Score pour LCW
        private int axelsenScore = 0; // Score pour Axelsen

        private Point crossLocation; // Position de la croix
        private bool showCross = false; // Indicateur pour afficher la croix

        private Label temporaryMessageLabel;
        private Timer messageTimer;



        public Form1()
        {
            
            InitializeComponent();

            // Initialiser le Label
            temporaryMessageLabel = new Label();
            temporaryMessageLabel.AutoSize = true;
            temporaryMessageLabel.Location = new Point(810,400);  // Choisissez la position souhaitée
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

            // Initialiser le Timer de LCW
            lcwTimer = new Timer();
            lcwTimer.Interval = 100;  // Intervalle en millisecondes pour chaque mouvement
            lcwTimer.Tick += LCWTimer_Tick;

            // Initialiser le Timer pour animer le déplacement du volant
            VolantTimer = new Timer();
            
            VolantTimer.Tick += ShuttlecockTimer_Tick;
            VolantTimer.Interval = 1;  // Intervalle en millisecondes pour chaque mouvement
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

        private void StartDelay()
        {
            delayTimer = new Timer();
            delayTimer.Interval = 2000; // 2 secondes
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

            // Vérifier si Axelsen touche le filet
            if (Axelsen.Bounds.IntersectsWith(Filet.Bounds))
            {
                lcwScore++;
                UpdateScore();
                ScoreLCW();

            }

            
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
            

            // Probabilité de 95% pour que LCW se déplace
            if (randomvolant.NextDouble() < 0.95)
            {
                lcwMoving = true;  // Indiquer que LCW doit se déplacer
            }

            // Démarrer les deux Timers
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
        private void ScoreAxelsen () 
        {
            if (axelsenScore % 2 == 0)
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
            if (axelsenScore == 21)
            {
                ShowTemporaryMessage("Axelsen a Gagné !");
                StartDelay();
                Application.Exit();
            }
        }
        private void ScoreLCW()
        {
            if (lcwScore % 2 == 0)
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
            if (lcwScore == 21)
            {
                ShowTemporaryMessage("LCW a Gagné !");
                StartDelay();
                Application.Exit();
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
                // Vérifier si le volant a atteint sa destination
                if (Math.Abs(x - shuttlecockDestination.X) <= 5 && Math.Abs(y - shuttlecockDestination.Y) <= 5 || Math.Abs(x - shuttlecockDestination2.X) <= 5 && Math.Abs(y - shuttlecockDestination2.Y) <= 5)
                {

                


                    showCross = false;
                    VolantTimer.Stop();  // Arrêter le mouvement une fois la destination atteinte
                                     // Attribuer un point au dernier joueur qui a touché le volant
                    if (lastPlayerTouched == "LCW")
                    {
                        // Définir la zone (par exemple, 500, 500, 200, 200)
                        movementZone = new Rectangle(550, 500, 800, 450);
                        Rectangle movementZonefilet = new Rectangle(500, 475, 900, 50);

                        // Vérifier si la destination du volant n'est pas dans cette zone
                        if (!movementZone.Contains(shuttlecockDestination) || movementZonefilet.Contains(shuttlecockDestination))
                        {

                            axelsenScore++;
                            UpdateScore();// Incrémenter le score de LCW
                            ScoreAxelsen();
                            // Faire quelque chose si la destination n'est pas dans la zone
                            ShowTemporaryMessage("FAUTE");



                    }
                        else
                        {
                            RotateVolantImage(180);
                            lcwScore++;
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
                            lcwScore++;
                            lcwTimer.Stop();
                            ShowTemporaryMessage("FAUTE");
                            UpdateScore();// Incrémenter le score d'Axelsen
                            ScoreLCW();

                        }
                        else
                        {
                        RotateVolantImage(180);
                            axelsenScore++;
                            UpdateScore();// Incrémenter le score d'Axelsen
                            lcwTimer.Stop();
                            ScoreAxelsen();

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
            string scoreText = $"LCW: {lcwScore} - Axelsen: {axelsenScore}";
            e.Graphics.DrawString(scoreText, font, brush, new Point(this.ClientSize.Width - 1100, 20));

            

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

            // Dessiner la ligne
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