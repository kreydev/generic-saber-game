using UnityEngine;
using UnityEngine.Timeline;
using System;
using kreydev;

[Serializable] public class BlockFrame : SignalEmitter
{
    [SerializeField] public BlockDir[] dirs;
}