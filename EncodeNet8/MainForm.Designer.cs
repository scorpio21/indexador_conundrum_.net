#nullable enable

namespace EncodeNet8;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;
    private MenuStrip menuStrip = null!;
    private ToolStripMenuItem archivoMenu = null!, edicionMenu = null!, herramientasMenu = null!, indexarMenu = null!, superficiesMenu = null!, ayudaMenu = null!, configuracionMenu = null!;
    private Button openButton = null!, saveButton = null!, deleteButton = null!, saveIndexButton = null!, clearMemoryButton = null!, viewButton = null!, encodeMapsButton = null!, newSearchButton = null!, greenButton = null!;
    private Button bodiesButton = null!, headsButton = null!, helmetsButton = null!, shieldsButton = null!, weaponsButton = null!, fxButton = null!;
    private ComboBox formatCombo = null!;
    private Label formatLabel = null!;
    private Label frameLabel = null!;
    private TextBox frameText = null!;
    private ListBox indexList = null!;
    private GroupBox dataGroup = null!, foldersGroup = null!;
    private Panel previewPanel = null!;
    private TextBox grhText = null!, bmpText = null!, framesText = null!, heightText = null!, widthText = null!, speedText = null!, xText = null!, yText = null!, tileHeightText = null!, tileWidthText = null!, indsText = null!, graphicsText = null!, maxGrhText = null!, referencesText = null!;
    private Label referencesLabel = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel statusLabel = null!;

    protected override void Dispose(bool disposing)
    { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        menuStrip = new MenuStrip();
        archivoMenu = new ToolStripMenuItem();
        edicionMenu = new ToolStripMenuItem();
        herramientasMenu = new ToolStripMenuItem();
        configuracionMenu = new ToolStripMenuItem();
        indexarMenu = new ToolStripMenuItem();
        ayudaMenu = new ToolStripMenuItem();
        superficiesMenu = new ToolStripMenuItem();
        openButton = new Button();
        saveButton = new Button();
        deleteButton = new Button();
        saveIndexButton = new Button();
        clearMemoryButton = new Button();
        viewButton = new Button();
        encodeMapsButton = new Button();
        newSearchButton = new Button();
        greenButton = new Button();
        bodiesButton = new Button();
        headsButton = new Button();
        helmetsButton = new Button();
        shieldsButton = new Button();
        weaponsButton = new Button();
        fxButton = new Button();
        formatCombo = new ComboBox();
        formatLabel = new Label();
        frameLabel = new Label();
        frameText = new TextBox();
        indexList = new ListBox();
        dataGroup = new GroupBox();
        grhLabel = new Label();
        bmpLabel = new Label();
        framesLabel = new Label();
        heightLabel = new Label();
        widthLabel = new Label();
        speedLabel = new Label();
        xLabel = new Label();
        yLabel = new Label();
        tileHeightLabel = new Label();
        tileWidthLabel = new Label();
        referencesLabel = new Label();
        grhText = new TextBox();
        bmpText = new TextBox();
        framesText = new TextBox();
        heightText = new TextBox();
        widthText = new TextBox();
        speedText = new TextBox();
        xText = new TextBox();
        yText = new TextBox();
        tileHeightText = new TextBox();
        tileWidthText = new TextBox();
        referencesText = new TextBox();
        foldersGroup = new GroupBox();
        indsLabel = new Label();
        graphicsLabel = new Label();
        maxGrhLabel = new Label();
        indsText = new TextBox();
        graphicsText = new TextBox();
        maxGrhText = new TextBox();
        previewPanel = new Panel();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        menuStrip.SuspendLayout();
        dataGroup.SuspendLayout();
        foldersGroup.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // menuStrip
        // 
        menuStrip.Items.AddRange(new ToolStripItem[] { archivoMenu, edicionMenu, herramientasMenu, indexarMenu, ayudaMenu });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(1142, 24);
        menuStrip.TabIndex = 0;
        // 
        // archivoMenu
        // 
        archivoMenu.Name = "archivoMenu";
        archivoMenu.Size = new Size(60, 20);
        archivoMenu.Text = "Archivo";
        // 
        // edicionMenu
        // 
        edicionMenu.Name = "edicionMenu";
        edicionMenu.Size = new Size(58, 20);
        edicionMenu.Text = "Edición";
        // 
        // herramientasMenu
        // 
        herramientasMenu.DropDownItems.AddRange(new ToolStripItem[] { configuracionMenu });
        herramientasMenu.Name = "herramientasMenu";
        herramientasMenu.Size = new Size(90, 20);
        herramientasMenu.Text = "Herramientas";
        // 
        // configuracionMenu
        // 
        configuracionMenu.Name = "configuracionMenu";
        configuracionMenu.Size = new Size(32, 19);
        configuracionMenu.Text = "Configuración";
        configuracionMenu.Click += configuracionMenu_Click;
        // 
        // indexarMenu
        // 
        indexarMenu.Name = "indexarMenu";
        indexarMenu.Size = new Size(117, 20);
        indexarMenu.Text = "Indexar Superficies";
        // 
        // ayudaMenu
        // 
        ayudaMenu.Name = "ayudaMenu";
        ayudaMenu.Size = new Size(53, 20);
        ayudaMenu.Text = "Ayuda";
        // 
        // superficiesMenu
        // 
        superficiesMenu.Name = "superficiesMenu";
        superficiesMenu.Size = new Size(76, 20);
        superficiesMenu.Text = "Superficies";
        // 
        // openButton
        // 
        openButton.Location = new Point(4, 70);
        openButton.Name = "openButton";
        openButton.Size = new Size(86, 30);
        openButton.TabIndex = 1;
        openButton.Text = "Graficos";
        openButton.Click += graphicsButton_Click;
        // 
        // saveButton
        // 
        saveButton.Location = new Point(224, 419);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(90, 30);
        saveButton.TabIndex = 2;
        saveButton.Text = "Guardar";
        saveButton.Click += saveButton_Click;
        // 
        // deleteButton
        // 
        deleteButton.Location = new Point(367, 422);
        deleteButton.Name = "deleteButton";
        deleteButton.Size = new Size(60, 30);
        deleteButton.TabIndex = 3;
        deleteButton.Text = "borrar";
        deleteButton.Click += deleteButton_Click;
        // 
        // saveIndexButton
        // 
        saveIndexButton.Location = new Point(229, 455);
        saveIndexButton.Name = "saveIndexButton";
        saveIndexButton.Size = new Size(160, 30);
        saveIndexButton.TabIndex = 4;
        saveIndexButton.Text = "Guardar Graficos.ind";
        saveIndexButton.Click += saveIndexButton_Click;
        // 
        // clearMemoryButton
        // 
        clearMemoryButton.Location = new Point(96, 470);
        clearMemoryButton.Name = "clearMemoryButton";
        clearMemoryButton.Size = new Size(120, 30);
        clearMemoryButton.TabIndex = 5;
        clearMemoryButton.Text = "Limpiar Memoria";
        clearMemoryButton.Click += clearMemoryButton_Click;
        // 
        // viewButton
        // 
        viewButton.Location = new Point(6, 404);
        viewButton.Name = "viewButton";
        viewButton.Size = new Size(86, 40);
        viewButton.TabIndex = 6;
        viewButton.Text = "Ver\nGraficos";
        viewButton.Click += viewButton_Click;
        // 
        // encodeMapsButton
        // 
        encodeMapsButton.Location = new Point(6, 460);
        encodeMapsButton.Name = "encodeMapsButton";
        encodeMapsButton.Size = new Size(86, 40);
        encodeMapsButton.TabIndex = 7;
        encodeMapsButton.Text = "Encode\nMapas";
        encodeMapsButton.Click += encodeMapsButton_Click;
        // 
        // newSearchButton
        // 
        newSearchButton.Location = new Point(98, 27);
        newSearchButton.Name = "newSearchButton";
        newSearchButton.Size = new Size(118, 25);
        newSearchButton.TabIndex = 21;
        newSearchButton.Text = "Nuevo/Buscar";
        newSearchButton.Click += newSearchButton_Click;
        // 
        // greenButton
        // 
        greenButton.Location = new Point(367, 491);
        greenButton.Name = "greenButton";
        greenButton.Size = new Size(60, 28);
        greenButton.TabIndex = 22;
        greenButton.Text = "f. verde";
        greenButton.Click += greenButton_Click;
        // 
        // bodiesButton
        // 
        bodiesButton.Location = new Point(4, 105);
        bodiesButton.Name = "bodiesButton";
        bodiesButton.Size = new Size(86, 31);
        bodiesButton.TabIndex = 8;
        bodiesButton.Text = "Bodies";
        bodiesButton.Click += kindButton_Click;
        // 
        // headsButton
        // 
        headsButton.Location = new Point(4, 139);
        headsButton.Name = "headsButton";
        headsButton.Size = new Size(86, 31);
        headsButton.TabIndex = 9;
        headsButton.Text = "Cabezas";
        headsButton.Click += kindButton_Click;
        // 
        // helmetsButton
        // 
        helmetsButton.Location = new Point(4, 173);
        helmetsButton.Name = "helmetsButton";
        helmetsButton.Size = new Size(86, 31);
        helmetsButton.TabIndex = 10;
        helmetsButton.Text = "Cascos";
        helmetsButton.Click += kindButton_Click;
        // 
        // shieldsButton
        // 
        shieldsButton.Location = new Point(4, 207);
        shieldsButton.Name = "shieldsButton";
        shieldsButton.Size = new Size(86, 31);
        shieldsButton.TabIndex = 11;
        shieldsButton.Text = "Escudos";
        shieldsButton.Click += kindButton_Click;
        // 
        // weaponsButton
        // 
        weaponsButton.Location = new Point(4, 241);
        weaponsButton.Name = "weaponsButton";
        weaponsButton.Size = new Size(86, 31);
        weaponsButton.TabIndex = 12;
        weaponsButton.Text = "Armas";
        weaponsButton.Click += kindButton_Click;
        // 
        // fxButton
        // 
        fxButton.Location = new Point(4, 275);
        fxButton.Name = "fxButton";
        fxButton.Size = new Size(86, 31);
        fxButton.TabIndex = 13;
        fxButton.Text = "Fx";
        fxButton.Click += kindButton_Click;
        // 
        // formatCombo
        // 
        formatCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        formatCombo.Items.AddRange(new object[] { "Integer original", "Long para índices" });
        formatCombo.Location = new Point(1211, 34);
        formatCombo.Name = "formatCombo";
        formatCombo.Size = new Size(150, 23);
        formatCombo.TabIndex = 16;
        formatCombo.Visible = false;
        // 
        // formatLabel
        // 
        formatLabel.AutoSize = true;
        formatLabel.Location = new Point(1211, 60);
        formatLabel.Name = "formatLabel";
        formatLabel.Size = new Size(55, 15);
        formatLabel.TabIndex = 15;
        formatLabel.Text = "Formato:";
        formatLabel.Visible = false;
        // 
        // frameLabel
        // 
        frameLabel.AutoSize = true;
        frameLabel.Location = new Point(10, 19);
        frameLabel.Name = "frameLabel";
        frameLabel.Size = new Size(48, 15);
        frameLabel.TabIndex = 23;
        frameLabel.Text = "Frames:";
        // 
        // frameText
        // 
        frameText.Location = new Point(64, 16);
        frameText.Name = "frameText";
        frameText.Size = new Size(135, 23);
        frameText.TabIndex = 24;
        // 
        // indexList
        // 
        indexList.ItemHeight = 15;
        indexList.Location = new Point(98, 58);
        indexList.Name = "indexList";
        indexList.Size = new Size(118, 394);
        indexList.TabIndex = 14;
        indexList.SelectedIndexChanged += indexList_SelectedIndexChanged;
        // 
        // dataGroup
        // 
        dataGroup.Controls.Add(grhLabel);
        dataGroup.Controls.Add(bmpLabel);
        dataGroup.Controls.Add(framesLabel);
        dataGroup.Controls.Add(heightLabel);
        dataGroup.Controls.Add(widthLabel);
        dataGroup.Controls.Add(speedLabel);
        dataGroup.Controls.Add(xLabel);
        dataGroup.Controls.Add(yLabel);
        dataGroup.Controls.Add(tileHeightLabel);
        dataGroup.Controls.Add(tileWidthLabel);
        dataGroup.Controls.Add(referencesLabel);
        dataGroup.Controls.Add(grhText);
        dataGroup.Controls.Add(bmpText);
        dataGroup.Controls.Add(framesText);
        dataGroup.Controls.Add(heightText);
        dataGroup.Controls.Add(widthText);
        dataGroup.Controls.Add(speedText);
        dataGroup.Controls.Add(xText);
        dataGroup.Controls.Add(yText);
        dataGroup.Controls.Add(frameText);
        dataGroup.Controls.Add(frameLabel);
        dataGroup.Controls.Add(tileHeightText);
        dataGroup.Controls.Add(tileWidthText);
        dataGroup.Controls.Add(referencesText);
        dataGroup.FlatStyle = FlatStyle.Flat;
        dataGroup.Location = new Point(222, 28);
        dataGroup.Name = "dataGroup";
        dataGroup.Size = new Size(205, 385);
        dataGroup.TabIndex = 17;
        dataGroup.TabStop = false;
        dataGroup.Text = "Datos del índice elegido";
        // 
        // grhLabel
        // 
        grhLabel.AutoSize = true;
        grhLabel.Location = new Point(15, 50);
        grhLabel.Name = "grhLabel";
        grhLabel.Size = new Size(29, 15);
        grhLabel.TabIndex = 0;
        grhLabel.Text = "Grh:";
        // 
        // bmpLabel
        // 
        bmpLabel.AutoSize = true;
        bmpLabel.Location = new Point(15, 80);
        bmpLabel.Name = "bmpLabel";
        bmpLabel.Size = new Size(82, 15);
        bmpLabel.TabIndex = 1;
        bmpLabel.Text = "Numero BMP:";
        // 
        // framesLabel
        // 
        framesLabel.AutoSize = true;
        framesLabel.Location = new Point(15, 110);
        framesLabel.Name = "framesLabel";
        framesLabel.Size = new Size(95, 15);
        framesLabel.TabIndex = 2;
        framesLabel.Text = "Numero Frames:";
        // 
        // heightLabel
        // 
        heightLabel.AutoSize = true;
        heightLabel.Location = new Point(15, 140);
        heightLabel.Name = "heightLabel";
        heightLabel.Size = new Size(32, 15);
        heightLabel.TabIndex = 3;
        heightLabel.Text = "Alto:";
        // 
        // widthLabel
        // 
        widthLabel.AutoSize = true;
        widthLabel.Location = new Point(17, 266);
        widthLabel.Name = "widthLabel";
        widthLabel.Size = new Size(45, 15);
        widthLabel.TabIndex = 4;
        widthLabel.Text = "Ancho:";
        // 
        // speedLabel
        // 
        speedLabel.AutoSize = true;
        speedLabel.Location = new Point(17, 296);
        speedLabel.Name = "speedLabel";
        speedLabel.Size = new Size(61, 15);
        speedLabel.TabIndex = 5;
        speedLabel.Text = "Velocidad:";
        // 
        // xLabel
        // 
        xLabel.AutoSize = true;
        xLabel.Location = new Point(15, 170);
        xLabel.Name = "xLabel";
        xLabel.Size = new Size(65, 15);
        xLabel.TabIndex = 6;
        xLabel.Text = "Posicion X:";
        // 
        // yLabel
        // 
        yLabel.AutoSize = true;
        yLabel.Location = new Point(15, 200);
        yLabel.Name = "yLabel";
        yLabel.Size = new Size(65, 15);
        yLabel.TabIndex = 7;
        yLabel.Text = "Posicion Y:";
        // 
        // tileHeightLabel
        // 
        tileHeightLabel.AutoSize = true;
        tileHeightLabel.Location = new Point(17, 326);
        tileHeightLabel.Name = "tileHeightLabel";
        tileHeightLabel.Size = new Size(63, 15);
        tileHeightLabel.TabIndex = 8;
        tileHeightLabel.Text = "Alto Titles:";
        // 
        // tileWidthLabel
        // 
        tileWidthLabel.AutoSize = true;
        tileWidthLabel.Location = new Point(17, 356);
        tileWidthLabel.Name = "tileWidthLabel";
        tileWidthLabel.Size = new Size(76, 15);
        tileWidthLabel.TabIndex = 9;
        tileWidthLabel.Text = "Ancho Titles:";
        // 
        // referencesLabel
        // 
        referencesLabel.AutoSize = true;
        referencesLabel.Location = new Point(15, 235);
        referencesLabel.Name = "referencesLabel";
        referencesLabel.Size = new Size(70, 15);
        referencesLabel.TabIndex = 10;
        referencesLabel.Text = "Referencias:";
        // 
        // grhText
        // 
        grhText.Location = new Point(112, 47);
        grhText.Name = "grhText";
        grhText.ReadOnly = true;
        grhText.Size = new Size(72, 23);
        grhText.TabIndex = 11;
        // 
        // bmpText
        // 
        bmpText.Location = new Point(112, 77);
        bmpText.Name = "bmpText";
        bmpText.Size = new Size(72, 23);
        bmpText.TabIndex = 12;
        // 
        // framesText
        // 
        framesText.Location = new Point(112, 107);
        framesText.Name = "framesText";
        framesText.Size = new Size(72, 23);
        framesText.TabIndex = 13;
        // 
        // heightText
        // 
        heightText.Location = new Point(112, 137);
        heightText.Name = "heightText";
        heightText.Size = new Size(55, 23);
        heightText.TabIndex = 14;
        // 
        // widthText
        // 
        widthText.Location = new Point(114, 263);
        widthText.Name = "widthText";
        widthText.Size = new Size(72, 23);
        widthText.TabIndex = 15;
        // 
        // speedText
        // 
        speedText.Location = new Point(114, 293);
        speedText.Name = "speedText";
        speedText.Size = new Size(72, 23);
        speedText.TabIndex = 16;
        // 
        // xText
        // 
        xText.Location = new Point(112, 167);
        xText.Name = "xText";
        xText.Size = new Size(55, 23);
        xText.TabIndex = 17;
        // 
        // yText
        // 
        yText.Location = new Point(112, 197);
        yText.Name = "yText";
        yText.Size = new Size(55, 23);
        yText.TabIndex = 18;
        // 
        // tileHeightText
        // 
        tileHeightText.Location = new Point(114, 323);
        tileHeightText.Name = "tileHeightText";
        tileHeightText.Size = new Size(72, 23);
        tileHeightText.TabIndex = 19;
        // 
        // tileWidthText
        // 
        tileWidthText.Location = new Point(114, 353);
        tileWidthText.Name = "tileWidthText";
        tileWidthText.Size = new Size(72, 23);
        tileWidthText.TabIndex = 20;
        // 
        // referencesText
        // 
        referencesText.Location = new Point(112, 232);
        referencesText.Name = "referencesText";
        referencesText.ReadOnly = true;
        referencesText.Size = new Size(55, 23);
        referencesText.TabIndex = 21;
        // 
        // foldersGroup
        // 
        foldersGroup.Controls.Add(indsLabel);
        foldersGroup.Controls.Add(graphicsLabel);
        foldersGroup.Controls.Add(maxGrhLabel);
        foldersGroup.Controls.Add(indsText);
        foldersGroup.Controls.Add(graphicsText);
        foldersGroup.Controls.Add(maxGrhText);
        foldersGroup.FlatStyle = FlatStyle.Flat;
        foldersGroup.Location = new Point(224, 517);
        foldersGroup.Name = "foldersGroup";
        foldersGroup.Size = new Size(205, 125);
        foldersGroup.TabIndex = 18;
        foldersGroup.TabStop = false;
        foldersGroup.Text = "Carpetas";
        // 
        // indsLabel
        // 
        indsLabel.AutoSize = true;
        indsLabel.Location = new Point(10, 28);
        indsLabel.Name = "indsLabel";
        indsLabel.Size = new Size(32, 15);
        indsLabel.TabIndex = 0;
        indsLabel.Text = "Inds:";
        // 
        // graphicsLabel
        // 
        graphicsLabel.AutoSize = true;
        graphicsLabel.Location = new Point(10, 58);
        graphicsLabel.Name = "graphicsLabel";
        graphicsLabel.Size = new Size(53, 15);
        graphicsLabel.TabIndex = 1;
        graphicsLabel.Text = "Graficos:";
        // 
        // maxGrhLabel
        // 
        maxGrhLabel.AutoSize = true;
        maxGrhLabel.Location = new Point(10, 88);
        maxGrhLabel.Name = "maxGrhLabel";
        maxGrhLabel.Size = new Size(34, 15);
        maxGrhLabel.TabIndex = 2;
        maxGrhLabel.Text = "GRH:";
        // 
        // indsText
        // 
        indsText.Location = new Point(98, 25);
        indsText.Name = "indsText";
        indsText.Size = new Size(90, 23);
        indsText.TabIndex = 3;
        indsText.Text = "init";
        // 
        // graphicsText
        // 
        graphicsText.Location = new Point(98, 55);
        graphicsText.Name = "graphicsText";
        graphicsText.Size = new Size(90, 23);
        graphicsText.TabIndex = 4;
        graphicsText.Text = "graficos";
        // 
        // maxGrhText
        // 
        maxGrhText.ForeColor = Color.Blue;
        maxGrhText.Location = new Point(98, 85);
        maxGrhText.Name = "maxGrhText";
        maxGrhText.ReadOnly = true;
        maxGrhText.Size = new Size(90, 23);
        maxGrhText.TabIndex = 5;
        maxGrhText.Text = "32720";
        // 
        // previewPanel
        // 
        previewPanel.BackColor = Color.WhiteSmoke;
        previewPanel.BorderStyle = BorderStyle.FixedSingle;
        previewPanel.Location = new Point(433, 33);
        previewPanel.Name = "previewPanel";
        previewPanel.Size = new Size(700, 575);
        previewPanel.TabIndex = 19;
        previewPanel.Paint += previewPanel_Paint;
        // 
        // statusStrip
        // 
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 668);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1142, 22);
        statusStrip.TabIndex = 20;
        // 
        // statusLabel
        // 
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(1127, 17);
        statusLabel.Spring = true;
        statusLabel.Text = "Abre un Graficos.ind para comenzar.";
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // MainForm
        // 
        BackColor = Color.FromArgb(192, 255, 192);
        ClientSize = new Size(1142, 690);
        Controls.Add(menuStrip);
        Controls.Add(newSearchButton);
        Controls.Add(openButton);
        Controls.Add(saveButton);
        Controls.Add(deleteButton);
        Controls.Add(saveIndexButton);
        Controls.Add(clearMemoryButton);
        Controls.Add(viewButton);
        Controls.Add(encodeMapsButton);
        Controls.Add(greenButton);
        Controls.Add(bodiesButton);
        Controls.Add(headsButton);
        Controls.Add(helmetsButton);
        Controls.Add(shieldsButton);
        Controls.Add(weaponsButton);
        Controls.Add(fxButton);
        Controls.Add(indexList);
        Controls.Add(formatLabel);
        Controls.Add(formatCombo);
        Controls.Add(dataGroup);
        Controls.Add(foldersGroup);
        Controls.Add(previewPanel);
        Controls.Add(statusStrip);
        MainMenuStrip = menuStrip;
        MinimumSize = new Size(1000, 600);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Ver Graficos";
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        dataGroup.ResumeLayout(false);
        dataGroup.PerformLayout();
        foldersGroup.ResumeLayout(false);
        foldersGroup.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label grhLabel = null!;
    private Label bmpLabel = null!;
    private Label framesLabel = null!;
    private Label heightLabel = null!;
    private Label widthLabel = null!;
    private Label speedLabel = null!;
    private Label xLabel = null!;
    private Label yLabel = null!;
    private Label tileHeightLabel = null!;
    private Label tileWidthLabel = null!;
    private Label indsLabel = null!;
    private Label graphicsLabel = null!;
    private Label maxGrhLabel = null!;
}
