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

   public Vector3[] saberOffset = { new Vector3(5f, -5, .2f), new Vector3(5f, -5, .2f)};


   void Start()
   {
      cm = CameraManager.Singleton;
      cam = CameraManager.Cam;

   }

   void Update()
   {
      camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, faceTrackingSensitivity * cm.FacePos, Time.deltaTime * faceTrackingLerpSpeed);
   
      mouse1delt = new(RawMouseInput.mice[0].deltaX, -RawMouseInput.mice[0].deltaY);
      mouse2delt = new(RawMouseInput.mice[1].deltaX, -RawMouseInput.mice[1].deltaY);

      saber1rot = new Vector2(Mathf.Clamp(saber1rot.x + (mouse1delt.x * mouseSensitivity.x), 91, 269), Mathf.Clamp(saber1rot.y - (mouse1delt.y * mouseSensitivity.y), -5, 175));
      saber2rot = new Vector2(Mathf.Clamp(saber2rot.x + (mouse2delt.x * mouseSensitivity.x), 91, 269), Mathf.Clamp(saber2rot.y - (mouse2delt.y * mouseSensitivity.y), -5, 175));
      saber1pos = new Vector2();
      saber2pos = new Vector2();

      RSaber.transform.localEulerAngles = new Vector3(-saber1rot.y, saber1rot.x, 0);
      RSaber.transform.localPosition = cam.ScreenToWorldPoint(new Vector3(saber1rot.x, saber1rot.y, -cam.transform.position.z)) + saberOffset[0];

      LSaber.transform.localEulerAngles = new Vector3(-saber2rot.y, saber2rot.x, 0);
      LSaber.transform.localPosition = cam.ScreenToWorldPoint(new Vector3(saber2rot.x, saber2rot.y, -cam.transform.position.z)) + saberOffset[1];
   }


}