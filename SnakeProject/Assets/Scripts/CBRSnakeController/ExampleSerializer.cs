using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class myCaseSerializer : CaseSerializer
{
    public override string serializeVariable(dynamic var)
    {
        if (var.GetType() == typeof(Direction))
        {
            if (var == Direction.LEFT) return "left";
            else if (var == Direction.RIGHT) return "right";
            else if (var == Direction.UP) return "up";
            else if (var == Direction.DOWN) return "down";
            else return "error";
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
            else if (var == "up") return Direction.UP;
            else if (var == "down") return Direction.DOWN;
            else return null;
        }
        else return base.unserializeVariable(var, type);
    }
}
