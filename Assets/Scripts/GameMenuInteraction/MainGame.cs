using PipeConnectGame.Common;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGame : UI_Scene
{
    enum MainGameObj
    {
        MainGamePanel,
    }

    enum MainGameTMP_Text
    {
        Stage_Level_Info,
        WinText,
    }

    enum MainGameImage
    {
        Info_Title,
        Complete,
    }

    enum MainGameButton
    {
        Exit,
        Home,
        LevelSelect,
        ReStart,
        NextLevel,
    }

    public bool isGameFinished;
    private bool isInit;
    private bool isCheckingWining;

    private TMP_Text titleText;
    private TMP_Text winText;
    private SpriteRenderer clickHighLight;
    private GridLayoutGroup boardGridLayoutGroup;
    private GameObject mainGamePanel;

    private Image completeImage;

    private LevelData currentLevelData;

    private Stack<Node> connectingNode = new Stack<Node>();       // 연결되어 있는 노드를 관리
    private List<Node> nodeList = new List<Node>();             // 전체 노드의 리스트
    //private List<RotateNode> rotateNodeList = new List<RotateNode>();
    public Dictionary<Vector2Int, Node> nodeGrid = new Dictionary<Vector2Int, Node>();  // 위치값에 맞는 노드

    public List<Color> nodeColorList = new List<Color>();
    public Image info_Title;
    //public Dictionary<Vector2Int, bool> rotateEdgePointsDic = new Dictionary<Vector2Int, bool>();

    private int currentStage;
    private int currentLevel;
    public int connectingCount;
    public override void Init()
    {
        isCam = true;
        isInit = false;
        isGameFinished = false;
        currentLevelData = Managers.Pipe.now_Level;
        connectingCount = 0;
        Managers.Ads.HideBanner();

        base.Init();

        Bind<GameObject>(typeof(MainGameObj));
        Bind<Button>(typeof(MainGameButton));
        Bind<TMP_Text>(typeof(MainGameTMP_Text));
        Bind<Image>(typeof(MainGameImage));

        clickHighLight = GameObject.Find("ClickHighLighter").GetComponent<SpriteRenderer>();

        titleText = GetTMP_Text((int)MainGameTMP_Text.Stage_Level_Info);
        titleText.text = $"{Managers.Pipe.currentStage} Level {Managers.Pipe.currentLevel}";
        winText = GetTMP_Text((int)MainGameTMP_Text.WinText);

        completeImage = GetImage((int)MainGameImage.Complete);

        GetButton((int)MainGameButton.Home).BindEvent((PointerEventData data) =>
        {
            Managers.Pipe.UnlockLevel();
            Managers.Scene.LoadScene(Define.Scene.MainMenu);
        }, Define.UIEvent.Click);
        GetButton((int)MainGameButton.LevelSelect).BindEvent((PointerEventData data) =>
        {
            Managers.Pipe.UnlockLevel();
            MainScene.isSelectMenu = true;
            MainScene.menu_Name = "Level_Menu";
            Managers.Scene.LoadScene(Define.Scene.MainMenu);            
        }, Define.UIEvent.Click);
        GetButton((int)MainGameButton.ReStart).BindEvent((PointerEventData data) =>
        {           
            Managers.Scene.LoadScene(Define.Scene.Game);
        }, Define.UIEvent.Click);
        GetButton((int)MainGameButton.NextLevel).BindEvent((PointerEventData data) =>
        {
            Managers.Pipe.UnlockLevel();
            Managers.Pipe.now_Level = Managers.Pipe.GetLevel();
            Managers.Scene.LoadScene(Define.Scene.Game);
        }, Define.UIEvent.Click);
        GetButton((int)MainGameButton.Exit).BindEvent((PointerEventData data) =>
        {
            if (!isGameFinished)
            {
                MainScene.isSelectMenu = true;
                MainScene.menu_Name = "Level_Menu";
                Managers.Scene.LoadScene(Define.Scene.MainMenu);
            }
        }, Define.UIEvent.Click);

        info_Title = GetImage((int)MainGameImage.Info_Title);

        //foreach (var item in currentLevelData.so_Edges)
        //{
        //    for (int i = 0; i < item.sm_RotateEdgePoints.Count; i++)    // 딕셔너리에 회전가능한 파이프의 위치와, 그 파이프가 꺾은 파이프인지에 대한 정보를 넘겨줌
        //    {
        //        rotateEdgePointsDic.Add(item.sm_RotateEdgePoints[i], item.sm_RotateEdgeisBend[i]);
        //    }
        //}

        completeImage.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);

        mainGamePanel = GetObject((int)MainGameObj.MainGamePanel);
        boardGridLayoutGroup = mainGamePanel.GetComponent<GridLayoutGroup>();

        SetBoard();
        isInit = true;
    }

    private void SetBoard()
    {
        currentStage = (int)Managers.Pipe.currentStage;
        currentLevel = Managers.Pipe.currentLevel;
        for (int i = 0; i <= currentStage + 1; i++)
        {
            nodeColorList.Add(Managers.Pipe.node_ColorLibrary.presets[i].color);
        }
        switch (currentStage)
        {
            case (int)DifficultyEnum.Easy:
                info_Title.color = new Color(150f / 255f, 255f / 255f, 150f / 255f, 1f);
                info_Title.SetAllDirty();
                boardGridLayoutGroup.padding.left = 80;
                boardGridLayoutGroup.padding.right = 80;
                boardGridLayoutGroup.padding.top = 140;
                boardGridLayoutGroup.padding.bottom = 140;
                boardGridLayoutGroup.cellSize = new Vector2(200f, 200f);
                boardGridLayoutGroup.spacing = new Vector2(5f, 5f);
                break;
            case (int)DifficultyEnum.Normal:
                info_Title.color = new Color(255f / 255f, 220f / 255f, 100f / 255f, 1f);
                info_Title.SetAllDirty();
                boardGridLayoutGroup.padding.left = 30;
                boardGridLayoutGroup.padding.right = 30;
                boardGridLayoutGroup.padding.top = 100;
                boardGridLayoutGroup.padding.bottom = 100;
                boardGridLayoutGroup.cellSize = new Vector2(180f, 180f);
                boardGridLayoutGroup.spacing = new Vector2(5f, 5f);
                break;
            case (int)DifficultyEnum.Hard:
                break;
        }
        SpawnNodes();

    }

    private void SpawnNodes()
    {
        int cellCount = currentStage + 4;
        Node spawnNode = null;
        //RotateNode rotateNode = null;

        for (int i = 0; i < cellCount; i++)
        {
            for (int j = 0; j < cellCount; j++)
            {
                //if (rotateEdgePointsDic.ContainsKey(new Vector2Int(i,j)))
                //{
                //    rotateNode = Managers.Resource.Instantiate("RotateCell", mainGamePanel.transform).GetComponent<RotateNode>();
                //    rotateNode.Init(rotateEdgePointsDic[new Vector2Int(i,j)]);
                //    rotateNode.gameObject.name = i.ToString() + j.ToString() + "Rotate";
                //    rotateNode.Pos2D = new Vector2Int(i,j);
                //    rotateNodeList.Add(rotateNode);

                //    int colorSpawnRotateNode = RotateNodeColor(i, j);
                //    rotateNode.SetColorAndPoint(colorSpawnRotateNode);
                //    continue;
                //}
                spawnNode = Managers.Resource.Instantiate("Cell", mainGamePanel.transform).GetComponent<Node>();
                spawnNode.Init();
                nodeList.Add(spawnNode);

                int isStartNode = NodeType(i, j);
                int colorSpawnNode = NodeColor(i, j);
                if (colorSpawnNode != -1)
                {
                    spawnNode.SetColorAndPoint(colorSpawnNode, isStartNode);
                }

                nodeGrid.Add(new Vector2Int(i, j), spawnNode);
                spawnNode.gameObject.name = i.ToString() + j.ToString();
                spawnNode.Pos2D = new Vector2Int(i, j);
            }
        }

        List<Vector2Int> offsetPos = new List<Vector2Int>()
            {Vector2Int.up,Vector2Int.down,Vector2Int.left,Vector2Int.right };

        foreach (var item in nodeGrid)
        {
            foreach (var offset in offsetPos)
            {
                var checkPos = item.Key + offset;
                if (nodeGrid.ContainsKey(checkPos))
                {
                    item.Value.SetEdge(offset, nodeGrid[checkPos]);
                }
            }
        }
        if (spawnNode != null)
        {
            switch (currentStage)
            {
                case (int)DifficultyEnum.Easy:
                    SetNode(160);
                    break;
                case (int)DifficultyEnum.Normal:
                    SetNode(140);
                    break;
                case (int)DifficultyEnum.Hard:
                    break;

            }
        }
    }

    private void SetNode(int size)
    {
        foreach (Node node in nodeList)
        {
            node.GetComponent<BoxCollider2D>().size = new Vector2(size + 20, size + 20);
            node.startpoint.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
            node.BG.GetComponent<RectTransform>().sizeDelta = new Vector2(size + 20, size + 20);
            node.topEdge.GetComponent<RectTransform>().sizeDelta = new Vector2(size / 2, size + 70);
            node.bottomEdge.GetComponent<RectTransform>().sizeDelta = new Vector2(size / 2, size + 70);
            node.rightEdge.GetComponent<RectTransform>().sizeDelta = new Vector2(size / 2, size + 70);
            node.leftEdge.GetComponent<RectTransform>().sizeDelta = new Vector2(size / 2, size + 70);
            node.highLight.GetComponent<RectTransform>().sizeDelta = new Vector2(size + 40, size + 40);
        }

        //foreach( RotateNode rotateNode in rotateNodeList)
        //{
        //    rotateNode.BG.GetComponent<RectTransform>().sizeDelta = new Vector2(size + 20, size + 20);
        //    rotateNode.normalRotateNode.GetComponent<RectTransform>().sizeDelta = new Vector2(size / 2, size+20);
        //    rotateNode.bendRotateNode.GetComponent<RectTransform>().sizeDelta = new Vector2(size / 2, size+20);
        //    rotateNode.hightLight.GetComponent<RectTransform>().sizeDelta = new Vector2(size + 40, size + 40);
        //}
    }

    public int NodeType(int i, int j)
    {
        List<Edge> edges = currentLevelData.so_Edges;
        Vector2Int point = new Vector2Int(i, j);
        for (int k = 0; k < edges.Count; k++)
        {
            if (edges[k].sm_StartPoint == point)
            {
                return 1;
            }
            else if (edges[k].sm_EndPoint == point)
            {
                return 2;
            }
        }
        return -1;
    }

    public int NodeColor(int i, int j)
    {
        List<Edge> edges = currentLevelData.so_Edges;
        Vector2Int point = new Vector2Int(i, j);
        for (int colorId = 0; colorId < edges.Count; colorId++)
        {
            if (edges[colorId].sm_StartPoint == point ||
                edges[colorId].sm_EndPoint == point)
            {
                return colorId;
            }
        }

        return -1;
    }

    public int RotateNodeColor(int i, int j)
    {
        List<Edge> edges = currentLevelData.so_Edges;
        Vector2Int point = new Vector2Int(i, j);
        for (int colorId = 0; colorId < edges.Count; colorId++)
        {
            for (int k = 0; k < edges[colorId].sm_RotateEdgePoints.Count; k++)
            {
                if (edges[colorId].sm_RotateEdgePoints[k] == point)
                    return colorId;
            }
        }

        return -1;
    }


    private Node nowNode;

    private void Update()
    {
        if (isGameFinished || !isInit) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    nowNode = null;
                    break;
                case TouchPhase.Moved:
                    MovedEvent_PipeNode(touch);
                    break;
                case TouchPhase.Ended:
                    EndedEvent_PipeNode();
                    break;
            }
        }
        if (isCheckingWining)
            CheckWin();
    }
    private void MovedEvent_PipeNode(Touch _touch)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(_touch.position);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D nowHit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (nowNode == null)
        {
            if (nowHit.collider != null && nowHit.collider.TryGetComponent(out Node _FirstNode)
               && _FirstNode.IsClickable) // 완료된 파이프는 클릭 불가
            {
                nowNode = _FirstNode;
                clickHighLight.gameObject.SetActive(true);
                clickHighLight.gameObject.transform.position = (Vector3)mousePos2D;
                clickHighLight.color = GetHighLightColor(nowNode.colorId);
                if (!connectingNode.Contains(_FirstNode))
                    connectingNode.Push(_FirstNode);
            }
            return;
        }

        clickHighLight.gameObject.transform.position = (Vector3)mousePos2D;

        if (nowHit.collider != null && nowHit.collider.TryGetComponent(out Node _tempNode)  // 닫아있는 콜라이더가 현재 콜라이더에서 벗어난다면
            && nowNode != _tempNode)
        {
            // 대각선으로 이동을 막기 위한 논리 추가
            Vector2Int currentPos = nowNode.Pos2D;
            Vector2Int targetPos = _tempNode.Pos2D;
            Vector2Int difference = targetPos - currentPos;

            // 대각선이 아니라 상하좌우 이동인지 확인
            if (Mathf.Abs(difference.x) + Mathf.Abs(difference.y) != 1)
            {
                Debug.Log("Cross Touch");
                return; // 대각선인 경우, 이동하지 않음
            }

            // 컬러가 다른색이 끝 노드로 연결된다면
            if ((nowNode.colorId != _tempNode.colorId && _tempNode.IsEndNode) || _tempNode.IsStartNode)
            {
                Debug.Log("Another Color End Node");
                return;
            }

            if (!connectingNode.Contains(_tempNode))
                connectingNode.Push(_tempNode);

            nowNode.UpdateInput(_tempNode);
            isCheckingWining = true;
            nowNode = null;
        }
        //else if(nowHit.collider != null && nowHit.collider.TryGetComponent(out RotateNode _rotateNode)
        //    && nowNode.colorId == _rotateNode.colorId)
        //{

        //}
    }

    private void EndedEvent_PipeNode()
    {        
        if (connectingNode.Count != 0 && !connectingNode.Peek().IsEndNode)
        {
            foreach (var node in connectingNode)
            {
                if (node.connectedNodeList.Count > 0)
                {
                    Node tempNode = node.connectedNodeList[0];
                    // 연결된 노드를 제거
                    if (tempNode.connectedNodeList.Contains(node))
                    {
                        node.connectedNodeList.Remove(tempNode);
                        tempNode.connectedNodeList.Remove(node);
                        node.RemoveEdge(tempNode);
                        tempNode.DeleteNode();                  // 모든 데이터들을 순회하여 삭제진행
                    }
                }
                if (node.IsStartNode)
                {
                    node.isConnectingComplete = false;
                    node.highLight.SetActive(false);
                }
            }
        }
        // 스택과 하이라이트 초기화
        nowNode = null;
        connectingNode.Clear();
        clickHighLight.gameObject.SetActive(false);
    }

    private void CheckWin()
    {
        connectingCount = 0;

        bool IsConnectWinning = true;

        foreach (var item in nodeList)
        {
            if (item.IsEndNode && item.IsWin)
                connectingCount++;
            item.SolveHighlight();
        }

        foreach (var item in connectingNode)
        {
            IsConnectWinning &= item.IsWin;
            if (!IsConnectWinning)
            {
                item.isConnectingComplete = false;
                isCheckingWining = false;
                return;
            }
            else
            {
                item.isConnectingComplete = true;
            }
        }
        connectingNode.Clear();
        isCheckingWining = false;

        if (connectingCount >= currentLevelData.so_Edges.Count)
        {
            //Managers.Sound.PlayBgm(Define.BGM.Count, true);
            Managers.Sound.PlaySFX(Define.SFX.Success, -1);
            //Managers.Sound.StopSfx(Define.SFX.Success);

            completeImage.gameObject.SetActive(true);
            winText.gameObject.SetActive(true);
            clickHighLight.gameObject.SetActive(false);

            --Managers.ads_Count;
            if (Managers.ads_Count <= 0)
            {
                Managers.Ads.ShowInterstitialAd();
                Managers.ads_Count = 3;
            }

            isGameFinished = true;
        }
    }

    public Color GetHighLightColor(int colorID)
    {
        Color result = nodeColorList[colorID % nodeColorList.Count];
        result.a = 0.4f;
        return result;
    }
}
