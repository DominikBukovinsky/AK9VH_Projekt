using UnityEngine;
using TMPro;
using System.Collections;

public class FailManager : MonoBehaviour
{
    [Header("Nastaveni")]
    public FrogController frog;
    public TextMeshProUGUI textHlasky;
    public float minimalniPad = 10f; 
    public float jakDlouhoZobrazit = 3f;

    [Header("Texty")]
    [TextArea]
    public string[] hlasky;

    private float maximalniVyskaVeVzduchu;
    private bool bylaVeVzduchu = false;

    void Start()
    {
        // POJISTKA: Hned na zacatku text vypneme, at nestrasi v menu
        if(textHlasky != null)
            textHlasky.gameObject.SetActive(false);

        // Nastavime startovni vysku na aktualni pozici zaby
        if(frog != null)
            maximalniVyskaVeVzduchu = frog.transform.position.y;
    }

    void Update()
    {
        // Pokud zaba jeste neexistuje (treba pri nacitani), nic nedelej
        if (frog == null) return;

        if (!frog.isGrounded)
        {
            bylaVeVzduchu = true;
            if (frog.transform.position.y > maximalniVyskaVeVzduchu)
            {
                maximalniVyskaVeVzduchu = frog.transform.position.y;
            }
        }
        else if (frog.isGrounded && bylaVeVzduchu)
        {
            CheckPad();
            bylaVeVzduchu = false;
            maximalniVyskaVeVzduchu = frog.transform.position.y;
        }
    }

    void CheckPad()
    {
        float vzdalenostPadu = maximalniVyskaVeVzduchu - frog.transform.position.y;

        if (vzdalenostPadu > minimalniPad)
        {
            ZobrazHlasku();
        }
    }

    void ZobrazHlasku()
    {
        if (hlasky.Length > 0)
        {
            int nahoda = Random.Range(0, hlasky.Length);
            textHlasky.text = hlasky[nahoda];
            
            textHlasky.gameObject.SetActive(true);

            StopAllCoroutines(); 
            StartCoroutine(SchovejText());
        }
    }

    IEnumerator SchovejText()
    {
        yield return new WaitForSeconds(jakDlouhoZobrazit);
        textHlasky.gameObject.SetActive(false);
    }
}