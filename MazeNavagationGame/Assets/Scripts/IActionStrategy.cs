using UnityEngine.AI;
using UnityEngine;

public interface IActionFunctionality
{
    bool CanExecute { get; }
    bool Complete { get; }
    bool CanStartThenStart();

    void Update(float deltaTime);

    void Stop()
    {

    }

    void CompleteAction()
    {

    }
}
