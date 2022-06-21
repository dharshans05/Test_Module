using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Self_Destroy : MonoBehaviour
{

    private Self_Destroy call;
    private GameObject Personel;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnEnable()
    {
        Personel = gameObject;
        call = Personel.GetComponent<Self_Destroy>();
        Invoke("Destroyed", 5);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Destroyed()
    {
        call.enabled = false;
        Personel.SetActive(false);
    }
}
