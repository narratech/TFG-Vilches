using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class myCaseSerializer : CaseSerializer
{
    public override string serializeVariable(dynamic var)
    {
        try
        {
            if (var.GetType() == typeof(Direction))
            {
                if (var == Direction.LEFT) return "left";
                else if (var == Direction.RIGHT) return "right";
                else if (var == Direction.UP) return "up";
                else if (var == Direction.DOWN) return "down";
                else return "null";
            }
            else if (var.GetType() == typeof(List<Direction>))
            {
                string fullString = "";
                foreach (Direction dir in var)
                {
                    if (dir == Direction.LEFT) fullString += "left";
                    else if (dir == Direction.RIGHT) fullString += "right";
                    else if (dir == Direction.UP) fullString += "up";
                    else if (dir == Direction.DOWN) fullString += "down";
                    else fullString += "null";
                    fullString += " ";
                }
                return fullString;
            }
            else
            {
                return base.serializeVariable((object)var);
            }
        }
        catch
        {
            return "Error";
        }
    }
    public override dynamic unserializeVariable(string var, string type)
    {
        if (type == "direction")
        {
            if (var == "left") return Direction.LEFT;
            else if (var == "right") return Direction.RIGHT;
            else if (var == "up") return Direction.UP;
            else if (var == "down") return Direction.DOWN;
            else return Direction.NULL;
        }
        else if(type == "directionList")
        {
            string[] dirs = var.Split(' ');
            List<Direction> list = new List<Direction>();
            for (int i=0;i<dirs.Length-1;i++)
            {
                if (dirs[i] == "left") list.Add(Direction.LEFT);
                else if (dirs[i] == "right") list.Add(Direction.RIGHT);
                else if (dirs[i] == "up") list.Add(Direction.UP);
                else if (dirs[i] == "down") list.Add(Direction.DOWN);
                else list.Add(Direction.NULL);
            }
            return list;
        }
        else return base.unserializeVariable(var, type);
    }
}
