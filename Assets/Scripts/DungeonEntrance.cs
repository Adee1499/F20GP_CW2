using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonEntrance : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            
            InventoryUI.Instance.UI_LoadingScreen.SetActive(true);
            
            SceneManager.LoadScene(2, LoadSceneMode.Additive);

            // Teleport the player
            var controller = other.GetComponent<CharacterController>();
            controller.enabled = false;
            other.transform.position = new Vector3(-35f, 0f, 5f);
            
            controller.enabled = true;

            // Deactivate the screen overlay panel after a short delay
            StartCoroutine(DeactivateScreenOverlay(1f));
        }
    }

    private IEnumerator DeactivateScreenOverlay(float delay) 
    {
        yield return new WaitForSeconds(delay);
        InventoryUI.Instance.UI_LoadingScreen.SetActive(false);
        SceneManager.UnloadSceneAsync(1);
    }
}
