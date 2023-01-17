using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BotNavMesh : MonoBehaviour
{
    public BotInfo[] botInfo;
    public Renderer RenderingObject;
    private int SpriteNumber;

    private AudioSource normSource;
    private AudioSource rageSource;

    private bool CanBeSeenactingAplier;
    

    [SerializeField] private bool DEBUGIsActive; 
    private NavMeshAgent navmeshagent;
    [SerializeField] private Transform PlayerTransform;
    private float rageSpeed;
    [SerializeField] private bool Raging;
    [SerializeField] private float toyChaseSpeed;
    private Vector3 beforePos;
    [SerializeField] private float stalkingStopDistance;
    [SerializeField] private float normalSpeed;
    public float ScreechingVolume;
    [SerializeField] private bool IsHiding;
    [SerializeField] private float TimeInHiding;
    [SerializeField] private float JumpZone;
    [SerializeField] private float HidingTimeMax;
    [SerializeField] private bool IsChasing;
    [SerializeField] private float TimeInChase;
    [SerializeField] private float ChasingTimeMax;
    [SerializeField] private int chasesExcaped;

    Camera _camera;
    Plane[] cameraFrustrum;
    Collider _collider;
    public bool canBeSeenWithoutWalls;
    public bool canBeSeenWithWalls;


    [SerializeField] public bool ActiveHunting;
    public NavMeshPath navMeshPath;

    public GameObject randomPointMarker;

    ArrayList hidingSpots = new ArrayList();
    public Transform HidingSpotHolder;


    private void Awake()
    {
        

        RandomSprite();

        if (!DEBUGIsActive) return;

        _camera = Camera.main;
        
        _collider = GetComponentInChildren<Collider>();
        
        navmeshagent = GetComponent<NavMeshAgent>();

        

        navmeshagent.speed = normalSpeed;
        rageSpeed = normalSpeed * 2;

        navMeshPath = new NavMeshPath();

        StartCoroutine(GoToNearestHidingSpot(20f));

        
        
    }

    private void Update()
    {
        if (!DEBUGIsActive) return;

        

        OnSightReaction();

        float EveryTenRepeat = 0f;

        EveryTenRepeat += Time.deltaTime;

        if (EveryTenRepeat >=30F && IsHiding)
        {
            CanBeSeenactingAplier = true;

            EveryTenRepeat = 0f;
        }
        
        if (IsHiding)
        {
            TimeInHiding += Time.deltaTime;

            ActiveHunting = false;
            TimeInChase = 0f;

            if (Vector3.Distance(transform.position, PlayerTransform.position) <= JumpZone)
            {
                int i = Random.Range(1, 3);
                switch (i)
                {
                    case 1:
                        normalChase();
                        break;
                    case 2:
                        toyChase();
                        break;
                    

                    default:
                        print("it broky");
                        break;
                }

                    

                print("Disengaging hiding");


                TimeInHiding = 0f;
                IsHiding = false;
                
            }

            if (TimeInHiding >= HidingTimeMax)
            {
                int i = Random.Range(1, 4);
                switch (i)
                {
                    case 1:
                        normalChase();
                        break;
                    case 2:
                        toyChase();
                        break;
                    case 3:
                        Stalk();
                        break;
                    default:
                        print("it broky");
                        break;
                }
                TimeInHiding = 0f;
                IsHiding = false;
            }
                
        }

        if (IsChasing)
        {
            TimeInChase += Time.deltaTime;

            TimeInHiding = 0f;

            if (TimeInChase >= ChasingTimeMax)
            {                 

                StartCoroutine(GoToNearestHidingSpot(20f));

                
                chasesExcaped++;
                TimeInChase = 0f;
                IsChasing = false;

            }
        }

        var bounds = _collider.bounds;

        cameraFrustrum = GeometryUtility.CalculateFrustumPlanes(_camera);
        if (GeometryUtility.TestPlanesAABB(cameraFrustrum, bounds))
        {
            canBeSeenWithoutWalls = true;
        }
        else
        {
            canBeSeenWithoutWalls = false;
        }

        transform.LookAt(PlayerTransform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.forward,out hit, Mathf.Infinity))
        {
            if (hit.collider.tag=="Solid Object")
            {
                canBeSeenWithWalls = false;
            }
            else if (hit.collider.tag == "Player")
            {
                canBeSeenWithWalls = true;
            }
        }

        if (chasesExcaped >= 3)
        {
            enterRage();
            chasesExcaped = 0;
        }

            
        if (ActiveHunting)
        {
            navmeshagent.destination = PlayerTransform.position;
        }

        CheckForInputs();
        

        
    }

    
    private void enterRage()
    {
        IsChasing = true;
        IsHiding = false;
        Raging = true;
        print("Initiating rage");
        navmeshagent.speed = rageSpeed;
        ActiveHunting = true;
        navmeshagent.stoppingDistance = 0;

    }

    private void normalChase()
    {
        print("normal chase");
        IsChasing = true;
        Raging = false;
        navmeshagent.speed = normalSpeed;
        ActiveHunting = true;
        navmeshagent.stoppingDistance = 0;
    }

    private void toyChase()
    {
        print("Initiating toy chase");
        IsChasing = true;
        Raging = false;
        navmeshagent.speed = toyChaseSpeed;
        ActiveHunting = true;
       navmeshagent.stoppingDistance = 0;

    }

    IEnumerator mockCharge(float ChargeLength, bool efe)
    {
        print("mock charging");

        ActiveHunting = true;
        beforePos = transform.position;

        navmeshagent.speed = 24;       

        yield return new WaitForSeconds(ChargeLength);

        ActiveHunting = false;
        navmeshagent.destination = beforePos;        
        if (efe)
            StartCoroutine(GoToNearestHidingSpot(50f));
    }

    private void Stalk()
    {
        print("stalking");

        navmeshagent.stoppingDistance = stalkingStopDistance;
        navmeshagent.destination = PlayerTransform.position;
        ActiveHunting = true;
    }

    public void SetAmbush(float radius)
    {
        print("setting ambush");

        ActiveHunting = false;

        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }

        navmeshagent.CalculatePath(finalPosition, navMeshPath);

        if (navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            SetAmbush(radius);

        }
        else
        {

            GameObject GoPoint = Instantiate(randomPointMarker, finalPosition, Quaternion.identity);
            if (Vector3.Distance(transform.position, GoPoint.transform.position) < 10f)
            {

                SetAmbush(radius);
            }
            else
            {
                navmeshagent.SetDestination(GoPoint.transform.position);
            }
            Destroy(GoPoint);
        }



    }

    private Vector3 ReturnClosestHidingSpot(ArrayList Spots)
    {
        Vector3 ClosestSpot = Vector3.zero;
        float MinimuDistance = Mathf.Infinity;

        foreach (Transform T in Spots)
        {
            float dist = Vector3.Distance(T.position, transform.position);

            if (dist < MinimuDistance)
            {
                ClosestSpot = T.position;
                MinimuDistance = dist;
            }
        }

        return ClosestSpot;
    }

    private void LoopThroughHidingSpots(Transform SpotHolder, ArrayList SpotList)
    {
        Transform spot;
        for (int i = 0; i < SpotHolder.childCount; i++)
        {
            spot = SpotHolder.GetChild(i).transform;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(spot.position, out hit, 2f, 1))
            {
                SpotList.Add(spot);
                //PrintValues(SpotList);
            }
            else
            {
                Debug.LogError("Spot Unreachable", spot);
            }
        }
    }

    IEnumerator GoToNearestHidingSpot(float AmbushSize)
    {
        print("going to nearest hiding spot");
        ActiveHunting = false;
        IsHiding = true;
        navmeshagent.destination = ReturnClosestHidingSpot(hidingSpots);
        yield return new WaitForSeconds(10f);
        //ActiveHunting = true;
        SetAmbush(AmbushSize);
    }


    public static void PrintValues(IEnumerable aHuman)
    {
        //ignore. just prints contents of list for debugging
        foreach (var human in aHuman)
        {
            Debug.Log(human);
        }
    }

    private void OnValidate()
    {
        LoopThroughHidingSpots(HidingSpotHolder, hidingSpots);
    }

    private void CheckForInputs()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            enterRage();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            toyChase();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(mockCharge(0.3f,true));
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetAmbush(15f);
            ActiveHunting = false;

        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            print(ReturnClosestHidingSpot(hidingSpots));
            StartCoroutine(GoToNearestHidingSpot(15f));
        }

    }

    private void RandomSprite()
    {
        SpriteNumber = Random.Range(0, botInfo.Length);

        RenderingObject.sharedMaterial = botInfo[SpriteNumber].material;

        normSource = botInfo[SpriteNumber].EmitterContainer.transform.Find("NormEmitter").GetComponent<AudioSource>();
        rageSource = botInfo[SpriteNumber].EmitterContainer.transform.Find("RageEmitter").GetComponent<AudioSource>();

        botInfo[SpriteNumber].EmitterContainer.SetActive(true);

        
        Debug.Log(botInfo[SpriteNumber].BotName + " summoned");
    }

    private void OnSightReaction()
    {
        if (canBeSeenWithoutWalls && canBeSeenWithWalls && IsHiding &&CanBeSeenactingAplier)
        {
            
            int i = Random.Range(1, 4);
            switch (i)
            {
                case 1:
                    normalChase();
                    break;
                case 2:
                    toyChase();
                    break;
                case 3:
                    StartCoroutine(mockCharge(0.5f, false));

                    break;
                default:
                    print("it broky");
                    break;
            }

            IsHiding = false;
            canBeSeenWithWalls = false;
            canBeSeenWithWalls = false;
            CanBeSeenactingAplier = false;
        }
    }

    [System.Serializable]
    public struct BotInfo
    {
        public string BotName;
        public Material material;
        public GameObject EmitterContainer;
    }

    private void LateUpdate()
    {
        normSource.Pause();
        rageSource.Pause();

        if (!DEBUGIsActive) return;
        if (Vector3.Distance(transform.position, PlayerTransform.position)>15)
        {
            navmeshagent.speed = 18f; 
        }

        if (IsHiding && IsChasing)
            IsChasing = true; ActiveHunting = true;
        if (!IsHiding && !IsChasing)
            IsChasing = true;

        if (!Raging)
        {
            normSource.UnPause();
            rageSource.Pause();
        }
        else
        {
            normSource.UnPause();
            rageSource.UnPause();
        }

        

    }
}
