using UnityEngine;

public class AutoMonoBehaviour : MonoBehaviour
{
    protected virtual void Reset()
    {
        LoadComponents();
        SetValues();
    }

    protected virtual void Awake()
    {
        LoadComponents();
        SetValues();
    }

    protected virtual void LoadComponents()
    {

    }

    protected virtual void SetValues()
    {

    }
}