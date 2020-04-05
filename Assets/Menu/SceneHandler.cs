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
        public void Load(int id)
        {
            StartCoroutine(nameof(LoadCorutine), id);
        }



        IEnumerator LoadCorutine(int id)
        {
            DontDestroyOnLoad(this.gameObject);

            EventSystem[] evts = FindObjectsOfType<EventSystem>();
            foreach (EventSystem cv in evts)
            {
                cv.enabled = false;
            }

            GameObject[] objs = GameObject.FindGameObjectsWithTag("SceneHider");
            List<Image> images = new List<Image>();
            foreach (GameObject obj in objs)
            {
                DontDestroyOnLoad(obj);
                if (obj.TryGetComponent<Image>(out Image imm))
                    images.Add(imm);
            }

            float transpend = 0;
            if (images.Count >= 1)
                while (transpend < 1)
                {
                    foreach (Image im in images)
                    {
                        Color t = im.color;
                        t.a = transpend;
                        im.color = t;
                    }

                    transpend += 0.01f;
                    yield return new WaitForSeconds(0.01f);
                }

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(id);

            while (!asyncOperation.isDone)
            {
                Debug.Log($"Loading : {asyncOperation.progress}");
                yield return new WaitForSeconds(0.01f);
                yield return null;
            }


            if (images.Count >= 1)
                while (transpend > 0)
                {
                    foreach (Image im in images)
                    {
                        Color t = im.color;
                        t.a = transpend;
                        im.color = t;
                    }

                    transpend -= 0.01f;
                    yield return new WaitForSeconds(0.01f);
                }

            evts = FindObjectsOfType<EventSystem>();
            foreach (EventSystem cv in evts)
            {
                cv.enabled = true;
            }

            foreach (GameObject im in objs)
                Destroy(im);

            SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
        }
    }
}
