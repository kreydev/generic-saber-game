using UnityEngine;
using UnityEngine.Timeline;
using System;

[Serializable] public class BlockFrame : SignalEmitter
{
    public BlockFrame(Block[] _blocks)
    {
        blocks = _blocks;
        Debug.Log($"Spawning new BlockFrame with blocks {blocks}.");
    }

    public Block[] blocks;
}