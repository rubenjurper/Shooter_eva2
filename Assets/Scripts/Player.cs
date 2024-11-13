using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float velocity;
    Transform shootPoint;
    bool canShoot = true;
    public int health;

    Animator anim;
    Rigidbody rig;
    LineRenderer line;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
        shootPoint = transform.GetChild(3);
        line = shootPoint.GetComponent<LineRenderer>();
        if (!GetComponent<PhotonView>().IsMine)
        {
            Destroy(transform.GetChild(2).gameObject);
        }
    }


    private void Update()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            Vector3 speed = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            speed = speed.normalized;

            rig.velocity = transform.forward * speed.z * velocity +
                transform.right * speed.x * velocity +
                transform.up * rig.velocity.y;

            //transform.Rotate(transform.up * Input.GetAxis("Mouse X"));

            transform.GetComponent<PhotonView>().RPC("Rotate", RpcTarget.All, Input.GetAxis("Mouse X"));

            anim.SetFloat("Velocity", rig.velocity.magnitude);

            if (canShoot && Input.GetButton("Fire1"))
            {
                StartCoroutine("Fire");
            }
        }
    }


    IEnumerator Fire()
    {
        canShoot = false;
        RaycastHit hit = new RaycastHit();
        line.enabled = true;
        line.SetPosition(0, shootPoint.position);
        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, 50))
        {
            line.SetPosition(1, hit.point);
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All);
            }
        }
        else
        {
            line.SetPosition(1, shootPoint.forward * 50);
        }
        yield return new WaitForSeconds(0.1f);
        line.enabled = false;
        yield return new WaitForSeconds(0.4f);
        canShoot = true;
    }

    [PunRPC]

    public void TakeDamage()
    {
        health--;
        if(health <= 0)
        {
            StopAllCoroutines();
            line.enabled=false;
            canShoot=false;
            rig.isKinematic = true;
            anim.SetTrigger("Dead");
            GetComponent<Collider>().enabled = false;
            this.enabled = false;
        }
    }
    [PunRPC]
    public void Rotate(float input)
    {
        transform.Rotate(transform.up * input);
    }
}
