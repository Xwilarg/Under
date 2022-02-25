using UnityEngine;

namespace VarVarGamejam.Map
{
    public record ObjPos
    {
        public ObjPos(Vector2Int pos, GameObject obj)
            => (Pos, Obj) = (pos, obj);

        public Vector2Int Pos;
        public GameObject Obj;
    }
}
