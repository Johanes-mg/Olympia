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
        private int chapitreActuel = 1;
        private int niveauDansChapitre = 1;
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
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est le roi des dieux et le maître de la foudre ?", Reponses = new[] { "Poséidon", "Zeus", "Hadès", "Apollon" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quelle est la déesse de la sagesse et de la stratégie militaire ?", Reponses = new[] { "Athéna", "Héra", "Aphrodite", "Artémis" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est la déesse de l'amour, de la beauté et de la séduction ?", Reponses = new[] { "Héra", "Athéna", "Aphrodite", "Déméter" }, BonneReponse = 2 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est la femme de Zeus et protectrice du mariage ?", Reponses = new[] { "Athéna", "Héra", "Déméter", "Hestia" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est le messager ailé des dieux, patron des voyageurs et des voleurs ?", Reponses = new[] { "Apollon", "Hermès", "Arès", "Dionysos" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est la déesse du foyer et de la maison ?", Reponses = new[] { "Hestia", "Héra", "Déméter", "Athéna" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est le dieu de la lumière, des arts et de la musique ?", Reponses = new[] { "Apollon", "Dionysos", "Hermès", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est la déesse de la chasse et de la nature sauvage ?", Reponses = new[] { "Athéna", "Artémis", "Héra", "Déméter" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quel dieu est le forgeron de l'Olympe ?", Reponses = new[] { "Héphaïstos", "Arès", "Apollon", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est le dieu du vin et de la fête ?", Reponses = new[] { "Dionysos", "Apollon", "Hermès", "Pan" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est la déesse de l'agriculture et des moissons ?", Reponses = new[] { "Héra", "Déméter", "Hestia", "Athéna" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est le dieu de la guerre brutale et sanguinaire ?", Reponses = new[] { "Apollon", "Arès", "Hermès", "Héphaïstos" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est la déesse de la sagesse, née adulte et armée ?", Reponses = new[] { "Athéna", "Héra", "Artémis", "Aphrodite" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quel est le nom romain de Zeus ?", Reponses = new[] { "Mars", "Jupiter", "Neptune", "Pluton" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quel est le nom romain d'Aphrodite ?", Reponses = new[] { "Vénus", "Minerve", "Junon", "Diane" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel dieu règne sur le monde souterrain et le royaume des morts ?", Reponses = new[] { "Arès", "Hadès", "Hermès", "Héphaïstos" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quelle est l'arme à trois dents du dieu de la mer, Poséidon ?", Reponses = new[] { "Le trident", "La foudre", "Le sceptre", "La harpe" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom romain de Poséidon, le dieu des océans ?", Reponses = new[] { "Mars", "Jupiter", "Neptune", "Pluton" }, BonneReponse = 2 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Sur quelle montagne sacrée les douze dieux principaux résident-ils ?", Reponses = new[] { "Le Parnasse", "L'Olympe", "L'Etna", "Le Taygète" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quelle est la ville grecque dont Athéna est la sainte patronne après avoir offert un olivier ?", Reponses = new[] { "Sparte", "Athènes", "Thèbes", "Corinthe" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel roi mythique de Phrygie a reçu le pouvoir de transformer en or tout ce qu'il touchait ?", Reponses = new[] { "Midas", "Crésus", "Tantale", "Sisyphe" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui a ouvert une jarre interdite, libérant tous les maux de l'humanité ?", Reponses = new[] { "Hélène", "Pandore", "Circé", "Médée" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quelle boîte mystérieuse ne contenait plus que l'Espérance à la fin ?", Reponses = new[] { "La boîte de Pandore", "La boîte d'Épiméthée", "La jarre d'Héra", "Le coffre d'Athéna" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel titan, père de Zeus, dévorait ses propres enfants à leur naissance ?", Reponses = new[] { "Atlas", "Cronos", "Japet", "Ouranos" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui est la titanide considérée comme la mère des grands dieux olympiens ?", Reponses = new[] { "Gaïa", "Rhéa", "Thémis", "Mnémosyne" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quelle divinité primordiale personnifie la Terre Mère ?", Reponses = new[] { "Gaïa", "Rhéa", "Héra", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui est le premier dieu primordial, né du Chaos ?", Reponses = new[] { "Gaïa", "Ouranos", "Éros", "Tartare" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel titan a été condamné par Zeus à porter la voûte céleste sur ses épaules ?", Reponses = new[] { "Prométhée", "Atlas", "Cronos", "Épiméthée" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui est le Titan qui porte le monde sur ses épaules ?", Reponses = new[] { "Atlas", "Cronos", "Prométhée", "Japet" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui est le géant aux cent bras qui aide Zeus contre les Titans ?", Reponses = new[] { "Briarée", "Typhon", "Encelade", "Polyphème" }, BonneReponse = 0 },

                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel monstre possède des serpents à la place des cheveux et pétrifie d'un simple regard ?", Reponses = new[] { "La Chimère", "Méduse", "L'Hydre", "Scylla" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel cheval ailé et majestueux est né du sang de la Méduse ?", Reponses = new[] { "Pégase", "Arion", "Bucéphale", "Xanthe" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel est le nom du chien à trois têtes qui garde l'entrée des Enfers ?", Reponses = new[] { "Cerbère", "Orthos", "Argos", "Sphinx" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel monstre marin, à moitié femme et à moitié serpent, est surnommé la Mère de tous les monstres ?", Reponses = new[] { "Échidna", "Scylla", "Charybde", "Méduse" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quelle créature mi-lion, mi-chèvre, mi-serpent crachait du feu avant d'être tuée par Bellérophon ?", Reponses = new[] { "La Chimère", "L'Hydre", "Le Sphinx", "Le Minotaure" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel monstre à cent têtes garde le Tartare ?", Reponses = new[] { "Cerbère", "Typhon", "Ladon", "Python" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel oiseau nocturne est le symbole de la déesse Athéna ?", Reponses = new[] { "Le corbeau", "L'aigle", "La chouette", "Le faucon" }, BonneReponse = 2 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Qui sont les trois Moires (Parques) ?", Reponses = new[] { "Les déesses du destin", "Les déesses de la vengeance", "Les déesses de la guerre", "Les déesses de l'amour" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Qui sont les Érinyes (Furies) ?", Reponses = new[] { "Les déesses de la vengeance", "Les déesses du destin", "Les muses", "Les nymphes" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quelle divinité personnifie le Sommeil et est le frère jumeau de Thanatos (la Mort) ?", Reponses = new[] { "Hypnos", "Morphée", "Éros", "Pan" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quelle titanide, déesse de la justice et de l'ordre divin, tient souvent une balance à la main ?", Reponses = new[] { "Thémis", "Métis", "Héra", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel vent du Nord, violent et glacial, a enlevé la nymphe Orithye ?", Reponses = new[] { "Borée", "Zéphyr", "Notos", "Euros" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quelle déesse, fille de Déméter, a été enlevée par Hadès pour devenir reine des Enfers ?", Reponses = new[] { "Perséphone", "Hécate", "Athéna", "Artémis" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "De quelle partie du corps de Zeus Athéna est-elle née, armée et casquée ?", Reponses = new[] { "Du cœur", "De la tête", "Du ventre", "De l'épaule" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel est le nom du festin des dieux ?", Reponses = new[] { "Le banquet", "L'ambroisie", "Le nectar", "L'Olympe" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui est le passeur des Enfers qui réclame une obole pour faire traverser le Styx ?", Reponses = new[] { "Charon", "Cerbère", "Hadès", "Minos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel est le nom du fleuve des Enfers dont les eaux donnent l'oubli ?", Reponses = new[] { "Le Styx", "Le Léthé", "L'Achéron", "Le Cocyte" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel est le nom du fleuve des serments des dieux ?", Reponses = new[] { "Le Styx", "Le Léthé", "L'Achéron", "Le Phlégéthon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Comment s'appelle le navire utilisé par Jason et les Argonautes pour trouver la Toison d'Or ?", Reponses = new[] { "L'Argo", "Le Pégase", "L'Odyssée", "La Trirème" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel roi de Corinthe a été condamné à pousser éternellement un rocher en haut d'une colline ?", Reponses = new[] { "Tantale", "Sisyphe", "Ixion", "Salmonée" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel héros grec est connu pour sa force légendaire et ses 12 travaux ?", Reponses = new[] { "Achille", "Ulysse", "Héraclès", "Persée" }, BonneReponse = 2 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel héros a vaincu le Minotaure dans le labyrinthe de Crète ?", Reponses = new[] { "Persée", "Thésée", "Jason", "Héraclès" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel est le nom du labyrinthe construit par Dédale ?", Reponses = new[] { "Le Labyrinthe de Crète", "Le Labyrinthe de Cnossos", "Les deux", "Aucun des deux" }, BonneReponse = 2 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel fils de Dédale est mort brûlé par le Soleil parce que ses ailes en cire ont fondu ?", Reponses = new[] { "Icare", "Égée", "Thésée", "Minos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel centaure sage et immortel a été le précepteur de héros comme Achille ?", Reponses = new[] { "Nessos", "Chiron", "Pholos", "Eurytion" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel est le seul point faible du héros Achille ?", Reponses = new[] { "Son œil", "Son talon", "Son épaule", "Son poignet" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui est le redoutable père d'Achille, roi des Myrmidons, qui a épousé la nymphe Thétis ?", Reponses = new[] { "Pélée", "Éaque", "Laomédon", "Priam" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui est le héros de l'Odyssée ?", Reponses = new[] { "Ulysse", "Achille", "Hector", "Ajax" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui est la magicienne qui transforme les compagnons d'Ulysse en pourceaux ?", Reponses = new[] { "Circé", "Calypso", "Médée", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui est la nymphe qui retient Ulysse pendant 7 ans ?", Reponses = new[] { "Circé", "Calypso", "Nausicaa", "Hélène" }, BonneReponse = 1 },

                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel prince de Troie a déclenché la guerre en enlevant la belle Hélène ?", Reponses = new[] { "Pâris", "Hector", "Énée", "Priam" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le héros troyen qui combat Achille dans l'Iliade ?", Reponses = new[] { "Pâris", "Hector", "Énée", "Priam" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel roi de Sparte est le mari d'Hélène ?", Reponses = new[] { "Agamemnon", "Ménélas", "Ulysse", "Nestor" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le chef des armées grecques devant Troie ?", Reponses = new[] { "Agamemnon", "Ménélas", "Ulysse", "Achille" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel devin aveugle de Thèbes est consulté par Ulysse aux Enfers ?", Reponses = new[] { "Tirésias", "Calchas", "Phinée", "Mélampous" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel musicien légendaire est descendu aux Enfers pour ramener son épouse Eurydice ?", Reponses = new[] { "Orphée", "Amphion", "Linos", "Marsyas" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a conduit les Argonautes à la conquête de la Toison d'Or ?", Reponses = new[] { "Jason", "Thésée", "Persée", "Héraclès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quelle princesse de Colchide et magicienne a aidé Jason avant de se venger ?", Reponses = new[] { "Médée", "Circé", "Calypso", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le jeune chasseur tombé amoureux de son reflet dans l'eau ?", Reponses = new[] { "Narcisse", "Hyacinthe", "Adonis", "Ganymède" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quelle reine des Amazones a été affrontée par Héraclès pour sa ceinture magique ?", Reponses = new[] { "Hippolyte", "Antiope", "Penthésilée", "Mélanippe" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui a tué la Méduse ?", Reponses = new[] { "Thésée", "Persée", "Ulysse", "Achille" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros grec est invulnérable sauf au talon ?", Reponses = new[] { "Ulysse", "Achille", "Persée", "Héraclès" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel titan a volé le feu sacré aux dieux pour l'offrir aux humains ?", Reponses = new[] { "Atlas", "Prométhée", "Cronos", "Japet" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel dieu de la lumière et des arts est le frère jumeau d'Artémis ?", Reponses = new[] { "Hermès", "Apollon", "Arès", "Dionysos" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est l'épouse légitime de Zeus et protectrice du mariage ?", Reponses = new[] { "Déméter", "Héra", "Hestia", "Thétis" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quelle pomme est à l'origine de la guerre de Troie ?", Reponses = new[] { "La pomme d'or", "La pomme de discorde", "Les deux", "Aucune" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui est le berger troyen qui jugea le concours de beauté entre Héra, Athéna et Aphrodite ?", Reponses = new[] { "Pâris", "Hector", "Énée", "Anchise" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quelle prophétesse troyenne est condamnée à prédire l'avenir sans jamais être crue ?", Reponses = new[] { "Cassandre", "Hélène", "Hécube", "Andromaque" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quel poète aveugle est considéré comme l'auteur de L'Iliade et L'Odyssée ?", Reponses = new[] { "Hésiode", "Homère", "Sophocle", "Eschyle" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Combien d'années a duré la guerre de Troie ?", Reponses = new[] { "5 ans", "10 ans", "15 ans", "20 ans" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quel est le nom du cheval utilisé par les Grecs pour entrer dans Troie ?", Reponses = new[] { "Le cheval de Troie", "Le cheval d'Arion", "Le cheval de Pégase", "Le cheval de Bucéphale" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quelle déesse est la protectrice de la ville de Troie ?", Reponses = new[] { "Athéna", "Héra", "Aphrodite", "Artémis" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quel héros grec a tué Hector ?", Reponses = new[] { "Ulysse", "Achille", "Ajax", "Diomède" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quel dieu soutient les Troyens pendant la guerre ?", Reponses = new[] { "Apollon", "Poséidon", "Athéna", "Héra" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quel héros troyen est le fils d'Aphrodite ?", Reponses = new[] { "Énée", "Hector", "Pâris", "Déiphobe" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui est la mère d'Achille ?", Reponses = new[] { "Thétis", "Héra", "Athéna", "Doris" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quel dieu guide la flèche qui tue Achille ?", Reponses = new[] { "Apollon", "Arès", "Zeus", "Poséidon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quel héros grec est connu pour sa ruse ?", Reponses = new[] { "Ulysse", "Achille", "Ajax", "Nestor" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Comment s'appelle le roi de Troie, père d'Hector et Pâris ?", Reponses = new[] { "Priam", "Laomédon", "Anchise", "Tithon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 3, NiveauChapitre = 2, Intitule = "Combien de temps Ulysse a-t-il mis pour rentrer à Ithaque ?", Reponses = new[] { "1 an", "5 ans", "10 ans", "20 ans" }, BonneReponse = 2 },

                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Quel cyclope est le fils de Poséidon et ennemi d'Ulysse ?", Reponses = new[] { "Polyphème", "Briarée", "Antiphatès", "Eurymédon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Quel dieu aide Ulysse à rentrer chez lui ?", Reponses = new[] { "Athéna", "Poséidon", "Arès", "Apollon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Quel est le nom du royaume d'Ulysse ?", Reponses = new[] { "Ithaque", "Sparte", "Thèbes", "Argos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Quelle épreuve Héraclès doit-il accomplir en premier ?", Reponses = new[] { "Le lion de Némée", "L'Hydre de Lerne", "Le sanglier d'Érymanthe", "La biche de Cérynie" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Combien de travaux Héraclès a-t-il dû accomplir ?", Reponses = new[] { "10", "12", "15", "20" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Quel animal Héraclès doit-il capturer pour son 4e travail ?", Reponses = new[] { "Le sanglier d'Érymanthe", "La biche de Cérynie", "Le taureau de Crète", "Les juments de Diomède" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Quel roi aide Héraclès dans ses travaux ?", Reponses = new[] { "Eurysthée", "Augias", "Laomédon", "Priam" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est le père d'Héraclès ?", Reponses = new[] { "Zeus", "Poséidon", "Amphitryon", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Quelle déesse persécute Héraclès toute sa vie ?", Reponses = new[] { "Héra", "Athéna", "Artémis", "Aphrodite" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Quel est le nom du centaure qui cause la mort d'Héraclès ?", Reponses = new[] { "Nessos", "Chiron", "Pholos", "Eurytion" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Quel dieu accueille Héraclès sur l'Olympe après sa mort ?", Reponses = new[] { "Zeus", "Apollon", "Arès", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Quel héros a ramené Alceste des Enfers ?", Reponses = new[] { "Héraclès", "Thésée", "Orphée", "Persée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est la femme d'Orphée ?", Reponses = new[] { "Eurydice", "Perséphone", "Calypso", "Circé" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Quel dieu est amoureux de Psyché ?", Reponses = new[] { "Éros", "Apollon", "Arès", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 1, Intitule = "Quelle déesse est jalouse de Psyché ?", Reponses = new[] { "Aphrodite", "Héra", "Athéna", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Quel héros a abandonné Ariane sur l'île de Naxos ?", Reponses = new[] { "Thésée", "Jason", "Persée", "Ulysse" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Quel dieu épouse Ariane après son abandon ?", Reponses = new[] { "Dionysos", "Apollon", "Arès", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Quel roi de Thèbes a épousé sa propre mère sans le savoir ?", Reponses = new[] { "Œdipe", "Laïos", "Créon", "Étéocle" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Quel est le nom du sphinx qui terrorisait Thèbes ?", Reponses = new[] { "Le Sphinx", "La Chimère", "L'Hydre", "Le Minotaure" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Quelle est la réponse à l'énigme du sphinx ?", Reponses = new[] { "L'Homme", "Le Lion", "L'Aigle", "Le Cheval" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Quelle prophétesse a révélé la vérité à Œdipe ?", Reponses = new[] { "Tirésias", "Cassandre", "Calchas", "Phinée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Quel fils d'Œdipe devient roi de Thèbes après lui ?", Reponses = new[] { "Étéocle", "Polynice", "Antigone", "Ismène" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Quel frère de Polynice refuse de lui rendre le trône ?", Reponses = new[] { "Étéocle", "Créon", "Laïos", "Œdipe" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Quelle est la fille d'Œdipe qui l'accompagne dans son exil ?", Reponses = new[] { "Antigone", "Ismène", "Jocaste", "Eurydice" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Quel dieu poursuit les meurtriers sans pitié ?", Reponses = new[] { "Les Érinyes", "Zeus", "Apollon", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Quel est le nom du rocher où Œdipe se crève les yeux ?", Reponses = new[] { "Le Cithéron", "Le Parnasse", "L'Olympe", "Le Taygète" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Quelle ville est maudite par les Érinyes après la mort d'Œdipe ?", Reponses = new[] { "Thèbes", "Athènes", "Sparte", "Corinthe" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le frère de Zeus et dieu des Enfers ?", Reponses = new[] { "Hadès", "Poséidon", "Arès", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est la déesse de la vengeance et de la justice ?", Reponses = new[] { "Némésis", "Thémis", "Héra", "Athéna" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Mortals, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est la déesse de la discorde ?", Reponses = new[] { "Éris", "Némésis", "Hébé", "Iris" }, BonneReponse = 0 },

                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qu'est-ce qui existait avant tous les dieux et le monde ?", Reponses = new[] { "Le Chaos", "L'Olympe", "Le Tartare", "Le Styx" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est le premier être né du Chaos ?", Reponses = new[] { "Gaïa", "Ouranos", "Éros", "Tartare" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est le père d'Ouranos (le Ciel) ?", Reponses = new[] { "Le Chaos", "Gaïa", "Éros", "Tartare" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui sont les trois premiers dieux primordiaux ?", Reponses = new[] { "Gaïa, Tartare, Éros", "Zeus, Poséidon, Hadès", "Cronos, Rhéa, Ouranos", "Athéna, Héra, Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quelle déesse est la mère des Titans ?", Reponses = new[] { "Gaïa", "Rhéa", "Thémis", "Mnémosyne" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Combien de Titans et Titanides existent ?", Reponses = new[] { "6", "12", "20", "3" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est le plus jeune des Titans ?", Reponses = new[] { "Cronos", "Japet", "Océan", "Hypérion" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quel Titan est le père de Zeus ?", Reponses = new[] { "Cronos", "Japet", "Océan", "Hypérion" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quelle est la mère de Zeus ?", Reponses = new[] { "Rhéa", "Gaïa", "Thémis", "Héra" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Où Rhéa cache-t-elle Zeus après sa naissance ?", Reponses = new[] { "En Crète", "À Athènes", "À Sparte", "À Thèbes" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quelle chèvre allaite Zeus dans sa grotte ?", Reponses = new[] { "Amalthée", "Io", "Europe", "Danaé" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quel objet Rhéa donne-t-elle à Cronos à la place de Zeus ?", Reponses = new[] { "Une pierre", "Un morceau de bois", "Un poisson", "Un linge" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quels êtres aident Zeus à libérer ses frères et sœurs ?", Reponses = new[] { "Les Cyclopes et les Hécatonchires", "Les Titans", "Les Géants", "Les Nymphes" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Combien de temps a duré la Titanomachie ?", Reponses = new[] { "10 ans", "1 an", "100 ans", "5 ans" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 1, Intitule = "Où les Titans vaincus sont-ils enfermés ?", Reponses = new[] { "Dans le Tartare", "Sur l'Olympe", "Aux Enfers", "Dans le Chaos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui est le roi des Enfers ?", Reponses = new[] { "Hadès", "Poséidon", "Zeus", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom du chien à trois têtes des Enfers ?", Reponses = new[] { "Cerbère", "Orthos", "Argos", "Sphinx" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Combien de fleuves traversent les Enfers ?", Reponses = new[] { "5", "3", "7", "2" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom du fleuve de la haine ?", Reponses = new[] { "Le Styx", "Le Léthé", "L'Achéron", "Le Cocyte" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom du fleuve du feu ?", Reponses = new[] { "Le Phlégéthon", "Le Styx", "Le Léthé", "Le Cocyte" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom du fleuve des lamentations ?", Reponses = new[] { "Le Cocyte", "Le Styx", "Le Léthé", "L'Achéron" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom du fleuve de l'oubli ?", Reponses = new[] { "Le Léthé", "Le Styx", "L'Achéron", "Le Cocyte" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui juge les âmes aux Enfers ?", Reponses = new[] { "Minos, Éaque et Rhadamanthe", "Hadès seul", "Cerbère", "Charon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom du champ des Enfers où vont les âmes vertueuses ?", Reponses = new[] { "Les Champs Élysées", "Le Tartare", "L'Asphodèle", "Le Pré de l'Oubli" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom du champ des Enfers où vont les âmes damnées ?", Reponses = new[] { "Le Tartare", "Les Champs Élysées", "L'Asphodèle", "Le Pré de l'Oubli" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui a tenté de libérer Perséphone des Enfers ?", Reponses = new[] { "Déméter", "Zeus", "Hadès", "Héra" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Combien de temps Perséphone doit-elle passer aux Enfers chaque année ?", Reponses = new[] { "6 mois", "3 mois", "1 mois", "9 mois" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quelle nymphe aide Orphée à traverser les Enfers ?", Reponses = new[] { "Eurydice", "Perséphone", "Calypso", "Circé" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel dieu règne sur le Tartare ?", Reponses = new[] { "Hadès", "Zeus", "Poséidon", "Cronos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui sont les gardiens du Tartare ?", Reponses = new[] { "Les Hécatonchires", "Les Cyclopes", "Les Titans", "Les Érinyes" }, BonneReponse = 0 },

                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel est le nom du sanctuaire d'Apollon où l'on consulte l'oracle ?", Reponses = new[] { "Delphes", "Dodone", "Olympie", "Épidaure" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Qui est la prêtresse qui rend les oracles à Delphes ?", Reponses = new[] { "La Pythie", "La Sibylle", "La Vestale", "La Prêtresse" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel dieu a tué le serpent Python pour s'approprier l'oracle ?", Reponses = new[] { "Apollon", "Zeus", "Hermès", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel roi de Thèbes consulte l'oracle avant la naissance d'Œdipe ?", Reponses = new[] { "Laïos", "Créon", "Œdipe", "Étéocle" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quelle malédiction pèse sur la famille des Labdacides ?", Reponses = new[] { "La mort par le fils", "La folie", "La stérilité", "La cécité" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel devin accompagne les Argonautes ?", Reponses = new[] { "Idmon", "Tirésias", "Calchas", "Phinée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel devin aveugle est consulté par Ulysse aux Enfers ?", Reponses = new[] { "Tirésias", "Calchas", "Phinée", "Mélampous" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quelle prophétesse accompagne Énée dans sa descente aux Enfers ?", Reponses = new[] { "La Sibylle de Cumes", "Cassandre", "La Pythie", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel est le nom de la sibylle de Cumes ?", Reponses = new[] { "La Sibylle", "La Pythie", "Cassandre", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel dieu inspire les prophéties ?", Reponses = new[] { "Apollon", "Zeus", "Hermès", "Hadès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel roi consulta l'oracle pour savoir comment vaincre les Perses ?", Reponses = new[] { "Léonidas", "Crésus", "Xerxès", "Darius" }, BonneReponse = 1 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quelle est la réponse ambiguë de l'oracle à Crésus ?", Reponses = new[] { "Un grand empire tombera", "Il sera riche", "Il sera tué", "Il gagnera" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel est le nom de l'oracle de Zeus à Dodone ?", Reponses = new[] { "Dodone", "Delphes", "Olympie", "Épidaure" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Comment les prêtresses de Dodone rendent-elles les oracles ?", Reponses = new[] { "En écoutant le vent dans les chênes", "En lisant les entrailles", "En dormant", "En chantant" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel est le nom du sanctuaire d'Asclépios où l'on guérit par le rêve ?", Reponses = new[] { "Épidaure", "Delphes", "Dodone", "Olympie" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel dieu a été jeté du haut de l'Olympe par Zeus ?", Reponses = new[] { "Héphaïstos", "Arès", "Apollon", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quelle déesse est la plus belle selon Pâris ?", Reponses = new[] { "Aphrodite", "Héra", "Athéna", "Artémis" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel dieu est le plus laid de l'Olympe ?", Reponses = new[] { "Héphaïstos", "Arès", "Dionysos", "Hadès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quelle déesse est la plus jalouse de l'Olympe ?", Reponses = new[] { "Héra", "Aphrodite", "Athéna", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel dieu est le plus sage de l'Olympe ?", Reponses = new[] { "Athéna", "Zeus", "Apollon", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quelle déesse est la plus fidèle à son mari ?", Reponses = new[] { "Héra", "Aphrodite", "Déméter", "Hestia" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel dieu est le plus volage de l'Olympe ?", Reponses = new[] { "Zeus", "Poséidon", "Apollon", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quelle déesse est la plus rancunière de l'Olympe ?", Reponses = new[] { "Héra", "Aphrodite", "Athéna", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel dieu est le plus farceur de l'Olympe ?", Reponses = new[] { "Hermès", "Apollon", "Dionysos", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel dieu est le plus puissant après Zeus ?", Reponses = new[] { "Poséidon", "Hadès", "Athéna", "Apollon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quelle déesse est la plus vénérée des mortels ?", Reponses = new[] { "Athéna", "Héra", "Aphrodite", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel est le seul dieu de l'Olympe à avoir une mère mortelle ?", Reponses = new[] { "Dionysos", "Héraclès", "Hermès", "Apollon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel dieu de l'Olympe est le plus jeune ?", Reponses = new[] { "Dionysos", "Apollon", "Hermès", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quel dieu de l'Olympe est le plus ancien ?", Reponses = new[] { "Hestia", "Zeus", "Poséidon", "Hadès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 2, NiveauChapitre = 2, Intitule = "Quelle déesse est la gardienne du foyer de l'Olympe ?", Reponses = new[] { "Hestia", "Héra", "Déméter", "Athéna" }, BonneReponse = 0 },

                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a tué le lion de Némée ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Achille" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a tué l'Hydre de Lerne ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a capturé le taureau de Crète ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a dompté les juments de Diomède ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Bellérophon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a nettoyé les écuries d'Augias ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a tué les oiseaux du lac Stymphale ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Bellérophon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a capturé la biche de Cérynie ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a capturé le sanglier d'Érymanthe ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Bellérophon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a obtenu la ceinture d'Hippolyte ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a capturé les bœufs de Géryon ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a cueilli les pommes d'or des Hespérides ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a ramené Cerbère des Enfers ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Orphée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a tué le Minotaure ?", Reponses = new[] { "Thésée", "Héraclès", "Persée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a tué la Méduse ?", Reponses = new[] { "Persée", "Thésée", "Héraclès", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 1, Intitule = "Quel héros a tué la Chimère ?", Reponses = new[] { "Bellérophon", "Persée", "Thésée", "Héraclès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quel héros a dompté Pégase ?", Reponses = new[] { "Bellérophon", "Persée", "Thésée", "Héraclès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quel héros a tué le Sphinx ?", Reponses = new[] { "Œdipe", "Thésée", "Héraclès", "Persée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Quel héros a conduit les Argonautes ?", Reponses = new[] { "Jason", "Thésée", "Persée", "Héraclès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a aidé Jason à conquérir la Toison d'Or ?", Reponses = new[] { "Médée", "Circé", "Calypso", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a trahi Jason après leur mariage ?", Reponses = new[] { "Médée", "Circé", "Calypso", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui est la magicienne qui a transformé les compagnons d'Ulysse en pourceaux ?", Reponses = new[] { "Circé", "Calypso", "Médée", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a retenu Ulysse pendant 7 ans ?", Reponses = new[] { "Calypso", "Circé", "Nausicaa", "Hélène" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a aidé Ulysse à rentrer à Ithaque ?", Reponses = new[] { "Athéna", "Poséidon", "Zeus", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a tué les prétendants de Pénélope ?", Reponses = new[] { "Ulysse", "Télémaque", "Athéna", "Pénélope" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui est la femme d'Ulysse ?", Reponses = new[] { "Pénélope", "Hélène", "Andromaque", "Hécube" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui est le fils d'Ulysse ?", Reponses = new[] { "Télémaque", "Astyanax", "Hector", "Pâris" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui est la femme d'Hector ?", Reponses = new[] { "Andromaque", "Hélène", "Pénélope", "Hécube" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui est la mère d'Hector ?", Reponses = new[] { "Hécube", "Andromaque", "Hélène", "Pénélope" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui est le père d'Hector ?", Reponses = new[] { "Priam", "Laomédon", "Anchise", "Agamemnon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui est le frère d'Hector ?", Reponses = new[] { "Pâris", "Énée", "Anténor", "Polydamas" }, BonneReponse = 0 },

                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est la femme d'Orphée ?", Reponses = new[] { "Eurydice", "Perséphone", "Calypso", "Circé" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Pourquoi Orphée ne peut-il pas ramener Eurydice des Enfers ?", Reponses = new[] { "Il se retourne trop tôt", "Il chante mal", "Il est chassé", "Il perd la lyre" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est le dieu amoureux de Psyché ?", Reponses = new[] { "Éros", "Apollon", "Arès", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est la déesse jalouse de Psyché ?", Reponses = new[] { "Aphrodite", "Héra", "Athéna", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a abandonné Ariane sur l'île de Naxos ?", Reponses = new[] { "Thésée", "Jason", "Persée", "Ulysse" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a épousé Ariane après son abandon ?", Reponses = new[] { "Dionysos", "Apollon", "Arès", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a épousé sa mère sans le savoir ?", Reponses = new[] { "Œdipe", "Laïos", "Créon", "Étéocle" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a tué son père sans le savoir ?", Reponses = new[] { "Œdipe", "Laïos", "Créon", "Étéocle" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est la mère d'Œdipe ?", Reponses = new[] { "Jocaste", "Antigone", "Ismène", "Eurydice" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a révélé la vérité à Œdipe ?", Reponses = new[] { "Tirésias", "Cassandre", "Calchas", "Phinée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est le fils aîné d'Œdipe ?", Reponses = new[] { "Étéocle", "Polynice", "Antigone", "Ismène" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est la fille d'Œdipe qui l'accompagne dans son exil ?", Reponses = new[] { "Antigone", "Ismène", "Jocaste", "Eurydice" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est le frère de Polynice ?", Reponses = new[] { "Étéocle", "Créon", "Laïos", "Œdipe" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est le roi de Thèbes après Œdipe ?", Reponses = new[] { "Créon", "Étéocle", "Polynice", "Laïos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est la femme de Créon ?", Reponses = new[] { "Eurydice", "Jocaste", "Antigone", "Ismène" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui a tué son frère à Thèbes ?", Reponses = new[] { "Étéocle", "Polynice", "Créon", "Laïos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui a enterré Polynice malgré l'interdiction ?", Reponses = new[] { "Antigone", "Ismène", "Jocaste", "Eurydice" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui a condamné Antigone à mort ?", Reponses = new[] { "Créon", "Étéocle", "Polynice", "Laïos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le fiancé d'Antigone ?", Reponses = new[] { "Hémon", "Créon", "Étéocle", "Polynice" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le fils de Créon ?", Reponses = new[] { "Hémon", "Étéocle", "Polynice", "Laïos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui se suicide après la mort d'Antigone ?", Reponses = new[] { "Hémon", "Créon", "Ismène", "Eurydice" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui se suicide après la mort d'Hémon ?", Reponses = new[] { "Eurydice", "Antigone", "Ismène", "Jocaste" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est la sœur d'Antigone ?", Reponses = new[] { "Ismène", "Jocaste", "Eurydice", "Hélène" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est la fille de Créon ?", Reponses = new[] { "Eurydice", "Antigone", "Ismène", "Jocaste" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est la femme de Laïos ?", Reponses = new[] { "Jocaste", "Antigone", "Ismène", "Eurydice" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le père d'Œdipe ?", Reponses = new[] { "Laïos", "Créon", "Étéocle", "Polynice" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui a ordonné l'exil d'Œdipe ?", Reponses = new[] { "Créon", "Laïos", "Étéocle", "Polynice" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui a guidé Œdipe dans son exil ?", Reponses = new[] { "Antigone", "Ismène", "Jocaste", "Eurydice" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Où Œdipe meurt-il ?", Reponses = new[] { "À Colone", "À Thèbes", "À Athènes", "À Sparte" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.DemiDieu, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui accueille Œdipe à Colone ?", Reponses = new[] { "Thésée", "Créon", "Étéocle", "Polynice" }, BonneReponse = 0 },

                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quel est le nom du bateau qui fait traverser les âmes ?", Reponses = new[] { "La barque de Charon", "Le radeau de Styx", "Le navire d'Hadès", "Le vaisseau des Morts" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quelle pièce doit-on donner à Charon ?", Reponses = new[] { "Une obole", "Une drachme", "Un talent", "Un statère" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Que devient-on si on n'a pas d'obole pour Charon ?", Reponses = new[] { "Une âme errante", "Un fantôme", "Un monstre", "Un dieu" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est le dieu qui règne sur les Enfers ?", Reponses = new[] { "Hadès", "Poséidon", "Zeus", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est la femme d'Hadès ?", Reponses = new[] { "Perséphone", "Déméter", "Héra", "Athéna" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui est la mère de Perséphone ?", Reponses = new[] { "Déméter", "Héra", "Athéna", "Aphrodite" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui a enlevé Perséphone ?", Reponses = new[] { "Hadès", "Zeus", "Poséidon", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Que mange Perséphone aux Enfers ?", Reponses = new[] { "Une grenade", "Une pomme", "Un raisin", "Une figue" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui a négocié le retour de Perséphone ?", Reponses = new[] { "Zeus", "Hadès", "Déméter", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui accompagne Perséphone aux Enfers ?", Reponses = new[] { "Hermès", "Hadès", "Déméter", "Zeus" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Quel est le nom du chien qui garde les Enfers ?", Reponses = new[] { "Cerbère", "Orthos", "Argos", "Sphinx" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Combien de têtes a Cerbère ?", Reponses = new[] { "3", "1", "2", "4" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui a ramené Cerbère sur Terre ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Ulysse" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui a tenté de kidnapper Perséphone ?", Reponses = new[] { "Pirithoos", "Thésée", "Héraclès", "Persée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 1, Intitule = "Qui a été puni aux Enfers pour avoir tenté de kidnapper Perséphone ?", Reponses = new[] { "Pirithoos", "Thésée", "Héraclès", "Persée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom du fleuve de la haine ?", Reponses = new[] { "Le Styx", "Le Léthé", "L'Achéron", "Le Cocyte" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom du fleuve du feu ?", Reponses = new[] { "Le Phlégéthon", "Le Styx", "Le Léthé", "Le Cocyte" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom du fleuve des lamentations ?", Reponses = new[] { "Le Cocyte", "Le Styx", "Le Léthé", "L'Achéron" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom du fleuve de l'oubli ?", Reponses = new[] { "Le Léthé", "Le Styx", "L'Achéron", "Le Cocyte" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Quel est le nom du fleuve de la douleur ?", Reponses = new[] { "L'Achéron", "Le Styx", "Le Léthé", "Le Cocyte" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui sont les trois juges des Enfers ?", Reponses = new[] { "Minos, Éaque, Rhadamanthe", "Zeus, Poséidon, Hadès", "Les Érinyes", "Les Moires" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui est le juge des âmes justes ?", Reponses = new[] { "Éaque", "Minos", "Rhadamanthe", "Hadès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui est le juge des âmes asiatiques ?", Reponses = new[] { "Rhadamanthe", "Minos", "Éaque", "Hadès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui est le juge des âmes européennes ?", Reponses = new[] { "Minos", "Rhadamanthe", "Éaque", "Hadès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Où vont les âmes vertueuses ?", Reponses = new[] { "Les Champs Élysées", "Le Tartare", "L'Asphodèle", "Le Pré de l'Oubli" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Où vont les âmes damnées ?", Reponses = new[] { "Le Tartare", "Les Champs Élysées", "L'Asphodèle", "Le Pré de l'Oubli" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Où vont les âmes neutres ?", Reponses = new[] { "L'Asphodèle", "Les Champs Élysées", "Le Tartare", "Le Pré de l'Oubli" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui sont les gardiens du Tartare ?", Reponses = new[] { "Les Hécatonchires", "Les Cyclopes", "Les Titans", "Les Érinyes" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui a été puni dans le Tartare pour avoir défié les dieux ?", Reponses = new[] { "Les Titans", "Les Géants", "Les Cyclopes", "Les Hécatonchires" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 1, NiveauChapitre = 2, Intitule = "Qui est le gardien des Enfers ?", Reponses = new[] { "Cerbère", "Charon", "Hadès", "Minos" }, BonneReponse = 0 },

                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Qui est la prêtresse de Delphes ?", Reponses = new[] { "La Pythie", "La Sibylle", "La Vestale", "La Prêtresse" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel dieu inspire la Pythie ?", Reponses = new[] { "Apollon", "Zeus", "Hermès", "Hadès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Qui a tué le serpent Python ?", Reponses = new[] { "Apollon", "Zeus", "Hermès", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Que signifie le nom Pythie ?", Reponses = new[] { "Celle qui pourrit", "Celle qui chante", "Celle qui danse", "Celle qui prie" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Qui a consulté l'oracle de Delphes avant la guerre de Troie ?", Reponses = new[] { "Agamemnon", "Ulysse", "Achille", "Ménélas" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Qui a consulté l'oracle de Delphes avant de fonder une colonie ?", Reponses = new[] { "Les colons grecs", "Les rois", "Les prêtres", "Les marchands" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel est le nom de l'oracle de Zeus à Dodone ?", Reponses = new[] { "Dodone", "Delphes", "Olympie", "Épidaure" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Comment les prêtresses de Dodone rendent-elles les oracles ?", Reponses = new[] { "En écoutant le vent dans les chênes", "En lisant les entrailles", "En dormant", "En chantant" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Quel est le nom du sanctuaire d'Asclépios ?", Reponses = new[] { "Épidaure", "Delphes", "Dodone", "Olympie" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Comment Asclépios guérit-il les malades ?", Reponses = new[] { "Par le rêve", "Par les plantes", "Par la chirurgie", "Par la magie" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Qui est le père d'Asclépios ?", Reponses = new[] { "Apollon", "Zeus", "Hermès", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Qui a tué Asclépios ?", Reponses = new[] { "Zeus", "Apollon", "Hadès", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Pourquoi Zeus a-t-il tué Asclépios ?", Reponses = new[] { "Pour avoir ressuscité les morts", "Pour avoir volé le feu", "Pour avoir défié les dieux", "Pour avoir trahi" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Qui est le fils d'Asclépios ?", Reponses = new[] { "Machaon", "Podalire", "Télesphore", "Hygie" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 1, Intitule = "Qui est la fille d'Asclépios ?", Reponses = new[] { "Hygie", "Machaon", "Podalire", "Télesphore" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Œdipe son destin ?", Reponses = new[] { "Tirésias", "Cassandre", "Calchas", "Phinée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Pâris la chute de Troie ?", Reponses = new[] { "Cassandre", "Hélène", "Hécube", "Andromaque" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Ulysse son long voyage ?", Reponses = new[] { "Tirésias", "Calchas", "Phinée", "Mélampous" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Énée la fondation de Rome ?", Reponses = new[] { "La Sibylle", "Cassandre", "La Pythie", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Jason la conquête de la Toison d'Or ?", Reponses = new[] { "Phinée", "Tirésias", "Calchas", "Mélampous" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Bellérophon la chute de Pégase ?", Reponses = new[] { "Phinée", "Tirésias", "Calchas", "Mélampous" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Crésus sa chute ?", Reponses = new[] { "La Pythie", "Cassandre", "La Sibylle", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Xerxès sa défaite ?", Reponses = new[] { "La Pythie", "Cassandre", "La Sibylle", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Alexandre sa conquête de l'Asie ?", Reponses = new[] { "La Pythie", "Cassandre", "La Sibylle", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à César sa mort ?", Reponses = new[] { "Un devin étrusque", "Cassandre", "La Pythie", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Auguste sa victoire ?", Reponses = new[] { "La Sibylle", "Cassandre", "La Pythie", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Néron sa chute ?", Reponses = new[] { "La Pythie", "Cassandre", "La Sibylle", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Constantin sa conversion ?", Reponses = new[] { "La Sibylle", "Cassandre", "La Pythie", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Julien l'Apostat sa mort ?", Reponses = new[] { "Un devin", "Cassandre", "La Pythie", "Hécate" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 2, NiveauChapitre = 2, Intitule = "Qui a prédit à Théodose sa victoire ?", Reponses = new[] { "La Sibylle", "Cassandre", "La Pythie", "Hécate" }, BonneReponse = 0 },

                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus beau des dieux ?", Reponses = new[] { "Apollon", "Arès", "Hermès", "Dionysos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est la plus belle des déesses ?", Reponses = new[] { "Aphrodite", "Héra", "Athéna", "Artémis" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus fort des dieux ?", Reponses = new[] { "Zeus", "Poséidon", "Hadès", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus rapide des dieux ?", Reponses = new[] { "Hermès", "Apollon", "Arès", "Dionysos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus sage des dieux ?", Reponses = new[] { "Athéna", "Zeus", "Apollon", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus jaloux des dieux ?", Reponses = new[] { "Héra", "Aphrodite", "Athéna", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus rancunier des dieux ?", Reponses = new[] { "Héra", "Aphrodite", "Athéna", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus farceur des dieux ?", Reponses = new[] { "Hermès", "Apollon", "Dionysos", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus laid des dieux ?", Reponses = new[] { "Héphaïstos", "Arès", "Dionysos", "Hadès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus jeune des dieux ?", Reponses = new[] { "Dionysos", "Apollon", "Hermès", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus vieux des dieux ?", Reponses = new[] { "Hestia", "Zeus", "Poséidon", "Hadès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus fidèle des dieux ?", Reponses = new[] { "Héra", "Aphrodite", "Déméter", "Hestia" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus volage des dieux ?", Reponses = new[] { "Zeus", "Poséidon", "Apollon", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le plus vénéré des mortels ?", Reponses = new[] { "Athéna", "Héra", "Aphrodite", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 1, Intitule = "Qui est le seul dieu à avoir une mère mortelle ?", Reponses = new[] { "Dionysos", "Héraclès", "Hermès", "Apollon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été jeté du haut de l'Olympe par Zeus ?", Reponses = new[] { "Héphaïstos", "Arès", "Apollon", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été jeté du haut de l'Olympe par Héra ?", Reponses = new[] { "Héphaïstos", "Arès", "Apollon", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été enchaîné par Zeus pour avoir aidé Héra ?", Reponses = new[] { "Héphaïstos", "Arès", "Apollon", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été puni pour avoir défié Zeus ?", Reponses = new[] { "Prométhée", "Atlas", "Cronos", "Japet" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été puni pour avoir défié Hadès ?", Reponses = new[] { "Pirithoos", "Thésée", "Héraclès", "Persée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été puni pour avoir défié Poséidon ?", Reponses = new[] { "Ulysse", "Ajax", "Achille", "Agamemnon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été puni pour avoir défié Athéna ?", Reponses = new[] { "Arachné", "Méduse", "Cassandre", "Hélène" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été puni pour avoir défié Aphrodite ?", Reponses = new[] { "Psyché", "Hélène", "Cassandre", "Arachné" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été puni pour avoir défié Héra ?", Reponses = new[] { "Héraclès", "Persée", "Thésée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été puni pour avoir défié Artémis ?", Reponses = new[] { "Actéon", "Orion", "Hippolyte", "Endymion" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été puni pour avoir défié Apollon ?", Reponses = new[] { "Marsyas", "Midas", "Pan", "Daphnis" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été puni pour avoir défié Dionysos ?", Reponses = new[] { "Penthée", "Lycurgue", "Orphée", "Midas" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été puni pour avoir défié Hermès ?", Reponses = new[] { "Battos", "Midas", "Pan", "Daphnis" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été puni pour avoir défié Arès ?", Reponses = new[] { "Halirrhothios", "Midas", "Pan", "Daphnis" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 3, NiveauChapitre = 2, Intitule = "Qui a été puni pour avoir défié Héphaïstos ?", Reponses = new[] { "Aphrodite", "Arès", "Midas", "Pan" }, BonneReponse = 0 },

                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est la mère de tous les monstres ?", Reponses = new[] { "Échidna", "Gaïa", "Héra", "Méduse" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est le père de tous les monstres ?", Reponses = new[] { "Typhon", "Cronos", "Ouranos", "Poséidon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a tué Typhon ?", Reponses = new[] { "Zeus", "Poséidon", "Hadès", "Apollon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a tué Échidna ?", Reponses = new[] { "Argos", "Héraclès", "Persée", "Thésée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est le chien de Géryon ?", Reponses = new[] { "Orthos", "Cerbère", "Argos", "Ladon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui garde les pommes d'or des Hespérides ?", Reponses = new[] { "Ladon", "Cerbère", "Orthos", "Argos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui est le dragon à cent têtes ?", Reponses = new[] { "Ladon", "Cerbère", "Orthos", "Argos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a tué Ladon ?", Reponses = new[] { "Héraclès", "Persée", "Thésée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a tué Orthos ?", Reponses = new[] { "Héraclès", "Persée", "Thésée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a tué le lion de Némée ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Achille" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a tué l'Hydre de Lerne ?", Reponses = new[] { "Héraclès", "Thésée", "Persée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a tué la Chimère ?", Reponses = new[] { "Bellérophon", "Persée", "Thésée", "Héraclès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a tué le Sphinx ?", Reponses = new[] { "Œdipe", "Thésée", "Héraclès", "Persée" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a tué le Minotaure ?", Reponses = new[] { "Thésée", "Héraclès", "Persée", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 1, Intitule = "Qui a tué la Méduse ?", Reponses = new[] { "Persée", "Thésée", "Héraclès", "Jason" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le roi des dieux et des hommes ?", Reponses = new[] { "Zeus", "Poséidon", "Hadès", "Apollon" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le dieu du tonnerre ?", Reponses = new[] { "Zeus", "Poséidon", "Hadès", "Arès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le dieu de la mer ?", Reponses = new[] { "Poséidon", "Zeus", "Hadès", "Océan" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le dieu des morts ?", Reponses = new[] { "Hadès", "Zeus", "Poséidon", "Thanatos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est la déesse de la sagesse ?", Reponses = new[] { "Athéna", "Héra", "Aphrodite", "Artémis" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est la déesse de l'amour ?", Reponses = new[] { "Aphrodite", "Héra", "Athéna", "Artémis" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le dieu de la guerre ?", Reponses = new[] { "Arès", "Apollon", "Hermès", "Héphaïstos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est la déesse du mariage ?", Reponses = new[] { "Héra", "Aphrodite", "Athéna", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le dieu du vin ?", Reponses = new[] { "Dionysos", "Apollon", "Hermès", "Pan" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est la déesse de la chasse ?", Reponses = new[] { "Artémis", "Athéna", "Héra", "Déméter" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le dieu du feu ?", Reponses = new[] { "Héphaïstos", "Arès", "Apollon", "Hermès" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est la déesse des moissons ?", Reponses = new[] { "Déméter", "Héra", "Athéna", "Hestia" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le dieu du commerce ?", Reponses = new[] { "Hermès", "Apollon", "Arès", "Dionysos" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est la déesse du foyer ?", Reponses = new[] { "Hestia", "Héra", "Déméter", "Athéna" }, BonneReponse = 0 },
                new Question { Niveau = Difficulte.Divin, Chapitre = 4, NiveauChapitre = 2, Intitule = "Qui est le dieu de la lumière ?", Reponses = new[] { "Apollon", "Hermès", "Arès", "Héphaïstos" }, BonneReponse = 0 }
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
            MenuPanel.Visibility = Visibility.Collapsed;
            ChangerFond("assets/fondblur.png");
            DifficultyPanel.Visibility = Visibility.Visible;
            TopBar.Visibility = Visibility.Visible;
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
            DifficultyPanel.Visibility = Visibility.Collapsed;
            ChapterPanel.Visibility = Visibility.Visible;
            MettreAJourChapitres();
        }

        private void MettreAJourChapitres()
        {
            ChapterTitle.Text = $"CHAPITRES - {LibelleDifficulte(difficulteChoisie).ToUpper()}";
            }

        private void Chapter_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            int chapitre = int.Parse(b.Tag?.ToString() ?? "1");
            chapitreActuel = chapitre;
            niveauDansChapitre = 1;
            vies = MaxVies;
            ChargerNiveau();
            ChapterPanel.Visibility = Visibility.Collapsed;
            GamePanel.Visibility = Visibility.Visible;
            AfficherVies();
            AfficherQuestion();
        }

        private void ChargerNiveau()
        {
            questionsPartie = toutesLesQuestions
                .Where(q => q.Niveau == difficulteChoisie && q.Chapitre == chapitreActuel && q.NiveauChapitre == niveauDansChapitre)
                .ToList();
            niveauActuel = 0;
        }

        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            if (GamePanel.Visibility == Visibility.Visible)
            {
                GamePanel.Visibility = Visibility.Collapsed;
                ChapterPanel.Visibility = Visibility.Visible;
            }
            else if (ChapterPanel.Visibility == Visibility.Visible)
            {
                ChapterPanel.Visibility = Visibility.Collapsed;
                DifficultyPanel.Visibility = Visibility.Visible;
            }
            else if (DifficultyPanel.Visibility == Visibility.Visible)
            {
                DifficultyPanel.Visibility = Visibility.Collapsed;
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
            ChapterPanel.Visibility = Visibility.Collapsed;
            DifficultyPanel.Visibility = Visibility.Collapsed;
            MenuPanel.Visibility = Visibility.Visible;
            TopBar.Visibility = Visibility.Collapsed;
        }

        private void Rejouer_Click(object sender, RoutedEventArgs e)
        {
            EndPanel.Visibility = Visibility.Collapsed;
            vies = MaxVies;
            niveauDansChapitre = 1;
            ChargerNiveau();
            GamePanel.Visibility = Visibility.Visible;
            AfficherVies();
            AfficherQuestion();
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
            LevelText.Text = $"CHAPITRE {chapitreActuel} - NIVEAU {niveauDansChapitre} - QUESTION {niveauActuel + 1} / {questionsPartie.Count}";
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
                if (niveauDansChapitre == 1)
                {
                    niveauDansChapitre = 2;
                    ChargerNiveau();
                    AfficherQuestion();
                    return;
                }
                else
                {
                    AfficherFin(true);
                    return;
                }
            }
            AfficherQuestion();
        }

        private void AfficherFin(bool victoire)
        {
            GamePanel.Visibility = Visibility.Collapsed;
            EndPanel.Visibility = Visibility.Visible;
            if (victoire)
            {
                EndTitle.Text = "CHAPITRE TERMINÉ";
                EndTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
                EndMessage.Text = $"Bravo ! Tu as terminé le chapitre {chapitreActuel}\navec {vies} vies restantes !";
            }
            else
            {
                EndTitle.Text = "DÉFAITE";
                EndTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                EndMessage.Text = $"Tu as perdu toutes tes vies...\nTu as atteint la question {niveauActuel + 1} du chapitre {chapitreActuel}.";
            }
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