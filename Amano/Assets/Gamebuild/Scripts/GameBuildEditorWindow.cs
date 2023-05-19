#if UNITY_EDITOR

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Gamebuild.Scripts;
using Unity.VisualScripting.Antlr3.Runtime;

public class GameBuildEditorWindow : EditorWindow
{
    

    

    private static GameBuildConfig data;
    private static string defaultPath = GameBuildBuilder.defaultPath;

    
    [SerializeField] private VisualTreeAsset _tree;
    
    private TextField tokenField;
    private Button buildButton;
    private Button copyLinkButton;
    private Button getStartedButton;
    private Button discordButton;
    private Button feedbackButton;
    private static string api_url = "https://app.gamebuild.io/";

    public string copylink;

    
    [MenuItem("Gamebuild/Gamebuild")]
    public static void ShowEditor()
    {
        var window = GetWindow<GameBuildEditorWindow>();
        window.titleContent = new GUIContent("Gamebuild");
    }
    


    private void CreateGUI()
    {
        _tree.CloneTree(rootVisualElement);

        buildButton = rootVisualElement.Q<Button>("build-button");
        copyLinkButton = rootVisualElement.Q<Button>("copylink-button");
        getStartedButton = rootVisualElement.Q<Button>("getstarted-button");
        discordButton = rootVisualElement.Q<Button>("discord-button");
        feedbackButton = rootVisualElement.Q<Button>("feedback-button");

        buildButton.clicked += OnBuildPressed;
        copyLinkButton.clicked += OnCopyLinkPressed;
        getStartedButton.clicked += OnGetStartedPressed;
        discordButton.clicked += OnDiscordPressed;
        feedbackButton.clicked += OnFeedbackPressed;
        
        tokenField = rootVisualElement.Q<TextField>("TokenText");
        tokenField.RegisterValueChangedCallback(HandleToken);

        copyLinkButton.clickable.target.visible = true;
        init();

    }
    

    private async void init()
    {
        if (tokenField.value.Length == 64)
        {
            copylink = await VALIDATE_TOKEN(tokenField.value);
            if (copylink.Length > 0)
            {
                ShowCopyLink();
            }
            else
            {
                HideCopyLink();
            }
            ShowCopyLink();
        }
        else
        {
            HideCopyLink();
        }
    }
    
    private async void HandleToken(ChangeEvent<string> value)
    {
     //Check length of value
     if (value.newValue.Length == 64)
     {
         copylink = await VALIDATE_TOKEN(value.newValue);
         if (copylink.Length > 0)
         {
             ShowCopyLink();
         }
         else
         {
             HideCopyLink();
         }
     }
     else
     {
         HideCopyLink();
     }
     
    }

    private void OnBuildPressed()
    {
        Debug.Log("Building");
        BuildAndZip();
    }

    
    private async void OnCopyLinkPressed()
    {
        if (tokenField.value.Length == 64)
        {
            if (!String.IsNullOrEmpty(copylink))
            {
                Debug.Log("CopyLinkPressed");
                Application.OpenURL(api_url + "projects/" + copylink);
            }
        }
        else
        {
            Debug.Log("Please provide a token first");
            return;
        }
        
    }

    private void OnGetStartedPressed()
    {
        Debug.Log("OnGetStartedPressed");
        Application.OpenURL("https://gamebuild.gitbook.io/gamebuild.io/");
     
        
    }

    private void OnDiscordPressed()
    {
        Debug.Log("OnDiscordPressed");
        Application.OpenURL("https://discord.gg/Mxw8mMzURA");
    }

    private void OnFeedbackPressed()
    {
        Debug.Log("OnFeedbackPressed");
        Application.OpenURL("https://tally.so/r/w2XMOe");
    }

    
    public void ShowCopyLink()
    {
        buildButton.SetEnabled(true);
        copyLinkButton.SetEnabled(true);
    }
    
