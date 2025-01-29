using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private struct NodeInfo
    {
        public bool fruitPresent;
        public bool snakePartPresent;
        public Vector3 centro;
        public NodeInfo(bool fruit, bool snakePart, Vector3 cent)
        {
            fruitPresent = fruit;
            snakePartPresent = snakePart;
            centro = cent;
        }
    }
    public static GameManager Instance;
    private NodeInfo[,] myNodos; // Informacion sobre las cosas presentes en los nodos
    private List<Vector2> freeNodes; // Nodos libres donde instanciar fruta

    [SerializeField]
    private GameObject fruit;

    private GameObject instantiatedFruit;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        freeNodes = new List<Vector2>();
        myNodos = new NodeInfo[28, 19];
        for (int i = -16; i < 12; i++)
        {
            for (int j = -9; j > -28; j--)
            {
                int nodeX = 16 + i;
                int nodeY = -9 - j;
                myNodos[nodeX, nodeY] = new NodeInfo(false,false, new Vector3(i, 0, 18 + j));
                if(i !=-16 && i != 11 && j != -9 && j != -27)
                {
                    freeNodes.Add(new Vector2(nodeX, nodeY));
                }
                
            }
        }
        instantiatedFruit = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (instantiatedFruit == null) InstantiateFruit();
    }

    public void InstantiateFruit()
    {
        Vector2 where = freeNodes[Random.Range(0, freeNodes.Count)];
        freeNodes.Remove(where);
        instantiatedFruit = Instantiate(fruit, myNodos[Mathf.RoundToInt(where.x), Mathf.RoundToInt(where.y)].centro,Quaternion.identity);
        myNodos[Mathf.RoundToInt(where.x), Mathf.RoundToInt(where.y)].fruitPresent = true;
    }


    public void occupieNode(int nodeX, int nodeY)
    {
        myNodos[nodeX,nodeY].snakePartPresent = true;
        freeNodes.Remove(new Vector2(nodeX, nodeY));
    }
    public void deOccupieNode(int nodeX, int nodeY)
    {
        myNodos[nodeX, nodeY].snakePartPresent = false;
        freeNodes.Add(new Vector2(nodeX, nodeY));
    }
    public bool isThereFruit(int nodeX, int nodeY)
    {
        return myNodos[nodeX, nodeY].fruitPresent;
    }
    public void eatFruit(int nodeX, int nodeY)
    {
        myNodos[nodeX,nodeY].fruitPresent = false;
        Destroy(instantiatedFruit);
        instantiatedFruit = null;
        // No ponemos en nodosFree porque habra serpiente que se haya comido la fruta
    }
    public bool isThereSnake(int nodeX, int nodeY)
    {
        return myNodos[nodeX, nodeY].snakePartPresent;
    }
}
