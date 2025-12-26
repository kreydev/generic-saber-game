using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;
using UnityEngine.SceneManagement;

public enum SFX {miss, bomb, slice}

public class GameManager : SignalReceiver, INotificationReceiver
{
   AudioClip[] missSFX;
   AudioClip[] bombSFX;
   AudioClip[] sliceSFX;
   public AudioMixer mixer;
   public AudioMixerSnapshot[] snaps = new AudioMixerSnapshot[2];
   public float rotThresh;
   public float BPM {get; private set;} = 135;
   public float scrollSpeed;
   public string LevelName {get; private set;} = "0-SabersUp";
   public double[] Freqs {get; private set;}
   Chuck.FloatArrayCallback freqCB;
   List<GameObject> vis = new();
   List<GameObject> visClones = new();
   public GameObject visBar;
   public Transform visualizer;
   public Transform blockHolder;
   public Transform editBlockHolder;
   readonly static Queue<Action> executionQueue = new();
   public float latency;
   public static GameManager Singleton {get { return FindFirstObjectByType<GameManager>(); }}
   PlayableDirector director;
   public bool hideEditMode;

   public int combo;
   public int score;
   public int maxCombo;
   public int totalNotes;
   public TMP_Text comboText;
   public TMP_Text scoreText;


      
   void Start()
   {
      Cursor.lockState = CursorLockMode.Confined;
      Cursor.visible = false;
      snaps[0] = mixer.FindSnapshot("Main");
      snaps[1] = mixer.FindSnapshot("InWall");
      Application.targetFrameRate = 1000;
      DontDestroyOnLoad(gameObject);

      editBlockHolder.gameObject.SetActive(!hideEditMode);
      foreach (var objj in editBlockHolder.GetComponentsInChildren<BlockObj>()) { DestroyImmediate(objj.gameObject); }

      for (int i = 0; i < 16; ++i)
      {
         vis.Add(Instantiate(visBar, visualizer));
         Quaternion rot = UnityEngine.Random.rotation;
         vis[i].transform.localPosition = new Vector3(i, 0, 0);
         vis[i].transform.rotation = rot;

      }
      for (int i = 0; i < 16; ++i)
      {
         vis.Add(Instantiate(visBar, visualizer));
         Quaternion rot = UnityEngine.Random.rotation;
         vis[i+16].transform.localPosition = new Vector3(-i, 0, 0);
         vis[i+16].transform.rotation = rot;
      }

      missSFX = Resources.LoadAll<AudioClip>("SFX/miss");
      bombSFX = Resources.LoadAll<AudioClip>("SFX/bomb");
      sliceSFX = Resources.LoadAll<AudioClip>("SFX/slice");

      print($"Loading level {LevelName} at {BPM} bpm.");

      freqCB = (values, num) => { Freqs = values; };

      Chuck.SetLogLevel(Chuck.LogLevel.Core);
      Chuck.Manager.Kill();
      Chuck.Manager.Initialize(mixer, "LevelMusic");
      Chuck.Manager.RunFile("LevelMusic", "LevelMusic.ck");
      Chuck.Manager.SetString("LevelMusic", "level", Application.streamingAssetsPath + "/" + LevelName);

      director = GetComponent<PlayableDirector>();
      director.Play();

      StartCoroutine(PlayChuck());
   }

   IEnumerator PlayChuck()
   {
      yield return new WaitForSeconds(latency);
      while (!Chuck.Manager.BroadcastEvent("LevelMusic", "LevelStart"))
      {
         yield return new WaitForFixedUpdate();         
      }
   }

   void FixedUpdate()
   {
      if (SceneManager.GetActiveScene().buildIndex != 1) Destroy(gameObject);
      
      scoreText.text = score.ToString();
      comboText.text = combo.ToString();

      if (director.state == PlayState.Paused) StartCoroutine(GotoEnd());
   }

   IEnumerator GotoEnd()
   {
      yield return new WaitForSeconds(latency + 2);
      SceneManager.LoadScene("2_EndScreen");
   }

   public void Update()
   {
      Chuck.Manager.GetFloatArray("LevelMusic", "freqs", freqCB);
      for (int i = 0; i < 16; ++i)
      {
         try {
            vis[i].transform.localScale = new Vector3(1, (float)Freqs[i] * 250 + 0.001f, 1);
            vis[16+i].transform.localScale = new Vector3(1, (float)Freqs[i] * 250 + 0.001f, 1);
            if ((float)Freqs[i] * 250 > rotThresh)
            {
               Quaternion rot = UnityEngine.Random.rotation;
               vis[i].transform.rotation = rot;
               vis[16+i].transform.rotation = rot;
               vis[i].transform.localScale *= 2;
               vis[i+16].transform.localScale *= 2;
               visClones.Add(Instantiate(vis[i],visualizer));
               visClones.Add(Instantiate(vis[i+16],visualizer));

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
      if (Application.isPlaying)
      {
         foreach (var block in ((BlockFrame)notification).blocks)
         {
            GameObject g = new();
            BlockObj b = g.AddComponent<BlockObj>();
            b.SetData(block, blockHolder);
            if (block.type == ObjType.Left || block.type == ObjType.Right) totalNotes++;
         }
      } else
      {
         foreach (var objj in editBlockHolder.GetComponentsInChildren<BlockObj>()) { DestroyImmediate(objj.gameObject); }
         foreach (var block in ((BlockFrame)notification).blocks)
         {
            GameObject g = new();
            BlockObj b = g.AddComponent<BlockObj>();
            b.SetData(block, editBlockHolder);
         }
      }
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

   public IEnumerator KillCB(GameObject obj)
   {
      yield return new WaitForSeconds(5);
      Destroy(obj.gameObject);
   }

   public void PlaySound(SFX sfx, Transform pos)
   {
      int index = UnityEngine.Random.Range(0, 3);
      float pitch = UnityEngine.Random.Range(-.1f, .1f);
      AudioSource aud = Instantiate(new GameObject(), (pos != null) ? pos : transform).AddComponent<AudioSource>();
      aud.outputAudioMixerGroup = mixer.FindMatchingGroups("sfx")[0];
      aud.pitch += pitch;
      aud.spatialize = true;
      aud.spatialBlend = .5f;
      aud.dopplerLevel = .3f;
      // print($"playing {sfx}{index} @ {pitch}");
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
      StartCoroutine(KillCB(aud.gameObject));
   }
}