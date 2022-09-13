using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCubes : MonoBehaviour
{
    public static GenerateCubes instance;
    public GameObject redCube, greenCube, blueCube;
    public Transform redCubeParent,greenCubeParent, blueCubeParent;
    public int minX,maxX,minZ,maxZ;
    public LayerMask layerMask;

    private void Awake()
    {
        if(instance == null) { instance = this; }
    }

    void Start()
    {
        
    }

    //0 red 1 blue 2 green
    public void GenerateCube(int number,KarakterAI karakterAI = null)
    {
        if(number == 0 )
        {
            Generate(redCube,redCubeParent,karakterAI);
        }
        if (number == 1)
        {
            Generate(blueCube, blueCubeParent);
        }
        if (number == 2)
        {
            Generate(greenCube, greenCubeParent, karakterAI);
        }
    }
    private void Generate(GameObject gameObject,Transform parent, KarakterAI karakterAI = null)
    {
        GameObject go = Instantiate(gameObject);
        go.transform.parent = parent;

        Vector3 desPos = GiveRandomPos();

        go.SetActive(false);

        Collider[] colliders = Physics.OverlapSphere(desPos, 1, layerMask);
        while(colliders.Length!=0)
        {
            Debug.Log("Çarptý: " + colliders[0].gameObject + " " + desPos);
            desPos =GiveRandomPos();
            colliders = Physics.OverlapSphere(desPos, 1, layerMask);
        }
        go.SetActive(true);
        go.transform.position = desPos;

        if(karakterAI != null)
        {
            karakterAI.targets.Add(go);
        }
    }
    private Vector3 GiveRandomPos()
    {
        return new Vector3(Random.Range(minX, maxX), redCube.transform.position.y, Random.Range(minZ,maxZ));
    }

}
