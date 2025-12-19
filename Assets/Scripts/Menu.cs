using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
   [SerializeField] TriggerBlock startTrigger;
   [SerializeField] GameObject readyMsg;
   [SerializeField] GameObject notReadyMsg;

   [SerializeField] bool isEnd;
   [SerializeField] TMP_Text scoretxt;
   [SerializeField] TMP_Text combotxt;


   void Start()
   {
      if (isEnd)
      {
         startTrigger.action = () => { SceneManager.LoadScene(0); };
         GameManager gm = GameManager.Singleton;
         scoretxt.text = $"{gm.score} points";
         combotxt.text = $"{gm.maxCombo} / {gm.totalNotes} notes";
      }
      else startTrigger.action = () => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); };
   }

   void Update()
   {
      if (Input.GetKeyDown(KeyCode.S))
      {
         print("Swapping Sabers!");
         RawMouseInput.swapped = !RawMouseInput.swapped;
      }
   }

   void FixedUpdate()
   {
      startTrigger.gameObject.SetActive(RawMouseInput.mice.Count == 2);
      readyMsg.SetActive(RawMouseInput.mice.Count == 2);
      notReadyMsg.SetActive(RawMouseInput.mice.Count != 2);
   }
}
