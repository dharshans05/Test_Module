using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

// Use a separate PlayerInput component for setting up input.
public class SimpleController_UsingPlayerInput : MonoBehaviour
{
    public float Move_Player_Speed;
    public float Rotate_Player_Speed;
    public float burstSpeed;
    

    private Vector2 m_Rotation;
    private Vector2 m_Look;
    private Vector2 m_Move;



    public static SimpleController_UsingPlayerInput SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject Projectile;
    public int amountToPool;
    public GameObject newProjectile;

    public Self_Destroy G;



    void Awake()
    {
        SharedInstance = this;
    }


    //Object Pooling
    void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(Projectile);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_Move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        m_Look = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                if (context.interaction is SlowTapInteraction)
                {
                    StartCoroutine(BurstFire((int)(context.duration * burstSpeed)));
                }
                else
                {
                    Fire();
                }
                break;
        }
    }

    public void Update()
    {
        // Update orientation first, then move. Otherwise move orientation will lag
        // behind by one frame.
        Look(m_Look);
        Move(m_Move);
    }

    private void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01)
            return;
        var Scaled_Move_Player_Speed = Move_Player_Speed * Time.deltaTime;
        // For simplicity's sake, we just keep movement in a single plane here. Rotate
        // direction according to world Y rotation of player.
        var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
        transform.position += move * Scaled_Move_Player_Speed;
    }

    private void Look(Vector2 rotate)
    {
        if (rotate.sqrMagnitude < 0.01)
            return;
        var Scaled_Rotate_Player_Speed = Rotate_Player_Speed * Time.deltaTime;
        m_Rotation.y += rotate.x * Scaled_Rotate_Player_Speed;
        m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * Scaled_Rotate_Player_Speed, -89, 89);
        transform.localEulerAngles = m_Rotation;
    }

    private IEnumerator BurstFire(int burstAmount)
    {
        for (var i = 0; i < burstAmount; ++i)
        {
            Fire();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Fire()
    {
        var transform = this.transform;


        newProjectile = SimpleController_UsingPlayerInput.SharedInstance.GetPooledObject();
        if (newProjectile != null)
        {
            newProjectile.SetActive(true);
            G = newProjectile.GetComponent<Self_Destroy>();
            G.enabled = true;
        }
 
        //var newProjectile = Instantiate(Projectile);
        newProjectile.transform.position = transform.position + transform.forward * 0.6f;
        newProjectile.transform.rotation = transform.rotation;
        const int size = 1;
        newProjectile.transform.localScale *= size;
        newProjectile.GetComponent<Rigidbody>().mass = Mathf.Pow(size, 3);
        newProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
        newProjectile.GetComponent<MeshRenderer>().material.color =
            new Color(Random.value, Random.value, Random.value, 1.0f);
      
    }
}
