using UnityEngine;

public class Dont : MonoBehaviour
{
    private static Dont instance;
    void Awake()
    {
        if(instance == null){ 
            instance = this;
			DontDestroyOnLoad(instance);
		}else{ 
            Destroy(gameObject);
        }
       
    }

    
}
