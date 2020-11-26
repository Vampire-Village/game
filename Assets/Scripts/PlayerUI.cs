using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Button infectButton;
    public Button reportButton;
    public Button interactButton;

    private GamePlayer player;

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
    }

    private void SetUI()
    {
        infectButton.gameObject.SetActive(false);
        reportButton.gameObject.SetActive(false);
        interactButton.gameObject.SetActive(false);

        switch (player.role)
        {
            case GamePlayer.Role.VampireLord:
                infectButton.gameObject.SetActive(true);
                break;
            case GamePlayer.Role.Villager:
                interactButton.gameObject.SetActive(true);
                break;
            case GamePlayer.Role.Infected:
                interactButton.gameObject.SetActive(true);
                infectButton.gameObject.SetActive(true);
                break;
            default:
                GameLogger.LogClient("Player role is not assigned, so UI is not initialized.");
                break;
        }
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
