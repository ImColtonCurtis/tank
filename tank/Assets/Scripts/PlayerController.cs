using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Net.Sockets;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject tankObj, tankExplosion;
    [SerializeField] Transform effectsFolder;

    Vector3 velocity;
    [SerializeField] Rigidbody myRigidBody;
    Transform myTransform;
    [SerializeField] Animator myAnim;

    [SerializeField] AudioSource tankMoving;

    [SerializeField] GameObject explosionSound;

    private void Start()
    {
        myTransform = transform;
    }

    public void Move(Vector3 _velocity)
    {
        velocity = new Vector3(_velocity.x, velocity.y, _velocity.y);
    }

    public void LookAt(float _rotation)
    {
        myTransform.eulerAngles = new Vector3(0, -_rotation + 90, 0);
    }

    public void FixedUpdate()
    {
        var pos = myRigidBody.position + velocity * Time.fixedDeltaTime;
        

        if (!GameManager.levelFailed)
        {
            myRigidBody.MovePosition(pos);
        }
    }

    private void Update()
    {
        if (GameManager.levelStarted)
        {
            tankMoving.volume = Mathf.Clamp(velocity.magnitude / 4.3f, 0, 1); // player moving sound
            tankMoving.pitch = 1 + Mathf.Clamp((Mathf.Clamp(velocity.magnitude / 2.6f, 0, 1.2f)-1f)/10, -0.1f, 0.02f); // player moving sound
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet" && !GameManager.levelFailed && !GameManager.levelPassed)
        {
            GameObject tempObj = Instantiate(explosionSound, transform.position, Quaternion.identity, effectsFolder);
            tempObj.GetComponent<ExplosionSounds>().isTank = true;

            myAnim.SetTrigger("bigShake");
            // Lost
            tankObj.SetActive(false);
            GameManager.levelFailed = true;
        }
    }

    void OnDisable()
    {
        Instantiate(tankExplosion, transform.position, transform.rotation, effectsFolder);
    }
}
