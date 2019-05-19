using System.Collections;
using System.Collections.Generic;
using UnityEngine; 


public class Singleton<T> : MonoBehaviour where T: MonoBehaviour 
{
    static T m_instance; 

    public static T Instance 
    {
       
        get{
            if(m_instance == null)
            {
                m_instance = GameObject.FindObjectOfType<T>(); //to find an object with correct type of component.

                if(m_instance == null) //if we couldn't find correct gameobject - create new one.
                {
                    GameObject singleton = new GameObject(typeof(T).Name); //ex) scoremanager.Instance
                    m_instance = singleton.AddComponent<T>();
                }
            }
            return m_instance;
        }
         
         
    }
     
    public virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this as T; // cast as specific component T
            transform.parent = null; //You can move your object to the root level simply by:
//          DontDestroyOnLoad(this.gameObject); // i.e. play music when transitioning into new scene. 
        }
        else //if another object exists as T,
        {
            Destroy(gameObject); //there can be only one in the scene. only one instance at a given time.
        }
        
    }
}
