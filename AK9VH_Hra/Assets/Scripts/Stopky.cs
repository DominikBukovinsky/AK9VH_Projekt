using UnityEngine;
using TMPro;

public class Stopky : MonoBehaviour
{
    public TextMeshProUGUI textCasovace;
    private float cas = 0f;

    void Update()
    {
        // --- OPRAVA PROTI SPAMU V KONZOLI ---
        // Pokud kolega zapomněl přiřadit text, skript se tady zastaví a nevyhodí chybu.
        if (textCasovace == null) return;
        // ------------------------------------

        cas += Time.deltaTime;

        float minuty = Mathf.FloorToInt(cas / 60);
        float vteriny = Mathf.FloorToInt(cas % 60);

        textCasovace.text = string.Format("{0:00}:{1:00}", minuty, vteriny);
    }


    public void ResetCas()
    {
        cas = 0f;
        // I tady pro jistotu kontrola, kdyby to někdo volal manuálně
        if (textCasovace != null) Update();
    }

    public void NacistCas(float ulozenyCas)
    {
        cas = ulozenyCas;
    }

    public float ZiskatAktualniCas()
    {
        return cas;
    }
}