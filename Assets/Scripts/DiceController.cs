using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace shGames.Gameplay
{
    public class DiceController : MonoBehaviour
    {
        private Rigidbody rb;
        [SerializeField] private float rollForce;
        [SerializeField] private float torqueAmount;
        
        [SerializeField] private Button rollButton;
        [SerializeField] private float rollTimeout = 5f;
        
        [SerializeField] private LayerMask _layerMask;
        
        [SerializeField] private TextMeshProUGUI dieText;
        
        private bool isRolling = false;
        private Action<int> onRollComplete;
        
        private const string rollDieError = "Die is not correctly rolled. Please roll it again.";
        

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            onRollComplete += (dieValue) => dieText.text = dieValue==0 ? rollDieError : $"You have rolled {dieValue}";
            rollButton.onClick.AddListener(RollDice);
        }
        
        public void RollDice()
        {
            rb.Sleep();
            Vector3 targetPosition =
                new Vector3(UnityEngine.Random.Range(-1f, 1f), 10f, UnityEngine.Random.Range(-1f, 1f));
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.AddForce(direction * rollForce, ForceMode.Impulse);
            rb.AddTorque(UnityEngine.Random.insideUnitSphere * torqueAmount,ForceMode.Impulse);
            StartCoroutine(WaitForDieToStop());
            dieText.text = "Rolling...";
        }

        private IEnumerator WaitForDieToStop()
        {
            rollButton.interactable = false;
            float timeout = Time.time + rollTimeout;
            while (!rb.IsSleeping() && Time.time< timeout)
            {
                yield return null;
            }
            yield return null;
            Debug.Log("ended rolling");
            onRollComplete?.Invoke(GetDieValue());
            rollButton.interactable = true;
        }

        int GetDieValue()
        {
            Vector3[] direction = new Vector3[]
            {
                -transform.forward,
                -transform.up,
                transform.right,
                -transform.right,
                transform.up,
                transform.forward
                
            };
            for (int i = 0; i < direction.Length; i++)
            {
                if (!Physics.Raycast(transform.position, direction[i], 1f, _layerMask)) continue;
                return i + 1;
            }
            Debug.Log("Something went wrong");
            return 0;
        }
    }
}
