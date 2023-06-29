using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonoBehavior<ClassType> : MonoBehaviour
    where ClassType : SingletonMonoBehavior<ClassType>
{
    /// <summary>
    /// The instance of this class, accessing this variable for the first time will
    /// cause it to first Find a instance of this class in the scene and if it cannot find
    /// it, it will instantiate a new one to assign to this variable.
    /// </summary>
    public static ClassType Instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType(typeof(ClassType)) as ClassType;

                if(m_instance == null)
                {
                    Debug.Log($"Instantiating singleton {typeof(ClassType).Name} instance.");

                    m_instance = (new GameObject(typeof(ClassType).Name)).AddComponent<ClassType>();
                }

                m_instance.Setup();
            }
            
            return m_instance;
        }
    } 

    private static ClassType m_instance = null;
    private bool initialized = false;

    private void Setup()
    {
        if(initialized)
        {
            Debug.LogWarning(this.GetType().Name + "'s Setup already done!");
            return;
        }

        initialized = true;

        AdditionalSetup();
    }

    /// <summary>
    /// Called when this class is instantiated or when the scene loads, if the class is already in the scene
    /// </summary>
    protected virtual void AdditionalSetup() {}


    /// <summary>
    /// Called when this class is destroyed
    /// </summary>
    protected virtual void AdditionalOnDestroy() {}

    // Check for existing instances and does setup if none is found
    protected void Start()
    {   
        if(m_instance != null)
        {
            if(m_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            m_instance = (ClassType) this;
        }

        if(!initialized)
            Setup();
    }

    protected void OnDestroy()
    {
        if(m_instance == this)
        {
            m_instance = null;
        }

        AdditionalOnDestroy();
    }
}
