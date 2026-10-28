#nullable enable

namespace EncodeNet8;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;
    private MenuStrip menuStrip = null!;
    private ToolStripMenuItem archivoMenu = null!, edicionMenu = null!, herramientasMenu = null!, indexarMenu = null!, superficiesMenu = null!, ayudaMenu = null!, configuracionMenu = null!;
    private ToolStripMenuItem guardarGraficosMenu = null!, guardarMenu = null!, cargarGraficosMenu = null!, cargarMenu = null!, indiceGuardadoMenu = null!, salirMenu = null!;
    private ToolStripMenuItem slot0Menu = null!, slot1Menu = null!, slot2Menu = null!, slot3Menu = null!, slot4Menu = null!, slot5Menu = null!, slot6Menu = null!, slot7Menu = null!, slot8Menu = null!, slot9Menu = null!;
    private ToolStripMenuItem nuevoIrMenu = null!, moverMenu = null!, copiarMenu = null!, borrarMenu = null!, clonarMenu = null!, colorFondoMenu = null!;
    private ToolStripMenuItem buscarGrhMenu = null!, buscarBmpsMenu = null!, buscarSiguienteMenu = null!, autoindexadorMenu = null!, repetidosMenu = null!, generarWeMenu = null!, acercaMenu = null!;
    private ToolStripSeparator archivoSeparator1 = null!, archivoSeparator2 = null!;
    private Button openButton = null!, saveButton = null!, deleteButton = null!, saveIndexButton = null!, clearMemoryButton = null!, viewButton = null!, encodeMapsButton = null!, newSearchButton = null!, greenButton = null!;
    private Button bodiesButton = null!, headsButton = null!, helmetsButton = null!, shieldsButton = null!, weaponsButton = null!, alasButton = null!, bootsButton = null!, fxButton = null!;
    private ComboBox formatCombo = null!;
    private Label formatLabel = null!;
    private Label frameLabel = null!;
    private TextBox frameText = null!;
    private ListBox indexList = null!;
    private GroupBox dataGroup = null!, foldersGroup = null!;
    private TextBox grhText = null!, bmpText = null!, framesText = null!, heightText = null!, widthText = null!, speedText = null!, xText = null!, yText = null!, tileHeightText = null!, tileWidthText = null!, indsText = null!, graphicsText = null!, maxGrhText = null!;
    private ComboBox directionCombo = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel statusLabel = null!;
    private Label LUlitError = null!;

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
        guardarGraficosMenu = new ToolStripMenuItem();
        guardarMenu = new ToolStripMenuItem();
        cargarGraficosMenu = new ToolStripMenuItem();
        cargarMenu = new ToolStripMenuItem();
        indiceGuardadoMenu = new ToolStripMenuItem();
        salirMenu = new ToolStripMenuItem();
        slot0Menu = new ToolStripMenuItem();
        slot1Menu = new ToolStripMenuItem();
        slot2Menu = new ToolStripMenuItem();
        slot3Menu = new ToolStripMenuItem();
        slot4Menu = new ToolStripMenuItem();
        slot5Menu = new ToolStripMenuItem();
        slot6Menu = new ToolStripMenuItem();
        slot7Menu = new ToolStripMenuItem();
        slot8Menu = new ToolStripMenuItem();
        slot9Menu = new ToolStripMenuItem();
        nuevoIrMenu = new ToolStripMenuItem();
        moverMenu = new ToolStripMenuItem();
        copiarMenu = new ToolStripMenuItem();
        borrarMenu = new ToolStripMenuItem();
        clonarMenu = new ToolStripMenuItem();
        colorFondoMenu = new ToolStripMenuItem();
        buscarGrhMenu = new ToolStripMenuItem();
        buscarBmpsMenu = new ToolStripMenuItem();
        buscarSiguienteMenu = new ToolStripMenuItem();
        autoindexadorMenu = new ToolStripMenuItem();
        repetidosMenu = new ToolStripMenuItem();
        generarWeMenu = new ToolStripMenuItem();
        acercaMenu = new ToolStripMenuItem();
        archivoSeparator1 = new ToolStripSeparator();
        archivoSeparator2 = new ToolStripSeparator();
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
        alasButton = new Button();
        bootsButton = new Button();
        fxButton = new Button();
        formatCombo = new ComboBox();
        formatLabel = new Label();
        indexList = new ListBox();
        dataGroup = new GroupBox();
        xLabel = new Label();
        yLabel = new Label();
        grhLabel = new Label();
        xText = new TextBox();
        bmpLabel = new Label();
        yText = new TextBox();
        framesLabel = new Label();
        heightLabel = new Label();
        widthLabel = new Label();
        speedLabel = new Label();
        tileHeightLabel = new Label();
        tileWidthLabel = new Label();
        grhText = new TextBox();
        bmpText = new TextBox();
        framesText = new TextBox();
        heightText = new TextBox();
        widthText = new TextBox();
        speedText = new TextBox();
        tileHeightText = new TextBox();
        tileWidthText = new TextBox();
        DescripcionAyuda = new Label();
        foldersGroup = new GroupBox();
        indsLabel = new Label();
        graphicsLabel = new Label();
        maxGrhLabel = new Label();
        indsText = new TextBox();
        graphicsText = new TextBox();
        maxGrhText = new TextBox();
        previewPanel = new BufferedPreviewPanel();
        LUlitError = new Label();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        usageStatusLabel = new ToolStripStatusLabel();
        frameText = new TextBox();
        frameLabel = new Label();
        directionCombo = new ComboBox();
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
        archivoMenu.DropDownItems.AddRange(new ToolStripItem[] { guardarGraficosMenu, guardarMenu, cargarGraficosMenu, cargarMenu, archivoSeparator1, indiceGuardadoMenu, archivoSeparator2, salirMenu });
        archivoMenu.Name = "archivoMenu";
        archivoMenu.Size = new Size(60, 20);
        archivoMenu.Text = "Archivo";
        // 
        // guardarGraficosMenu
        // 
        guardarGraficosMenu.Name = "guardarGraficosMenu";
        guardarGraficosMenu.ShortcutKeys = Keys.Control | Keys.S;
        guardarGraficosMenu.Text = "Guardar Graficos.ind";
        guardarGraficosMenu.Click += saveButton_Click;
        // 
        // guardarMenu
        // 
        guardarMenu.Name = "guardarMenu";
        guardarMenu.Text = "Guardar...";
        guardarMenu.Click += saveIndexButton_Click;
        // 
        // cargarGraficosMenu
        // 
        cargarGraficosMenu.Name = "cargarGraficosMenu";
        cargarGraficosMenu.ShortcutKeys = Keys.Control | Keys.O;
        cargarGraficosMenu.Text = "Cargar Graficos.ind";
        cargarGraficosMenu.Click += openButton_Click;
        // 
        // cargarMenu
        // 
        cargarMenu.Name = "cargarMenu";
        cargarMenu.Text = "Cargar...";
        cargarMenu.Click += openButton_Click;
        // 
        // indiceGuardadoMenu
        // 
        indiceGuardadoMenu.DropDownItems.AddRange(new ToolStripItem[] { slot0Menu, slot1Menu, slot2Menu, slot3Menu, slot4Menu, slot5Menu, slot6Menu, slot7Menu, slot8Menu, slot9Menu });
        indiceGuardadoMenu.Name = "indiceGuardadoMenu";
        indiceGuardadoMenu.Text = "Indice de guardado";
        // 
        // saved index slots
        // 
        slot0Menu.Name = "slot0Menu"; slot0Menu.Tag = 0; slot0Menu.Text = "Sobreescribir"; slot0Menu.Click += savedIndexMenu_Click;
        slot1Menu.Name = "slot1Menu"; slot1Menu.Tag = 1; slot1Menu.Text = "1"; slot1Menu.Click += savedIndexMenu_Click;
        slot2Menu.Name = "slot2Menu"; slot2Menu.Tag = 2; slot2Menu.Text = "2"; slot2Menu.Click += savedIndexMenu_Click;
        slot3Menu.Name = "slot3Menu"; slot3Menu.Tag = 3; slot3Menu.Text = "3"; slot3Menu.Click += savedIndexMenu_Click;
        slot4Menu.Name = "slot4Menu"; slot4Menu.Tag = 4; slot4Menu.Text = "4"; slot4Menu.Click += savedIndexMenu_Click;
        slot5Menu.Name = "slot5Menu"; slot5Menu.Tag = 5; slot5Menu.Text = "5"; slot5Menu.Click += savedIndexMenu_Click;
        slot6Menu.Name = "slot6Menu"; slot6Menu.Tag = 6; slot6Menu.Text = "6"; slot6Menu.Click += savedIndexMenu_Click;
        slot7Menu.Name = "slot7Menu"; slot7Menu.Tag = 7; slot7Menu.Text = "7"; slot7Menu.Click += savedIndexMenu_Click;
        slot8Menu.Name = "slot8Menu"; slot8Menu.Tag = 8; slot8Menu.Text = "8"; slot8Menu.Click += savedIndexMenu_Click;
        slot9Menu.Name = "slot9Menu"; slot9Menu.Tag = 9; slot9Menu.Text = "9"; slot9Menu.Click += savedIndexMenu_Click;
        // 
        // salirMenu
        // 
        salirMenu.Name = "salirMenu";
        salirMenu.Text = "Salir";
        salirMenu.Click += salirMenu_Click;
        // 
        // edicionMenu
        // 
        edicionMenu.DropDownItems.AddRange(new ToolStripItem[] { nuevoIrMenu, moverMenu, copiarMenu, borrarMenu, clonarMenu, colorFondoMenu });
        edicionMenu.Name = "edicionMenu";
        edicionMenu.Size = new Size(58, 20);
        edicionMenu.Text = "Edición";
        // 
        nuevoIrMenu.Name = "nuevoIrMenu"; nuevoIrMenu.ShortcutKeys = Keys.Control | Keys.F; nuevoIrMenu.Text = "Nuevo/Ir A"; nuevoIrMenu.Click += newSearchButton_Click;
        moverMenu.Name = "moverMenu"; moverMenu.Text = "Mover"; moverMenu.Click += moveMenu_Click;
        copiarMenu.Name = "copiarMenu"; copiarMenu.Text = "Copiar"; copiarMenu.Click += copyMenu_Click;
        borrarMenu.Name = "borrarMenu"; borrarMenu.Text = "Borrar"; borrarMenu.ShortcutKeys = Keys.Delete; borrarMenu.Click += deleteButton_Click;
        clonarMenu.Name = "clonarMenu"; clonarMenu.Text = "Clonar..."; clonarMenu.Click += cloneMenu_Click;
        colorFondoMenu.Name = "colorFondoMenu"; colorFondoMenu.Text = "Color de fondo..."; colorFondoMenu.Click += colorMenu_Click;
        // 
        // herramientasMenu
        // 
        herramientasMenu.DropDownItems.AddRange(new ToolStripItem[] { buscarGrhMenu, buscarBmpsMenu, buscarSiguienteMenu, autoindexadorMenu, repetidosMenu, configuracionMenu });
        herramientasMenu.Name = "herramientasMenu";
        herramientasMenu.Size = new Size(90, 20);
        herramientasMenu.Text = "Herramientas";
        // 
        // configuracionMenu
        // 
        configuracionMenu.Name = "configuracionMenu";
        configuracionMenu.Size = new Size(150, 22);
        configuracionMenu.Text = "Configuración";
        configuracionMenu.Click += configuracionMenu_Click;
        buscarGrhMenu.Name = "buscarGrhMenu"; buscarGrhMenu.Text = "Buscar Grh con bmp..."; buscarGrhMenu.Click += buscarGrhMenu_Click;
        buscarBmpsMenu.Name = "buscarBmpsMenu"; buscarBmpsMenu.Text = "Buscar Bmps sin indexar (Cry)"; buscarBmpsMenu.Click += buscarBmpsMenu_Click;
        buscarSiguienteMenu.Name = "buscarSiguienteMenu"; buscarSiguienteMenu.ShortcutKeys = Keys.F3; buscarSiguienteMenu.Text = "Buscar siguiente"; buscarSiguienteMenu.Click += buscarSiguienteMenu_Click;
        autoindexadorMenu.Name = "autoindexadorMenu"; autoindexadorMenu.Text = "Autoindexador"; autoindexadorMenu.Click += autoindexadorMenu_Click;
        repetidosMenu.Name = "repetidosMenu"; repetidosMenu.Text = "Buscar Grh Repetidos"; repetidosMenu.Click += repetidosMenu_Click;
        // 
        // indexarMenu
        // 
        indexarMenu.DropDownItems.Add(generarWeMenu);
        indexarMenu.Name = "indexarMenu";
        indexarMenu.Size = new Size(117, 20);
        indexarMenu.Text = "Indexar Superficies";
        generarWeMenu.Name = "generarWeMenu"; generarWeMenu.Text = "Generar Indices Para WE"; generarWeMenu.Click += generarWeMenu_Click;
        // 
        // ayudaMenu
        // 
        ayudaMenu.DropDownItems.Add(acercaMenu);
        ayudaMenu.Name = "ayudaMenu";
        ayudaMenu.Size = new Size(53, 20);
        ayudaMenu.Text = "Ayuda";
        acercaMenu.Name = "acercaMenu"; acercaMenu.Text = "Acerca de..."; acercaMenu.Click += acercaMenu_Click;
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
        saveIndexButton.Size = new Size(125, 30);
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
        greenButton.Location = new Point(367, 456);
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
        // alasButton
        // 
        alasButton.Location = new Point(4, 275);
        alasButton.Name = "alasButton";
        alasButton.Size = new Size(86, 31);
        alasButton.TabIndex = 13;
        alasButton.Text = "Alas";
        alasButton.Click += kindButton_Click;
        // 
        // bootsButton
        // 
        bootsButton.Location = new Point(4, 309);
        bootsButton.Name = "bootsButton";
        bootsButton.Size = new Size(86, 31);
        bootsButton.TabIndex = 14;
        bootsButton.Text = "Botas";
        bootsButton.Click += kindButton_Click;
        // 
        // fxButton
        // 
        fxButton.Location = new Point(4, 343);
        fxButton.Name = "fxButton";
        fxButton.Size = new Size(86, 31);
        fxButton.TabIndex = 15;
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
        dataGroup.Controls.Add(xLabel);
        dataGroup.Controls.Add(yLabel);
        dataGroup.Controls.Add(grhLabel);
        dataGroup.Controls.Add(xText);
        dataGroup.Controls.Add(bmpLabel);
        dataGroup.Controls.Add(yText);
        dataGroup.Controls.Add(framesLabel);
        dataGroup.Controls.Add(heightLabel);
        dataGroup.Controls.Add(widthLabel);
        dataGroup.Controls.Add(speedLabel);
        dataGroup.Controls.Add(tileHeightLabel);
        dataGroup.Controls.Add(tileWidthLabel);
        dataGroup.Controls.Add(grhText);
        dataGroup.Controls.Add(bmpText);
        dataGroup.Controls.Add(framesText);
        dataGroup.Controls.Add(heightText);
        dataGroup.Controls.Add(widthText);
        dataGroup.Controls.Add(speedText);
        dataGroup.Controls.Add(tileHeightText);
        dataGroup.Controls.Add(tileWidthText);
        dataGroup.Controls.Add(DescripcionAyuda);
        dataGroup.FlatStyle = FlatStyle.Flat;
        dataGroup.Location = new Point(222, 58);
        dataGroup.Name = "dataGroup";
        dataGroup.Size = new Size(252, 355);
        dataGroup.TabIndex = 17;
        dataGroup.TabStop = false;
        dataGroup.Text = "Datos del índice elegido";
        // 
        // xLabel
        // 
        xLabel.AutoSize = true;
        xLabel.Location = new Point(19, 202);
        xLabel.Name = "xLabel";
        xLabel.Size = new Size(65, 15);
        xLabel.TabIndex = 28;
        xLabel.Text = "Posicion X:";
        // 
        // yLabel
        // 
        yLabel.AutoSize = true;
        yLabel.Location = new Point(19, 232);
        yLabel.Name = "yLabel";
        yLabel.Size = new Size(65, 15);
        yLabel.TabIndex = 29;
        yLabel.Text = "Posicion Y:";
        // 
        // grhLabel
        // 
        grhLabel.AutoSize = true;
        grhLabel.Location = new Point(19, 25);
        grhLabel.Name = "grhLabel";
        grhLabel.Size = new Size(29, 15);
        grhLabel.TabIndex = 0;
        grhLabel.Text = "Grh:";
        // 
        // xText
        // 
        xText.Location = new Point(116, 199);
        xText.Name = "xText";
        xText.Size = new Size(72, 23);
        xText.TabIndex = 30;
        // 
        // bmpLabel
        // 
        bmpLabel.AutoSize = true;
        bmpLabel.Location = new Point(19, 55);
        bmpLabel.Name = "bmpLabel";
        bmpLabel.Size = new Size(82, 15);
        bmpLabel.TabIndex = 1;
        bmpLabel.Text = "Numero BMP:";
        // 
        // yText
        // 
        yText.Location = new Point(116, 229);
        yText.Name = "yText";
        yText.Size = new Size(72, 23);
        yText.TabIndex = 31;
        // 
        // framesLabel
        // 
        framesLabel.AutoSize = true;
        framesLabel.Location = new Point(19, 85);
        framesLabel.Name = "framesLabel";
        framesLabel.Size = new Size(95, 15);
        framesLabel.TabIndex = 2;
        framesLabel.Text = "Numero Frames:";
        // 
        // heightLabel
        // 
        heightLabel.AutoSize = true;
        heightLabel.Location = new Point(19, 115);
        heightLabel.Name = "heightLabel";
        heightLabel.Size = new Size(32, 15);
        heightLabel.TabIndex = 3;
        heightLabel.Text = "Alto:";
        // 
        // widthLabel
        // 
        widthLabel.AutoSize = true;
        widthLabel.Location = new Point(19, 144);
        widthLabel.Name = "widthLabel";
        widthLabel.Size = new Size(45, 15);
        widthLabel.TabIndex = 4;
        widthLabel.Text = "Ancho:";
        // 
        // speedLabel
        // 
        speedLabel.AutoSize = true;
        speedLabel.Location = new Point(19, 173);
        speedLabel.Name = "speedLabel";
        speedLabel.Size = new Size(61, 15);
        speedLabel.TabIndex = 5;
        speedLabel.Text = "Velocidad:";
        // 
        // tileHeightLabel
        // 
        tileHeightLabel.AutoSize = true;
        tileHeightLabel.Location = new Point(19, 264);
        tileHeightLabel.Name = "tileHeightLabel";
        tileHeightLabel.Size = new Size(63, 15);
        tileHeightLabel.TabIndex = 8;
        tileHeightLabel.Text = "Alto Titles:";
        // 
        // tileWidthLabel
        // 
        tileWidthLabel.AutoSize = true;
        tileWidthLabel.Location = new Point(19, 294);
        tileWidthLabel.Name = "tileWidthLabel";
        tileWidthLabel.Size = new Size(76, 15);
        tileWidthLabel.TabIndex = 9;
        tileWidthLabel.Text = "Ancho Titles:";
        // 
        // grhText
        // 
        grhText.Location = new Point(116, 22);
        grhText.Name = "grhText";
        grhText.ReadOnly = true;
        grhText.Size = new Size(72, 23);
        grhText.TabIndex = 11;
        // 
        // bmpText
        // 
        bmpText.Location = new Point(116, 52);
        bmpText.Name = "bmpText";
        bmpText.Size = new Size(72, 23);
        bmpText.TabIndex = 12;
        // 
        // framesText
        // 
        framesText.Location = new Point(116, 82);
        framesText.Name = "framesText";
        framesText.Size = new Size(72, 23);
        framesText.TabIndex = 13;
        // 
        // heightText
        // 
        heightText.Location = new Point(116, 112);
        heightText.Name = "heightText";
        heightText.Size = new Size(72, 23);
        heightText.TabIndex = 14;
        // 
        // widthText
        // 
        widthText.Location = new Point(116, 141);
        widthText.Name = "widthText";
        widthText.Size = new Size(72, 23);
        widthText.TabIndex = 15;
        // 
        // speedText
        // 
        speedText.Location = new Point(116, 170);
        speedText.Name = "speedText";
        speedText.Size = new Size(72, 23);
        speedText.TabIndex = 16;
        // 
        // tileHeightText
        // 
        tileHeightText.Location = new Point(116, 261);
        tileHeightText.Name = "tileHeightText";
        tileHeightText.Size = new Size(72, 23);
        tileHeightText.TabIndex = 19;
        // 
        // tileWidthText
        // 
        tileWidthText.Location = new Point(116, 291);
        tileWidthText.Name = "tileWidthText";
        tileWidthText.Size = new Size(72, 23);
        tileWidthText.TabIndex = 20;
        // 
        // DescripcionAyuda
        // 
        DescripcionAyuda.Location = new Point(19, 229);
        DescripcionAyuda.Name = "DescripcionAyuda";
        DescripcionAyuda.Size = new Size(220, 45);
        DescripcionAyuda.TabIndex = 26;
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
        foldersGroup.Location = new Point(222, 483);
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
        previewPanel.Location = new Point(480, 58);
        previewPanel.Name = "previewPanel";
        previewPanel.Size = new Size(653, 550);
        previewPanel.TabIndex = 19;
        previewPanel.Paint += previewPanel_Paint;
        // 
        // LUlitError
        // 
        LUlitError.BackColor = SystemColors.Control;
        LUlitError.BorderStyle = BorderStyle.FixedSingle;
        LUlitError.ForeColor = Color.DarkRed;
        LUlitError.Location = new Point(2, 610);
        LUlitError.Name = "LUlitError";
        LUlitError.Size = new Size(1130, 58);
        LUlitError.TabIndex = 20;
        // 
        // statusStrip
        // 
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, usageStatusLabel });
        statusStrip.Location = new Point(0, 668);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1142, 22);
        statusStrip.TabIndex = 21;
        // 
        // statusLabel
        // 
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(955, 17);
        statusLabel.Spring = true;
        statusLabel.Text = "Abre un Graficos.ind para comenzar.";
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // usageStatusLabel
        // 
        usageStatusLabel.Name = "usageStatusLabel";
        usageStatusLabel.Size = new Size(172, 17);
        usageStatusLabel.Text = "Veces que el GRH es utilizado: 0";
        // 
        // frameText
        // 
        frameText.Location = new Point(295, 28);
        frameText.Name = "frameText";
        frameText.Size = new Size(820, 23);
        frameText.TabIndex = 25;
        // 
        // frameLabel
        // 
        frameLabel.AutoSize = true;
        frameLabel.Location = new Point(241, 32);
        frameLabel.Name = "frameLabel";
        frameLabel.Size = new Size(48, 15);
        frameLabel.TabIndex = 26;
        frameLabel.Text = "Frames:";
        // 
        // directionCombo
        // 
        directionCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        directionCombo.FormattingEnabled = true;
        directionCombo.Items.AddRange(new object[] { "Subir", "Derecha", "Abajo", "Izquierda" });
        directionCombo.Location = new Point(321, 28);
        directionCombo.Name = "directionCombo";
        directionCombo.Size = new Size(89, 23);
        directionCombo.TabIndex = 27;
        directionCombo.Visible = false;
        // 
        // MainForm
        // 
        BackColor = Color.FromArgb(192, 255, 192);
        ClientSize = new Size(1142, 690);
        Controls.Add(directionCombo);
        Controls.Add(frameLabel);
        Controls.Add(frameText);
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
        Controls.Add(alasButton);
        Controls.Add(bootsButton);
        Controls.Add(fxButton);
        Controls.Add(indexList);
        Controls.Add(formatLabel);
        Controls.Add(formatCombo);
        Controls.Add(dataGroup);
        Controls.Add(foldersGroup);
        Controls.Add(previewPanel);
        Controls.Add(LUlitError);
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
    private Label tileHeightLabel = null!;
    private Label tileWidthLabel = null!;
    private Label indsLabel = null!;
    private Label graphicsLabel = null!;
    private Label maxGrhLabel = null!;
    private BufferedPreviewPanel previewPanel = null!;
    private Label DescripcionAyuda;
    private ToolStripStatusLabel usageStatusLabel;
    private Label xLabel;
    private Label yLabel;
}
