using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Unity.VisualScripting;

public enum SFX {miss, bomb, slice}

public class GameManager : SignalReceiver, INotificationReceiver
{
   AudioClip[] missSFX;
   AudioClip[] bombSFX;
   AudioClip[] sliceSFX;
   public AudioMixer mixer;
   public float rotThresh;
   public float BPM {get; private set;} = 135;
   public string LevelName {get; private set;} = "0-SabersUp";
   public double[] Freqs {get; private set;}
   Chuck.FloatArrayCallback freqCB;
   List<GameObject> vis = new();
   List<GameObject> visClones = new();
   public GameObject cubee;
   public Transform visHolder;
   readonly static Queue<Action> executionQueue = new();
   public float latency;

   void Start()
   {
      Application.targetFrameRate = 1000;
      for (int i = 0; i < 16; ++i)
      {
         vis.Add(Instantiate(cubee, visHolder));
         Quaternion rot = UnityEngine.Random.rotation;
         vis[i].transform.localPosition = new Vector3(i, 0, 0);
         vis[i].transform.rotation = rot;

      }
      for (int i = 0; i < 16; ++i)
      {
         vis.Add(Instantiate(cubee, visHolder));
         Quaternion rot = UnityEngine.Random.rotation;
         vis[i+16].transform.localPosition = new Vector3(-i, 0, 0);
         vis[i+16].transform.rotation = rot;
      }

      missSFX = Resources.LoadAll<AudioClip>("SFX/miss");
      bombSFX = Resources.LoadAll<AudioClip>("SFX/bomb");
      sliceSFX = Resources.LoadAll<AudioClip>("SFX/slice");

      print($"Loading level {LevelName} at {BPM} bpm.");

      Chuck.Manager.Initialize(mixer, "LevelMusic");
      Chuck.Manager.RunFile("LevelMusic", "LevelMusic.ck");
      Chuck.Manager.SetString("LevelMusic", "level", Application.streamingAssetsPath + "/" + LevelName);
      Chuck.Manager.StartListeningForChuckEvent("LevelMusic", "LevelDone", () => {  });

      freqCB = (values, num) => { Freqs = values; };
   }

   public void Update()
   {
      Chuck.Manager.GetFloatArray("LevelMusic", "freqs", freqCB);
      for (int i = 0; i < 16; ++i)
      {
         try {
            vis[i].transform.localScale = new Vector3(1, (float)Freqs[i] * 250, 1);
            vis[16+i].transform.localScale = new Vector3(1, (float)Freqs[i] * 250, 1);
            if ((float)Freqs[i] * 250 > rotThresh)
            {
               Quaternion rot = UnityEngine.Random.rotation;
               vis[i].transform.rotation = rot;
               vis[16+i].transform.rotation = rot;
               vis[i].transform.localScale *= 2;
               vis[i+16].transform.localScale *= 2;
               visClones.Add(Instantiate(vis[i],visHolder));
               visClones.Add(Instantiate(vis[i+16],visHolder));

            }
         } catch {}
      }
      List<GameObject> tokill = new();
      foreach (var v in visClones)
      {
         v.transform.localScale = Vector3.Lerp( v.transform.localScale, Vector3.zero, Time.deltaTime * 2f ) ;
         v.GetComponent<Renderer>().material.color -= new Color(0, 0, 0, Time.deltaTime * 2f);
         if (v.transform.localScale.x < .5f) { tokill.Add(v); }
      }
      foreach (var v in tokill) { visClones.Remove(v);  Destroy(v);}

      lock (executionQueue)
      {
         while (executionQueue.Count > 0)
         {
            print("Dequeueing...");
            executionQueue.Dequeue().Invoke();
         }
      }

   }

   public new void OnNotify(Playable playable, INotification notification, object obj)
   {
      print(((BlockFrame)notification).blocks.ToCommaSeparatedString());
   }

   public static void Enqueue(Action action)
    {
      lock (executionQueue)
      {
         executionQueue.Enqueue(action);
      }
    }

   void OnApplicationQuit()
   {
      Chuck.Manager.Quit();
   }

   public void SpawnBlockFrame(Block[] _blocks)
   {
      Instantiate(new BlockFrame(_blocks));
   }

   public void PlaySound(SFX sfx, Transform pos)
   {
      int index = UnityEngine.Random.Range(0, 3);
      float pitch = UnityEngine.Random.Range(-.2f, .2f);
      AudioSource aud = Instantiate(new AudioSource(), pos);
      aud.pitch += pitch;
      switch (sfx)
      {
         case SFX.miss:
            aud.PlayOneShot(missSFX[index]);
            break;
         case SFX.bomb:
            aud.PlayOneShot(bombSFX[index]);
            break;
         case SFX.slice:
            aud.PlayOneShot(sliceSFX[index]);
            break;
         default: break;
      }
   }
}