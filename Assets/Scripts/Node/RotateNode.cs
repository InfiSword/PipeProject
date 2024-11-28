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

    public void Init(bool isBend)
    {
        mainGame = FindObjectOfType<MainGame>();
        if (isBend)
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
    public void IsCorrectDirection(Vector2Int offset)
    {
        switch (currentRotation)
        {
            case 0:
                vec_1 = new Vector2Int(0, -1);  // left
                vec_2 = new Vector2Int(1, 0);   // bottom
                break;
            case 90:
                break;
            case 180:
                break; // offset == Vector2Int.up || offset == Vector2Int.down;
            case 270:
                //return offset == Vector2Int.left || offset == Vector2Int.right;
                break;
            default:
                break;
        }
    }

    public void CorrectBendOrNormal()
    {

    }

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
