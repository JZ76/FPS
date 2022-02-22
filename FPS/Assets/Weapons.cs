using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public int selectedWeapon = 0;

    public Animator Animator;

    public GameObject scopeOverlay;

    public GameObject aimOverlay;

    public GameObject weaponCamera;

    public Camera mainCamera;

    public float scopedFOV = 15f;

    public float normalFOV = 60f;

    private bool isScoped = false;

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        // temp is the index of weapon array, in my case, M1911 is 0, M4_8 is 1, M107 is 2, RPG is 3 
        int temp = selectedWeapon;
        // if scroll wheel go down, the index increase
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            selectedWeapon++;
            selectedWeapon %= transform.childCount;
        }
        // if scroll wheel go up, the index decrease
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            selectedWeapon--;
            if (selectedWeapon < 0)
            {
                selectedWeapon += transform.childCount;
            }
        }
        // alternatively, player can select weapon by using number key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedWeapon = 3;
        }

        if (temp != selectedWeapon)
        {
            SelectWeapon();
        }
        // can also scoped
        isScoped = Input.GetButton("Fire2");
        Animator.SetBool("isScoped", isScoped);
        // sniper rifle can also open scope
        if (isScoped && selectedWeapon == 2)
        {
            StartCoroutine(OnScoped());
        }
        else
        {
            OnUnScoped();
        }
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            weapon.gameObject.SetActive(i++ == selectedWeapon);
        }
    }

    void OnUnScoped()
    {
        // switch to aim UI
        scopeOverlay.SetActive(false);
        aimOverlay.SetActive(true);
        weaponCamera.SetActive(true);
        mainCamera.fieldOfView = normalFOV;
    }

    IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(0.15f);
        scopeOverlay.SetActive(true);
        aimOverlay.SetActive(false);
        weaponCamera.SetActive(false);
        mainCamera.fieldOfView = scopedFOV;
    }
}
