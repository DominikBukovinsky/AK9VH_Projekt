using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditsManager : MonoBehaviour
{
    [Header("Propojení")]
    public VideoPlayer videoPlayer;
    public Image blackScreenImage;
    public GameObject textObject;
    public AudioSource backgroundMusic; // <--- NOVÉ: Sem přetáhneš Audio Source

    [Header("Časování")]
    public float casDoZacatkuFadu = 7.0f;
    public float trvaniFadu = 2.0f;

    [Header("Titulky a Menu")]
    public float rychlostRolovani = 100f;
    public float konecnaPoziceY = 1500f;
    public string nazevMenuSceny = "Menu";

    private bool titulkyJedou = false;

    void Start()
    {
        StartCoroutine(PrehrajSekvenci());
    }

    IEnumerator PrehrajSekvenci()
    {
        // 1. PŘÍPRAVA
        if (blackScreenImage != null)
        {
            blackScreenImage.color = new Color(0, 0, 0, 0); 
            blackScreenImage.gameObject.SetActive(true);
        }
        
        if (textObject != null) textObject.SetActive(false);
        
        // Video hraje, hudba zatím mlčí (protože PlayOnAwake je vypnuté)
        if (videoPlayer != null) videoPlayer.Play();

        // 2. ČEKÁNÍ
        yield return new WaitForSeconds(casDoZacatkuFadu);

        // 3. FADE OUT
        float uplynulyCas = 0f;
        while (uplynulyCas < trvaniFadu)
        {
            uplynulyCas += Time.deltaTime;
            float alpha = Mathf.Clamp01(uplynulyCas / trvaniFadu);
            if (blackScreenImage != null)
            {
                blackScreenImage.color = new Color(0, 0, 0, alpha);
            }
            yield return null;
        }

        // 4. START TITULKŮ A HUDBY
        SpustitTitulky();
    }

    void SpustitTitulky()
    {
        if (videoPlayer != null) videoPlayer.gameObject.SetActive(false);
        if (blackScreenImage != null) blackScreenImage.color = new Color(0, 0, 0, 1);

        // Zapneme text
        if (textObject != null)
        {
            textObject.SetActive(true);
            titulkyJedou = true;
        }

        // --- ZDE SE POUŠTÍ HUDBA ---
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
    }

    void Update()
    {
        if (titulkyJedou && textObject != null)
        {
            textObject.transform.Translate(Vector3.up * rychlostRolovani * Time.deltaTime);

            if (textObject.transform.localPosition.y > konecnaPoziceY)
            {
                SceneManager.LoadScene(nazevMenuSceny);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            if (!titulkyJedou) SpustitTitulky();
            else SceneManager.LoadScene(nazevMenuSceny);
        }
    }
}