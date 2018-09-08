using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoogleService : MonoBehaviour {

    public PictureFactory pictureFactory;
    public Text buttonText;
    private const string API_KEY = "AIzaSyBAVpA4rKfMPP5AcqqCobOCNF1S5uqa8Yw";

    public void GetPicture() {
        StartCoroutine(PictureRoutiine());
    }

    IEnumerator PictureRoutiine() {
        buttonText.transform.parent.gameObject.SetActive(false);
        string query = buttonText.text;
        query = WWW.EscapeURL(query + " memes");
        //Esto es para eliminar las nuevas imagenes para una nueva busqueda
        pictureFactory.DeleteOldPictures();
        Vector3 cameraForward = Camera.main.transform.forward;

        //Buscar solo 10 imagenes segun configuracion en google panel
        int rowNum = 1;
        for (int i = 1; i < 60; i+= 10)
        {
            string url = "https://www.googleapis.com/customsearch/v1?q=" + query + "&cx=017319053153040316754%3Aduciwq-c0iq&filter=1&num=10&searchType=image&start=" + i + "&fields=items%2Flink&key=" + API_KEY;

            WWW www = new WWW(url);
            yield return www;
            //Para crear imagenes esto tomara una lista de string
            pictureFactory.CreateImages(ParseResponse(www.text), rowNum, cameraForward);
            rowNum++;
            
        }
        yield return new WaitForSeconds(5f);
        buttonText.transform.parent.gameObject.SetActive(true);
    }

    //Parse the response en json
    List<string> ParseResponse(string text) {
        List<string> urlList = new List<string>();
        string[] urls = text.Split('\n');
        foreach (string line in urls)
        {   //Quizas cambiar string link a links
            if (line.Contains("link"))
            {
                string url = line.Substring(12, line.Length - 13);
                //Despues de agregar la URL a la lista vamos a verificar si contiene jpeg o png, por que hay una forma de filtrar esto en google API y en Custom Search API
                if (url.Contains(".jpg") || url.Contains(".png"))
                {
                    urlList.Add(url);
                }
            }
        }
        return urlList;

    }


}
