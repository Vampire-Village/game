using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Button infectButton;
    public TMP_Text infectText;
    public Button pingButton;
    public TMP_Text pingText;
    public Button interactButton;

    public GameObject vampInfMinimap;
    public GameObject villagerMinimap;

    private GamePlayer player;

    private void Start()
    {
        GamePlayer.OnPlayerSpawned.AddListener(SetPlayer);
        GamePlayer.OnLocalRoleUpdated.AddListener(SetUI);

        infectButton.onClick.AddListener(Infect);
        pingButton.onClick.AddListener(Ping);
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
        pingButton.gameObject.SetActive(false);
        interactButton.gameObject.SetActive(false);

        vampInfMinimap.gameObject.SetActive(false);
        villagerMinimap.gameObject.SetActive(false);

        switch (player.role)
        {
            case Role.VampireLord:
                interactButton.gameObject.SetActive(true);
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
                pingButton.gameObject.SetActive(true);
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
    }

    private void AssignVilMinimap()
    {
        CreateRenderTexture rtScript = player.gameObject.transform.Find("MinimapCamera").GetComponent<CreateRenderTexture>();
        rtScript.AssignRenderTexture(1);
    }

    private void Infect()
    {
        player.vampireLord.Infect();
    }

    private void Ping()
    {
        player.infected.Ping();
    }

    private void Interact()
    {
        player.controller.Interact();
    }

    private void OnDestroy()
    {
        GamePlayer.OnPlayerSpawned.RemoveListener(SetPlayer);
        GamePlayer.OnLocalRoleUpdated.RemoveListener(SetUI);

        infectButton.onClick.RemoveListener(Infect);
        pingButton.onClick.RemoveListener(Ping);
        interactButton.onClick.RemoveListener(Interact);
    }
}
