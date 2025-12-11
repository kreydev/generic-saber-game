using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public enum SFX {miss, bomb, slice}

public class GameManager : MonoBehaviour
{
   AudioClip[] missSFX;
   AudioClip[] bombSFX;
   AudioClip[] sliceSFX;

   public AudioMixer mixer;

   public float BPM {get; private set;} = 135;
   public string LevelName {get; private set;} = "0-SabersUp";

   public double[] freqs;
   
   Chuck.FloatArrayCallback freqCB;

   public void OnGot(double[] values, ulong num)
   {
      freqs = values;
      Debug.Log("Received float array with " + num + " elements.");
      print(freqs.ToCommaSeparatedString());
   }

   void Start()
   {
      missSFX = Resources.LoadAll<AudioClip>("SFX/miss");
      bombSFX = Resources.LoadAll<AudioClip>("SFX/bomb");
      sliceSFX = Resources.LoadAll<AudioClip>("SFX/slice");

      print($"Loading level {LevelName} at {BPM} bpm.");

      Chuck.Manager.Initialize(mixer, "LevelMusic");
      Chuck.Manager.RunFile("LevelMusic", "LevelMusic.ck");
      Chuck.Manager.SetString("LevelMusic", "level", LevelName);
      Chuck.Manager.SignalEvent("LevelMusic", "LevelStarted");
   
      freqCB = new Chuck.FloatArrayCallback(OnGot);
   }

   void Update()
   {
      Chuck.Manager.GetFloatArray("LevelMusic", "freqs", freqCB);
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

