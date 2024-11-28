using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolManager
{
    #region Pool
    class Pool
    {
        public GameObject Original { get; private set; }        // 풀로 저장할 오브젝트
        public Transform Root { get; set; }                     // 처음 Transform 위치

        Stack<Poolable> _poolStack = new Stack<Poolable>();     // Poolable를 저장하는 Stack

        public void Init(GameObject original, int count = 5, bool isUI = false)    // Pool 초기화
        {
            Original = original;                                    // 풀 오브젝트 받아오기
            if (!isUI)                                              // UI가 아니라면?
            {
                Root = new GameObject().transform;                  // 게임 오브젝트를 새로 만들고, 그 Transform가져오기
                Root.name = $"{original.name}_Root";                // Root 오브젝트 이름 설정
            }
            else
                Root = Managers.Pool.ui_Root;

            for (int i = 0; i < count; i++)                     // 카운트 만큼 실행
                Push(Create());
        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);       //GameObject타입의 Original클론을 생성하고, 그걸 go에 저장
            go.name = Original.name;                                        // go의 이름은 Original의 이름임
            return go.GetOrAddComponent<Poolable>();                        // go에 Poolable스크립트를 가져오거나, 추가해줌
        }

        public void Push(Poolable poolable)                // Poolable컴포넌트를 pool 시킴
        {
            if (poolable == null)
                return;

            poolable.transform.SetParent(Root);           // Poolable의 위치를 Root의 부모로함
            poolable.gameObject.SetActive(false);       // SetActive로 초기화
            poolable.IsUsing = false;                   // 사용 불가능한 상태로 초기화

            _poolStack.Push(poolable);                  // 스텍에 추가
        }

        public Poolable Pop(Transform parent)           // 풀에서 가져온다.
        {
            Poolable poolable;

            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();        // 스택에 하나라도 있으면, 제거하고, 제거한 스택을 poolable로 저장
            else
                poolable = Create();                // 스택에 하나라도 없으면, Create 함수를 실행하고 poolable에 저장

            poolable.gameObject.SetActive(true);    // SetActive On

            // DontDestroyOnLoad 해제 용도
            if (parent == null)                 // parent가 null이라면
                poolable.transform.SetParent(Managers.Scene.CurrentScene.transform);  // poolable의 위치를 CurrentScene의 부모로 들어감

            poolable.transform.SetParent(parent);        // null이 아니면, parent 부모로 들어감 ??
            poolable.IsUsing = true;                   // Using 온

            return poolable;                           // poolable ON
        }
    }
    #endregion

    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();        // 키값 string과 value값 pool인 딕셔너리를 만듦, 여러 개의 Pool을 들고 있는게 PoolManager가 될꺼임
    Transform ui_Root;
    Transform _root;
    Transform normal_Root;

    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;       // root가 없으면, 해당 이름의 오브젝트를 생성하고, 트랜스폼을받아옴

            ui_Root = new GameObject { name = "@UI_Pool_Root" }.transform;  

            normal_Root = new GameObject { name = "@Normal_Pool_Root" }.transform;            

            Object.DontDestroyOnLoad(_root);                                // 파괴되지 않음
            Object.DontDestroyOnLoad(normal_Root);
            Object.DontDestroyOnLoad(ui_Root);

            ui_Root.SetParent(_root);
            normal_Root.SetParent(_root);
        }
    }

    public void CreatePool(GameObject original, int count = 5, bool isUI = false)              // Pool을 만듬
    {
        Pool pool = new Pool();                 // 객체 생성
        if (!isUI)                               // UI가 아니라면?
        {
            pool.Init(original, count);         // pool 초기화
            pool.Root.parent = normal_Root;           // pool의 Root의 위치를 _root의 부모로한다.
        }
        else
        {
            pool.Init(original, count, true);   // UI Init
            pool.Root.parent = ui_Root;
        }
        _pool.Add(original.name, pool);     // 딕셔너리 추가
    }

    public void Push(Poolable poolable)                     // Pool에다 넣어라, 만약, 해당 UI오브젝트나, 다른 오브젝트를 비활성화 할려면, 여기 함수가 실행
    {
        string name = poolable.gameObject.name;
        if (_pool.ContainsKey(name) == false)               // 딕셔너리에 해당 이름의 string이 있는지 확인함
        {
            GameObject.Destroy(poolable.gameObject);        // 없다면 파괴
            return;
        }

        _pool[name].Push(poolable);         // 있다면 해당 이름의 Pool 클래스의 Push 함수를 실행
    }

    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_pool.ContainsKey(original.name) == false)      // 해당 이름의 string 키 값이 존재하는지 확인
        {
            if (original.GetComponent<Canvas>() == null)
                CreatePool(original);                           // CreatePool함수를 실행
            else
                CreatePool(original, 1, true);                  // UI라고 판단하고 풀을 만듦
        }
        return _pool[original.name].Pop(parent);            // 해당 Pool의 Pop함수 실행
    }

    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;
        return _pool[name].Original;
    }

    public void Clear()
    {
        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

        _pool.Clear();
    }
}
