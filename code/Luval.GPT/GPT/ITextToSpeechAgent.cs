namespace Luval.GPT.GPT
{
    public interface ITextToSpeechAgent
    {
        Task<Stream> CreateAudioStreamAsync(string text);
    }
}