using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Goap
{
    public class Goal_KeepFed : Goal_Base
    {
        [SerializeField] float foodDecayRate = 0.5f;
        [SerializeField] GameObject textPrefab;

        TextMeshProUGUI text;
        private float foodAmount = 100;

        private void Start()
        {
            text = Instantiate(textPrefab).GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
        }

        public override void OnTickGoal()
        {
            base.OnTickGoal();
            foodAmount -= foodDecayRate * Time.deltaTime;

            if (foodAmount <= -10)
                Destroy(gameObject);

            text.text = "H: " + Mathf.FloorToInt(foodAmount) + " / 100";
            text.transform.position = transform.position + new Vector3(0, 0.5f);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Collision happened.");

            if (collision.CompareTag("Enemy"))
            {
                Destroy(collision.gameObject);
                foodAmount += 25;
            }
        }

        public override int CalculatePriority()
        {
            
            int priority = foodAmount < 50 ? 50 : 20;
            return priority;
        }

        public override bool CanRun()
        {
            bool canRun = foodAmount < 75 ? true : false;
            return canRun;
        }

        public override void OnGoalActivated()
        {
            base.OnGoalActivated();
            Debug.Log("Goal_KeepFed activated.");
        }

        public override Dictionary<string, object> GetDesiredState()
        {
            return new Dictionary<string, object> { { "is_hungry", false } };
        }
    }
}