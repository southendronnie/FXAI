using Microsoft.JSInterop;

public class HighchartsInterop
{
    private readonly IJSRuntime _js;

    public HighchartsInterop(IJSRuntime js) => _js = js;

    public async Task RenderChartAsync(string containerId, object chartOptions)
    {
        await _js.InvokeVoidAsync("renderHighchart", containerId, chartOptions);
    }
}
