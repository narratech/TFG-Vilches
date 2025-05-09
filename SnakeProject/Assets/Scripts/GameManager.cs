using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private struct NodeInfo
    {
        public enum snakePresent
        {
            player1, player2,none
        }
        public snakePresent wichSnake;
        public bool fruitPresent;
        public bool snakePartPresent;
        public Vector3 centro;

        public NodeInfo(bool fruit, bool snakePart, Vector3 cent)
        {
            fruitPresent = fruit;
            snakePartPresent = snakePart;
            wichSnake = snakePresent.none;
            centro = cent;
        }
    }
    public static GameManager Instance;
    private NodeInfo[,] myNodos; // Informacion sobre las cosas presentes en los nodos
    private List<Vector2> freeNodes; // Nodos libres donde instanciar fruta

    [SerializeField]
    private GameObject fruit;
    private Vector2 fruitNode;
    [SerializeField]
    private GameObject player1;
    private List<Vector2> player1Nodes;
    private Vector2 player1Dir;
    [SerializeField]
    private GameObject player2;
    private List<Vector2> player2Nodes;
    private Vector2 player2Dir;
    [SerializeField]
    private GameObject GuiManager;

    private GameObject instantiatedFruit;

    private bool keepPlaying;
    private int player1Score;
    private int player2Score;

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        freeNodes = new List<Vector2>();
        myNodos = new NodeInfo[30, 19];
        for (int i = -17; i < 13; i++)
        {
            for (int j = -9; j > -28; j--)
            {
                int nodeX = 17 + i;
                int nodeY = -9 - j;
                myNodos[nodeX, nodeY] = new NodeInfo(false,false, new Vector3(i, 0, 18 + j));
                if(i >=-16 && i <= 11 && j <= -8 && j >= -26)
                {
                    freeNodes.Add(new Vector2(nodeX, nodeY));
                }
                
            }
        }
        instantiatedFruit = null;
        keepPlaying = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (instantiatedFruit == null) InstantiateFruit();
    }
    #region getters
    public Vector2 getFruitNode()
    {
        return fruitNode;
    }
    public List<Vector2> getPlayer1Positions()
    {
        return player1Nodes;
    }
    public List<Vector2> getPlayer2Positions()
    {
        return player2Nodes;
    }
    public Vector2 getPlayer1Dir()
    {
        return player1Dir;
    }
    public Vector2 getPlayer2Dir()
    {
        return player2Dir;
    }
    #endregion
    #region setters
    public void setPlayer1Positions(List<Vector2> positions)
    {
         player1Nodes = positions;
    }
    public void setPlayer2Positions(List<Vector2> positions)
    {
        player2Nodes = positions;
    }
    public void setPlayer1Dir(Vector2 dir)
    {
        player1Dir = dir;
    }
    public void setPlayer2Dir(Vector2 dir)
    {
        player2Dir = dir;
    }

    #endregion

    public void InstantiateFruit()
    {
        Vector2 where = freeNodes[Random.Range(0, freeNodes.Count)];
        freeNodes.Remove(where);
        instantiatedFruit = Instantiate(fruit, myNodos[Mathf.RoundToInt(where.x), Mathf.RoundToInt(where.y)].centro,Quaternion.identity);
        myNodos[Mathf.RoundToInt(where.x), Mathf.RoundToInt(where.y)].fruitPresent = true;
        fruitNode = new Vector2(Mathf.RoundToInt(where.x), Mathf.RoundToInt(where.y));
    }


    public void occupieNode(int nodeX, int nodeY, bool playerOne)
    {
        myNodos[nodeX,nodeY].snakePartPresent = true;
        myNodos[nodeX,nodeY].wichSnake = playerOne? NodeInfo.snakePresent.player1 : NodeInfo.snakePresent.player2;
        freeNodes.Remove(new Vector2(nodeX, nodeY));
    }
    public void deOccupieNode(int nodeX, int nodeY)
    {
        myNodos[nodeX, nodeY].snakePartPresent = false;
        myNodos[nodeX,nodeY].wichSnake = NodeInfo.snakePresent.none;
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
    public bool isThereSnake(int nodeX, int nodeY, bool playerOne)
    {
        if(myNodos[nodeX, nodeY].snakePartPresent)
        {
            if ((myNodos[nodeX, nodeY].wichSnake == NodeInfo.snakePresent.player1 && !playerOne) ||
                (myNodos[nodeX, nodeY].wichSnake == NodeInfo.snakePresent.player2 && playerOne)) return true;
            else return false;
        }
        else return false;
    }

    public void lostGame(bool isPlayerOne)
    {
        keepPlaying = false;
        GuiManager.GetComponent<GUIManager>().ShowWinText(!isPlayerOne); // Gana el que no pierde, facil :D
    }
    public void OnRetryReset()
    {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(0);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0); // Recarga la escena
        // DUDA: Reiniciar puntuacion
    }
    public bool getKeepPlaying()
    {
        return keepPlaying;
    }

    public float getPlayer1Score()
    {
        return player1Score;
    }

    public float getPlayer2Score()
    {
        return player2Score;
    }
    public void addPlayer1Score(int points)
    {
        player1Score += points;
        GuiManager.GetComponent<GUIManager>().ChangeP1Points(player1Score);
    }

    public void addPlayer2Score(int points)
    {
         player2Score+= points;
        GuiManager.GetComponent<GUIManager>().ChangeP2Points(player2Score);
    }
}
