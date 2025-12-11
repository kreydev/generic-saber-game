using UnityEngine;
using UnityEngine.Timeline;
using System;

[Serializable] public class BlockFrame : SignalEmitter
{
    public BlockFrame(Block[] _blocks, float _bpm)
    {
        blocks = _blocks;
        bpm = _bpm;
        Debug.Log($"Spawning new BlockFrame with BPM {bpm} and blocks {blocks}.");
    }

    public Block[] blocks;
    public float bpm;
}