using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SM = UnityEngine.SceneManagement;
using System.Linq;

namespace SteelSkies
{
    [Serializable]
    public class SceneManager
    {
        public delegate void SceneChange(Scenes newScene, Scenes oldScene);
        public event SceneChange OnSceneChanged;

        public Scenes scene = Scenes.prescene;
        public Scenes CurrentScene
        {
            get
            {
                return scene;
            }
        }

        Scenes old;
        int i;
        int max;

        public void Initialize()
        {
            Console.Console.Add("map" , this, "LoadSceneConsole");
            Console.Console.Add("maps", this, "ListAllScenes");
        }

        public void LoadSceneConsole(string name)
        {
            LoadScene(name);
        }

        public void LoadScene(string name)
        {
            Scenes newScene = Scenes.none;
            Enum.TryParse(name, out newScene);

            if (newScene != Scenes.none)
            {
                old = scene;
                
                SM.SceneManager.LoadScene(name);

                if (OnSceneChanged != null)
                    OnSceneChanged.Invoke(newScene, old);
            }
            else
            {
                Console.Console.PrintErrorMessage("Uknown scene " + name + "!");
            }
        }

        public void LoadScene(Scenes s)
        {
            if (s != Scenes.none)
            {
                old = scene;

                SM.SceneManager.LoadScene(s.ToString());

                if (OnSceneChanged != null)
                    OnSceneChanged.Invoke(s, old);
            }
            else
            {
                Console.Console.PrintErrorMessage("Uknown scene " + s + "!");
            }
        }
        public void LoadScene(int i)
        {
            i = Mathf.Clamp(i, 0, Enum.GetValues(typeof(Scenes)).Length - 1);
            LoadScene((Scenes)i);
        }

        public void ReloadCurrentScene()
        {
            LoadScene(CurrentScene);
        }

        public void ListAllScenes()
        {
            max = Enum.GetValues(typeof(Scenes)).Length;
            Console.Console.WriteLine(" - All maps - ");
            Console.Console.WriteLine("Name");
            Console.Console.WriteLine(@"----------------------------------------------");
            for (i = 1; i < max; i++)
            {
                Console.Console.WriteLine(((Scenes)i).ToString());
            }
            Console.Console.WriteLine(@"----------------------------------------------");
            Console.Console.WriteLine("");
        }
    }
}