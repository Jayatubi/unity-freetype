using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class FreeTypeFontUpdater : MonoBehaviour
{

    public static void ScheduleUpdate(System.Action handler)
    {
        var name = "FreeTypeFontUpdater";
        var go = GameObject.Find(name);
        if (go == null)
        {
            go = new GameObject(name);
            go.hideFlags = HideFlags.HideAndDontSave;
        }
        var updater = go.GetComponent<FreeTypeFontUpdater>();
        if (updater == null)
        {
            updater = go.AddComponent<FreeTypeFontUpdater>();
        }
        updater.StartCoroutine(UpdateImpl(handler));
    }

    private static IEnumerator UpdateImpl(System.Action handler)
    {
        yield return 0;
        handler();
    }
}