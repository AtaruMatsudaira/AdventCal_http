using System.Diagnostics;
using System.Net.Http;
using UnityEngine;
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.Networking;
using VRM;

public class HttpCompareSample : MonoBehaviour
{
    [SerializeField] private Transform unityWebRequestModelRoot;
    [SerializeField] private Transform yetAnotherHttpModelRoot;

    [SerializeField] private TMP_Text unityWebRequestLabel;
    [SerializeField] private TMP_Text yetAnotherHttpHandlerLabel;

    private const string URI = "Your local vrm url";

    private void Start()
    {
        RunAsync().Forget();
    }

    private async UniTaskVoid RunAsync()
    {
        var sw = new Stopwatch();

        #region UnityWebRequest

        sw.Start();
        var uwrBin = await GetUnityWebAsync();
        sw.Stop();

        GenerateModelAsync(uwrBin, unityWebRequestModelRoot).Forget();
        unityWebRequestLabel.SetText($"Elapsed:{sw.ElapsedMilliseconds}");

        #endregion

        #region YetAnotherHttpHandler

        sw.Restart();
        var yahBin = await GetYetAnotherAsync();
        sw.Stop();

        GenerateModelAsync(yahBin, yetAnotherHttpModelRoot).Forget();
        yetAnotherHttpHandlerLabel.SetText($"Elapsed:{sw.ElapsedMilliseconds}");

        #endregion
    }

    private async UniTask<byte[]> GetUnityWebAsync()
    {
        var client = UnityWebRequest.Get(URI);
        await client.SendWebRequest();

        var bytes = client.downloadHandler.data;

        return bytes;
    }

    private async UniTask<byte[]> GetYetAnotherAsync()
    {
        using var handler = new YetAnotherHttpHandler();
        var httpClient = new HttpClient(handler);

        var bytes = await httpClient.GetByteArrayAsync(URI);

        return bytes;
    }

    private async UniTaskVoid GenerateModelAsync(byte[] bytes, Transform root)
    {
        var model = await VrmUtility.LoadBytesAsync(root.name, bytes);
        model.ShowMeshes();
        model.Root.transform.parent = root;
        model.Root.transform.localPosition = Vector3.zero;
        model.Root.transform.localRotation = Quaternion.Euler(0,180,0);
    }
}