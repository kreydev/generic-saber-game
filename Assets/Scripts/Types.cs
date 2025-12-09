using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Timeline;
using UnityEngine;


namespace kreydev
{
    [Serializable] public enum BlockDir
    {
        XX,
        TL, TT, TR,
        LL, MM, RR,
        BL, BB, BR
    };

    [Serializable] public class BDRow
    {
        [SerializeField] public List<BlockDir> Row;
    }    
}