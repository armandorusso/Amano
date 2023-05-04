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
    
    private Button buildButton;
    private Button copyLinkButton;
    private Button getStartedButton;
    private Button discordButton;
    private Button feedbackButton;
    private bool built;

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

        copyLinkButton.clickable.target.visible = false;

    }

    private void OnBuildPressed()
    {
        Debug.Log("Building");
        BuildAndZip();
    }

    private void OnCopyLinkPressed()
    {
        Debug.Log("CopyLinkPressed");
        Application.OpenURL("https://gamebuild.io/projects/" + copylink);
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
        copyLinkButton.clickable.target.visible = true;
    }
    
    public void HideCopyLink()
    {
        copyLinkButton.clickable.target.visible = false;
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

    private void LoadGamebuildConfig()
    {
        var serializedObject = new SerializedObject(data);
        // fetches the values of the real instance into the serialized one
        serializedObject.Update();
        copylink = serializedObject.FindProperty("Copylink").stringValue;
        built = serializedObject.FindProperty("built").boolValue;


        if (built)
        {
            ShowCopyLink();
        }
        else
        {
            HideCopyLink();
        }

        if (copylink.Length == 0)
        {
            GenerateToken();
        }
    }

    private void GenerateToken()
    {
        
        RandomNumberGenerator rng = new RNGCryptoServiceProvider();
        byte[] tokenData = new byte[32];
        rng.GetBytes(tokenData);
        string token = UrlEncode(Convert.ToBase64String(tokenData));;
        var serializedObject = new SerializedObject(data);
        // fetches the values of the real instance into the serialized one
        var configtoken = serializedObject.FindProperty("Copylink");
        configtoken.stringValue = token;
        serializedObject.ApplyModifiedProperties();
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
            if (copylink == null)
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

            string upload_url = await GET_UPLOAD_URL(copylink, projectname, studioname);
            
            Debug.Log(upload_url);
            
            Upload(targetfile, upload_url, copylink, projectname, studioname);
            
            
            //PlayFlowBuilder.cleanUp(zipFile);
            
            var serializedObject = new SerializedObject(data);
            // fetches the values of the real instance into the serialized one
            built = true;
            var configbuilt = serializedObject.FindProperty("built");
            configbuilt.boolValue = built;
            serializedObject.ApplyModifiedProperties();

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
    
    
    
    public static async Task<string> SUCCESS(string token, string projectname, string studioname)
    {
        string output = "";
        try
        {
            string actionUrl = "https://gamebuild.io/" + "success";

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
            string actionUrl = "https://gamebuild.io/" + "upload_url";

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
        LoadGamebuildConfig();
    }
    
    public void OnInspectorUpdate()
    {
        Repaint();
    }
    
}

#endif
