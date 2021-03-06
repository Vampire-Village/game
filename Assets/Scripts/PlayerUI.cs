using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Button infectButton;
    public TMP_Text infectText;
    public Button reportButton;
    public TMP_Text reportText;
    public Button interactButton;

    public GameObject vampInfMinimap;
    public GameObject villagerMinimap;

    private GamePlayer player;

    public GameObject render;
    public RenderTexture rt;
    public Camera cam;

    private void Start()
    {
        GamePlayer.OnPlayerSpawned.AddListener(SetPlayer);
        GamePlayer.OnRoleUpdated.AddListener(SetUI);

        infectButton.onClick.AddListener(Infect);
        reportButton.onClick.AddListener(Report);
        interactButton.onClick.AddListener(Interact);
    }

    private void SetPlayer()
    {
        player = GamePlayer.local;
        player.vampireLord.RegisterUI(this);
    }

    private void SetUI()
    {
        infectButton.gameObject.SetActive(false);
        reportButton.gameObject.SetActive(false);
        interactButton.gameObject.SetActive(false);

        vampInfMinimap.gameObject.SetActive(false);
        villagerMinimap.gameObject.SetActive(false);

        switch (player.role)
        {
            case Role.VampireLord:
                infectButton.gameObject.SetActive(true);
                vampInfMinimap.gameObject.SetActive(true);
                AssignVampMinimap();
                break;
            case Role.Villager:
                interactButton.gameObject.SetActive(true);
                villagerMinimap.gameObject.SetActive(true);
                AssignVilMinimap();
                break;
            case Role.Infected:
                interactButton.gameObject.SetActive(true);
                reportButton.gameObject.SetActive(true);
                vampInfMinimap.gameObject.SetActive(true);
                AssignVampMinimap();
                break;
            default:
                GameLogger.LogClient("Player role is not assigned, so UI is not initialized.");
                break;
        }
    }

    private void AssignVampMinimap()
    {
        CreateRenderTexture rtScript = player.gameObject.transform.Find("MinimapCamera").GetComponent<CreateRenderTexture>();
        rtScript.AssignRenderTexture(0);

        /*
        render = GameObject.Find("VampireMinimap/Minimap/MinimapRender").gameObject;
        cam = player.gameObject.GetComponentInChildren<Camera>();
        // get render texture from the player's camera
        //rt = cam.targetTexture;
        rt = player.gameObject.GetComponentInChildren<CreateRenderTexture>().renderTexture; 
        // assign render texture into MinimapRender
        render.GetComponent<RawImage>().texture = rt;
        */
        Debug.Log("this is vfine.");
    }

    private void AssignVilMinimap()
    {
        player.gameObject.GetComponentInChildren<CreateRenderTexture>().AssignRenderTexture(1);
        Debug.Log("this is fine.");
        /*
        GameObject render = this.transform.Find("VillagerMinimap/Minimap/MinimapRender").gameObject;
        //GameObject render = minimap.transform.Find("MinimapRender").gameObject;
        RenderTexture rt = player.transform.Find("MinimapCamera").gameObject.GetComponent<Camera>().targetTexture;
        render.GetComponent<RawImage>().texture = rt;*/
    }

    private void Infect()
    {
        player.vampireLord.Infect();
    }

    private void Report()
    {
        player.infected.Report();
    }

    private void Interact()
    {
        player.controller.Interact();
    }

    private void OnDestroy()
    {
        GamePlayer.OnPlayerSpawned.RemoveListener(SetPlayer);
        GamePlayer.OnRoleUpdated.RemoveListener(SetUI);

        infectButton.onClick.RemoveListener(Infect);
        reportButton.onClick.RemoveListener(Report);
        interactButton.onClick.RemoveListener(Interact);
    }
}
