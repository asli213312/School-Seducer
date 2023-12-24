namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public interface ILocalizator<TTranslation> where TTranslation : ITranslation
    {
        string GetTranslation(string languageCode, int indexElement);
    }
}