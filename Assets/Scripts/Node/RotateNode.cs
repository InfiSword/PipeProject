using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateNode : BaseNode
{
    public GameObject BG;
    public GameObject normalRotateNode;
    public GameObject bendRotateNode;
    public GameObject highLight;

    private GameObject activeNode;
    public Vector2Int vec_1 { get; private set; }
    public Vector2Int vec_2 { get; private set; }
    private Dictionary<Node, Vector2Int> connectedEdges;

    private MainGame mainGame;

    public bool isVec_1_Clear { get; private set; }
    public bool isVec_2_Clear { get; private set; }
    public bool isBend;
    public int currentRotation;
    public int colorId;

    public override void Init()
    {
        mainGame = FindObjectOfType<MainGame>();
        currentRotation = 0;
        if (isBend)
        {
            normalRotateNode.SetActive(false);
            bendRotateNode.SetActive(true); ;
            vec_1 = new Vector2Int(0, -1);      // left방향
            vec_2 = new Vector2Int(1, 0);       // down방향
            activeNode = bendRotateNode;
        }
        else
        {
            normalRotateNode.SetActive(true);
            bendRotateNode.SetActive(false);
            vec_1 = new Vector2Int(-1, 0);      // up방향
            vec_2 = new Vector2Int(1, 0);       // down방향  
            activeNode = normalRotateNode;
        }
        connectedEdges = new Dictionary<Node, Vector2Int>();
        highLight.SetActive(false);
    }

    // 회전 시킴
    private void RotateNodeFunc()
    {
        // 90도씩 회전시킴
        currentRotation = (currentRotation + 90) % 360;
        transform.Rotate(0, 0, 90);
    }

    public void SetEdge(Vector2Int offset, Node node)
    {
        if (offset == new Vector2Int(-1, 0))
        {
            connectedEdges[node] = offset;      // topEdge
            return;
        }
        else if (offset == new Vector2Int(1, 0))
        {
            connectedEdges[node] = offset;      // bottom
            return;
        }
        else if (offset == new Vector2Int(0, 1))
        {
            connectedEdges[node] = offset;      // right
            return;
        }
        else if (offset == new Vector2Int(0, -1))
        {
            connectedEdges[node] = offset;      // left
            return;
        }
    }


    // 이어나갈 방향을 정해주는 함수 
    // 이어나갈 방향을 추가로 지정해서 만들어줘야 함
    public void SetRotation_CorrectDirection()
    {
        RotateNodeFunc();
        if (!isBend)
        {
            switch (currentRotation)
            {
                case 270:
                case 0:
                    vec_1 = new Vector2Int(-1, 0);      // up방향
                    vec_2 = new Vector2Int(1, 0);       // down방향            
                    break;
                case 90:                    
                case 180:
                    vec_1 = new Vector2Int(0, -1);      // left방향 
                    vec_2 = new Vector2Int(0, 1);       // right방향
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (currentRotation)
            {
                case 270:
                    vec_1 = new Vector2Int(0, -1);
                    vec_2 = new Vector2Int(-1, 0);
                    break;
                case 0:
                    vec_1 = new Vector2Int(0, -1);      // left방향
                    vec_2 = new Vector2Int(1, 0);       // down방향            
                    break;
                case 90:
                    vec_1 = new Vector2Int(1, 0);
                    vec_2 = new Vector2Int(0, 1);
                    break;
                case 180:
                    vec_1 = new Vector2Int(0, 1);      // right방향 
                    vec_2 = new Vector2Int(-1, 0);       // up방향
                    break;
                default:
                    break;
            }
        }       
    }

    // 색깔과 목표지점 세팅( 꺾은 파이프를 해당 파이프 색깔 지점을 지정해줌 )
    public void SetColorAndPoint(int _colorId)
    {
        colorId = _colorId;
        if (activeNode == normalRotateNode)
            activeNode.GetComponent<Image>().color = mainGame.nodeColorList[colorId % mainGame.nodeColorList.Count];
        else if (activeNode == bendRotateNode)
        {
            for (int i = 0; i < activeNode.transform.childCount; i++)
            {
                activeNode.GetComponentsInChildren<Image>(true)[i].color = mainGame.nodeColorList[colorId % mainGame.nodeColorList.Count];
            }
        }
    }

    public void UpdateRotateNode(Node nowNode, Node connectingNode = null)
    {
        int connectingAble_Vec_1x = Pos2D.x + vec_1.x;
        int connectingAble_Vec_1y = Pos2D.y + vec_1.y;

        int connectingAble_Vec_2x = Pos2D.x + vec_2.x;
        int connectingAble_Vec_2y = Pos2D.y + vec_2.y;

        Vector2Int vec1 = new Vector2Int(Mathf.Abs(connectingAble_Vec_1x), Mathf.Abs(connectingAble_Vec_1y));
        Vector2Int vec2 = new Vector2Int(Mathf.Abs(connectingAble_Vec_2x), Mathf.Abs(connectingAble_Vec_2y));

        // 각각의 노드 위치를 나타냄 
        //Debug.Log("Pos2D: " + Pos2D);
        //Debug.Log("nowNode.Pos2D: " + nowNode.Pos2D);
        //Debug.Log("vec1: " + vec1);
        //Debug.Log("vec2: " + vec2);

        if (vec1 == nowNode.Pos2D || vec2 == nowNode.Pos2D)
        {
            if (vec1 == nowNode.Pos2D)
            {   //connectingNode.vectorEdges[vec_1].SetActive(true);
                isVec_1_Clear = true;
                nowNode.UpdateRotateNode(this, vec_1, connectingNode);
            }
            else if (vec2 == nowNode.Pos2D)
            {   //connectingNode.vectorEdges[vec_2].SetActive(true);                           
                isVec_2_Clear = true;
                nowNode.UpdateRotateNode(this, vec_2, connectingNode);
            }
            highLight.SetActive(true);
            highLight.GetComponent<Image>().color = mainGame.GetHighLightColor(colorId);
        }
    }

    public void Reset_RotateNode()
    {
        highLight.SetActive(false);
        isVec_1_Clear = false;
        isVec_2_Clear = false;
    }
}
