using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layout : MonoBehaviour
{
    public enum layoutType { horizontal,vertical}
    public enum layoutDirection {left,right,up,down}


    [Header("LayoutType:")]
    public layoutType _type;

   
    [Header("Direction:")]
    public layoutDirection _direction;  

    [Header("Reference Object:")]
    [SerializeField] GameObject refObject;


    [Header("list:")]
    [SerializeField] List<GameObject> objects = new List<GameObject>();

    private void Awake()
    {
        float zlength = refObject.transform.localScale.z;
        float xlength = refObject.transform.localScale.x;

        switch (_type) 
        {
             case layoutType.horizontal:

                    for (int i = 1;i<objects.Count;++i)
                    {
                        if(_direction == layoutDirection.right)
                          objects[i].transform.position = new Vector3(refObject.transform.position.x, refObject.transform.position.y,refObject.transform.position.z - i* zlength);
                        else
                          objects[i].transform.position = new Vector3(refObject.transform.position.x, refObject.transform.position.y, refObject.transform.position.z + i* zlength);
                    }

                    break;


                case layoutType.vertical:
                break;

        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
