using UnityEngine;

public class ScreenCameraController : MonoBehaviour
{
    [Tooltip("Objekt hráče, který má kamera sledovat.")]
    public Transform player;

    [Tooltip("Přesná výška jedné 'obrazovky' (v Unity jednotkách).")]
    public float screenHeight = 10f; // Musí odpovídat tvému nastavení (Camera Size * 2)

    [Tooltip("Horizontální pozice, na kterou se kamera vycentruje.")]
    public float screenCenterX = 0f;

    private int currentScreenIndex = 0;
    private float cameraZ;
    private Camera cam; // <-- PŘIDALI JSME ODKAZ NA KAMERU

    void Start()
    {
        cam = GetComponent<Camera>(); // <-- ULOŽÍME SI KOMPONENTU KAMERY
        
        // Uložíme si původní Z pozici kamery (obvykle -10)
        cameraZ = transform.position.z;

        // Okamžitě přichytíme kameru na obrazovku, kde hráč začíná
        SnapToPlayerScreen();
    }

    // Používáme LateUpdate, aby se kamera pohnula až POTÉ, 
    // co se hráč pohnul ve svém Update()
    void LateUpdate()
    {
        if (player == null) return; // Pojistka, kdyby hráč neexistoval

        // 1. Zjistíme, na jaké obrazovce by hráč MĚL BÝT
        int newScreenIndex = Mathf.FloorToInt(player.position.y / screenHeight);

        // 2. Pokud se index změnil, je čas pohnout kamerou
        if (newScreenIndex != currentScreenIndex)
        {
            currentScreenIndex = newScreenIndex;
            SnapCameraToCurrentScreen();
        }
    }

    void SnapCameraToCurrentScreen()
    {
        // 3. Vypočítáme cílovou Y pozici STŘEDU nové obrazovky
        float targetY = (currentScreenIndex * screenHeight) + (screenHeight / 2f);

        // 4. Přesuneme kameru OKAMŽITĚ (bez plynulého pohybu)
        transform.position = new Vector3(screenCenterX, targetY, cameraZ);
    }

    // Tuto funkci voláme na startu
    void SnapToPlayerScreen()
    {
        currentScreenIndex = Mathf.FloorToInt(player.position.y / screenHeight);
        SnapCameraToCurrentScreen();
    }


    // ***************************************************************
    // ***** ZDE ZAČÍNÁ NOVÝ KÓD PRO ZOBRAZENÍ HRANIC V EDITORU *****
    // ***************************************************************
    
    void OnDrawGizmos()
    {
        // Tuto funkci Unity volá automaticky POUZE v editoru (Scene view)
        
        // 1. Zjistíme šířku obrazovky
        // Musíme si "chytit" komponentu kamery
        if (cam == null)
        {
            cam = GetComponent<Camera>(); // Zkusíme ji chytit, i když hra neběží
        }
        if (cam == null) return; // Pokud ji nenajdeme, nic neděláme

        // Výška je (Size * 2), což je naše screenHeight
        // Šířka je (Výška * Poměr stran kamery)
        float screenWidth = screenHeight * cam.aspect; 

        // 2. Nastavíme barvu pro ohraničení
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Oranžová, poloprůhledná

        // 3. Vykreslíme několik obrazovek pro přehlednost
        // Můžeme nakreslit třeba 20 obrazovek nahoru a 5 dolů
        for (int i = -5; i < 20; i++)
        {
            // Vypočítáme střed Y pro tuto obrazovku
            // Stejná logika jako ve SnapCameraToCurrentScreen()
            float centerY = (i * screenHeight) + (screenHeight / 2f);

            // Sestavíme střed a velikost boxu
            Vector3 center = new Vector3(screenCenterX, centerY, 0);
            Vector3 size = new Vector3(screenWidth, screenHeight, 0.1f);

            // Vykreslíme drátěný obdélník
            Gizmos.DrawWireCube(center, size);
        }
    }
}