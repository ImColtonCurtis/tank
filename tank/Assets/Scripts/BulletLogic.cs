using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    int bounceCount;
    public int ricochetAmount = 0;

    public GameObject sparkEffectObj, steamObj, tinyExplosionObj, dustExplosionObj;

    GameObject collidedObject, thisBulletTrail, tempObj;

    Transform effectsFolder, myTransform;

    Vector3 lastBouncePos = new Vector3(420, 420, 420);

    public int tankOwner = 0; // if 1: owner is player, if 2: ownder is enemy (for bullet decrease count)

    public EnemyLogicPro myEnemy;
    public  ControlsLogic myplayer;
    public bool isRocket;
    bool steamInstantiated;

    Animator myAnim;

    [SerializeField] AudioSource[] pelletWal, rocketWall;
    [SerializeField] AudioSource pelletFired, rocketFired;
    [SerializeField] GameObject explosionSound;

    // Start is called before the first frame update
    void Awake()
    {
        myTransform = transform;

        // find camrea
        myAnim = GameObject.Find("Camera_Shake").GetComponent<Animator>();

        bounceCount = 0;
        steamInstantiated = false;
    }

    private void Update()
    {
        Ray bulletRay = new Ray(transform.position, transform.forward);
        RaycastHit bulletRayHitInfo;
        if (Physics.Raycast(bulletRay, out bulletRayHitInfo, 100)) // hit object
        {
            if (bulletRayHitInfo.collider.gameObject.tag == "Tank") // hit enemy tank
            {
                // tell enemy that a bullet is incomming
                bulletRayHitInfo.collider.gameObject.GetComponent<EnemyLogicPro>().bulletIncomming = true;
                bulletRayHitInfo.collider.gameObject.GetComponent<EnemyLogicPro>().incombbingBulletRot = transform.eulerAngles;
                Debug.DrawLine(bulletRay.origin, bulletRayHitInfo.point, Color.grey);
            }
        }
        if (isRocket && !steamInstantiated)
        {
            thisBulletTrail = Instantiate(steamObj, transform.position, transform.rotation, effectsFolder);
            steamInstantiated = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Wall")
        {
            myAnim.SetTrigger("lightShake");
            if (Vector3.Distance(lastBouncePos, transform.position) > 0.5f)
            {
                if (bounceCount >= ricochetAmount)
                {
                    Destroy(gameObject);
                }
                else // ricochet
                {
                    if (isRocket)
                    {
                        rocketWall[GameManager.bulletSound % 2].Play();  // rocket hit wall sound
                    }
                    else
                    {
                        pelletWal[GameManager.bulletSound % 2].Play();  // pellet hit wall sound
                    }
                    GameManager.bulletSound++;

                    tempObj = Instantiate(sparkEffectObj, myTransform.position, myTransform.rotation, effectsFolder);

                    Vector3 reflectDir = Vector3.Reflect(transform.forward, collision.GetContact(0).normal);
                    float rot = 90 - Mathf.Atan2(reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
                    myTransform.eulerAngles = new Vector3(0, rot, 0);

                    // velocity is 5 (maxspeed)
                    if (!isRocket)
                        myTransform.GetComponent<Rigidbody>().velocity = 5 * myTransform.forward;
                    else
                        myTransform.GetComponent<Rigidbody>().velocity = 8f * myTransform.forward;
                    bounceCount++;
                }
            }
            lastBouncePos = transform.position;
        }
        else if (collision.transform.tag == "Bullet" || collision.transform.tag == "Tank" || collision.transform.tag == "Player")
        {            
            Destroy(gameObject, Time.deltaTime);
        }
        collidedObject = collision.gameObject;
    }

    void LateUpdate()
    {
        if (thisBulletTrail != null)
            thisBulletTrail.transform.position = myTransform.position;
    }

    void OnEnable()
    {
        transform.GetComponent<SphereCollider>().enabled = false;
        StartCoroutine(EnableCollider());

        if (isRocket)
            rocketFired.Play();
        else
            pelletFired.Play();

        GameObject tempHolder;
        if (transform.parent.parent.gameObject != null)
            tempHolder = transform.parent.parent.gameObject;
        else
            tempHolder = null;

        if (tempHolder != null)
        {
            for (int i = 0; i < tempHolder.transform.childCount; i++)
            {
                if (transform.parent.parent.GetChild(i).tag == "EffectsFolder")
                    effectsFolder = transform.parent.parent.GetChild(i);
            }
        }
    }

    void OnDisable()
    {
        if (tankOwner == 1)
            myplayer.liveBullets--;
        else if (tankOwner == 2)
            myEnemy.liveBullets--;
        else
            Debug.Log("Error!");

        GameObject tempObj = Instantiate(explosionSound, transform.position, Quaternion.identity, effectsFolder);
        if (isRocket)
            tempObj.GetComponent<ExplosionSounds>().isRocket = true;

        if (collidedObject != null)
        {
            if (collidedObject.tag == "Bullet")
            {
                tempObj = Instantiate(tinyExplosionObj, transform.position, transform.rotation, effectsFolder);
            }
            else
            {
                tempObj = Instantiate(dustExplosionObj, transform.position, transform.rotation, effectsFolder);
            }
        }
        else
        {
            tempObj = Instantiate(dustExplosionObj, transform.position, transform.rotation, effectsFolder);
        }
    }

    IEnumerator EnableCollider()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        transform.GetComponent<SphereCollider>().enabled = true;
    }
}
