using GSpawn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pool : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Vector3 networkVelocity;
    private Vector3 networkAngularVelocity;

    // ���� �ӵ� ���� ����
    private float positionLerpSpeed = 10f;
    private float rotationLerpSpeed = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        networkPosition = rb.position;
        networkRotation = rb.rotation;
    }

    private void Update()
    {
            rb.position = Vector3.Lerp(rb.position, networkPosition, Time.deltaTime * positionLerpSpeed);
            rb.rotation = Quaternion.Lerp(rb.rotation, networkRotation, Time.deltaTime * rotationLerpSpeed);
            rb.velocity = networkVelocity;
            rb.angularVelocity = networkAngularVelocity;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // �����͸� �����մϴ�.
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
        }
        else
        {
            // �����͸� �����մϴ�.
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkVelocity = (Vector3)stream.ReceiveNext();
            networkAngularVelocity = (Vector3)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void SetActiveRPC(bool b)
    {
        gameObject.SetActive(b);
        Debug.Log("�۵�");
    }
}
