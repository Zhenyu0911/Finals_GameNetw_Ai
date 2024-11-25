using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    [SerializeField] private bool isPersist;

    private static T instance;
    public static T Instance{
    get
        {
            // Check if there is no instance
            if (instance == null)
            {
                // Attempt to look for an existing gameobject with the generic type
                instance = (T)FindObjectOfType(typeof(T));
                // If there's no component of type T in the scene generate a new one
                if (instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = typeof(T).Name;
                    instance = (T)go.AddComponent(typeof(T));
                }
            }
            return instance;
        }
   }

   protected virtual void Awake(){
        //If there is no instance, set self as the instance
        if(instance == null){
            instance = this as T;
        }
        //If there is already an existing instance
        if(instance != null){
            // check if self is not the actual instance
            // that means there is already an instance defined and we need to delete self
            if(instance != this as T){
                Destroy(this.gameObject);
            }
        }

        if(isPersist){
            DontDestroyOnLoad(this.gameObject);
        }
   }
}
