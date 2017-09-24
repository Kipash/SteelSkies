using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using AponiBackend;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

[assembly: AssemblyVersion("1.2.*")]
namespace Aponi
{
    public class AppServices : MonoBehaviour
    {
        public static bool Initiliazed;

        static AppServices instance;
        public static AppServices Instance
        {
            get
            {
                return instance;
            }
        }
        
        [Header("- Data -")]
        public FileConfig fileConfig;

        [Header("- Services -")]
        public AppUIManager AppUI;
        public ConsoleInput ConsoleManger;
        public SceneManager SceneManager;
        [HideInInspector] public CVarManager cVarManager;
        [HideInInspector] public AppInput AppInput = new AppInput();
        public AudioManager AudioManager;
        public PrefabPoolManager PoolManager;

        [Header("- Settings -")]
        public bool DebugFeatures;


        DateTime t;
        TimeSpan diff;

        static string version;
        public static string VersionNumber
        {
            get
            {
                if (string.IsNullOrEmpty(version))
                {
                    version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                return version;
            }
        }

        public static void OutputVersion()
        {
            Console.Console.WriteLine(string.Format("Starting version {0}", VersionNumber));
        }

        public static void ForceQuit()
        {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

            Console.Console.WriteLine(string.Format("Exiting version {0}", VersionNumber));

        }

        void Awake()
        {
            t = DateTime.Now;

            Initiliazed = true;

            SetupInstance();
            InitializeBackend();
            InitializeServices();

            Console.Console.AddStatic("v", typeof(AppServices), "OutputVersion");
            Console.Console.AddStatic("version", typeof(AppServices), "OutputVersion");

            Console.Console.AddStatic("quit", typeof(AppServices), "ForceQuit");
            Console.Console.AddStatic("exit", typeof(AppServices), "ForceQuit");

            diff = (DateTime.Now - t);
            Console.Console.WriteLine(string.Format("All Game services loaded in {0} ms ({1} tics)", diff.TotalMilliseconds, diff.Ticks), true);
        }

        GameObject go;
        private void Update()
        {
            AppInput.CheckAnyKey();
            ConsoleManger.Update();
            if (DebugFeatures)
            {
                AppUI.CurrentKeysText.text = Input.inputString;
            }

            //for (int i = 0; i < 50; i++)
            //{
            //    PoolManager.DeactivatePrefab(PoolManager.GetPooledPrefab(PooledPrefabs.Bullet));
            //}
            
            
            if (Input.GetKey(KeyCode.F1))
            {
                go = PoolManager.GetPooledPrefab(PooledPrefabs.Bullet);
                go = PoolManager.GetPooledPrefab(PooledPrefabs.ChargedBullet);
            }
            else if(Input.GetKey(KeyCode.F2))
            {
                PoolManager.DeactivatePrefab(go);
            }
        }

        void SetupInstance()
        {
            if (instance == null)
                instance = this;
            else if (instance.gameObject != null)
            {
                Destroy(gameObject);
                throw new Exception("AppServices: multiple instances detected");
            }
            else
                instance = this;
        }

        void InitializeBackend()
        {
            DataStorage.Load(fileConfig);
        }
        void InitializeServices()
        {
            ConsoleManger.Initialize();

            AppUI.SetVersion(Application.platform + " " + VersionNumber);
            AppInput.Initialize();
            SceneManager.Initialize();
            cVarManager.Initialize();
            PoolManager.Initialize();
            AudioManager.Initialize();
        }
    }
}