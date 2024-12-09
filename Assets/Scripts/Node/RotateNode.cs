using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateNode : MonoBehaviour
{
    public GameObject BG;
    public GameObject normalRotateNode;
    public GameObject bendRotateNode;
    public GameObject hightLight;

    private GameObject activeNode;
    private Vector2Int vec_1;
    private Vector2Int vec_2;
    public Vector2Int Pos2D
    { get; set; }

    private MainGame mainGame;
    private bool isBend;
    public int currentRotation;
    public int colorId;

    public void Init(bool _isBend)
    {
        mainGame = FindObjectOfType<MainGame>();
        currentRotation = 0;
        if (_isBend)
        {
            normalRotateNode.SetActive(false);
            bendRotateNode.SetActive(true);
            isBend = true;
            activeNode = bendRotateNode;
        }
        else
        {
            normalRotateNode.SetActive(true);
            bendRotateNode.SetActive(false);
            isBend = false;
            activeNode = normalRotateNode;
        }
        hightLight.SetActive(false);
    }

    // 회전 시킴
    public void RotateNodeFunc()
    {
        // 90도씩 회전시킴
        currentRotation = (currentRotation + 90) % 360;
        transform.Rotate(0, 0, 90);
    }

    // 이어나갈 방향을 정해주는 함수 
    // 이어나갈 방향을 추가로 지정해서 만들어줘야 함
    public void SetCorrectDirection()
    {
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
                case 0:
                    vec_1 = new Vector2Int(0, -1);      // left방향
                    vec_2 = new Vector2Int(1, 0);       // down방향            
                    break;
                case 90:
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

    public void UpdateRotateNode()
    {

    }
}
