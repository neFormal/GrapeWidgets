using System.Threading.Tasks;
using UnityEngine;

[ExecuteInEditMode]
public class Blur : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private int passes = 5;
    
    [SerializeField]
    [Range(0, 5000)]
    private int passesDelayMs = 200;

    private RenderTexture targetTexture;
    private RenderTexture tempTexture;

    public async Task BlurIt(RenderTexture texture)
    {
        targetTexture = texture;
        tempTexture = new RenderTexture(targetTexture);

        await Process(passes);
    }

    public async Task BlurIt(RenderTexture texture, RenderTexture temp)
    {
        targetTexture = texture;
        tempTexture = temp;

        await Process(passes);
    }

    private async Task Process(int passes)
    {
        for (int i = 0; i < passes; i++)
        {
            tempTexture.DiscardContents();
            Graphics.Blit(targetTexture, tempTexture, material);
            targetTexture.DiscardContents();
            Graphics.Blit(tempTexture, targetTexture);

            await Task.Delay(passesDelayMs);
        }
        tempTexture.DiscardContents();
        tempTexture = null;
    }
}
