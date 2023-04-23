using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsLogic : MonoBehaviour
{
    bool leftTouchDown, rightTouchDown;

    [SerializeField] GameObject muzzleEffect;
    [SerializeField] Transform bulletsFolder, muzzleFlashFolder, effectsFolder;
    [SerializeField] Animator camAnim;

    // Base movement
    Vector3 anchorPosL, targetPosL, anchorPosR, targetPosR;
    Vector2 moveInputL;
    float targetAngleL, anchorRadiusL = 0.0192f/2.6f; // set anchor radius // was: 0.0192f/2f
    float moveSpeed;
    [SerializeField] PlayerController controller;

    // Bullet firing
    float targetAngleR, anchorRadiusR = 0.0192f/3.1f; // set anchor radius // was:  0.0192f/2f
    [SerializeField] Transform headTransform, bulletSpawnPos;
    bool rechargingBullet = false;
    float fireRate;
    GameObject tempBullet;
    [SerializeField] GameObject bulletObj;
    [SerializeField] Animator barrelAnim;

    [SerializeField] Animator myAnim;

    public int liveBullets = 0;
    int bulletsAmount = 5;

    [SerializeField] GameObject noIcon;

    [SerializeField] Animator soundAnim;

    int leftTouchID = -1, rightTouchID = -1;

    int cheatCounter;
    void Awake()
    {
        leftTouchDown = false;
        rightTouchDown = false;

        liveBullets = 0;
        bulletsAmount = 5;

        moveSpeed = 3.4f; // PlayerPrefs.GetFloat("PlayerMovementSpeed", 4.9f); // was 2.7f
        fireRate = 0.2f; // PlayerPrefs.GetFloat("PlayerFireRate", 0.5f);

        cheatCounter = 0;

        if (PlayerPrefs.GetInt("SoundStatus", 1) == 1)
        {
            noIcon.SetActive(false);
            AudioListener.volume = 1;
        }
        else
        {
            noIcon.SetActive(true);
            AudioListener.volume = 0;
        }
        leftTouchID = -2;
        rightTouchID = -3;
    }

    void OnTouchDown(Vector4 point)
    {
        // cheat: top-right, top-right, top-left, bottom-right
        // top right tap
        if (!GameManager.levelStarted && (cheatCounter == 0 || cheatCounter == 1) && point.x >= 0.03f && point.y >= 8f)
        {
            cheatCounter++;
        }
        // top left tap
        else if (!GameManager.levelStarted && (cheatCounter == 2) && point.x <= -0.03f && point.y >= 8f)
        {
            cheatCounter++;
        }
        // bottom right tap
        else if (!GameManager.levelStarted && (cheatCounter == 3) && point.x >= 0.03f && point.y <= 7.92f)
        {
            cheatCounter = 0;
            if (!GameManager.cheatOn)
                GameManager.cheatOn = true;
            else
                GameManager.cheatOn = false;
        }

        else if (!GameManager.levelStarted && point.x <= -0.01f && point.y <= 7.92f) // bottom left button clicked
        {
            if (PlayerPrefs.GetInt("SoundStatus", 1) == 1)
            {
                PlayerPrefs.SetInt("SoundStatus", 0);
                noIcon.SetActive(true);
                AudioListener.volume = 0;
            }
            else
            {
                PlayerPrefs.SetInt("SoundStatus", 1);
                noIcon.SetActive(false);
                AudioListener.volume = 1;
            }
            soundAnim.SetTrigger("Blob");
        }
        else
        {
            if ((!leftTouchDown || !rightTouchDown) && !GameManager.levelPassed)
            {
                if (!GameManager.playerMoved && point.x >= 0)
                {
                    GameManager.playerMoved = true;
                }
                if (!GameManager.playerFired && point.x <= 0)
                {
                    GameManager.playerFired = true;
                }

                if (!GameManager.levelStarted)
                    GameManager.levelStarted = true;
                if (!GameManager.levelFailed && !GameManager.levelPassed)
                {
                    if (point.x <= 0 && !leftTouchDown)
                    {
                        anchorPosL = new Vector3(point.x, point.y, anchorPosL.z);
                        targetPosL = new Vector3(point.x, point.y, targetPosL.z);

                        leftTouchID = (int)point.w;

                        leftTouchDown = true;

                    }
                    else if (point.x > 0 && !rightTouchDown)
                    {
                        anchorPosR = new Vector3(point.x, point.y, anchorPosR.z);
                        targetPosR = new Vector3(point.x, point.y, targetPosR.z);

                        rightTouchID = (int)point.w;

                        rightTouchDown = true;
                    }
                }
            }
            if (GameManager.levelPassed && !GameManager.loadLevelTime)
            {
                GameManager.loadLevelTime = true;
            }
        }
    }

    // first tap, both actions start
    // second tap, everything is all fine

    void OnTouchStay(Vector4 point)
    {        
        if (!GameManager.levelFailed && !GameManager.levelPassed)
        {
            // MOVEMENT
            if (leftTouchDown && leftTouchID == point.w)
            {
                targetPosL = new Vector3(point.x, point.y, targetPosL.z);
                targetAngleL = Mathf.Rad2Deg * (Mathf.Atan2(targetPosL.y - anchorPosL.y, targetPosL.x - anchorPosL.x));
                if (targetAngleL < 0)
                    targetAngleL += 360;

                Vector3 moveDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * targetAngleL), Mathf.Sin(Mathf.Deg2Rad * targetAngleL), targetPosL.z);
                moveInputL = new Vector2(Mathf.Clamp(moveDirection.x, -1, 1) * Mathf.Clamp(Mathf.Abs(targetPosL.x - anchorPosL.x) / anchorRadiusL, 0, 1),
                    Mathf.Clamp(moveDirection.y, -1, 1) * Mathf.Clamp(Mathf.Abs(targetPosL.y - anchorPosL.y) / anchorRadiusL, 0, 1));

                Vector3 moveVelocity = moveInputL * moveSpeed;
                controller.Move(moveVelocity);
                controller.LookAt(targetAngleL);

                // if finger position is greater than (anchorRadius * 2), then reset anchor to be (anchorRadius)
                if (Vector3.Distance(anchorPosL, targetPosL) >= (anchorRadiusL * 2))
                    anchorPosL = Vector3.Lerp(anchorPosL, targetPosL, 0.5f);
            }

            // AIM
            if (rightTouchDown && rightTouchID == point.w)
            {
                targetPosR = new Vector3(point.x, point.y, targetPosR.z);
                targetAngleR = Mathf.Rad2Deg * (Mathf.Atan2(targetPosR.y - anchorPosR.y, targetPosR.x - anchorPosR.x));
                if (targetAngleR < 0)
                    targetAngleR += 360;
               
                headTransform.eulerAngles = new Vector3(-90, -targetAngleR + 90, 0);

                // fire bullets
                if (!rechargingBullet && liveBullets < bulletsAmount)
                    StartCoroutine(FireBullet());             

                // if finger position is greater than (anchorRadius * 2), then reset anchor to be (anchorRadius)
                if (Vector3.Distance(anchorPosR, targetPosR) >= (anchorRadiusR * 2))
                    anchorPosR = Vector3.Lerp(anchorPosR, targetPosR, 0.5f);
            }
        }
        else if (GameManager.levelPassed)
        {
            controller.Move(Vector3.zero);
        }
    }

    IEnumerator FireBullet()
    {
        rechargingBullet = true;
                           
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Instantiate(muzzleEffect, muzzleFlashFolder.transform.position, muzzleFlashFolder.transform.rotation, muzzleFlashFolder);

        barrelAnim.SetTrigger("Fire");
        myAnim.SetTrigger("lightShake");

        tempBullet = Instantiate(bulletObj, bulletSpawnPos.transform.position, bulletSpawnPos.transform.rotation, bulletsFolder);
        liveBullets++;
        tempBullet.GetComponent<BulletLogic>().myplayer = transform.GetComponent<ControlsLogic>();
        tempBullet.GetComponent<BulletLogic>().ricochetAmount = 1;
        tempBullet.GetComponent<BulletLogic>().tankOwner = 1;  // is player tank
        tempBullet.transform.localEulerAngles = new Vector3(0, tempBullet.transform.localEulerAngles.y, 0);
        tempBullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
        tempBullet.GetComponent<Rigidbody>().velocity = 5 * bulletSpawnPos.transform.forward;

        yield return new WaitForSeconds(fireRate);

        rechargingBullet = false;
    }

    // if left finger is moving, and right finger lifts up on left side, tank stops moving

    void OnTouchUp(Vector4 point)
    {
        if (GameManager.levelStarted && !GameManager.levelFailed && !GameManager.levelPassed)
        {
            if (leftTouchID == point.w)
            {
                moveInputL = Vector2.zero;
                Vector3 moveVelocity = moveInputL.normalized * moveSpeed;
                controller.Move(moveVelocity);

                leftTouchDown = false;
                leftTouchID = -1;
            }
            if (rightTouchID == point.w)
            {
                rightTouchDown = false;
                rightTouchID = -1;
            }
        }
    }

    void OnTouchExit(Vector4 point)
    {
        if (GameManager.levelStarted && !GameManager.levelFailed && !GameManager.levelPassed)
        {
            if (leftTouchID == point.w)
            {
                moveInputL = Vector2.zero;
                Vector3 moveVelocity = moveInputL.normalized * moveSpeed;
                controller.Move(moveVelocity);

                leftTouchDown = false;
                leftTouchID = -1;
            }
            if (rightTouchID == point.w)
            {
                rightTouchDown = false;
                rightTouchID = -1;
            }
        }
    }
}
