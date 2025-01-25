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
        public BodyPart(Vector2 dir, GameObject part)
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
    protected bool growing;
    protected Vector2 headNode;

    [SerializeField]
    protected Vector3 myDirection;
    [SerializeField]
    GameObject enemySnake;
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




    // Start is called before the first frame update
    protected virtual void Start()
    {
        bodyParts = new List<BodyPart>();
        headPart = new BodyPart(new Vector3(playerOne ? 1 : -1, 0, 0), headPartObj);
        tailPart = new BodyPart(new Vector3(playerOne ? 1 : -1, 0, 0), tailPartObj);
        bodyParts.Add(new BodyPart(new Vector3(playerOne ? 1 : -1, 0, 0), bodyPartObj));
        // La x total mide 26 nodos, empezando en -16 <-> 11
        // La y total mide 19 nodos empezando en 9 <-> -9
        myNodos = new Nodo[28, 19];
        for (int i = -16; i < 12; i++)
        {
            for (int j = -9; j > -28; j--)
            {
                int nodeX = 16 + i;
                int nodeY = -9 - j;
                myNodos[nodeX, nodeY] = new Nodo(new Vector3(i, 0, 18 + j), new Vector3(0, 0, 0));
            }
        }
        growthNeeded = false;
        growing = false;
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


    //protected void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.isTrigger && collision.collider.gameObject.tag == "Fruit")
    //    {
    //        growthNeeded = true;
    //        Destroy(collision.collider.gameObject);
    //    }
    //}

    /// <summary>
    /// Se encarga de mover cada parte del cuerpo de la serpiente segun pasa por los nodos y, si necesita cambiar la direccion,
    /// la cambia
    /// </summary>
    protected void Move()
    {
        int nodeX = 16 + Mathf.RoundToInt(headPart.parte.transform.position.x); // Para que no se cambie la direccion hasta haber alcanzado el nodo
        int nodeY = 9 - Mathf.RoundToInt(headPart.parte.transform.position.z);
        // Te mueves en la direccion que diga ese nodo si es diferente a tu dirección
        if (myNodos[nodeX, nodeY].direccion != new Vector3(0, 0, 0) && myNodos[nodeX, nodeY].direccion != headPart.direccion)
        {

            headPart.parte.transform.Rotate(Vector3.up, myNodos[nodeX, nodeY].rotationNeeded);

            headPart.direccion = myNodos[nodeX, nodeY].direccion;
            myDirection = headPart.direccion;
            // Movimiento discreto mejor, por nodos, no continuo con delta.
            headPart.parte.transform.position = myNodos[nodeX + Mathf.RoundToInt(headPart.direccion.x), nodeY - Mathf.RoundToInt(headPart.direccion.z)].centro;
        }
        else
        {
            headPart.parte.transform.position = myNodos[nodeX + Mathf.RoundToInt(headPart.direccion.x), nodeY - Mathf.RoundToInt(headPart.direccion.z)].centro;
        }
        nodeX = 16 + Mathf.RoundToInt(headPart.parte.transform.position.x); // Para que no se cambie la direccion hasta haber alcanzado el nodo
        nodeY = 9 - Mathf.RoundToInt(headPart.parte.transform.position.z);
        Debug.Log(nodeX + ", " + nodeY);
        for (int i = 0; i < bodyParts.Count; i++)
        {
            nodeX = 16 + Mathf.RoundToInt(bodyParts[i].parte.transform.position.x);
            nodeY = 9 - Mathf.RoundToInt(bodyParts[i].parte.transform.position.z);
            BodyPart myPart = bodyParts[i];

            if (myNodos[nodeX, nodeY].direccion != new Vector3(0, 0, 0) && myNodos[nodeX, nodeY].direccion != myPart.direccion)
            {

                myPart.parte.transform.Rotate(Vector3.up, myNodos[nodeX, nodeY].rotationNeeded);


                myPart.direccion = myNodos[nodeX, nodeY].direccion; // Se guarda la direccion a seguir

                myPart.parte.transform.position = myNodos[nodeX + Mathf.RoundToInt(myPart.direccion.x), nodeY - Mathf.RoundToInt(myPart.direccion.z)].centro; // Puede que el cuerpo este tomando un giro
            }
            else
            {
                myPart.parte.transform.position = myNodos[nodeX + Mathf.RoundToInt(myPart.direccion.x), nodeY - Mathf.RoundToInt(myPart.direccion.z)].centro;
            }
            bodyParts[i] = myPart;
        }
        nodeX = 16 + Mathf.RoundToInt(tailPart.parte.transform.position.x);
        nodeY = 9 - Mathf.RoundToInt(tailPart.parte.transform.position.z);
        // Para que nop gire sin parar en el mismo nodo
        if (!growthNeeded)
        {
            if (myNodos[nodeX, nodeY].direccion != new Vector3(0, 0, 0) && myNodos[nodeX, nodeY].direccion != tailPart.direccion)
            {
                // Falta rotar las cosas

                tailPart.direccion = myNodos[nodeX, nodeY].direccion; // La cola tiene la direccion
                tailPart.parte.transform.Rotate(Vector3.up, myNodos[nodeX, nodeY].rotationNeeded);

                tailPart.parte.transform.position = myNodos[nodeX + Mathf.RoundToInt(tailPart.direccion.x), nodeY - Mathf.RoundToInt(tailPart.direccion.z)].centro;
                
                myNodos[nodeX, nodeY].direccion = new Vector3(0, 0, 0); // Si pasa la cola, se reinicia el nodo para otro giro
                myNodos[nodeX, nodeY].rotationNeeded = 0;
            }
            else
            {
                tailPart.parte.transform.position = myNodos[nodeX + Mathf.RoundToInt(tailPart.direccion.x), nodeY - Mathf.RoundToInt(tailPart.direccion.z)].centro;
            }
        }
        else growSomething();

    }

    protected void growSomething()
    {
        int nodeX = 16 + (int)tailPart.parte.transform.position.x;
        int nodeY = 9 + (int)tailPart.parte.transform.position.z;
        tailPart.parte.transform.position = myNodos[nodeX, nodeY].centro; //Para que la cola no se descoloque.
        if (!growing)
        {
            int headNodeX = 16 + (int)headPart.parte.transform.position.x;
            int headNodeY = 9 + (int)headPart.parte.transform.position.z;
            growing = true;
            headNode = new Vector2(headNodeX, headNodeY);
        }
        else
        {
            int headNodeX = 16 + (int)headPart.parte.transform.position.x;
            int headNodeY = 9 + (int)headPart.parte.transform.position.z;
            Vector2 newHeadPosition = new Vector2(headNodeX, headNodeY);
            // Si la cabeza ha cambiado de nodo desde la ultima vez, es que debe haber alcanzado uno nuevo
            // por lo que hay espacio para el nuevo cuerpo.
            if (newHeadPosition != headNode)
            {
                int previusNodeX = 16 + (int)bodyParts[bodyParts.Count - 1].parte.transform.position.x -
                   Mathf.RoundToInt(bodyParts[bodyParts.Count - 1].direccion.x);
                int previusNodeY = 9 + (int)bodyParts[bodyParts.Count - 1].parte.transform.position.z -
                   Mathf.RoundToInt(bodyParts[bodyParts.Count - 1].direccion.z);
                GameObject newBodyPart = Instantiate(bodyPartObj, myNodos[previusNodeX, previusNodeY].centro, bodyParts[bodyParts.Count - 1].parte.transform.rotation);
                bodyParts.Add(new BodyPart(bodyParts[bodyParts.Count - 1].direccion, newBodyPart));
                growing = false;
                growthNeeded = false;
            }
        }

    }
}
