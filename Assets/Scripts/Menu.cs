using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject[] mapButtons;
    [SerializeField]
    private GameObject[] typeButtons;
    [SerializeField]
    private GameObject playButton;
    [SerializeField]
    private GameObject quadMapButtons;
    [SerializeField]
    private GameObject rectMapButtons;

    private MapSettings selectedMapSettings;

    private void Start()
    {
        rectMapButtons.SetActive(false);
        quadMapButtons.SetActive(true);
        FadeNotClickedButtons(mapButtons[0], mapButtons);
        selectedMapSettings = Settings.Map4x4;
        FadeNotClickedButtons(typeButtons[0], typeButtons);
    }

    public void OnMapButtonClick(GameObject clicked)
    {
        FadeNotClickedButtons(clicked, mapButtons);
        switch (clicked.name)
        {
            case "4x4":
                selectedMapSettings = Settings.Map4x4;
                break;
            case "5x5":
                selectedMapSettings = Settings.Map5x5;
                break;
            case "6x6":
                selectedMapSettings = Settings.Map6x6;
                break;
            case "8x8":
                selectedMapSettings = Settings.Map8x8;
                break;
            case "3x5":
                selectedMapSettings = Settings.Map3x5;
                break;
            case "4x6":
                selectedMapSettings = Settings.Map4x6;
                break;
            case "5x8":
                selectedMapSettings = Settings.Map5x8;
                break;
            case "6x9":
                selectedMapSettings = Settings.Map6x9;
                break;
        }
    }

    public void OnTypeButtonClick(GameObject clicked)
    {
        FadeNotClickedButtons(clicked, typeButtons);
        if (clicked.gameObject.name == "QuadType")
        {
            rectMapButtons.SetActive(false);
            quadMapButtons.SetActive(true);
            FadeNotClickedButtons(mapButtons[0], mapButtons);
            selectedMapSettings = Settings.Map4x4;
        }
        else
        {
            rectMapButtons.SetActive(true);
            quadMapButtons.SetActive(false);
            FadeNotClickedButtons(mapButtons[4], mapButtons);
            selectedMapSettings = Settings.Map3x5;
        }
    }

    private void FadeNotClickedButtons(GameObject clicked, GameObject[] buttons)
    {
        foreach (GameObject btn in buttons)
        {
            Image image = btn.GetComponent<Image>();
            Color color = image.color;
            color.a = btn != clicked ? 155f / 255 : 1f;
            image.color = color;
        }
    }

    public void OnPlayButtonClick()
    {
        Settings.SelectedMapSettings = selectedMapSettings;
        SceneManager.LoadScene("Game");
    }
}
