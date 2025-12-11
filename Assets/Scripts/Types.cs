using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Timeline;
using UnityEngine;

[Serializable] public enum Hand
{
    Left, Right
}

[Serializable] public enum BlockDir
{
    TL, TT, TR,
    LL,     RR,
    BL, BB, BR
};

[Serializable] public struct Block
{
    public BlockDir dir;
    public Vector2 coord;
    public Hand hand;
    public override string ToString()
    {
        return $"[{coord.x}, {coord.y} {dir}]";
    }
}