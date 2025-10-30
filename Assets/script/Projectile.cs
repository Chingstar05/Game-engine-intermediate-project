using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile2 : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wolf"))
        {
            Enemy en = other.GetComponent<Enemy>();
            if (en != null) en.TakeDamage(damage);


            Destroy(gameObject);
        }
        else if (other.CompareTag("Hunter"))
        {
            Enemy1 en = other.GetComponent<Enemy1>();
            if (en != null) en.TakeDamage(damage);
        }
    }
}
