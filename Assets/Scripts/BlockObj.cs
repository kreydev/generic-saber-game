using System.Diagnostics;
using UnityEngine;

public class BlockObj : MonoBehaviour
{
   GameManager gm;
   float speed = 0;
   public void SetData(Block block, Transform parent)
   {
      gm = GameManager.Singleton;
      speed = gm.scrollSpeed * 60f / gm.BPM;
      transform.parent = parent;

      coord = block.coord;
      dir = block.dir;
      type = block.type;

      if (type == ObjType.Left || type == ObjType.Right)
      {
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

      switch (type)
      {
         case ObjType.Left:
            Instantiate(Resources.Load("Prefabs/CubeL"), transform);
            break;

         case ObjType.Right:
            Instantiate(Resources.Load("Prefabs/CubeR"), transform);
            break;

         case ObjType.Bomb:
            Instantiate(Resources.Load("Prefabs/Bomb"), transform);
            break;

         case ObjType.Wall:
            Instantiate(Resources.Load("Prefabs/Wall"), transform);
            break;

         default: break;
      }

      transform.localPosition = new Vector3(coord.x * 3, coord.y * -3);
   }

   public BlockDir dir;
   public Vector2 coord;
   public ObjType type;

   void Update()
   {
      transform.localPosition += Vector3.back * speed * Time.deltaTime;
   }
   public override string ToString()
   {
      return $"*[{( (type == ObjType.Left || type == ObjType.Right) ? $"{dir} " : "" )}{type} @ {coord.x}, {coord.y} (v={speed})]";
   }
}
