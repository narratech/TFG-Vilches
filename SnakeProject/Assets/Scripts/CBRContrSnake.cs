using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    LEFT,RIGHT
}
class myCaseSerializer: CaseSerializer
{
    public string serializeVariable(Direction direction)
    {
        if (direction == Direction.LEFT) return "left";
        else if (direction == Direction.RIGHT) return "right";
        else return "null";
    }
    public override dynamic unserializeVariable(string var, string type)
    {
        if (type == "diretion")
        {
            if (var == "left") return Direction.LEFT;
            else if (var == "right") return Direction.RIGHT;
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
    #endregion
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        caseSerializer = new myCaseSerializer();
        myBrain = new CBRBrain("Prueba1",new myComparer(),0.95f);
        myBrain.setCaseSerializer(caseSerializer);
        reviseCounter = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (keepPlaying)
        {
            base.Update();
            HandleInput();
            if (elapsedTime > 1 / speed)
            {
                Move();
                if (playerOne)
                {
                    GameManager.Instance.setPlayer1Positions(snakePositions);
                    GameManager.Instance.setPlayer1Dir(myDirection);
                }
                else
                {
                    GameManager.Instance.setPlayer2Positions(snakePositions);
                    GameManager.Instance.setPlayer2Dir(myDirection);
                }
                elapsedTime = 0;
            }
            reviseCounter++;
        }
    }

    void HandleInput()
    {
        Direction myDir = myBrain.CBRCycle(formACase(), (System.Object[] args) => 
        {
            System.Random rnd = new System.Random();
            int xd = rnd.Next(0, 2);
            if(xd == 0) return Direction.RIGHT;
            else return Direction.LEFT;
            
        });
        if (myDir == Direction.LEFT)
        {
            turnLeft();
        }
        else if (myDir == Direction.RIGHT)
        {
            turnRigth();
        }
        else
        {

        }
        if (reviseCounter >= 5) myBrain.setEvaluateNextCase(true);

    }

    CaseCBRv2 formACase()
    {
        CaseCBRv2 query = new CaseCBRv2();
        query.setProperty("position:vector2", headNode);
        myBrain.setWeigth("position", 0.1f);
        query.setProperty("headDirection:vector3",this.myDirection);
        myBrain.setWeigth("headDirection", 0.15f);
        query.setProperty("myPartsNodes:vector2List", playerOne? GameManager.Instance.getPlayer1Positions() : 
            GameManager.Instance.getPlayer2Positions());
        myBrain.setWeigth("myPartsNodes", 0.25f);
        query.setProperty("otherSnakePartsNode:vector2List", !playerOne ? GameManager.Instance.getPlayer1Positions() :
            GameManager.Instance.getPlayer2Positions());
        myBrain.setWeigth("otherSnakePartsNode", 0.25f);
        query.setProperty("otherSnakeDir:vector2", !playerOne ? GameManager.Instance.getPlayer1Dir() :
           GameManager.Instance.getPlayer2Dir());
        myBrain.setWeigth("otherSnakeDir", 0.15f);
        query.setProperty("fruitPos:vector2", GameManager.Instance.getFruitNode());
        myBrain.setWeigth("fruitPos", 0.1f);
        return query;

    }
}
