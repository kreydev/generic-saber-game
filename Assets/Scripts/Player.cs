using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
   CameraManager cm;
   Camera cam;
   public Transform camTransform;
   public Vector2 faceTrackingSensitivity;
   public float faceTrackingLerpSpeed;

   [SerializeField] Transform LSaber;
   [SerializeField] Transform RSaber;

   Vector2 saber1rot;
   Vector2 saber2rot;
   Vector2 saber1pos;
   Vector2 saber2pos;
   Vector2 mouse1delt;
   Vector2 mouse2delt;
   public Vector2 mouseSensitivity;
   public Vector2 movScalars;

   public Vector3[] saberOffset = { new Vector3(5f, -5, .2f), new Vector3(5f, -5, .2f)};


   void Start()
   {
      cm = CameraManager.Singleton;
      cam = CameraManager.Cam;

   }

   void Update()
   {
      if (RawMouseInput.mice.Count < 2) return;
      
      camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, faceTrackingSensitivity * cm.FacePos, Time.deltaTime * faceTrackingLerpSpeed);
   
      mouse1delt = new(RawMouseInput.mice[0].deltaX, -RawMouseInput.mice[0].deltaY);
      mouse2delt = new(RawMouseInput.mice[1].deltaX, -RawMouseInput.mice[1].deltaY);

      saber1rot = new Vector2(Mathf.Clamp(saber1rot.x + (mouse1delt.x * mouseSensitivity.x * Time.deltaTime), 91, 269), Mathf.Clamp(saber1rot.y - (mouse1delt.y * mouseSensitivity.y * Time.deltaTime), -5, 175));
      saber2rot = new Vector2(Mathf.Clamp(saber2rot.x + (mouse2delt.x * mouseSensitivity.x * Time.deltaTime), 91, 269), Mathf.Clamp(saber2rot.y - (mouse2delt.y * mouseSensitivity.y * Time.deltaTime), -5, 175));

      saber1pos = new Vector2(Mathf.Clamp(saber1pos.x + (mouse1delt.x * mouseSensitivity.x * movScalars.x * Time.deltaTime), -9f, 2f), Mathf.Clamp(saber1pos.y + (mouse1delt.y * mouseSensitivity.y * movScalars.y * Time.deltaTime), -3f, 3f));
      saber2pos = new Vector2(Mathf.Clamp(saber2pos.x + (mouse2delt.x * mouseSensitivity.x * movScalars.x * Time.deltaTime), -2f, 9f), Mathf.Clamp(saber2pos.y + (mouse2delt.y * mouseSensitivity.y * movScalars.y * Time.deltaTime), -3f, 3f));

      RSaber.transform.localEulerAngles = new Vector3(-saber1rot.y, saber1rot.x, 0);
      RSaber.transform.localPosition = saberOffset[0] + (Vector3)saber1pos;

      LSaber.transform.localEulerAngles = new Vector3(-saber2rot.y, saber2rot.x, 0);
      LSaber.transform.localPosition = saberOffset[1] + (Vector3)saber2pos;
   }
}