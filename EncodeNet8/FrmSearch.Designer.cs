namespace EncodeNet8;

partial class FrmSearch
{
    private System.ComponentModel.IContainer components = null!;
    private ListBox resultsList = null!;
    private Label progressLabel = null!;
    private Button closeButton = null!;
    private ContextMenuStrip optionsMenu = null!;
    private ToolStripMenuItem indexAsMenu = null!;
    private ToolStripMenuItem indexAsBodyMenu = null!;
    private ToolStripMenuItem indexAsAnimationMenu = null!;
    private ToolStripMenuItem indexAsGraphicMenu = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        resultsList = new ListBox();
        progressLabel = new Label();
        closeButton = new Button();
        optionsMenu = new ContextMenuStrip(components);
        indexAsMenu = new ToolStripMenuItem();
        indexAsBodyMenu = new ToolStripMenuItem();
        indexAsAnimationMenu = new ToolStripMenuItem();
        indexAsGraphicMenu = new ToolStripMenuItem();
        optionsMenu.SuspendLayout();
        SuspendLayout();

        resultsList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        resultsList.ContextMenuStrip = optionsMenu;
        resultsList.FormattingEnabled = true;
        resultsList.HorizontalScrollbar = true;
        resultsList.Location = new Point(16, 16);
        resultsList.Name = "resultsList";
        resultsList.Size = new Size(698, 342);
        resultsList.TabIndex = 0;
        resultsList.SelectedIndexChanged += resultsList_SelectedIndexChanged;
        resultsList.DoubleClick += resultsList_DoubleClick;

        progressLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        progressLabel.AutoEllipsis = true;
        progressLabel.Location = new Point(16, 373);
        progressLabel.Name = "progressLabel";
        progressLabel.Size = new Size(698, 24);
        progressLabel.TextAlign = ContentAlignment.MiddleCenter;

        closeButton.Anchor = AnchorStyles.Bottom;
        closeButton.DialogResult = DialogResult.Cancel;
        closeButton.Location = new Point(278, 410);
        closeButton.Name = "closeButton";
        closeButton.Size = new Size(175, 30);
        closeButton.Text = "Salir";
        closeButton.UseVisualStyleBackColor = true;

        indexAsMenu.Text = "Indexar como...";
        indexAsMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            indexAsBodyMenu, indexAsAnimationMenu, indexAsGraphicMenu
        });
        indexAsBodyMenu.Text = "Como body";
        indexAsAnimationMenu.Text = "Como animación";
        indexAsGraphicMenu.Text = "Como gráfico independiente";
        optionsMenu.Items.Add(indexAsMenu);
        optionsMenu.Name = "optionsMenu";
        optionsMenu.Size = new Size(220, 26);
        indexAsBodyMenu.Click += indexAsBodyMenu_Click;
        indexAsAnimationMenu.Click += indexAsAnimationMenu_Click;
        indexAsGraphicMenu.Click += indexAsGraphicMenu_Click;
        optionsMenu.Opening += optionsMenu_Opening;

        AcceptButton = closeButton;
        CancelButton = closeButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(730, 458);
        Controls.Add(resultsList);
        Controls.Add(progressLabel);
        Controls.Add(closeButton);
        MinimumSize = new Size(500, 300);
        Name = "FrmSearch";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Búsqueda";
        FormClosing += FrmSearch_FormClosing;
        optionsMenu.ResumeLayout(false);
        ResumeLayout(false);
    }
}
