using System.Xml.Serialization;

interface IActionStrategy
{
    bool CanExecute { get; }
    bool Complete { get; }

    void Start()
    {

    }

    void Update(float deltaTime)
    {

    }

    void Stop()
    {

    }
}