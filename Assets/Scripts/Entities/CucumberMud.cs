using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
         }
      }

      protected void OnDestroy()
      {
         for (int i = 0; i < entitieList.Count; i++)
         {
            entitieList[i].SetBaseSpeed();
         }
      }

      void Update() {
         for (int i = 0; i < entitieList.Count; i++)
            if(!entitieList[i])
               entitieList.Remove(entitieList[i]);
      }
   }
}