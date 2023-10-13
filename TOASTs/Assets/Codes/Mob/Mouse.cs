using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : DefaultMob
{
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {


        anim.SetBool("HasTarget", true);
    }




}
