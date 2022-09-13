using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class KarakterKontrol : MonoBehaviour
{
    private Camera cam;
    private Animator animator;

    public Transform collectablesMainObject;
    public GameObject prevObject;
    public List<GameObject> cubes = new List<GameObject>();
    public List<GameObject> targets = new List<GameObject>();
    public float turnSpeed, speed, lerpValue;
    public LayerMask layer;

    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetMouseButton(0))
        {
            Movement();
        }
        else
        {
            if(animator.GetBool("running"))
            {
                animator.SetBool("running", false);
            }
        }
    }
    private void Movement()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.transform.localPosition.z;

        Ray ray = cam.ScreenPointToRay(mousePos);

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit,Mathf.Infinity,layer))
        {
            //transform.DOMove(hit.point,lerpValue)

            Vector3 hitVec = hit.point;
            hitVec.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, Vector3.Lerp(transform.position, hitVec, lerpValue), Time.deltaTime * speed);
            Vector3 newMovePoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.rotation =Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(newMovePoint-transform.position), Time.deltaTime * turnSpeed);

            if(!animator.GetBool("running"))
            {
                animator.SetBool("running", true);
            }
        }
    }
    private void OnTriggerEnter(Collider target)
    {
        if (target.gameObject.tag.StartsWith(transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1)))
        {
            target.transform.SetParent(collectablesMainObject);
            Vector3 pos = prevObject.transform.localPosition;

            pos.y += 0.22f;
            pos.z = 0;
            pos.x = 0;

            target.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f);

            target.transform.DOLocalMove(pos, 0.2f);
            prevObject = target.gameObject;
            cubes.Add(target.gameObject);

            
            target.tag = "Untagged";

            //WE MADE IT BY HAND THERE IS NO NEED ENUM. THIS IS OUR CHARACTER
            GenerateCubes.instance.GenerateCube(1);
        }
        if(cubes.Count >1 && target.gameObject.tag == "AlignR" || cubes.Count>1 && target.gameObject.tag != "Align" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1) && target.gameObject.tag.StartsWith("Align"))
        {
            GameObject obj = cubes[cubes.Count-1].gameObject;
            cubes.RemoveAt(cubes.Count - 1);
            Destroy(obj);

            target.GetComponent<MeshRenderer>().material = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
            target.GetComponent<MeshRenderer>().enabled = true;

            target.tag = "Align" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1);

            prevObject = cubes[cubes.Count - 1];
        }
    }
}
