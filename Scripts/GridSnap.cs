using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Script that handles moving and placing furniture along a grid
public class GridSnap : MonoBehaviour
{
    public GameObject target;
    public GameObject structure;
    Vector3 truePosition;
    public float gridSize, xoffset, yoffset, zoffset;
    public Bounds bounds;
    public Vector3 up = new Vector3(0,0,0), down = new Vector3(0,0,0), left = new Vector3(0,0,0), right = new Vector3(0,0,0), screenTouch, offset;
    public bool test;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        truePosition.x = Mathf.Floor(target.transform.position.x/gridSize)*gridSize + xoffset;
        truePosition.y = Mathf.Floor(target.transform.position.y/gridSize)*gridSize + yoffset;
        truePosition.z = Mathf.Floor(target.transform.position.z/gridSize)*gridSize + zoffset;

        if(bounds != null){
 
            if(up != null && down != null && left != null && right != null){
                if(bounds.Contains(new Vector3(truePosition.x, bounds.center.y, truePosition.z))){
                    structure.transform.position = truePosition;
                    bool found = false;
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("furniture:floor_bed")){

                        if(structure.gameObject.GetComponent<BoxCollider>().bounds.Contains(item.transform.localPosition) && (structure != item)){
                            structure.GetComponent<FurnitureMove>().placeable = false;
                            found = true;
                            break;
                        }

                    }
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("furniture:wall")){
                        if(structure.gameObject.GetComponent<BoxCollider>().bounds.Contains(item.transform.localPosition) && (structure != item)){
                            structure.GetComponent<FurnitureMove>().placeable = false;
                            found = true;
                            break;
                        }
                    }
                    if(!found){
                        structure.GetComponent<FurnitureMove>().placeable = true;
                        structure.GetComponent<FurnitureMove>().placementLight.SetActive(false);
                    }
                    else if(found){
                        structure.GetComponent<FurnitureMove>().placementLight.SetActive(true);
                        structure.GetComponent<FurnitureMove>().placementRenderer.material.color = Color.red;
                    }
                }
            }
            
        }
        if(test)
            structure.transform.position = truePosition;
        
    }
}
