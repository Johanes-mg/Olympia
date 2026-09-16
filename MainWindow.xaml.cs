using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NLayer.NAudioSupport;

namespace Olympia
{
    public partial class MainWindow : Window
    {
        private const int MaxVies = 9;
        private const int DelaiAuto = 1200;
        private int vies = MaxVies;
        private int niveauActuel = 0;
        private bool dejaRepondu = false;
        private Difficulte difficulteChoisie;
        private List<Question> toutesLesQuestions = new List<Question>();
        private List<Question> questionsPartie = new List<Question>();
        private Image[] iconesVies;
        private DispatcherTimer timer;
        private IWavePlayer? outputDevice;
        private WaveStream? audioStream;
        private VolumeSampleProvider? volumeProvider;
        private bool audioActif = true;
        private double volumeActuel = 0.5;

        public MainWindow()
        {
            InitializeComponent();
            iconesVies = new[] { Life1, Life2, Life3, Life4, Life5, Life6, Life7, Life8, Life9 };
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(DelaiAuto) };
            timer.Tick += Timer_Tick;
            ChargerQuestions();
            InitialiserAudio();
        }

        private void ChargerQuestions()
        {
            toutesLesQuestions = new List<Question>
            {
                // Mortels (20)
                new Question { Niveau = Difficulte.Mortals, Intitule = "Qui est le roi des dieux et le maître de la foudre ?", Reponses = new[] { "Poséidon", "Zeus", "Hadès", "Apollon" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Quelle est la déesse de la sagesse et de la stratégie militaire ?", Reponses = new[] { "Athéna", "Héra", "Aphrodite", "Artémis" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Quel dieu règne sur le monde souterrain et le royaume des morts ?", Reponses = new[] { "Arès", "Hadès", "Hermès", "Héphaïstos" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Quel héros grec est connu pour sa force légendaire et ses 12 travaux ?", Reponses = new[] { "Achille", "Ulysse", "Héraclès", "Persée" }, BonneReponse = 2 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Quel monstre possède des serpents à la place des cheveux et pétrifie d'un simple regard ?", Reponses = new[] { "La Chimère", "Méduse", "L'Hydre", "Scylla" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Quel oiseau nocturne est le symbole de la déesse Athéna ?", Reponses = new[] { "Le corbeau", "L'aigle", "La chouette", "Le faucon" }, BonneReponse = 2 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Quelle est l'arme à trois dents du dieu de la mer, Poséidon ?", Reponses = new[] { "Le trident", "La foudre", "Le sceptre", "La harpe" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Qui est la déesse de l'amour, de la beauté et de la séduction ?", Reponses = new[] { "Héra", "Athéna", "Aphrodite", "Déméter" }, BonneReponse = 2 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Sur quelle montagne sacrée les douze dieux principaux résident-ils ?", Reponses = new[] { "Le Parnasse", "L'Olympe", "L'Etna", "Le Taygète" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Qui est le messager ailé des dieux, également patron des voyageurs et des voleurs ?", Reponses = new[] { "Apollon", "Hermès", "Arès", "Dionysos" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Qui est le dieu de la guerre brutale et sanguinaire ?", Reponses = new[] { "Apollon", "Arès", "Hermès", "Héphaïstos" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Qui est la déesse du foyer et de la maison ?", Reponses = new[] { "Hestia", "Héra", "Déméter", "Athéna" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Qui est le dieu du vin et de la fête ?", Reponses = new[] { "Dionysos", "Apollon", "Hermès", "Pan" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Qui est la déesse de la chasse et de la nature sauvage ?", Reponses = new[] { "Athéna", "Artémis", "Héra", "Déméter" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Quel dieu est le forgeron de l'Olympe ?", Reponses = new[] { "Héphaïstos", "Arès", "Apollon", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Qui est la déesse de l'agriculture et des moissons ?", Reponses = new[] { "Héra", "Déméter", "Hestia", "Athéna" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Qui est le dieu de la lumière, des arts et de la musique ?", Reponses = new[] { "Apollon", "Dionysos", "Hermès", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Quel est le nom romain de Zeus ?", Reponses = new[] { "Mars", "Jupiter", "Neptune", "Pluton" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Quel est le nom romain d'Aphrodite ?", Reponses = new[] { "Vénus", "Minerve", "Junon", "Diane" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Intitule = "Qui est la femme de Zeus ?", Reponses = new[] { "Athéna", "Héra", "Déméter", "Hestia" }, BonneReponse = 1 },

                // Demi-Dieu (20)
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Quel héros a vaincu le Minotaure dans le labyrinthe de Crète ?", Reponses = new[] { "Persée", "Thésée", "Jason", "Héraclès" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Quel centaure sage et immortel a été le précepteur de nombreux héros comme Achille ?", Reponses = new[] { "Nessos", "Chiron", "Pholos", "Eurytion" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Quel titan a été condamné par Zeus à porter la voûte céleste sur ses épaules ?", Reponses = new[] { "Prométhée", "Atlas", "Cronos", "Épiméthée" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Quel est le seul point faible du héros Achille ?", Reponses = new[] { "Son œil", "Son talon", "Son épaule", "Son poignet" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Qui a ouvert une jarre interdite, libérant ainsi tous les maux de l'humanité ?", Reponses = new[] { "Hélène", "Pandore", "Circé", "Médée" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Quel titan a volé le feu sacré aux dieux pour l'offrir aux humains ?", Reponses = new[] { "Atlas", "Prométhée", "Cronos", "Japet" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Quel dieu de la lumière et des arts est le frère jumeau de la déesse Artémis ?", Reponses = new[] { "Hermès", "Apollon", "Arès", "Dionysos" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Quel est le nom du chien à trois têtes qui garde l'entrée des Enfers ?", Reponses = new[] { "Cerbère", "Orthos", "Argos", "Sphinx" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Qui est l'épouse légitime de Zeus et la protectrice du mariage et du couple ?", Reponses = new[] { "Déméter", "Héra", "Hestia", "Thétis" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Quel poète aveugle est considéré comme l'auteur de L'Iliade et L'Odyssée ?", Reponses = new[] { "Hésiode", "Homère", "Sophocle", "Eschyle" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Qui a tué la Méduse ?", Reponses = new[] { "Thésée", "Persée", "Ulysse", "Achille" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Qui est le héros de l'Odyssée ?", Reponses = new[] { "Ulysse", "Achille", "Hector", "Ajax" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Quel héros a conduit les Argonautes à la conquête de la Toison d'Or ?", Reponses = new[] { "Jason", "Thésée", "Persée", "Héraclès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Qui est le héros troyen qui combat Achille dans l'Iliade ?", Reponses = new[] { "Pâris", "Hector", "Énée", "Priam" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Quel roi de Sparte est le mari d'Hélène ?", Reponses = new[] { "Agamemnon", "Ménélas", "Ulysse", "Nestor" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Qui est le chef des armées grecques devant Troie ?", Reponses = new[] { "Agamemnon", "Ménélas", "Ulysse", "Achille" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Quel est le nom du labyrinthe construit par Dédale ?", Reponses = new[] { "Le Labyrinthe de Crète", "Le Labyrinthe de Cnossos", "Les deux", "Aucun des deux" }, BonneReponse = 2 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Qui est la magicienne qui transforme les compagnons d'Ulysse en pourceaux ?", Reponses = new[] { "Circé", "Calypso", "Médée", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Qui est la nymphe qui retient Ulysse pendant 7 ans ?", Reponses = new[] { "Circé", "Calypso", "Nausicaa", "Hélène" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Intitule = "Quel héros grec est invulnérable sauf au talon ?", Reponses = new[] { "Ulysse", "Achille", "Persée", "Héraclès" }, BonneReponse = 1 },

                // Divin (20)
                new Question { Niveau = Difficulte.Divin, Intitule = "Qui est le passeur des Enfers qui réclame une obole pour faire traverser le Styx aux âmes ?", Reponses = new[] { "Charon", "Cerbère", "Hadès", "Minos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Quel titan, père de Zeus, dévorait ses propres enfants à leur naissance ?", Reponses = new[] { "Atlas", "Cronos", "Japet", "Ouranos" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Qui est la titanide considérée comme la mère des grands dieux olympiens ?", Reponses = new[] { "Gaïa", "Rhéa", "Thémis", "Mnémosyne" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Quel forgeron divin, boiteux et rejeté, a construit les palais de l'Olympe ?", Reponses = new[] { "Héphaïstos", "Dédale", "Arès", "Prométhée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Quel roi de Corinthe a été condamné à pousser éternellement un rocher en haut d'une colline ?", Reponses = new[] { "Tantale", "Sisyphe", "Ixion", "Salmonée" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Quelle divinité primordiale personnifie la Terre Mère ?", Reponses = new[] { "Gaïa", "Rhéa", "Héra", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Comment s'appelle le navire utilisé par Jason et les Argonautes pour trouver la Toison d'Or ?", Reponses = new[] { "L'Argo", "Le Pégase", "L'Odyssée", "La Trirème" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Quelle prophétesse troyenne est condamnée à prédire l'avenir sans jamais être crue ?", Reponses = new[] { "Cassandre", "Hélène", "Hécube", "Andromaque" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Quel sculpteur est tombé éperdument amoureux de la statue qu'il avait lui-même taillée ?", Reponses = new[] { "Dédale", "Pygmalion", "Praxitèle", "Phidias" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Qui est le géant aux cent bras qui aide Zeus contre les Titans ?", Reponses = new[] { "Briarée", "Typhon", "Encelade", "Polyphème" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Qui sont les trois Moires (Parques) ?", Reponses = new[] { "Les déesses du destin", "Les déesses de la vengeance", "Les déesses de la guerre", "Les déesses de l'amour" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Qui sont les Érinyes (Furies) ?", Reponses = new[] { "Les déesses de la vengeance", "Les déesses du destin", "Les muses", "Les nymphes" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Quel monstre à cent têtes garde le Tartare ?", Reponses = new[] { "Cerbère", "Typhon", "Ladon", "Python" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Qui est le berger troyen qui jugea le concours de beauté entre Héra, Athéna et Aphrodite ?", Reponses = new[] { "Pâris", "Hector", "Énée", "Anchise" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Quelle pomme est à l'origine de la guerre de Troie ?", Reponses = new[] { "La pomme d'or", "La pomme de discorde", "Les deux", "Aucune" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Quel est le nom du fleuve des Enfers dont les eaux donnent l'oubli ?", Reponses = new[] { "Le Styx", "Le Léthé", "L'Achéron", "Le Cocyte" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Quel est le nom du fleuve des serments des dieux ?", Reponses = new[] { "Le Styx", "Le Léthé", "L'Achéron", "Le Phlégéthon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Qui est le Titan qui porte le monde sur ses épaules ?", Reponses = new[] { "Atlas", "Cronos", "Prométhée", "Japet" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Qui est le premier dieu primordial, né du Chaos ?", Reponses = new[] { "Gaïa", "Ouranos", "Éros", "Tartare" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Intitule = "Quel est le nom du festin des dieux ?", Reponses = new[] { "Le banquet", "L'ambroisie", "Le nectar", "L'Olympe" }, BonneReponse = 1 }
            };
        }

        private void InitialiserAudio()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                string resourceName = "Olympia.assets.greek_themesong.mp3";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) return;

                var memoryStream = new System.IO.MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var builder = new Mp3FileReaderBase.FrameDecompressorBuilder(wf => new Mp3FrameDecompressor(wf));
                audioStream = new Mp3FileReaderBase(memoryStream, builder);

                var sampleProvider = audioStream.ToSampleProvider();
                volumeProvider = new VolumeSampleProvider(sampleProvider);
                volumeProvider.Volume = (float)volumeActuel;

                outputDevice = new WaveOutEvent();
                outputDevice.Init(volumeProvider);

                outputDevice.PlaybackStopped += (s, e) =>
                {
                    if (audioActif && audioStream != null && outputDevice != null)
                    {
                        audioStream.Position = 0;
                        outputDevice.Play();
                    }
                };

                outputDevice.Play();
            }
            catch { }
        }

        private void Audio_Click(object sender, RoutedEventArgs e)
        {
            audioActif = !audioActif;
            if (audioActif)
            {
                outputDevice?.Play();
                AudioIcon.Source = new BitmapImage(new Uri("assets/volume.png", UriKind.Relative));
            }
            else
            {
                outputDevice?.Pause();
                AudioIcon.Source = new BitmapImage(new Uri("assets/muet.png", UriKind.Relative));
            }
        }

        private void ResetOpacity(UIElement element)
        {
            element.BeginAnimation(UIElement.OpacityProperty, null);
            element.Opacity = 1;
        }

        private void ChangerFond(string chemin)
        {
            BackgroundImage.Source = new BitmapImage(new Uri(chemin, UriKind.Relative));
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOut.Completed += (s, ev) =>
            {
                MenuPanel.Visibility = Visibility.Collapsed;
                ResetOpacity(MenuPanel);
                ChangerFond("assets/fondblur.png");
                ResetOpacity(DifficultyPanel);
                DifficultyPanel.Visibility = Visibility.Visible;
                TopBar.Visibility = Visibility.Visible;

                var fadeIn = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                fadeIn.Completed += (s2, ev2) => ResetOpacity(DifficultyPanel);
                DifficultyPanel.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };

            MenuPanel.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void Difficulty_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string tag = b.Tag?.ToString() ?? string.Empty;

            switch (tag)
            {
                case "Mortals": difficulteChoisie = Difficulte.Mortals; break;
                case "Demi-Dieu": difficulteChoisie = Difficulte.DemiDieu; break;
                case "Divin": difficulteChoisie = Difficulte.Divin; break;
            }

            questionsPartie = toutesLesQuestions.Where(q => q.Niveau <= difficulteChoisie).OrderBy(q => Guid.NewGuid()).ToList();
            if (questionsPartie.Count > 15) questionsPartie = questionsPartie.Take(15).ToList();

            niveauActuel = 0;
            vies = MaxVies;

            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOut.Completed += (s, ev) =>
            {
                DifficultyPanel.Visibility = Visibility.Collapsed;
                ResetOpacity(DifficultyPanel);
                ResetOpacity(GamePanel);
                GamePanel.Visibility = Visibility.Visible;
                AfficherVies();
                AfficherQuestion();

                var fadeIn = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                fadeIn.Completed += (s2, ev2) => ResetOpacity(GamePanel);
                GamePanel.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };

            DifficultyPanel.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            if (GamePanel.Visibility == Visibility.Visible)
            {
                GamePanel.Visibility = Visibility.Collapsed;
                ResetOpacity(DifficultyPanel);
                DifficultyPanel.Visibility = Visibility.Visible;
            }
            else if (DifficultyPanel.Visibility == Visibility.Visible)
            {
                DifficultyPanel.Visibility = Visibility.Collapsed;
                ResetOpacity(MenuPanel);
                MenuPanel.Visibility = Visibility.Visible;
                TopBar.Visibility = Visibility.Collapsed;
                ChangerFond("assets/fond.png");
            }
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            RetourMenu();
        }

        private void RetourMenu()
        {
            timer.Stop();
            EndPanel.Visibility = Visibility.Collapsed;
            ChangerFond("assets/fond.png");
            GamePanel.Visibility = Visibility.Collapsed;
            ResetOpacity(GamePanel);
            DifficultyPanel.Visibility = Visibility.Collapsed;
            ResetOpacity(DifficultyPanel);
            ResetOpacity(MenuPanel);
            MenuPanel.Visibility = Visibility.Visible;
            TopBar.Visibility = Visibility.Collapsed;
        }

        private void Rejouer_Click(object sender, RoutedEventArgs e)
        {
            EndPanel.Visibility = Visibility.Collapsed;
            Difficulty_Click(new Button { Tag = LibelleDifficulteTag(difficulteChoisie) }, new RoutedEventArgs());
        }

        private string LibelleDifficulteTag(Difficulte d)
        {
            switch (d)
            {
                case Difficulte.Mortals: return "Mortals";
                case Difficulte.DemiDieu: return "Demi-Dieu";
                case Difficulte.Divin: return "Divin";
                default: return "Mortals";
            }
        }

        private void MenuFin_Click(object sender, RoutedEventArgs e)
        {
            RetourMenu();
        }

        private void AfficherVies()
        {
            string pleine = "assets/catfull.png";
            string vide = "assets/catempty.png";

            for (int i = 0; i < MaxVies; i++)
            {
                string chemin = i < vies ? pleine : vide;
                iconesVies[i].Source = new BitmapImage(new Uri(chemin, UriKind.Relative));
            }
        }

        private void AfficherQuestion()
        {
            dejaRepondu = false;
            FeedbackText.Text = "";

            Question q = questionsPartie[niveauActuel];
            LevelText.Text = $"NIVEAU {niveauActuel + 1} / {questionsPartie.Count}  —  {LibelleDifficulte(q.Niveau).ToUpper()}";
            QuestionText.Text = q.Intitule;

            Button[] boutons = { Answer1, Answer2, Answer3, Answer4 };
            for (int i = 0; i < boutons.Length; i++)
            {
                boutons[i].Content = q.Reponses[i];
                boutons[i].IsEnabled = true;
                boutons[i].Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A3A5C"));
            }

            AnimerFadeIn(GamePanel);
        }

        private string LibelleDifficulte(Difficulte d)
        {
            switch (d)
            {
                case Difficulte.Mortals: return "Mortel";
                case Difficulte.DemiDieu: return "Demi-Dieu";
                case Difficulte.Divin: return "Divin";
                default: return "";
            }
        }

        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            if (dejaRepondu) return;
            dejaRepondu = true;

            Button bouton = (Button)sender;
            int index = int.Parse(bouton.Tag?.ToString() ?? "0");
            Question q = questionsPartie[niveauActuel];

            Button[] boutons = { Answer1, Answer2, Answer3, Answer4 };
            foreach (var b in boutons) b.IsEnabled = false;

            if (index == q.BonneReponse)
            {
                bouton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71"));
                FeedbackText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71"));
                FeedbackText.Text = "BONNE RÉPONSE";
                AnimerPulse(bouton);
                timer.Start();
            }
            else
            {
                bouton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                boutons[q.BonneReponse].Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71"));
                FeedbackText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                FeedbackText.Text = "MAUVAISE RÉPONSE";

                vies--;
                AfficherVies();

                if (vies <= 0)
                {
                    AfficherFin(false);
                    return;
                }

                timer.Start();
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            timer.Stop();
            PasserSuivant();
        }

        private void PasserSuivant()
        {
            niveauActuel++;

            if (niveauActuel >= questionsPartie.Count)
            {
                AfficherFin(true);
                return;
            }

            AfficherQuestion();
        }

        private void AfficherFin(bool victoire)
        {
            GamePanel.Visibility = Visibility.Collapsed;
            ResetOpacity(GamePanel);
            ResetOpacity(EndPanel);
            EndPanel.Visibility = Visibility.Visible;

            if (victoire)
            {
                EndTitle.Text = "VICTOIRE";
                EndTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
                EndMessage.Text = $"Bravo ! Tu as terminé les {questionsPartie.Count} niveaux\navec {vies} vies restantes !";
            }
            else
            {
                EndTitle.Text = "DÉFAITE";
                EndTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                EndMessage.Text = $"Tu as perdu toutes tes vies...\nTu as atteint le niveau {niveauActuel + 1}.";
            }

            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            fadeIn.Completed += (s2, ev2) => ResetOpacity(EndPanel);
            EndPanel.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }

        private void AnimerFadeIn(UIElement element)
        {
            var anim = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            element.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void AnimerPulse(UIElement element)
        {
            var anim = new DoubleAnimation
            {
                From = 1.0,
                To = 1.08,
                Duration = TimeSpan.FromMilliseconds(150),
                AutoReverse = true,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            var scale = new ScaleTransform(1, 1);
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            element.RenderTransform = scale;
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }
    }
}