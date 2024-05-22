using System;
using UnityEngine;

public interface ISaveAble
{
    public string SaveId { get; }
}

[Serializable]
public class Saveable<T> : ISaveAble where T : class, ISaveAble
{
    public string SaveId { get; set; }

    string ISaveAble.SaveId => SaveId;

    public void Save()
    {
        BeforeSave();

        SaveSystemByTornike.Save(this as T);

        Saved();
    }

    public void Load(out T loadedInfo, Action onLoaded = null)
    {
        if (!SaveSystemByTornike.TryLoad(this as T, out loadedInfo))
        {
            Save();
            Load(out loadedInfo);
            return;
        }

        onLoaded?.Invoke();
        Loaded();

        SaveSystemByTornike.OnSave -= Save;
        SaveSystemByTornike.OnSave += Save;
    }

    public virtual void BeforeSave() { }

    public virtual void Saved() { }

    public virtual void Loaded() { }
}