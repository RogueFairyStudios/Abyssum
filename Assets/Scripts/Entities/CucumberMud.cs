using System.Collections.Generic;

using UnityEngine;

using DEEP.Entities.Player;

namespace DEEP.Entities{
   public class CucumberMud : MonoBehaviour
   {
      [SerializeField] private List<EntityBase> entitieList = new List<EntityBase>();  
      
      protected void OnTriggerStay(Collider col) {
         //Debug.Log(col.gameObject);
         
         EntityBase obj = col.gameObject.GetComponentInParent<EntityBase>();
         if (obj != null && !entitieList.Contains(obj))
         {
            obj.SetSlow();
            entitieList.Add(obj);
         }      
      }

      protected void OnTriggerExit(Collider col) {
         //Debug.Log(col.gameObject);
         
         EntityBase obj = col.gameObject.GetComponentInParent<EntityBase>();
         if (obj != null && entitieList.Contains(obj))
         {
            obj.SetBaseSpeed();
            entitieList.Remove(obj);

            if(obj.gameObject.CompareTag("Player"))
            {
               PlayerController.Instance.HUD.StopConstantScreenFeedback();
               GetComponent<LiquidSound>().Stop();
            }
         }
      }

      protected void OnDestroy()
      {
         for (int i = 0; i < entitieList.Count; i++)
         {
            entitieList[i].SetBaseSpeed();
            if(entitieList[i].gameObject.CompareTag("Player"))
            {
               PlayerController.Instance.HUD.StopConstantScreenFeedback();
               GetComponent<LiquidSound>().Stop();
            }
         }
      }

      void Update() {
         for (int i = 0; i < entitieList.Count; i++)
            if(!entitieList[i])
               entitieList.Remove(entitieList[i]);
      }
   }
}