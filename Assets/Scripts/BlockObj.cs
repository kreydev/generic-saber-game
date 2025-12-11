using UnityEngine;

public class BlockObj : MonoBehaviour
{
   public BlockObj(Block block)
   {
      coord = block.coord;
      dir = block.dir;
      hand = block.hand;

      switch (dir)
      {
         case BlockDir.TL:
            transform.rotation = Quaternion.Euler(0, 0, -135);
            break;

         case BlockDir.TT:
            transform.rotation = Quaternion.Euler(0, 0, 180);
            break;

         case BlockDir.TR:
            transform.rotation = Quaternion.Euler(0, 0, 135);
            break;

         case BlockDir.LL:
            transform.rotation = Quaternion.Euler(0, 0, -90);
            break;

         case BlockDir.RR:
            transform.rotation = Quaternion.Euler(0, 0, 90);
            break;

         case BlockDir.BL:
            transform.rotation = Quaternion.Euler(0, 0, -45);
            break;

         case BlockDir.BB:
            transform.rotation = Quaternion.Euler(0, 0, 0);
            break;

         case BlockDir.BR:
            transform.rotation = Quaternion.Euler(0, 0, 45);
            break;
         
         default: break;
      }
   }

   public BlockDir dir;
   public Vector2 coord;
   public Hand hand;

}
