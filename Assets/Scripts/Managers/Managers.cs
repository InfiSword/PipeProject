using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

    #region Core

    AdsManager _ads = new AdsManager();
    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    UIManager _ui = new UIManager();
    SoundManager _sound;
    PipeGameManager _pipe;

    public static AdsManager Ads { get { return Instance._ads; } }
    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static PipeGameManager Pipe { get { return Instance._pipe; } }
    #endregion

    public static PlayerData nowPlayerData;

    public static int ads_Count = 3;

    void Start()
    {
        Init();
	}

    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            GameObject sound = GameObject.Find("@Sound");
            GameObject pipe = GameObject.Find("@Pipe");
            GameObject frame = GameObject.Find("@Frame");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                sound = new GameObject { name = "@Sound" };
                pipe = new GameObject { name = "@Pipe" };
                frame = new GameObject { name = "@Frame" };
                sound.transform.SetParent(go.transform);
                pipe.transform.SetParent(go.transform);
                frame.transform.SetParent(go.transform);
            }

            DontDestroyOnLoad(go);

            s_instance = go.GetOrAddComponent<Managers>();

            s_instance._sound = sound.GetOrAddComponent<SoundManager>();

            s_instance._pipe = pipe.GetOrAddComponent<PipeGameManager>();

            frame.GetOrAddComponent<FrameRateManager>();

            s_instance._data.Init();
            s_instance._pool.Init();
            s_instance._sound.Init();
            s_instance._pipe.Init();
            s_instance._ads.Init();

            nowPlayerData = new PlayerData();

            s_instance._data.LoadData();
        }
    }

    public static void Clear()
    {
        Input.Clear();
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        //Pool.Clear();
    }
}
