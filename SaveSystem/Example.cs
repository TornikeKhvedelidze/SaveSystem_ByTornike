using System;
using UnityEngine;

[Serializable]
public class ExampleSaveSystem : Saveable<ExampleSaveSystem>
{
    public string Name;
    public float one;
    public int two;
    public bool three;
    public float passedTime;
    public DateTime time;

    public override void BeforeSave()
    {
        TimeSpan timePassed = DateTime.Now - time;

        float timePassedInSeconds = (float)timePassed.TotalSeconds;

        float roundedTimePassed = (float)Math.Round(timePassedInSeconds, 2);

        passedTime = roundedTimePassed;
    }

    public override void Saved()
    {
        Debug.Log(passedTime);
    }

    public override void Loaded()
    {
        time = DateTime.Now;
    }
}


public class Example : MonoBehaviour
{
    [SerializeField] private ExampleSaveSystem _exampleSaveSystem;

    private void OnEnable()
    {
        _exampleSaveSystem.Load(out _exampleSaveSystem, Loaded);
    }
    private void OnDisable()
    {
        _exampleSaveSystem.Save();
    }

    private void OnDestroy()
    {
        _exampleSaveSystem.Save();
    }
    private void OnApplicationQuit()
    {
        _exampleSaveSystem.Save();
    }

    private void Loaded()
    {

    }
}
