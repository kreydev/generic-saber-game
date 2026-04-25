using System;
using System.Collections;
using UnityEngine;

public class TriggerBlock : MonoBehaviour
{
    public Action action;

    IEnumerator runaction()
    {
        print("running callback");
        yield return new WaitForSeconds(3);
        action();
    }

    void OnCollisionEnter(Collision other)
    {
        foreach (var c in GetComponentsInChildren<Collider>())
        {
            if (c.gameObject.GetComponent<Rigidbody>() == null)
                c.gameObject.AddComponent<Rigidbody>().AddExplosionForce(50, other.GetContact(0).point, 1, .1f, ForceMode.Impulse );
            else
                c.gameObject.GetComponent<Rigidbody>().AddExplosionForce(50, other.GetContact(0).point, 1, .1f, ForceMode.Impulse );
        }
        Destroy(GetComponent<Rigidbody>());
        if (other.gameObject.CompareTag("Lsaber") || other.gameObject.CompareTag("Rsaber"))
        {
            StartCoroutine(runaction());
        }
    }
}
