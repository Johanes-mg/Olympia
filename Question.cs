namespace Olympia
{
    public enum Difficulte
    {
        Mortals,
        DemiDieu,
        Divin
    }

    public class Question
    {
        public string Intitule { get; set; } = "";
        public string[] Reponses { get; set; } = new string[4];
        public int BonneReponse { get; set; }
        public Difficulte Niveau { get; set; }
    }
}