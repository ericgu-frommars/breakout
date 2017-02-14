using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Breakout.Behaviour
{
    class VFXUnit : MonoBehaviour
    {
        private ParticleSystem[] m_particleSystemArray;

        void Start()
        {
        
        }

        public void Init()
        {
            m_particleSystemArray = gameObject.GetComponentsInChildren<ParticleSystem>();
        }

        public void Play()
        {
            foreach (ParticleSystem particleSystem in m_particleSystemArray)
            {
                particleSystem.Play();
            }
        }

        public void Stop()
        {
            foreach (ParticleSystem particleSystem in m_particleSystemArray)
            {
                particleSystem.Stop();
            }
        }

        public void Clear()
        {
            foreach (ParticleSystem particleSystem in m_particleSystemArray)
            {
                particleSystem.Clear();
            }
        }

        public bool IsPlaying()
        {
            foreach (ParticleSystem particleSystem in m_particleSystemArray)
            {
                if (particleSystem.isPlaying)
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// controller to play visual FX
    /// </summary>
    public class VFXController : MonoBehaviour
    {
        private Dictionary<string, List<VFXUnit>> m_vfxUnitDict = new Dictionary<string, List<VFXUnit>>();

	    void Start () 
        {	
		    PreloadVFX ("Prefab/VFX/CFXM_Explosion",10);
	    }        

	    void PreloadVFX(string vfxName, int num)
	    {
		    List<VFXUnit> vfxUnitList = new List<VFXUnit>();

		    for (int i = 0; i < num; i++) 
		    {
			    GameObject vfxObj = GameObject.Instantiate (Resources.Load<GameObject> (vfxName), Vector3.zero, Quaternion.identity) as GameObject;
			    vfxObj.transform.parent = gameObject.transform;
                VFXUnit vfxUnit = vfxObj.AddComponent<VFXUnit>();
                vfxUnit.Init();
                vfxUnitList.Add(vfxUnit);

			    vfxObj.SetActive (false);			    
		    }

		    m_vfxUnitDict.Add( vfxName, vfxUnitList );
	    }

        public void DisplayVFX(string vfxName, Vector3 pos, float waitTime = 5.0f)
	    {
		    if (!m_vfxUnitDict.ContainsKey (vfxName)) 
		    {
			    List<VFXUnit> vfxUnitList = new List<VFXUnit>();

			    GameObject vfxObj = GameObject.Instantiate (Resources.Load<GameObject> (vfxName), pos, Quaternion.identity) as GameObject;
			    vfxObj.transform.parent = gameObject.transform;
                VFXUnit vfxUnit = vfxObj.AddComponent<VFXUnit>();
                vfxUnit.Init();
			    vfxUnitList.Add(vfxUnit);

			    m_vfxUnitDict.Add( vfxName, vfxUnitList );
			    PlayVFX( vfxUnit, waitTime );
		    }
		    else
		    {
			    List<VFXUnit> vfxUnitList = m_vfxUnitDict[vfxName];

			    bool isFoundIdle = false;
			    foreach(VFXUnit vfxUnit in vfxUnitList)
			    {
				    if (!vfxUnit.IsPlaying())
				    {
                        isFoundIdle = true;
					    vfxUnit.gameObject.SetActive(true);
                        vfxUnit.gameObject.transform.position = pos;
					    					    
					    PlayVFX(vfxUnit, waitTime);
					    break;
				    }
			    }

			    if( !isFoundIdle )
			    {
				    GameObject vfxObj = GameObject.Instantiate (Resources.Load<GameObject> (vfxName), pos, Quaternion.identity) as GameObject;
				    vfxObj.transform.parent = gameObject.transform;
                    VFXUnit vfxUnit = vfxObj.AddComponent<VFXUnit>();
                    vfxUnit.Init();
                    vfxUnitList.Add(vfxUnit);
				   
				    PlayVFX(vfxUnit, waitTime);
			    }
		    }
	    }

	    void PlayVFX( VFXUnit vfxUnit, float waitTime )
	    {
            //StopAllCoroutines();
		    
		    
			vfxUnit.Play();
		   
		    RecycleVFX(vfxUnit, waitTime);
	    }

	    void RecycleVFX(VFXUnit vfxUnit, float waitTime )
	    {
		    StartCoroutine(WaitAndRecycleVFX(vfxUnit,waitTime));
	    }

	    IEnumerator WaitAndRecycleVFX( VFXUnit vfxUnit, float waitTime )
	    {
		    yield return new WaitForSeconds(waitTime);

		    //recycle
		    vfxUnit.Stop ();
		    vfxUnit.Clear ();
		    vfxUnit.gameObject.SetActive(false);
	    }
    }
}
