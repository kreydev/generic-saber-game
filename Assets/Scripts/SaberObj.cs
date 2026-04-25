using UnityEngine;

public enum HitState
{
    Pre, Good, Bad
}


public class SaberObj : MonoBehaviour
{
    GameManager gm;
    public HitState state;
    public bool saber;
    void Awake () {gm = GameManager.Singleton;}

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Wall")) return;

        BlockObj block = col.GetComponentInParent<BlockObj>();
        if (col.CompareTag("TopCol") && !block.triggered)
        {
            print("hit topcol");
            if ((block.type == ObjType.Left && saber) || (block.type == ObjType.Right && !saber))
            {
                gm.score += gm.combo;
                gm.combo += 1;
                if (gm.combo > gm.maxCombo) gm.maxCombo = gm.combo;
                print("good hit");
                state = HitState.Good;
                block.triggered = true;
            }
            else if ((block.type == ObjType.Right && saber) || (block.type == ObjType.Left && !saber))
            {
                gm.combo = 0;
                print("bad hit (mismatch)");
                state = HitState.Bad;
                block.triggered = true;
            }
        } else if (col.CompareTag("BottomCol") && !block.triggered)
        {
            gm.combo = 0;
            print("bad hit (bottomcol)");
            state = HitState.Bad;
            block.triggered = true;
        }
    }
}
