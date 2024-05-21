using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveSystemByTornike : Singleton<SaveSystemByTornike>
{
    [SerializeField] private float _saveDelay = 5;
    [SerializeField] private bool _debugOn;

    public static Action OnSave;

    private static Dictionary<Type, List<ISaveAble>> _dictionary = new();

    private void Start()
    {
        Application.quitting += InvokeSave;

        StartCoroutine(Save_Coroutine());
    }

    private void OnApplicationPause(bool pause)
    {
        InvokeSave();
    }

    public static void Save<T>(T element) where T : ISaveAble
    {
        LoadAllElementOfType(element);

        var type = typeof(T);
        var path = GetFilePath(type.Name);

        var desiredList = _dictionary[type];
        var desiredElement = desiredList.Find(h => h.SaveId == element.SaveId);

        if (desiredElement != null)
        {
            desiredList.Remove(desiredElement);
        }

        desiredList.Add(element);

        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, desiredList);
        }

        TryDebug($"Saved Id: {element.SaveId} at: {path}");
    }

    public static bool TryLoad<T>(T element, out T desiredElement) where T : class, ISaveAble
    {
        LoadAllElementOfType(element);

        var type = typeof(T);

        if (!_dictionary.ContainsKey(type))
        {
            _dictionary[type] = new List<ISaveAble>();
        }

        var desiredList = _dictionary[type];
        desiredElement = desiredList.Find(h => h.SaveId == element.SaveId) as T;

        if (desiredElement != null)
        {
            TryDebug($"Loaded Id: {desiredElement.SaveId}");
            return true;
        }

        TryDebug($"Couldn't found Id: {element.SaveId}");
        return false;
    }

    private static void LoadAllElementOfType<T>(T element) where T :  ISaveAble
    {
        var type = typeof(T);
        var path = GetFilePath(type.Name);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                _dictionary[type] = formatter.Deserialize(stream) as List<ISaveAble>;
            }
        }
    }

#if UNITY_EDITOR
    [MenuItem("SaveSystem/Clear All Data")]
#endif
    public static void ClearAllInfo()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath);

        foreach (string file in files)
        {
            File.Delete(file);
        }

        Debug.Log($"Cleared all saved information");
    }

    private static string GetFilePath(string typeName)
    {
        return Path.Combine(Application.persistentDataPath, typeName + ".dat");
    }

    private static void TryDebug(string value)
    {
        if (!Instance._debugOn) return;

        Debug.Log(value);
    }

    public static void InvokeSave()
    {
        OnSave?.Invoke();
    }

    IEnumerator Save_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_saveDelay);

            InvokeSave();
        }
    }

}
