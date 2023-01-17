using UnityEngine;

public class LoreSystemScript : MonoBehaviour
{
    public Transform CamHolder;
    public GameObject ExitDoor;

    public bool HasCrossKey;
    public bool HasMysteryKey;
    public bool HasZodiacKey;

    public bool HasCrossTotem;
    public bool HasMysteryTotem;
    public bool HasZodiacTotem;

    public bool CrossPedistalActive;
    public bool MysteryPedistalActive;
    public bool ZodiacPedistalActive;


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit1;
            if (Physics.Raycast(CamHolder.position, CamHolder.transform.forward, out hit1, 10f))
            {
                switch (hit1.collider.tag)
                {
                    case "BigChungKey":
                        HasMysteryKey = true;
                        Destroy(hit1.collider.gameObject);
                        break;

                    case "NecroKey":
                        HasCrossKey = true;
                        Destroy(hit1.collider.gameObject);
                        break;

                    case "HeartKey":
                        HasZodiacKey = true;
                        Destroy(hit1.collider.gameObject);
                        break;

                    case "ZodiacTotem":
                        HasZodiacTotem = true;
                        Destroy(hit1.collider.gameObject);
                        break;

                    case "CrossTotem":
                        HasCrossTotem = true;
                        Destroy(hit1.collider.gameObject);
                        break;

                    case "MysteryTotem":
                        HasMysteryTotem = true;
                        Destroy(hit1.collider.transform.parent.gameObject);
                        break;

                    case "BigChungDoor":
                        if (HasMysteryKey)
                            OpenDoo(hit1.collider.gameObject);
                        break;

                    case "NecroDoor":
                        if (HasCrossKey)
                            OpenDoo(hit1.collider.gameObject);
                        break;

                    case "HeartDoor":
                        if (HasZodiacKey)
                            OpenDoo(hit1.collider.gameObject);
                        break;

                    case "ZodiacPedistal":
                        if (HasZodiacTotem)
                        {
                            ZodiacPedistalActive = true;
                            EnableTotem(hit1.collider.gameObject);
                        }                            
                        break;

                    case "MysteryPedistal":
                        if (HasMysteryTotem)
                        {
                            MysteryPedistalActive = true;
                            EnableTotem(hit1.collider.gameObject);
                        }
                        break;

                    case "CrossPedistal":
                        if (HasCrossTotem)
                        {
                            CrossPedistalActive = true;
                            EnableTotem(hit1.collider.gameObject);
                        }
                        break;
                            
                    default:
                        break;
                }
            }
        }      
        
        if (CrossPedistalActive && MysteryPedistalActive && ZodiacPedistalActive)
        {
            OpenDoo(ExitDoor);
        }
    }

    private void EnableTotem(GameObject Parent)
    {
        GameObject Totem = Parent.transform.Find("Totem").gameObject;
        Totem.SetActive(true);
    }
    private void OpenDoo(GameObject Door)
    {
        Transform LeftHinge = Door.transform.Find("Left Hinge");
        Transform RightHinge = Door.transform.Find("Right Hinge");
        GameObject Lock = Door.transform.Find("Lock").gameObject;

        RightHinge.Rotate(new Vector3(0, 90, 0));
        LeftHinge.Rotate(new Vector3(0, -90, 0));
        Destroy(Lock);
        Door.GetComponent<Collider>().enabled = false;
        
    }
    


}
    



    





