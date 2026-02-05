using UnityEngine;
using TMPro;
using UnityEngine.UI;


using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button reiniciarButton;
    public Button menuButton;
    
    private bool gameOverActivo = false;

  void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameOverPanel != null) 
            gameOverPanel.SetActive(false);
        if (reiniciarButton != null)
            reiniciarButton.onClick.AddListener(ReiniciarEscena);
        if (menuButton != null)
            menuButton.onClick.AddListener(IrAlMenu);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOverActivo)
        {
            ReiniciarEscena();
        }
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.M))
        {
            IrAlMenu();
        }
    }
public void GameOver()
    {
        if (gameOverActivo) return;

        gameOverActivo=true;

        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        if(gameOverText  != null) 
        {
            gameOverText.text = "GAME OVER\n\nR - Reiniciar\nESC - Menu Principal";
        }
    }
    
    public void ReiniciarEscena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void IrAlMenu()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene("Menu");
    }
}


