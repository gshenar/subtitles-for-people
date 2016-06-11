using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections;

public class Subtitle : MonoBehaviour {
    public int counter = 0;
    public int textCounter = 0;
    private Vector3 collectionStartingOffsetFromCamera;
    private List<string> subtitleText;
    private string currentText;

    private string fullText;

    // Use this for initialization
    void Start () {
        collectionStartingOffsetFromCamera = gameObject.transform.localPosition;
        subtitleText = new List<string>();
    //    subtitleText.Add("Welcome to the deaf man experience");
   //     subtitleText.Add("We hope you enjoy your time here");
   //     subtitleText.Add("This is a very magical app");
   //     subtitleText.Add("As people speak you will be able to \n see exactly what they say");
  //      subtitleText.Add("Nobody will ever know you are \n deaf again!!");
  //      subtitleText.Add("Enjoy");



   //     fullText = "In ancient manuscripts, another means to divide sentences into paragraphs was a line break (newline) followed by an initial at the beginning of the next paragraph. An initial is an oversize capital letter, sometimes outdented beyond the margin of text. This style can be seen, for example, in the original Old English manuscript of Beowulf. Outdenting is still used in English typography, though not commonly.[4] Modern English typography usually indicates a new paragraph by indenting the first line. This style can be seen in the (handwritten) United States Constitution from 1787. For additional ornamentation, a hedera leaf or other symbol can be added to the inter-paragraph whitespace, or put in the indentation space.";

        //This will override with the server text
        GetTextFromServer();
     }

    // Update is called once per frame
    void Update() {
        // UpdateWithSubtitleArray();
        UpdateWithCurrentText();
    }


    public void UpdateWithCurrentText()
    {
        if (subtitleText.Count > 0)
        {
            if (counter % 200 == 0)
            {
                GetTextFromServerOneLineAtATime();
                var textMesh = GetComponent<TextMesh>();
                var veryCurrentText = currentText;

                var splitIndex = veryCurrentText.Length / 2;

                if(veryCurrentText.Length > 35)
                {
                    int i = 35;
                    while(i > 20)
                    {
                        if(veryCurrentText[i] == ' ')
                        {
                            splitIndex = i;
                            i = 0;
                        }
                        i--;
                    }
                }   
                                      
                textMesh.text = veryCurrentText.Substring(0, splitIndex) + "\n" + veryCurrentText.Substring(splitIndex);
                textCounter++;
            }
            counter++;
        }
    }

    public void GetTextFromServerOneLineAtATime()
    {
        WWW www = new WWW("http://10.1.10.56:8080/");
        while (!www.isDone)  //wait until www isdone
            ;

        if (www.error != null)
            return;
        print("made request " + www.text);
        currentText = www.text;
    }



    public void GetTextFromServer()
    {
        WWW www = new WWW("http://10.1.11.199:8080/");
        while (!www.isDone)  //wait until www isdone
            ;

        if (www.error != null)
            return;
        
        fullText = www.text;
        print(fullText);
        subtitleText = SplitByLength(fullText, 35).ToList();    
    }

    void UpdateWithSubtitleArray()
    {
        if(subtitleText.Count > 0)
        {
            if (counter % 200 == 0 && textCounter < subtitleText.Count - 1)
            {
                var textMesh = GetComponent<TextMesh>();
                textMesh.text = subtitleText[textCounter] + "\n" + subtitleText[textCounter + 1];
                textCounter++;
            }
            counter++;
        }
    }

    private IList<string> SplitByLength(string str, int maxLength)
    {
        var result = new List<string>();
        int index = 0;
        while(index < str.Length)
        {
            var c = maxLength;
            while(c > 0)
            {
                if(index + c >= str.Length)
                {
                    print(str.Length - index);
                    result.Add(str.Substring(index, str.Length - index));
                    index += maxLength;
                    c = 0;
                } else if(str[index + c] == ' ')
                {
                    result.Add(str.Substring(index, System.Math.Min(c, str.Length - index)));
                    index += c;
                    c = 0;
                } else
                {
                    c--;
                }
            }  
            if(c == 1) {
                result.Add(str.Substring(index, maxLength));
                index += maxLength;
            }
        }
        return result;
      //  for (int index = 0; index < str.Length; index += maxLength)
      //  {
      //      yield return str.Substring(index, System.Math.Min(maxLength, str.Length - index));
      //  }
    }

    private void DoMatrix()
    {
        var textMesh = GetComponent<TextMesh>();
        textMesh.text = textMesh.text + "1" + "0";
        counter++;
        if (counter % 30 == 0)
        {
            textMesh.text += "\n";
        }
        if (counter % 47 == 0)
        {
            transform.Translate(0, 1, 0);
        }
    }

    private void SmartPositioning()
    {
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        Vector3 newPosition = new Vector3(headPosition.x, headPosition.y, headPosition.z-4);
        gameObject.transform.position = newPosition;
    }

    private void PositionCorrectly()
    {
        // Update the Hologram Collection's position so it shows up
        // where the Fitbox left off. Start with the camera's localRotation...
        Quaternion camQuat = Camera.main.transform.localRotation;

        // ... ignore pitch by disabling rotation around the x axis
        camQuat.x = 0;

        // Rotate the vector and factor y back into the position
        Vector3 newPosition = camQuat * collectionStartingOffsetFromCamera;
        newPosition.y = collectionStartingOffsetFromCamera.y;

        // Position was "Local Position" so add that to where the camera is now
        gameObject.transform.position = Camera.main.transform.position + newPosition;

        // Rotate the Hologram Collection to face the user.
        Quaternion toQuat = Camera.main.transform.localRotation * gameObject.transform.rotation;
        toQuat.x = 0;
        toQuat.z = 0;
        gameObject.transform.rotation = toQuat;
    }
}
