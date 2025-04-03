using System.Collections.Generic;
using UnityEngine;

public class SnakeControl : MonoBehaviour
{
    protected struct Nodo
    {
        public Vector3 centro;
        public Vector3 direccion;
        public float rotationNeeded;
        public Nodo(Vector3 cent, Vector3 dir)
        {
            centro = cent;
            direccion = dir;
            rotationNeeded = 0;
        }
    }
    protected struct BodyPart
    {
        public Vector3 direccion;
        public GameObject parte;
        public BodyPart(Vector3 dir, GameObject part)
        {
            parte = part;
            direccion = dir;
        }
    }
    protected Nodo[,] myNodos;
    protected List<BodyPart> bodyParts;
    protected BodyPart headPart;
    protected BodyPart tailPart;

    protected float elapsedTime;
    protected bool growthNeeded;
    protected Vector2 headNode;

    [SerializeField]
    protected Vector3 myDirection;
    [SerializeField]
    protected bool playerOne;
    [SerializeField]
    protected GameObject headPartObj;
    [SerializeField]
    protected GameObject bodyPartObj;
    [SerializeField]
    protected GameObject tailPartObj;
    [SerializeField]
    protected float speed;
    protected bool keepPlaying;
    protected List<Vector2> snakePositions;




    // Start is called before the first frame update
    protected virtual void Start()
    {
        bodyParts = new List<BodyPart>();
        headPart = new BodyPart(new Vector3(playerOne ? 1 : -1, 0, 0), headPartObj);
        tailPart = new BodyPart(new Vector3(playerOne ? 1 : -1, 0, 0), tailPartObj);
        bodyParts.Add(new BodyPart(new Vector3(playerOne ? 1 : -1, 0, 0), bodyPartObj));
        snakePositions = new List<Vector2>();
        // La x total mide 26 nodos, empezando en -16 <-> 11 /-17 y 12 son limites
        // La y total mide 19 nodos empezando en 8 <-> -8 / 9 y -9 son limites
        myNodos = new Nodo[30, 19];
        for (int i = -17; i < 13; i++)
        {
            for (int j = -9; j > -28; j--)
            {
                int nodeX = 17 + i;
                int nodeY = -9 - j;
                myNodos[nodeX, nodeY] = new Nodo(new Vector3(i, 0, 18 + j), new Vector3(0, 0, 0));
            }
        }
        growthNeeded = false;
        keepPlaying = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        elapsedTime += Time.deltaTime;
    }
    /// <summary>
    /// Utilidad para pasar de posiciones reales a nodo Y
    /// </summary>
    /// <param name="zPos"> Posición de z sacada de un transform</param>
    /// <returns>Nodo Y en el que se está</returns>
    protected int posToYNode(float zPos)
    {
        return -4 - Mathf.RoundToInt(zPos); // La y total mide 8 nodos empezando en 4
    }
    /// <summary>
    /// Utilidad para pasar de posiciones reales a nodo X
    /// </summary>
    /// <param name="xPos"> Posición de x sacada de un transform</param>
    /// <returns>Nodo X en el que se está</returns>
    protected int posToXNode(float xPos)
    {
        return 10 + Mathf.RoundToInt(xPos); // La y total mide 8 nodos empezando en 4
    }
    public void setKeepPlaying(bool keepPlay)
    {
        keepPlaying = keepPlay;
    }

