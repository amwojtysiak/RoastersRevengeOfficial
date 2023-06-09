using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    public PlayerMove3 Move;

    public bool IsFacingRight;
    public bool IsShootingRight;

    public float fireRate = 0;
    public int Damage = 10;
    public LayerMask whatToHit;

    public Transform BulletTrailPrefab;
    public Transform HitPrefab;
    public Transform MuzzleFlashPrefab;
    float timeToSpawnEffect = 0;
    public float effectSpawnRate = 10;

    float timeToFire = 0;
    Transform firePoint;

    Vector2 right_vector; //Adam
    Vector2 left_vector; //Adam
    Vector2 shotVector; //Adam

    public AudioClip gunShotAudio1;

    // Start is called before the first frame update
    void Awake()
    {
        firePoint = transform.Find("FirePoint");
        if (firePoint == null)
        {
            Debug.LogError("No Firepoint");
        }

        right_vector = new Vector2(10000, 0); //Adam
        left_vector = new Vector2(-10000, 0); //Adam

        IsFacingRight = true;
        IsShootingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireRate == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButton("Fire1") && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1/fireRate;
                Shoot();
            }
        }
        
        if (IsShootingRight != IsFacingRight)
        {
            Turn();
        }
    }

    void Shoot ()
    {
        if (Move.IsFacingRight == true)
        {
            shotVector = right_vector;
        }
        else
        {
            shotVector = left_vector;
        }
        
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, shotVector, 100, whatToHit);
        
        
        //Debug.DrawLine(firePointPosition, (shotVector-firePointPosition)*100, Color.red);
        if (hit.collider != null)
        {
            //Debug.DrawLine(firePointPosition, hit.point, Color.white);
            Debug.Log("We hit " + hit.collider.name + " and did " + Damage + " damage.");
       
            SpriteRenderer spriteObj = hit.collider.gameObject.GetComponent<SpriteRenderer>();
            if (spriteObj != null)
            {
                //spriteObj.color = Color.black;
                hit.collider.gameObject.SendMessage("DamageObject", Damage);
            }
                
            
        }

        if (Time.time >= timeToSpawnEffect)
        {
            Vector3 hitPos;
            Vector3 hitNormal;

            if (hit.collider == null)
            {
                hitPos = (shotVector - firePointPosition) * 100;
                hitNormal = new Vector3(9999, 9999, 9999);
            }   
            else
            {
                hitPos = hit.point;
                hitNormal = hit.normal;
            }
               

            Effect(hitPos, hitNormal);
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
    }

    void Effect(Vector3 hitPos, Vector3 hitNormal)
    {
        Transform trail = Instantiate(BulletTrailPrefab, firePoint.position, firePoint.rotation) as Transform;
        LineRenderer lr = trail.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPos);
        }

        if (IsShootingRight == false)
        {
            trail.transform.Rotate(0, 180, 0);
        }

        Destroy(trail.gameObject, 0.1f);

        if (hitNormal != new Vector3(9999, 9999, 9999))
        {
            Instantiate(HitPrefab, hitPos, Quaternion.FromToRotation(Vector3.forward, hitNormal));
        }

        Transform flashClone = Instantiate(MuzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
        flashClone.parent = firePoint;
        float size = Random.Range(0.04f, 0.06f);
        flashClone.localScale = new Vector3(size, size, 0);

        AudioSource.PlayClipAtPoint(gunShotAudio1, new Vector2(firePoint.position.x, firePoint.position.y), 5f);

        Destroy(flashClone.gameObject, 0.03f);
    }

    void Turn()
    {
        Quaternion rotation = transform.localRotation;
        
        if (rotation.y > 0) 
        {
            rotation.y -= 180;
        }
        else
        {
            rotation.y += 180;
        }
        
        IsShootingRight = IsFacingRight;
    }
}
