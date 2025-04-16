using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    LEFT,RIGHT, NODIRECTION
}
class myCaseSerializer: CaseSerializer
{
    public override string serializeVariable(dynamic var)
    {
        if (var.GetType() == typeof(Direction))
        {
            if (var == Direction.LEFT) return "left";
            else if (var == Direction.RIGHT) return "right";
            else return "noDirection";
        }
        else
        {
            return base.serializeVariable((object)var);
        }
    }
    public override dynamic unserializeVariable(string var, string type)
    {
        if (type == "direction")
        {
            if (var == "left") return Direction.LEFT;
            else if (var == "right") return Direction.RIGHT;
            else if (var == "noDirection") return Direction.NODIRECTION;
            else return null;
        }
        else return base.unserializeVariable(var, type);
    }
}
//Hacer tmb un case comparer que mire el score de la serpiente tras 5 nodos
class myComparer : CaseComparer
{
    public Tuple<CaseCBRv2, float> computeSimilarity(in CaseCBRv2 query, in CaseCBRv2 caseToLook, Dictionary<string, float> weigths)
    {
        float maxDistance = (Math.Abs(0 - 30) + Math.Abs(0 - 19));
        float similarity = 0;
        similarity += CaseUtility.computeV2ManhattanSimilarity(query.getProperty("position"), 
           caseToLook.getProperty("position"), maxDistance) * weigths["position"];
        similarity += CaseUtility.computeV2ManhattanSimilarity(query.getProperty("fruitPos"),
           caseToLook.getProperty("fruitPos"), maxDistance) * weigths["fruitPos"];
        similarity += CaseUtility.computeV3ManhattanSimilarity(query.getProperty("headDirection"),
           caseToLook.getProperty("headDirection"), maxDistance) * weigths["headDirection"];
        similarity += CaseUtility.computeV2ManhattanSimilarity(query.getProperty("otherSnakeDir"),
           caseToLook.getProperty("otherSnakeDir"), maxDistance) * weigths["otherSnakeDir"];
        similarity += CaseUtility.computeV2ListManhattanSimilarity(query.getProperty("myPartsNodes"),
           caseToLook.getProperty("myPartsNodes"), maxDistance) * weigths["myPartsNodes"];
        similarity += CaseUtility.computeV2ListManhattanSimilarity(query.getProperty("myPartsNodes"),
           caseToLook.getProperty("myPartsNodes"), maxDistance) * weigths["myPartsNodes"];
        similarity += CaseUtility.computeV2ListManhattanSimilarity(query.getProperty("otherSnakePartsNode"),
          caseToLook.getProperty("otherSnakePartsNode"), maxDistance) * weigths["otherSnakePartsNode"];

        return new Tuple<CaseCBRv2,float>(caseToLook, similarity);
    }
}

public class CBRContrSnake : SnakeControl
{
    #region private
    CBRBrain myBrain;
    myCaseSerializer caseSerializer;
    int reviseCounter;
    bool humanControl = true;
    Direction lastDirectionPicked = Direction.NODIRECTION;
    #endregion
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        caseSerializer = new myCaseSerializer();
        myBrain = new CBRBrain("Prueba1",caseSerializer,new myComparer(),0.95f);
        reviseCounter = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (GameManager.Instance.getKeepPlaying())
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.G)) myBrain.persistCases();
            if(Input.GetKeyDown(KeyCode.M)) humanControl = !humanControl;
            if(humanControl) HandleHumanInput();
            if (elapsedTime > 1 / speed)
            {
                Move();
                if (playerOne)
                {
                    GameManager.Instance.setPlayer1Positions(snakePositions);
                    GameManager.Instance.setPlayer1Dir(new Vector2(myDirection.x, myDirection.z));
                }
                else
                {
                    GameManager.Instance.setPlayer2Positions(snakePositions);
                    GameManager.Instance.setPlayer2Dir(new Vector2(myDirection.x,myDirection.z));
                }
                if(!humanControl)HandleInput();
                else myBrain.learnFromHuman(formACase(), lastDirectionPicked);
                elapsedTime = 0;
                if(!humanControl)reviseCounter++;
                lastDirectionPicked = Direction.NODIRECTION;
            }
            
        }
    }

    void HandleInput()
    {
        Direction myDir = myBrain.CBRCycle(formACase(), (System.Object[] args) => 
        {
            System.Random rnd = new System.Random();
            int xd = rnd.Next(0, 3);
            if(xd == 0) return Direction.RIGHT;
            else if (xd == 1) return Direction.LEFT;
            else return Direction.NODIRECTION;
            
        });
        if (myDir == Direction.LEFT)
        {
            turnLeft();
        }
        else if (myDir == Direction.RIGHT)
        {
            turnRigth();
        }
        if (reviseCounter >= 3) myBrain.setEvaluateNextCase(true);

    }
    void HandleHumanInput()
    {
        if ((playerOne && Input.GetKeyDown(KeyCode.A)) || (!playerOne && Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            turnLeft();
            lastDirectionPicked = Direction.LEFT;
        }
        else if ((playerOne && Input.GetKeyDown(KeyCode.D)) || (!playerOne && Input.GetKeyDown(KeyCode.RightArrow)))
        {
            turnRigth();
            lastDirectionPicked= Direction.RIGHT;
        }
    }

    CaseCBRv2 formACase()
    {
        CaseCBRv2 query = new CaseCBRv2();
        query.setAnswerType("direction");
        query.setProperty("position:vector2", headNode);
        myBrain.setWeigth("position", 0.1f);
        query.setProperty("headDirection:vector3",this.myDirection);
        myBrain.setWeigth("headDirection", 0.15f);
        if(playerOne)
        {
            List<Vector2>myList = new List<Vector2>(GameManager.Instance.getPlayer1Positions());
            query.setProperty("myPartsNodes:vector2List", myList);
            List<Vector2>myList2 = new List<Vector2>(GameManager.Instance.getPlayer2Positions());
            query.setProperty("otherSnakePartsNode:vector2List", myList2);
            query.setProperty("otherSnakeDir:vector2", GameManager.Instance.getPlayer2Dir());
        }
        else
        {
            List<Vector2> myList = new List<Vector2>(GameManager.Instance.getPlayer2Positions());
            query.setProperty("myPartsNodes:vector2List", myList);
            List<Vector2> myList2 = new List<Vector2>(GameManager.Instance.getPlayer1Positions());
            query.setProperty("otherSnakePartsNode:vector2List", myList2);
            query.setProperty("otherSnakeDir:vector2", GameManager.Instance.getPlayer1Dir());
        }
        
        myBrain.setWeigth("myPartsNodes", 0.25f);
        myBrain.setWeigth("otherSnakePartsNode", 0.25f);
        myBrain.setWeigth("otherSnakeDir", 0.15f);
        query.setProperty("fruitPos:vector2", GameManager.Instance.getFruitNode());
        myBrain.setWeigth("fruitPos", 0.1f);
        return query;

    }
}
