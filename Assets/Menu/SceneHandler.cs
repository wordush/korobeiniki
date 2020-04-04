using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Menu
{
    public class SceneHandler : MonoBehaviour
    {
        private static bool _intialize;

        public List<Image> dark;
        public float multiplyer;
        public bool dar;

        public delegate void SceneLoadHandler();
        public event SceneLoadHandler ReadyToLoad;
        [FormerlySerializedAs("AlreadyLoaded")] public bool alreadyLoaded;

        [FormerlySerializedAs("NextSceneID")] public int nextSceneId;

        private void Start()
        {
            if (!_intialize)
            {
                DontDestroyOnLoad(this);
                _intialize = true;
            }
            dar = false;
            ReadyToLoad += Load;
        }


        void Awake()
        {
            foreach(Image im in dark)
            {
                Color temp = new Color(255, 255, 255, 1);
                im.color = temp;
            }

        }

        public void Update()
        {
            if (dar)
            {
                EventSystem[] objs = FindObjectsOfType<EventSystem>();
                foreach (EventSystem cv in objs)
                {
                    cv.enabled = false;
                }
                foreach (Image im in dark)
                {
                    Color temp = im.color;
                    temp.a = Mathf.Clamp(temp.a + Time.deltaTime * multiplyer, 0, 1);
                    im.color = temp;
                }
            }
            else
            {
                foreach (Image im in dark)
                {
                    Color temp = im.color;
                    temp.a = Mathf.Clamp(temp.a - Time.deltaTime * multiplyer, 0, 1);
                    im.color = temp;
                }

                EventSystem[] objs = FindObjectsOfType<EventSystem>();
                foreach (EventSystem cv in objs)
                {
                    cv.enabled = true;
                }
            }

            if (dark?[0].color.a > 0.995f && !alreadyLoaded)
            {
                alreadyLoaded = true;
                ReadyToLoad?.Invoke();
            }
        }

        private void StartLoading()
        {
            Darker();
        }

        private void Load()
        {
            FindObjectOfType<SceneHandler>().StartCoroutine(nameof(LoadCorutine));
        }



        private void Darker()
        {
            FindObjectOfType<SceneHandler>().dar = true;
        }

        private void Lighter()
        {
            FindObjectOfType<SceneHandler>().dar = false;
        }

        public void NiceSceneLoader(int sceneId)
        {
            Debug.Log("Started to load");
            alreadyLoaded = false;
            nextSceneId = sceneId;
            StartLoading();
        }

        IEnumerator LoadCorutine()
        {
            yield return null;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextSceneId);

            while (!asyncOperation.isDone)
            {
                Debug.Log($"Loading : {asyncOperation.progress}");
                yield return null;
            }
            Lighter();
        }
    }
}
