using UnityEngine.AI;
using UnityEngine;

interface IAction
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

    void CompleteAction()
    {

    }
}
