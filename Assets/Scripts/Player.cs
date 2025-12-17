using UnityEngine;

public class Player : MonoBehaviour
{
   CameraManager cm;
   public Transform camTransform;
   public Vector2 faceTrackingSensitivity;
   public float faceTrackingLerpSpeed;

   void Start()
   {
      cm = CameraManager.Singleton;
   }

   void Update()
   {
      camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, faceTrackingSensitivity * cm.FacePos, Time.deltaTime * faceTrackingLerpSpeed);
   }
}