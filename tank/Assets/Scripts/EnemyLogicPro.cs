using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLogicPro : MonoBehaviour
{
    [SerializeField] GameObject muzzleEffect;
    [SerializeField] Transform bulletsFolder, muzzleFlashFolder, effectsFolder;
    [SerializeField] GameObject tankObj, tankExplosion;

    Vector3 velocity;
    [SerializeField] Rigidbody myRigidBody;
    Transform myTransform;

    // fire mechanics
    bool rechargingBullet = false, launchingBullet = false;
    [SerializeField] Animator barrelAnim;
    GameObject tempBullet;
    [SerializeField] GameObject bulletObj;
    [SerializeField] Transform headTransform, bulletSpawnPos;

    [SerializeField] GameObject explosionSound;

    Animator myAnim;

    // Enemy Stats
    float moveSpeed;
    [Range(0, 1)]
    [SerializeField] float fireRate;
    [Range(0, 1)]
    [SerializeField] int bulletType;
    [Range(1, 5)]
    [SerializeField] int bulletAmount;
    [Range(0, 2)]
    [SerializeField] int ricochetAmount;
    [Range(0, 4)]
    [SerializeField] int turretSeeking;
    [Range(0, 4)]
    [SerializeField] int movementSeeking;


    [SerializeField] Transform movePositionTransform, visionCetnerObj, visionRaycastObj, fakeBarrelTransform, fakeHeadTransform;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] LayerMask rayMasks;

    Transform playerTarget;
    [SerializeField] bool isSpecial;

    public int liveBullets = 0;
    public bool bulletIncomming;

    bool hasBarrelSight, hasLineOfSight, turningBarrel, movingTank, resetPath, bulletDodgeSet, tankMoved;

    public Vector3 incombbingBulletRot;

    [SerializeField] AudioSource tankMoving;

    private void Awake()
    {
        // find player
        if (!isSpecial)
            playerTarget = GameObject.Find("Player_tank").transform;
        else
        {
            StartCoroutine(WaitToSet());
        }
        moveSpeed = navMeshAgent.speed;

        // find camrea
        myAnim = GameObject.Find("Camera_Shake").GetComponent<Animator>();

        rechargingBullet = false;
        launchingBullet = false;

        hasBarrelSight = false;
        hasLineOfSight = false;
        bulletIncomming = false;

        turningBarrel = false;

        movingTank = false;
        resetPath = false;

        bulletDodgeSet = false;
        tankMoved = false;

        incombbingBulletRot = Vector3.zero;

        movePositionTransform.position = new Vector3(transform.position.x, -5.17f, transform.position.z);
    }

    IEnumerator WaitToSet()
    {
        yield return new WaitForSeconds(0.5f);
        playerTarget = GameObject.Find("enemy_tank").transform;
    }

    private void OnEnable()
    {
        // start with random rotation
        headTransform.localEulerAngles = new Vector3(headTransform.localEulerAngles.x, Random.Range(0, 360), headTransform.localEulerAngles.z);

        GameManager.tanksAlive++;
    }

    private void Start()
    {
        myTransform = transform;
        liveBullets = 0;
    }

    private void Update()
    {
        if (GameManager.levelStarted && !GameManager.levelPassed && !GameManager.levelFailed)
        {
            // BARREL SIGHT
            BarrelSight();

            // LINE OF SIGHT
            LineOfSight();

            // TURRET HANDLING
            TurretHandling();

            // MOVEMENT HANDLING
            MovementHandling();
        }
        else if (!GameManager.levelStarted || transform.localPosition.y > -5.17f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, -5.17f, transform.localPosition.z);
        }

        if (!GameManager.levelPassed && !GameManager.levelFailed && GameManager.levelStarted)
        {
            tankMoving.volume = Mathf.Clamp(navMeshAgent.velocity.magnitude / 4.3f, 0, 1); // player moving sound
            tankMoving.pitch = 1 + Mathf.Clamp((Mathf.Clamp(navMeshAgent.velocity.magnitude / 2.6f, 0, 1.2f) - 1f) / 10, -0.1f, 0.02f); // player moving sound
        }
    }

    void BarrelSight()
    {
        Ray barrelRay = new Ray(bulletSpawnPos.position, bulletSpawnPos.forward);
        RaycastHit barrelRayHitInfo;
        string enemyTarget = "Tank";

        if (Physics.Raycast(barrelRay, out barrelRayHitInfo, 100, rayMasks)) // hit object
        {
            if (barrelRayHitInfo.collider.tag == enemyTarget && barrelRayHitInfo.collider.gameObject != gameObject) // hit player
            {
                hasBarrelSight = true;
                Debug.DrawLine(barrelRay.origin, barrelRayHitInfo.point, Color.blue);
            }
            else if (barrelRayHitInfo.collider.tag == "Wall") // hit obstacle
            {
                Debug.DrawLine(barrelRay.origin, barrelRayHitInfo.point, Color.red);
                // Ricochet Ray
                if (ricochetAmount >= 1)
                {
                    Vector3 reflectDir = Vector3.Reflect(barrelRay.direction, barrelRayHitInfo.normal);
                    Ray ricochetRay = new Ray(barrelRayHitInfo.point, reflectDir);
                    RaycastHit ricochetRayHitInfo;
                    if (Physics.Raycast(ricochetRay, out ricochetRayHitInfo, 100, rayMasks)) // hit object
                    {
                        if (ricochetRayHitInfo.collider.tag == enemyTarget && barrelRayHitInfo.collider.gameObject != gameObject) // hit player
                        {
                            hasBarrelSight = true;
                            Debug.DrawLine(ricochetRay.origin, ricochetRayHitInfo.point, Color.blue);
                        }
                        else if (ricochetRayHitInfo.collider.tag == "Wall") // hit obstacle
                        {
                            Debug.DrawLine(ricochetRay.origin, ricochetRayHitInfo.point, Color.red);
                            // Ricochet Ray
                            if (ricochetAmount >= 2)
                            {
                                Vector3 reflectDir2 = Vector3.Reflect(ricochetRay.direction, ricochetRayHitInfo.normal);
                                Ray ricochetRay2 = new Ray(ricochetRayHitInfo.point, reflectDir2);
                                RaycastHit ricochetRayHitInfo2;
                                if (Physics.Raycast(ricochetRay2, out ricochetRayHitInfo2, 100, rayMasks)) // hit object
                                {
                                    if (ricochetRayHitInfo2.collider.tag == enemyTarget && barrelRayHitInfo.collider.gameObject != gameObject) // hit player
                                    {
                                        hasBarrelSight = true;
                                        Debug.DrawLine(ricochetRay2.origin, ricochetRayHitInfo2.point, Color.blue);
                                    }
                                    else if (ricochetRayHitInfo.collider.tag == "Wall") // hit obstacle
                                    {
                                        Debug.DrawLine(ricochetRay2.origin, ricochetRayHitInfo2.point, Color.red);
                                        hasBarrelSight = false;
                                    }
                                    else // hit fellow enemy tank
                                        hasBarrelSight = false;
                                }
                                else // hit fellow enemy tank
                                    hasBarrelSight = false;
                            }
                            else // hit fellow enemy tank
                                hasBarrelSight = false;
                        }
                        else // hit fellow enemy tank
                            hasBarrelSight = false;
                    }
                    else // hit fellow enemy tank
                        hasBarrelSight = false;
                }
                else // hit fellow enemy tank
                    hasBarrelSight = false;
            }
            else // hit fellow enemy tank
                hasBarrelSight = false;
        }
        else // hit fellow enemy tank
            hasBarrelSight = false;
    }

    void LineOfSight()
    {
        Vector3 targetDirection = playerTarget.position - visionCetnerObj.position;
        Vector3 newDirection = Vector3.RotateTowards(visionCetnerObj.forward, targetDirection, 1, 0);
        visionCetnerObj.rotation = Quaternion.LookRotation(newDirection);
        visionCetnerObj.localEulerAngles = new Vector3(0, visionCetnerObj.localEulerAngles.y, 0);

        Ray sightRay = new Ray(visionRaycastObj.position, visionRaycastObj.forward);
        RaycastHit sightHitInfo;

        if (Physics.Raycast(sightRay, out sightHitInfo, 100, rayMasks)) // hit something
        {
            if (sightHitInfo.collider.tag == "Player") // hit player
            {
                hasLineOfSight = true;
                Debug.DrawLine(sightRay.origin, sightHitInfo.point, Color.yellow);
            }
            else // hit not player
            {
                hasLineOfSight = false;
                Debug.DrawLine(sightRay.origin, sightHitInfo.point, Color.green);
            }
        }
        else
            hasLineOfSight = false;
    }

    void TurretHandling()
    {
        switch (turretSeeking)
        {
            case 0: // NONE
                if (!turningBarrel && (!hasBarrelSight || launchingBullet))
                    StartCoroutine(RandomBarrelTurn()); // randomly turn barrel 
                break;
            case 1: // MILD
                if (!turningBarrel && (!hasBarrelSight || launchingBullet))
                    StartCoroutine(TurnTowardsPlayer(1.5f, 0.5f, 0.75f)); // turn barrel 
                break;
            case 2: // NORMAL
                if (!turningBarrel && (!hasBarrelSight || launchingBullet))
                    StartCoroutine(TurnTowardsPlayer(2f, 0.25f, 0.5f)); // turn barrel 
                break;
            case 3: // STRONG
                if (!turningBarrel && (!hasBarrelSight || launchingBullet))
                    StartCoroutine(TurnTowardsPlayer(2.5f, 0.125f, 0.25f)); // turn barrel 
                break;
            case 4: // IMPOSSIBLE
                if (!turningBarrel && (!hasBarrelSight || launchingBullet))
                    StartCoroutine(TurnTowardsPlayer(2.5f, 0.125f, 0.25f)); // turn barrel 
                break;
            default:
                Debug.Log("Error!");
                break;
        }
        if (hasBarrelSight) // barrel can see player
            PrepareBullet(); // prep barrel fire
    }

    IEnumerator RandomBarrelTurn()
    {
        turningBarrel = true;

        Vector3 startingRot = headTransform.localEulerAngles;

        float angleToAdd = (Random.Range(45f, 150f));
        if (Random.Range(0, 2) == 1)
            angleToAdd *= -1;
        float endingAngle = (startingRot.y + angleToAdd) % 360;

        Vector3 endingRot = new Vector3(startingRot.x, endingAngle, startingRot.z);
        float timer = 0, totalTime = Mathf.Abs(endingRot.y - startingRot.y) / 0.75f;

        // warmp up
        float waitTime = Random.Range(0f, 0.5f);
        if (!hasBarrelSight || launchingBullet)
            yield return new WaitForSecondsRealtime(waitTime);

        while (timer <= totalTime)
        {
            if (hasBarrelSight && !launchingBullet)
                break;

            headTransform.localEulerAngles = Vector3.Lerp(startingRot, endingRot, timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }

        // cool down
        waitTime = Random.Range(0.25f, 1.25f);
        if (!hasBarrelSight || launchingBullet)
            yield return new WaitForSecondsRealtime(waitTime);

        turningBarrel = false;
    }

    IEnumerator TurnTowardsPlayer(float turnSpeed, float maxWaitTime, float maxCoolDownTime)
    {
        turningBarrel = true;

        Vector3 startingRot = headTransform.localEulerAngles;

        Vector3 targetDirection = playerTarget.position - headTransform.position;
        Vector3 newDirection = Vector3.RotateTowards(headTransform.forward, targetDirection, 1 * Mathf.Deg2Rad, 0);
        Quaternion newRotation = Quaternion.LookRotation(newDirection);
        Vector3 endRotation = new Vector3(270f, newRotation.eulerAngles.y, newRotation.eulerAngles.z);

        if (!hasLineOfSight) // find a viable angle
        {
            bool sightFound = false;
            // rotate fake barrel sightly until a viable angle is found
            int adder = 1;
            if (Random.Range(0, 2) == 0)
                adder = -1;
            for (int i = 0; i <= 360; i++)
            {
                Ray fakeBarrelRay = new Ray(fakeBarrelTransform.position, fakeBarrelTransform.forward);
                RaycastHit fakeBarrelRayHitInfo;
                if (Physics.Raycast(fakeBarrelRay, out fakeBarrelRayHitInfo, 100, rayMasks)) // hit object
                {
                    if (fakeBarrelRayHitInfo.collider.tag == "Player") // hit player
                    {
                        // set head angle to be fake barrel angle                        
                        Debug.DrawLine(fakeBarrelRay.origin, fakeBarrelRayHitInfo.point, Color.magenta);
                        sightFound = true;
                        break;
                    }
                    else if (fakeBarrelRayHitInfo.collider.tag == "Wall") // hit obstacle
                    {
                        Debug.DrawLine(fakeBarrelRay.origin, fakeBarrelRayHitInfo.point, Color.white);
                        // Ricochet Ray
                        if (ricochetAmount >= 1)
                        {
                            Vector3 reflectDir = Vector3.Reflect(fakeBarrelRay.direction, fakeBarrelRayHitInfo.normal);
                            Ray ricochetRay = new Ray(fakeBarrelRayHitInfo.point, reflectDir);
                            RaycastHit ricochetRayHitInfo;
                            if (Physics.Raycast(ricochetRay, out ricochetRayHitInfo, 100, rayMasks)) // hit object
                            {
                                if (ricochetRayHitInfo.collider.tag == "Player") // hit player
                                {
                                    // set head angle to be fake barrel angle                        
                                    Debug.DrawLine(ricochetRay.origin, ricochetRayHitInfo.point, Color.magenta);
                                    sightFound = true;
                                    break;
                                }
                                else if (ricochetRayHitInfo.collider.tag == "Wall") // hit obstacle
                                {
                                    Debug.DrawLine(ricochetRay.origin, ricochetRayHitInfo.point, Color.white);
                                    // Ricochet Ray
                                    if (ricochetAmount >= 2)
                                    {
                                        Vector3 reflectDir2 = Vector3.Reflect(ricochetRay.direction, ricochetRayHitInfo.normal);
                                        Ray ricochetRay2 = new Ray(ricochetRayHitInfo.point, reflectDir2);
                                        RaycastHit ricochetRayHitInfo2;
                                        if (Physics.Raycast(ricochetRay2, out ricochetRayHitInfo2, 100, rayMasks)) // hit object
                                        {
                                            if (ricochetRayHitInfo2.collider.tag == "Player") // hit player
                                            {
                                                // set head angle to be fake barrel angle                        
                                                Debug.DrawLine(ricochetRay2.origin, ricochetRayHitInfo2.point, Color.magenta);
                                                sightFound = true;
                                                break;
                                            }
                                            else if (ricochetRayHitInfo.collider.tag == "Wall") // hit obstacle
                                            {
                                                Debug.DrawLine(ricochetRay2.origin, ricochetRayHitInfo2.point, Color.white);
                                                hasBarrelSight = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // rotate fake head
                fakeHeadTransform.localEulerAngles = new Vector3(270, (fakeHeadTransform.localEulerAngles.y + adder) %360, (fakeHeadTransform.localEulerAngles.z));
                sightFound = false;
            }
            if (sightFound)
            {
                endRotation = fakeHeadTransform.localEulerAngles;
                sightFound = false;
            }
            else
            {
                // rotate randomly
                float angleToAdd = (Random.Range(45f, 150f));
                if (Random.Range(0, 2) == 1)
                    angleToAdd *= -1;
                float endingAngle = (startingRot.y + angleToAdd) % 360;

                endRotation = new Vector3(startingRot.x, endingAngle, startingRot.z);
            }
        }

        float timer = 0, totalTime = Mathf.Abs(endRotation.y - startingRot.y) / turnSpeed;

        // warmp up
        float waitTime = Random.Range(0f, maxWaitTime);
        if (!hasBarrelSight || launchingBullet)
            yield return new WaitForSecondsRealtime(waitTime);
        // turn logic
        while (timer < totalTime)
        {
            if (hasBarrelSight && !launchingBullet) // breakout
                break;
            headTransform.localEulerAngles = Vector3.Lerp(startingRot, endRotation, timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
        // cool down
        waitTime = Random.Range(0f, maxCoolDownTime);
        if (!hasBarrelSight || launchingBullet)
        {
            headTransform.localEulerAngles = endRotation;
            yield return new WaitForSecondsRealtime(waitTime);
        }
        
        turningBarrel = false;
    }

    void MovementHandling()
    {
        switch (movementSeeking)
        {
            case 0: // NONE
                Wander();
                break;
            case 1: // MILD
                BigWander();
                break;
            case 2: // NORMAL
                MoveTowardsPlayer();
                break;
            case 3: // STRONG
                if (bulletIncomming || bulletDodgeSet)
                    RunFromBullet();
                else
                    MoveTowardsPlayer();
                bulletIncomming = false;
                break;
            case 4: // IMPOSSIBLE
                if (bulletIncomming || bulletDodgeSet)
                    RunFromBullet();
                else
                    MoveTowardsPlayer();
                bulletIncomming = false;
                break;
            default:
                Debug.Log("Error!");
                break;
        }
        // clamp tank height position
        transform.localPosition = new Vector3(transform.localPosition.x, -5.17f, transform.localPosition.z);
    }

    void Wander()
    {
        if (!movingTank)
        {
            float radius = 1.5f;
            if (isSpecial)
                radius = Random.Range(1.5f, 12f);
            Vector3 origin = new Vector3(transform.position.x, -5.17f, transform.position.z);

            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += origin;
            NavMeshHit meshHit;
            movePositionTransform.position = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out meshHit, radius, 1))
            {
                movePositionTransform.position = new Vector3(Mathf.Clamp(meshHit.position.x, -3.14f, 3.14f), -5.17f, Mathf.Clamp(meshHit.position.z, -10.2f, -1.76f));
                movingTank = true;
            }
        }
        else
        {
            navMeshAgent.destination = movePositionTransform.position; // move tank
            if (!navMeshAgent.pathPending)
            {
                if (!navMeshAgent.hasPath || (navMeshAgent.velocity.sqrMagnitude <= 0.4f && navMeshAgent.remainingDistance < 0.185f) || navMeshAgent.velocity.sqrMagnitude == 0.025f)
                {
                    movingTank = false;
                }
            }
        }        
    }

    void BigWander()
    {
        if (!hasLineOfSight)
        {
            if (!movingTank)
            {
                // (0, -6) : 9 width X 11 height
                float radius = 4.5f;
                Vector3 origin = new Vector3(0, -5.17f, -6f);

                Vector3 randomDirection = Random.insideUnitSphere * radius;
                randomDirection += origin;
                NavMeshHit meshHit;
                movePositionTransform.position = Vector3.zero;
                if (NavMesh.SamplePosition(randomDirection, out meshHit, radius, 1))
                {
                    movePositionTransform.position = new Vector3(Mathf.Clamp(meshHit.position.x, -3.14f, 3.14f), -5.17f, Mathf.Clamp(meshHit.position.z, -10.2f, -1.76f));
                    movingTank = true;
                }
            }
            else
            {
                navMeshAgent.destination = movePositionTransform.position; // move tank
                if (!navMeshAgent.pathPending)
                {
                    if (!navMeshAgent.hasPath || (navMeshAgent.velocity.sqrMagnitude <= 0.4f && navMeshAgent.remainingDistance < 0.185f) || navMeshAgent.velocity.sqrMagnitude == 0.025f)
                    {
                        movingTank = false;
                    }
                }
            }
            resetPath = false;
        }
        else
        {
            if (!resetPath)
            {
                movePositionTransform.position = new Vector3(transform.position.x, -5.17f, transform.position.z);
                navMeshAgent.destination = movePositionTransform.position;
                resetPath = true;
            }
            Wander();
        }
    }

    void MoveTowardsPlayer()
    {
        if (!isSpecial)
        {
            if (moveSpeed > 0 && !hasLineOfSight)
            {
                movePositionTransform.position = playerTarget.position;
                navMeshAgent.destination = movePositionTransform.position;
                resetPath = false;
            }
            else if (hasLineOfSight)
            {
                if (!resetPath)
                {
                    movePositionTransform.position = new Vector3(transform.position.x, -5.17f, transform.position.z);
                    navMeshAgent.destination = movePositionTransform.position;
                    resetPath = true;
                }
                Wander();
            }
        }
        else
            Wander();
    }

    void RunFromBullet()
    {
        if (!bulletDodgeSet) // set "safe" location
        {
            movePositionTransform.position = new Vector3(transform.position.x, -5.17f, transform.position.z);
            movePositionTransform.eulerAngles = incombbingBulletRot;
            float newAngle = 45f;
            if (Random.Range(0, 2) == 0)
            newAngle *= -1;
            movePositionTransform.localEulerAngles = new Vector3(movePositionTransform.localEulerAngles.x, movePositionTransform.localEulerAngles.y + newAngle, movePositionTransform.localEulerAngles.z); // rotate transform towards "safe" direction
            movePositionTransform.Translate(movePositionTransform.forward * 1.25f, Space.World);  // moves transform 1 unity in that direction
            movePositionTransform.position = new Vector3(Mathf.Clamp(movePositionTransform.position.x, -3.14f, 3.14f), -5.17f, Mathf.Clamp(movePositionTransform.position.z, -10.2f, -1.76f));
        }
        // move towards dodge location
        navMeshAgent.destination = movePositionTransform.position; // move tank

        if (bulletDodgeSet)
        {
            if (!navMeshAgent.pathPending)
            {
                if (!navMeshAgent.hasPath || (navMeshAgent.velocity.sqrMagnitude <= 0.4f && navMeshAgent.remainingDistance < 0.185f) || navMeshAgent.velocity.sqrMagnitude == 0.025f)
                {
                    movePositionTransform.position = new Vector3(transform.position.x, -5.17f, transform.position.z);
                    navMeshAgent.destination = movePositionTransform.position;
                    tankMoved = true;
                }
            }
        }
        if (!tankMoved)
            bulletDodgeSet = true;
        else
        {
            tankMoved = false;
            bulletDodgeSet = false;
        }
    }

    void PrepareBullet()
    {
        if (!rechargingBullet && liveBullets < bulletAmount)
            StartCoroutine(FireBullet());
    }

    IEnumerator FireBullet()
    {
        rechargingBullet = true;

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        launchingBullet = true;

        Instantiate(muzzleEffect, muzzleFlashFolder.transform.position, muzzleFlashFolder.transform.rotation, muzzleFlashFolder);

        barrelAnim.SetTrigger("Fire");
        myAnim.SetTrigger("lightShake");

        tempBullet = Instantiate(bulletObj, bulletSpawnPos.transform.position, bulletSpawnPos.transform.rotation, bulletsFolder);
        liveBullets++;
        if (bulletType == 1) // set bullet to rocket
            tempBullet.GetComponent<BulletLogic>().isRocket = true;

        tempBullet.GetComponent<BulletLogic>().myEnemy = transform.GetComponent<EnemyLogicPro>();
        tempBullet.GetComponent<BulletLogic>().ricochetAmount = ricochetAmount;
        tempBullet.GetComponent<BulletLogic>().tankOwner = 2; // is enemy tank
        tempBullet.transform.localEulerAngles = new Vector3(0, tempBullet.transform.localEulerAngles.y, 0);
        tempBullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
        if (bulletType == 0)
        {
            tempBullet.GetComponent<Rigidbody>().velocity = 5 * bulletSpawnPos.transform.forward;
        }
        else  // set bullet to rocket 
        {
            tempBullet.GetComponent<Rigidbody>().velocity = 8f * bulletSpawnPos.transform.forward;
        }

        yield return new WaitForSeconds(fireRate);

        launchingBullet = false;
        rechargingBullet = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet" && !GameManager.levelFailed && !GameManager.levelPassed)
        {
            GameObject tempObj = Instantiate(explosionSound, transform.position, Quaternion.identity, effectsFolder);
            tempObj.GetComponent<ExplosionSounds>().isTank = true;

            myAnim.SetTrigger("shake");
            GameManager.tanksAlive--;
            tankObj.SetActive(false);
        }
    }

    void OnDisable()
    {
        Instantiate(tankExplosion, transform.position, transform.rotation, effectsFolder);
    }
}
