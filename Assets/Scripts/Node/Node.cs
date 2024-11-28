using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public GameObject BG;
    public GameObject startpoint;
    public GameObject endPoint;
    public GameObject topEdge;
    public GameObject leftEdge;
    public GameObject rightEdge;
    public GameObject bottomEdge;
    public GameObject highLight;

    private MainGame mainGame;
    private Dictionary<Node, GameObject> connectedEdges;
    public List<Node> connectedNodeList;
    public Vector2Int Pos2D;
    public int colorId;

    public bool IsWin
    {
        get
        {
            if (startpoint.activeSelf || endPoint.activeSelf)
            {
                return connectedNodeList.Count == 1;
            }

            return connectedNodeList.Count == 2;
        }
    }

    public bool IsClickable
    {
        get
        {
            if (IsStartNode)
            {
                return true;
            }
            else if (connectedNodeList.Count == 2)
            {
                if (connectedNodeList[0].isConnectingComplete && connectedNodeList[1].isConnectingComplete)
                    return false;
            }
            else if (IsEndNode)
                return false;
            
            return connectedNodeList.Count > 0;
        }
    }

    public bool isConnectingComplete = false;

    public bool IsStartNode => startpoint.activeSelf;
    public bool IsEndNode => endPoint.activeSelf;

    public void Init()
    {
        startpoint.SetActive(false);
        endPoint.SetActive(false);
        topEdge.SetActive(false);
        bottomEdge.SetActive(false);
        leftEdge.SetActive(false);
        rightEdge.SetActive(false);
        highLight.SetActive(false);

        mainGame = FindObjectOfType<MainGame>();
        connectedEdges = new Dictionary<Node, GameObject>();
        connectedNodeList = new List<Node>();
    }


    public void SetColorAndPoint(int _colorId, int isStart = -1)
    {
        colorId = _colorId;
        switch (isStart)
        {
            case 1:
                startpoint.SetActive(true);
                startpoint.GetComponent<Image>().color = mainGame.nodeColorList[colorId % mainGame.nodeColorList.Count];
                break;
            case 2:
                endPoint.SetActive(true);
                endPoint.GetComponent<Image>().color = mainGame.nodeColorList[colorId % mainGame.nodeColorList.Count];
                break;
            case -1:
                Debug.Log("NotPoint");
                break;
        }
    }

    public void SetEdge(Vector2Int offset, Node node)
    {
        if (offset == new Vector2Int(-1, 0))
        {
            connectedEdges[node] = topEdge;
            return;
        }

        if (offset == new Vector2Int(1, 0))
        {
            connectedEdges[node] = bottomEdge;
            return;
        }

        if (offset == new Vector2Int(0, 1))
        {
            connectedEdges[node] = rightEdge;
            return;
        }

        if (offset == new Vector2Int(0, -1))
        {
            connectedEdges[node] = leftEdge;
            return;
        }
    }

    public void UpdateInput(Node connectedNode)
    {
        // 에지 목록에 없는 노드는 포함도 시키지 않음
        if (!connectedEdges.ContainsKey(connectedNode))
        {
            return;
        }

        // 연결한 노드가 이미 있는 노드로 리스트에 존재한다면?
        // 해당 파트의 노드를 삭제시킴
        if (connectedNodeList.Contains(connectedNode))
        {
            connectedNodeList.Remove(connectedNode);
            connectedNode.connectedNodeList.Remove(this);
            RemoveEdge(connectedNode);
            DeleteNode();
            connectedNode.DeleteNode();
            return;
        }

        //자기 자신의 노드가 2개이상 연결되버렸다면?
        if (connectedNodeList.Count == 2)
        {
            if (mainGame.connectingCount > 0)
                mainGame.connectingCount--;

            Node tempNode = connectedNodeList[0];

            if (!tempNode.IsConnectedToEndNode())   // 끝 노드와 연결되어 있지 않다면
            {
                connectedNodeList.Remove(tempNode);    // tempNode를 삭제
                tempNode.connectedNodeList.Remove(this);
                RemoveEdge(tempNode);
                tempNode.DeleteNode();
            }
            else
            {
                tempNode = connectedNodeList[1];
                connectedNodeList.Remove(tempNode);
                tempNode.connectedNodeList.Remove(this);
                RemoveEdge(tempNode);
                tempNode.DeleteNode();
            }
        }

        // 연결된 노드가 2개이상 연결되어 있다면
        if (connectedNode.connectedNodeList.Count == 2)
        {
            if (mainGame.connectingCount > 0)
                mainGame.connectingCount--;

            Node tempNode = connectedNode.connectedNodeList[0];
            connectedNode.connectedNodeList.Remove(tempNode);
            tempNode.connectedNodeList.Remove(connectedNode);
            connectedNode.RemoveEdge(tempNode);
            tempNode.DeleteNode();     
        }

        //색깔이 다르고, 1개 연결되어 있는 노드가 존재한다면
        if (connectedNode.connectedNodeList.Count == 1 && connectedNode.colorId != colorId)
        {
            if (mainGame.connectingCount > 0)
                mainGame.connectingCount--;

            Node tempNode = connectedNode.connectedNodeList[0];
            connectedNode.connectedNodeList.Remove(tempNode);
            tempNode.connectedNodeList.Remove(connectedNode);
            connectedNode.RemoveEdge(tempNode);
            tempNode.DeleteNode();
        }

        if(connectedNode.IsStartNode)
        {
            Debug.Log($"IsConnecting{this.gameObject.name} is StartNode");
            DeleteNode();
            return;
        }

        // 나 자신의 시작 노드를 가르키게 된다면? , 내가 연결하고자 하는 노드가 시작노드라면?
        if (connectedNodeList.Count == 1 && IsStartNode)
        {     
            if (mainGame.connectingCount > 0)
                mainGame.connectingCount--;
            
            Node tempNode = connectedNodeList[0];
            connectedNodeList.Remove(tempNode);
            tempNode.connectedNodeList.Remove(this);
            RemoveEdge(tempNode);
            tempNode.DeleteNode();           
        }

        //연결된 노드가, 끝 노드고, 연결된 갯수가 1개일 때
        if (connectedNode.connectedNodeList.Count == 1 && connectedNode.IsEndNode)
        {
            if (mainGame.connectingCount > 0)
                mainGame.connectingCount--;

            Node tempNode = connectedNode.connectedNodeList[0];
            connectedNode.connectedNodeList.Remove(tempNode);
            tempNode.connectedNodeList.Remove(connectedNode);
            connectedNode.RemoveEdge(tempNode);
            tempNode.DeleteNode();
        }
        AddEdge(connectedNode);
    }
    private void AddEdge(Node connectedNode)
    {
        connectedNode.colorId = colorId;
        connectedNode.connectedNodeList.Add(this);
        connectedNodeList.Add(connectedNode);
        GameObject connectedEdge = connectedEdges[connectedNode];
        connectedEdge.SetActive(true);
        GameObject connectedEdge2 = connectedNode.connectedEdges[this];
        connectedEdge2.SetActive(true);
        connectedEdge.GetComponent<Image>().color = mainGame.nodeColorList[colorId % mainGame.nodeColorList.Count];
        connectedEdge2.GetComponent<Image>().color = mainGame.nodeColorList[colorId % mainGame.nodeColorList.Count];
    }

    public void RemoveEdge(Node node)
    {
        GameObject edge = connectedEdges[node];
        edge.SetActive(false);        
        edge = node.connectedEdges[this];
        edge.SetActive(false);
    }

    public void DeleteNode()
    {
        Node startNode = this;
        while (startNode != null)
        {
            startNode.isConnectingComplete = false;
            startNode.highLight.SetActive(false);

            Node tempNode = null;
            if (startNode.connectedNodeList.Count != 0)
            {
                tempNode = startNode.connectedNodeList[0];  
                tempNode.connectedNodeList.Remove(startNode);

                startNode.RemoveEdge(tempNode);
                startNode.connectedNodeList.Clear();
            }
            startNode = tempNode;
        }
    }

    public bool IsConnectedToEndNode(List<Node> checkedNode = null)
    {
        if (checkedNode == null)
        {
            checkedNode = new List<Node>();
        }

        if (IsEndNode)
        {
            return true;
        }

        foreach (var item in connectedNodeList)
        {
            if (!checkedNode.Contains(item))
            {
                checkedNode.Add(item);
                return item.IsConnectedToEndNode(checkedNode);
            }
        }

        return false;
    }

    public void SolveHighlight()
    {
        if (connectedNodeList.Count == 0)
        {
            highLight.SetActive(false);
            return;
        }

        List<Node> checkingNodes = new List<Node>() { this };
        List<Node> resultNodes = new List<Node>() { this };

        while (checkingNodes.Count > 0)
        {
            foreach (var item in checkingNodes[0].connectedNodeList)   // (1,1 / 1,2)
            {
                if (!resultNodes.Contains(item))
                {
                    resultNodes.Add(item);
                    checkingNodes.Add(item);
                }
            }

            checkingNodes.Remove(checkingNodes[0]);
        }

        checkingNodes.Clear();

        // 시작 노드와 끝 노드를 탐색
        foreach (var item in resultNodes)
        {
            if (item.IsStartNode || item.IsEndNode)
            {
                checkingNodes.Add(item);
            }
        }

        if (checkingNodes.Count == 2)
        {
            highLight.SetActive(true);
            highLight.GetComponent<Image>().color = mainGame.GetHighLightColor(colorId);
        }
        else
        {
            highLight.SetActive(false);
        }

    }

}