    public void HideCopyLink()
    {
        buildButton.SetEnabled(false);
        copyLinkButton.SetEnabled(false);
    }
    
    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        // if no data exists yet create and reference a new instance
        if (!data)
        {
            // as first option check if maybe there is an instance already
            // and only the reference got lost
            // won't work ofcourse if you moved it elsewhere ...
            data = AssetDatabase.LoadAssetAtPath<GameBuildConfig>("Assets/Gamebuild/Gamebuildconfig.asset");
            // if that was successful we are done
            if (data) return;

            // otherwise create and reference a new instance
            data = CreateInstance<GameBuildConfig>();

            AssetDatabase.CreateAsset(data, "Assets/Gamebuild/Gamebuildconfig.asset");
            AssetDatabase.Refresh();
        }
    }

    public string UrlEncode(string str)
    {
        if (str == null || str == "")
        {
            return null;
        }

        byte[] bytesToEncode = System.Text.UTF8Encoding.UTF8.GetBytes(str);
        String returnVal = System.Convert.ToBase64String(bytesToEncode);
        return returnVal.TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }


    private async void BuildAndZip()
    {
        
        try
        {
            if (String.IsNullOrEmpty(tokenField.value))
            {
                return;
            }

            GameBuildBuilder.BuildServer(false);
            EditorUtility.DisplayProgressBar("Gamebuild", "Zipping Files", 0.4f);
            string zipFile = GameBuildBuilder.ZipServerBuild(copylink);

            string directoryToZip = Path.GetDirectoryName(defaultPath);
            string targetfile = Path.Combine(directoryToZip, @".." + Path.DirectorySeparatorChar + copylink + ".zip");
            EditorUtility.DisplayProgressBar("Gamebuild", "Uploading Files", 0.75f);
            string projectname = PlayerSettings.productName;
            string studioname = PlayerSettings.companyName;

            string upload_url = await GET_UPLOAD_URL(tokenField.value, projectname, studioname);
            
            Debug.Log(upload_url);
            
            Upload(targetfile, upload_url, tokenField.value, projectname, studioname);
            
            
            //PlayFlowBuilder.cleanUp(zipFile);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private static void Upload(string fileLocation,string upload_url, string token, string projectname, string studioname)
    {
        try
        {
            Uri actionUrl = new Uri(upload_url);

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/zip");
                client.UploadFile(actionUrl, "PUT", fileLocation);
                Debug.Log("File uploaded successfully");
                
                // If upload is success
                SUCCESS(token, projectname, studioname);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    
    public static async Task<string> VALIDATE_TOKEN(string token)
    {
        string output = "";
        try
        {
            string actionUrl = api_url + "validate_token";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("token", token);
                HttpResponseMessage response = await client.GetAsync(actionUrl);
                if (response.IsSuccessStatusCode)
                {
                    output = await response.Content.ReadAsStringAsync();
                    output = output.Trim('"');
                    return output;
                }
                else
                {
                    Debug.Log($"Invalid Token: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
        //Escape output string
        return output;
    }
    
    
    
    public static async Task<string> SUCCESS(string token, string projectname, string studioname)
    {
        string output = "";
        try
        {
            string actionUrl = api_url + "success";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("token", token);
                client.DefaultRequestHeaders.Add("projectname", projectname);
                client.DefaultRequestHeaders.Add("studioname", studioname);

                HttpResponseMessage response = await client.GetAsync(actionUrl);
                if (response.IsSuccessStatusCode)
                {
                    output = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Debug.LogError($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
        //Escape output string
        output = output.Trim('"');
        return output;
    }
    
    public static async Task<string> GET_UPLOAD_URL(string token, string projectname, string studioname)
    {
        string output = "";
        try
        {
            string actionUrl = api_url + "upload_url";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("token", token);
                client.DefaultRequestHeaders.Add("projectname", projectname);
                client.DefaultRequestHeaders.Add("studioname", studioname);

                HttpResponseMessage response = await client.GetAsync(actionUrl);
                if (response.IsSuccessStatusCode)
                {
                    output = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Debug.LogError($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
        //Escape output string
        output = output.Trim('"');
        return output;
    }

    private void OnGUI()
    {
    }
    
    public void OnInspectorUpdate()
    {
        Repaint();
    }
    
}

#endif