    /// <summary>
    /// Se encarga de mover cada parte del cuerpo de la serpiente segun pasa por los nodos y, si necesita cambiar la direccion,
    /// la cambia
    /// </summary>
    protected void Move()
    {
        
        int nodeX = 17 + Mathf.RoundToInt(headPart.parte.transform.position.x); // Para que no se cambie la direccion hasta haber alcanzado el nodo
        int nodeY = 9 - Mathf.RoundToInt(headPart.parte.transform.position.z);
        // Te mueves en la direccion que diga ese nodo si es diferente a tu dirección
        if (myNodos[nodeX, nodeY].direccion != new Vector3(0, 0, 0) && myNodos[nodeX, nodeY].direccion != headPart.direccion)
        {

            headPart.parte.transform.Rotate(Vector3.up, myNodos[nodeX, nodeY].rotationNeeded);

            headPart.direccion = myNodos[nodeX, nodeY].direccion;
            myDirection = headPart.direccion;
            
        }
        // Movimiento discreto mejor, por nodos, no continuo con delta.
        headPart.parte.transform.position = myNodos[nodeX + Mathf.RoundToInt(headPart.direccion.x), nodeY - Mathf.RoundToInt(headPart.direccion.z)].centro;
        Vector2 pos = new Vector2(myNodos[nodeX + Mathf.RoundToInt(headPart.direccion.x), nodeY - Mathf.RoundToInt(headPart.direccion.z)].centro.x,
            myNodos[nodeX + Mathf.RoundToInt(headPart.direccion.x), nodeY - Mathf.RoundToInt(headPart.direccion.z)].centro.z);
        snakePositions.Add(pos);
        headNode = pos;

            GameManager.Instance.occupieNode(nodeX + Mathf.RoundToInt(headPart.direccion.x), nodeY - Mathf.RoundToInt(headPart.direccion.z));

            nodeX = 17 + Mathf.RoundToInt(headPart.parte.transform.position.x); // Para comprobar si hay fruta o serpiente
            nodeY = 9 - Mathf.RoundToInt(headPart.parte.transform.position.z);
            // Te comes la fruta
            if (GameManager.Instance.isThereFruit(nodeX, nodeY))
            {
                growthNeeded = true;
                GameManager.Instance.eatFruit(nodeX, nodeY);
            }

            for (int i = 0; i < bodyParts.Count; i++)
            {
                nodeX = 17 + Mathf.RoundToInt(bodyParts[i].parte.transform.position.x);
                nodeY = 9 - Mathf.RoundToInt(bodyParts[i].parte.transform.position.z);
                BodyPart myPart = bodyParts[i];

                if (myNodos[nodeX, nodeY].direccion != new Vector3(0, 0, 0) && myNodos[nodeX, nodeY].direccion != myPart.direccion)
                {

                    myPart.parte.transform.Rotate(Vector3.up, myNodos[nodeX, nodeY].rotationNeeded);


                    myPart.direccion = myNodos[nodeX, nodeY].direccion; // Se guarda la direccion a seguir


                }
                myPart.parte.transform.position = myNodos[nodeX + Mathf.RoundToInt(myPart.direccion.x), nodeY - Mathf.RoundToInt(myPart.direccion.z)].centro;
                GameManager.Instance.occupieNode(nodeX + Mathf.RoundToInt(myPart.direccion.x), nodeY - Mathf.RoundToInt(myPart.direccion.z));
                bodyParts[i] = myPart;
                pos = new Vector2(myNodos[nodeX + Mathf.RoundToInt(myPart.direccion.x), nodeY - Mathf.RoundToInt(myPart.direccion.z)].centro.x,
                myNodos[nodeX + Mathf.RoundToInt(myPart.direccion.x), nodeY - Mathf.RoundToInt(myPart.direccion.z)].centro.z);
                snakePositions.Add(pos);
            }
            nodeX = 17 + Mathf.RoundToInt(tailPart.parte.transform.position.x);
            nodeY = 9 - Mathf.RoundToInt(tailPart.parte.transform.position.z);
            // Para que no gire sin parar en el mismo nodo
            if (!growthNeeded)
            {
                if (myNodos[nodeX, nodeY].direccion != new Vector3(0, 0, 0) && myNodos[nodeX, nodeY].direccion != tailPart.direccion)
                {
                    // Falta rotar las cosas

                    tailPart.direccion = myNodos[nodeX, nodeY].direccion; // La cola tiene la direccion
                    tailPart.parte.transform.Rotate(Vector3.up, myNodos[nodeX, nodeY].rotationNeeded);

                    myNodos[nodeX, nodeY].direccion = new Vector3(0, 0, 0); // Si pasa la cola, se reinicia el nodo para otro giro
                    myNodos[nodeX, nodeY].rotationNeeded = 0;
                }

                tailPart.parte.transform.position = myNodos[nodeX + Mathf.RoundToInt(tailPart.direccion.x), nodeY - Mathf.RoundToInt(tailPart.direccion.z)].centro;
                GameManager.Instance.occupieNode(nodeX + Mathf.RoundToInt(tailPart.direccion.x), nodeY - Mathf.RoundToInt(tailPart.direccion.z));
                pos = new Vector2(myNodos[nodeX + Mathf.RoundToInt(tailPart.direccion.x), nodeY - Mathf.RoundToInt(tailPart.direccion.z)].centro.x,
                myNodos[nodeX + Mathf.RoundToInt(tailPart.direccion.x), nodeY - Mathf.RoundToInt(tailPart.direccion.z)].centro.z);
                snakePositions.Add(pos);

                GameManager.Instance.deOccupieNode(nodeX, nodeY); // Si la cola pasa, hay que desocupar el nodo, no queda más serpiente.

            }
            else growSomething();
        if (headPart.parte.transform.position.x < -16 || headPart.parte.transform.position.x > 11 ||
       headPart.parte.transform.position.z < -8 || headPart.parte.transform.position.z > 8) // Si has perdido
        {
            GameManager.Instance.lostGame(playerOne);
        }

    }

