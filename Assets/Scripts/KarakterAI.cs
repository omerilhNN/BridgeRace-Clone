using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum Character
{
    zero =0,
    two =2
}
public class KarakterAI : MonoBehaviour
{
    public GameObject targetsParent;
    public List<GameObject> targets = new List<GameObject>();
    public float radius = 2f;
    public Transform collectablesMainObject;
    public GameObject prevObject;
    public List<GameObject> cubes = new List<GameObject>();
    public Transform[] ropes;

    public Character characterEnum;
    private Animator animator;
    private NavMeshAgent agent;
    private bool hasTarget = false;
    private Vector3 targetTransform;



  
    void Start()
    {
        for(int i = 0; i < targetsParent.transform.childCount; i++)
        {
            targets.Add(targetsParent.transform.GetChild(i).gameObject);
        }
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    
    void Update()
    {
        if(!hasTarget && targets.Count >0 )
        {
            ChooseTarget();
        }
    }
    void ChooseTarget()
    {
        int randomNumber = Random.Range(0, 3);

      if(randomNumber==0 && cubes.Count >=5)
        {
            int randomRope = Random.Range(0, ropes.Length);
            List<Transform> ropesNonActiveChild = new List<Transform>();

            foreach(Transform item in ropes[randomRope])
            {
                if (!item.GetComponent<MeshRenderer>().enabled || item.GetComponent<MeshRenderer>().enabled && item.gameObject.tag != "Diz" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1))
                {
                    ropesNonActiveChild.Add(item);
                }
            }
            targetTransform = cubes.Count > ropesNonActiveChild.Count ? ropesNonActiveChild[ropesNonActiveChild.Count - 1].position : ropesNonActiveChild[cubes.Count].position;

        }
      else
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            List<Vector3> ourColors = new List<Vector3>();

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].tag.StartsWith(transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1)))
                {
                    ourColors.Add(hitColliders[i].transform.position);
                }
            }
            if (ourColors.Count > 0)
            {
                targetTransform = ourColors[0];
            }
            else
            {
                int random = Random.Range(0, targets.Count);
                targetTransform = targets[random].transform.position;
            }
        }
        agent.SetDestination(targetTransform);
        if(!animator.GetBool("running"))
        {
            animator.SetBool("running", true);
        }
        hasTarget = true;   
    }
    private void OnTriggerEnter(Collider target)
    {
        if(target.gameObject.tag.StartsWith(transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1)))
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

            targets.Remove(target.gameObject);
            target.tag = "Untagged";
            hasTarget = false;

            GenerateCubes.instance.GenerateCube((int)characterEnum,this);
        }
        else if (target.gameObject.tag == "AlignR" 
            || target.gameObject.tag != "Align" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0,1) 
            && target.transform.tag.StartsWith("Align"))
        {
            if(cubes.Count > 1)
            {
                GameObject obj = cubes[cubes.Count - 1].gameObject;
                cubes.RemoveAt(cubes.Count - 1);
                Destroy(obj);

                target.GetComponent<MeshRenderer>().material = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
                target.GetComponent<MeshRenderer>().enabled = true;

                target.tag = "Align" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1);
            }
            else
            {
                prevObject = cubes[0].gameObject;
                hasTarget = false;

            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
        
    }
}
