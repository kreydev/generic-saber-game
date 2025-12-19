using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   [SerializeField] TriggerBlock startTrigger;
   [SerializeField] GameObject readyMsg;
   [SerializeField] GameObject notReadyMsg;

   void Start()
   {
      startTrigger.action = () => { SceneManager.LoadScene("Game"); };
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