    protected void growSomething()
    {
        int previusNodeX = 17 + Mathf.RoundToInt(bodyParts[bodyParts.Count - 1].parte.transform.position.x -
           Mathf.RoundToInt(bodyParts[bodyParts.Count - 1].direccion.x));
        int previusNodeY = 9 - Mathf.RoundToInt(bodyParts[bodyParts.Count - 1].parte.transform.position.z -
           Mathf.RoundToInt(bodyParts[bodyParts.Count - 1].direccion.z));
        GameObject newBodyPart = Instantiate(bodyPartObj, myNodos[previusNodeX, previusNodeY].centro, bodyParts[bodyParts.Count - 1].parte.transform.rotation);
        Vector3 dir = bodyParts[bodyParts.Count - 1].direccion;
        bodyParts.Add(new BodyPart(dir, newBodyPart));
        growthNeeded = false;
    }
    protected void turnRigth()
    {
        int nodeX = 17 + Mathf.RoundToInt(headPart.parte.transform.position.x);
        int nodeY = 9 - Mathf.RoundToInt(headPart.parte.transform.position.z);
        //Marcas el siguiente nodo de tu direccion para giro
        Vector3 newDirect = Quaternion.AngleAxis(90, Vector3.up) * myDirection;
        newDirect.x = Mathf.RoundToInt(newDirect.x);
        newDirect.z = Mathf.RoundToInt(newDirect.z);
        newDirect = newDirect.normalized;
        myNodos[nodeX, nodeY].direccion = newDirect;
        myNodos[nodeX, nodeY].rotationNeeded = 90;
    }
    protected void turnLeft()
    {
        int nodeX = 17 + Mathf.RoundToInt(headPart.parte.transform.position.x);
        int nodeY = 9 - Mathf.RoundToInt(headPart.parte.transform.position.z);
        //Marcas el siguiente nodo de tu direccion para giro
        Vector3 newDirect = Quaternion.AngleAxis(-90, Vector3.up) * myDirection;
        newDirect.x = Mathf.RoundToInt(newDirect.x);
        newDirect.z = Mathf.RoundToInt(newDirect.z);
        newDirect = newDirect.normalized;
        myNodos[nodeX, nodeY].direccion = newDirect;
        myNodos[nodeX, nodeY].rotationNeeded = -90;
    }
}
