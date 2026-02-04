using UnityEditor.Search;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Declaracion Singleton
    private static GameManager instance; //Definicion de la fortaleza de datos
    public static GameManager Instance
    {
        get
        {
            if (instance == null) Debug.Log("No hay Game Manager");
            return instance;
        }
    }
    //Fin del singleton

    //TODA LAS VARIABLES DE LA FORTALEZA DEBAN SER PUBLICAS
    public float playerHealth;
    public float maxHealth = 100;
    public int playerPoints;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


}


