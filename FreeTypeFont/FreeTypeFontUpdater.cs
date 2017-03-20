using UnityEngine;

[ExecuteInEditMode]
public class FreeTypeFontUpdater : MonoBehaviour
{
    private static FreeTypeFontUpdater s_instance;

    private event System.Action m_updateQueue;

    public static void ScheduleUpdate(System.Action handler)
    {
        if (s_instance == null)
        {
            var name = "FreeTypeFontUpdater";
            var go = GameObject.Find(name);
            if (go == null)
            {
                go = new GameObject(name);
            }
            s_instance = go.GetComponent<FreeTypeFontUpdater>();
            if (s_instance == null)
            {
                s_instance = go.AddComponent<FreeTypeFontUpdater>();
                s_instance.hideFlags = HideFlags.HideAndDontSave;
            }
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(s_instance);
            }
        }

        s_instance.m_updateQueue += handler;
    }

    void Update()
    {
        if (m_updateQueue != null)
        {
            m_updateQueue();
            m_updateQueue = null;
        }
    }

    void OnDestroy()
    {
        s_instance = null;
    }
}