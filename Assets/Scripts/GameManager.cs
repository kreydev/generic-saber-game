using UnityEngine;

public enum SFX {miss, bomb, slice}

public class GameManager : MonoBehaviour
{
   AudioClip[] missSFX;
   AudioClip[] bombSFX;
   AudioClip[] sliceSFX;


   void Start()
   {
      missSFX = Resources.LoadAll<AudioClip>("SFX/miss");
      bombSFX = Resources.LoadAll<AudioClip>("SFX/bomb");
      sliceSFX = Resources.LoadAll<AudioClip>("SFX/slice");
   }

   public void SpawnBlockFrame(Block[] _blocks)
   {
      Instantiate(new BlockFrame(_blocks, 120));
   }

   public void PlaySound(SFX sfx, Transform pos)
   {
      int index = Random.Range(0, 3);
      float pitch = Random.Range(-.2f, .2f);
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

