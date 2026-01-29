namespace Phorg;

public interface IPrompt
{
    T Ask<T>(string question, T? def = default);
    void Say(string message);
    void Success(string message);
    void Warn(string message);
}
