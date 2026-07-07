using Godot;

public static class TooltipHelper
{
    private const string TooltipNodeName = "TooltipLabel";

    public static void ShowTooltip(Control parent, string text)
    {
        var tooltip = parent.GetNodeOrNull<Label>(TooltipNodeName);
        if (tooltip == null)
        {
            tooltip = new Label();
            tooltip.Name = TooltipNodeName;
            tooltip.Visible = false;
            tooltip.MouseFilter = Control.MouseFilterEnum.Ignore;
            tooltip.Modulate = new Color(0.95f, 0.95f, 0.85f);
            tooltip.AutowrapMode = TextServer.AutowrapMode.Word;
            tooltip.MaxLinesVisible = 6;
            tooltip.CustomMinimumSize = new Vector2(60, 14);
            var bg = new StyleBoxFlat();
            bg.BgColor = new Color(0.05f, 0.05f, 0.08f, 0.92f);
            bg.BorderColor = new Color(0.6f, 0.6f, 0.6f);
            bg.BorderWidthLeft = 1;
            bg.BorderWidthRight = 1;
            bg.BorderWidthTop = 1;
            bg.BorderWidthBottom = 1;
            bg.ContentMarginLeft = 4;
            bg.ContentMarginRight = 4;
            bg.ContentMarginTop = 2;
            bg.ContentMarginBottom = 2;
            tooltip.AddThemeStyleboxOverride("normal", bg);
            parent.AddChild(tooltip);
        }
        var viewport = parent.GetViewport();
        if (viewport == null) return;
        float mouseX = viewport.GetMousePosition().X;
        float mouseY = viewport.GetMousePosition().Y;
        tooltip.OffsetLeft = Mathf.Clamp(mouseX + 8, 2, 300);
        tooltip.OffsetTop = Mathf.Clamp(mouseY - tooltip.Size.Y - 4, 2, 170);
        tooltip.Text = text;
        tooltip.Show();
    }

    public static void HideTooltip(Control parent)
    {
        var tooltip = parent.GetNodeOrNull<Label>(TooltipNodeName);
        if (tooltip != null)
            tooltip.Hide();
    }
}
