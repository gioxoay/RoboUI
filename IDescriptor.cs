namespace RoboUI
{
    public interface IDescriptor
    {
        void Deserialize(string source);

        string Serialize();
    }
}
