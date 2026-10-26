#nullable enable

namespace EncodeNet8;

partial class AutoIndexerForm
{
    private System.ComponentModel.IContainer? components = null;
    private Button normalButton = null!;
    private Button advancedButton = null!;
    private Button simpleButton = null!;
    private GroupBox normalFrame = null!;
    private GroupBox advancedFrame = null!;
    private GroupBox simpleFrame = null!;
    private TextBox bitmapText = null!;
    private TextBox startGrh = null!;
    private TextBox columns = null!;
    private TextBox rows = null!;
    private TextBox cellWidth = null!;
    private TextBox cellHeight = null!;
    private TextBox speed = null!;
    private TextBox normalFrames = null!;
    private TextBox normalFirstIndex = null!;
    private TextBox normalHeight = null!;
    private TextBox normalWidth = null!;
    private TextBox normalBmp = null!;
    private TextBox normalColumns = null!;
    private TextBox normalRows = null!;
    private TextBox normalTotal = null!;
    private TextBox advancedFrames = null!;
    private TextBox advancedFirstIndex = null!;
    private TextBox advancedHeight = null!;
    private TextBox advancedWidth = null!;
    private TextBox advancedBmp = null!;
    private TextBox advancedColumns = null!;
    private TextBox advancedRows = null!;
    private TextBox advancedTotal = null!;
    private TextBox simpleBmp = null!;
    private TextBox simpleGrh = null!;
    private CheckBox createAnimation = null!;
    private CheckBox autoIndexCheck = null!;
    private RadioButton sameBitmapOption = null!;
    private RadioButton consecutiveBitmapOption = null!;
    private RadioButton alkonOption = null!;
    private RadioButton classicOption = null!;
    private RadioButton npcOption = null!;
    private Button createButton = null!;
    private Button normalSearchButton = null!;
    private Button normalCalculateButton = null!;
    private Button advancedSearchButton = null!;
    private Button advancedCalculateButton = null!;
    private Button simpleSearchButton = null!;
    private Button simpleIndexButton = null!;
    private Button advancedCreateButton = null!;
    private ComboBox advancedType = null!;
    private ComboBox advancedBodyCombo = null!;
    private TextBox advancedAutoIndexText = null!;
    private TextBox advancedOffsetText = null!;
    private Label advancedBodyLabel = null!;
    private Label advancedOffsetLabel = null!;
    private Label normalFramesLabel = null!;
    private Label normalColumnsLabel = null!;
    private Label normalRowsLabel = null!;
    private Label normalTotalLabel = null!;
    private Label normalFirstIndexLabel = null!;
    private Label normalHeightLabel = null!;
    private Label normalWidthLabel = null!;
    private Label normalBmpLabel = null!;
    private Label advancedFramesLabel = null!;
    private Label advancedColumnsLabel = null!;
    private Label advancedRowsLabel = null!;
    private Label advancedTotalLabel = null!;
    private Label advancedFirstIndexLabel = null!;
    private Label advancedHeightLabel = null!;
    private Label advancedWidthLabel = null!;
    private Label advancedBmpLabel = null!;
    private Label simpleBmpLabel = null!;
    private Label simpleGrhLabel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        normalButton = new Button();
        advancedButton = new Button();
        simpleButton = new Button();
        normalFrame = new GroupBox();
        normalFramesLabel = new Label();
        normalFrames = new TextBox();
        normalColumnsLabel = new Label();
        normalColumns = new TextBox();
        normalRowsLabel = new Label();
        normalRows = new TextBox();
        normalTotalLabel = new Label();
        normalTotal = new TextBox();
        normalFirstIndexLabel = new Label();
        normalFirstIndex = new TextBox();
        normalSearchButton = new Button();
        normalHeightLabel = new Label();
        normalHeight = new TextBox();
        normalWidthLabel = new Label();
        normalWidth = new TextBox();
        normalCalculateButton = new Button();
        normalBmpLabel = new Label();
        normalBmp = new TextBox();
        sameBitmapOption = new RadioButton();
        consecutiveBitmapOption = new RadioButton();
        createButton = new Button();
        advancedFrame = new GroupBox();
        advancedType = new ComboBox();
        advancedBodyCombo = new ComboBox();
        advancedAutoIndexText = new TextBox();
        advancedOffsetText = new TextBox();
        advancedFramesLabel = new Label();
        advancedFrames = new TextBox();
        advancedColumnsLabel = new Label();
        advancedColumns = new TextBox();
        advancedRowsLabel = new Label();
        advancedRows = new TextBox();
        advancedTotalLabel = new Label();
        advancedTotal = new TextBox();
        advancedFirstIndexLabel = new Label();
        advancedFirstIndex = new TextBox();
        advancedSearchButton = new Button();
        advancedHeightLabel = new Label();
        advancedHeight = new TextBox();
        advancedWidthLabel = new Label();
        advancedWidth = new TextBox();
        advancedCalculateButton = new Button();
        advancedBmpLabel = new Label();
        advancedBodyLabel = new Label();
        advancedOffsetLabel = new Label();
        advancedBmp = new TextBox();
        advancedCreateButton = new Button();
        alkonOption = new RadioButton();
        classicOption = new RadioButton();
        npcOption = new RadioButton();
        autoIndexCheck = new CheckBox();
        simpleFrame = new GroupBox();
        simpleBmpLabel = new Label();
        simpleBmp = new TextBox();
        simpleGrhLabel = new Label();
        simpleGrh = new TextBox();
        simpleSearchButton = new Button();
        simpleIndexButton = new Button();
        createAnimation = new CheckBox();
        normalFrame.SuspendLayout();
        advancedFrame.SuspendLayout();
        simpleFrame.SuspendLayout();
        SuspendLayout();
        //
        // normalButton
        //
        normalButton.Location = new Point(32, 8);
        normalButton.Name = "normalButton";
        normalButton.Size = new Size(105, 23);
        normalButton.TabIndex = 0;
        normalButton.Text = "Animacion normal";
        normalButton.UseVisualStyleBackColor = true;
        normalButton.Click += normalButton_Click;
        //
        // advancedButton
        //
        advancedButton.Location = new Point(152, 8);
        advancedButton.Name = "advancedButton";
        advancedButton.Size = new Size(185, 23);
        advancedButton.TabIndex = 1;
        advancedButton.Text = "Animacion Avanzada";
        advancedButton.UseVisualStyleBackColor = true;
        advancedButton.Click += advancedButton_Click;
        //
        // simpleButton
        //
        simpleButton.Location = new Point(352, 8);
        simpleButton.Name = "simpleButton";
        simpleButton.Size = new Size(65, 23);
        simpleButton.TabIndex = 2;
        simpleButton.Text = "simple";
        simpleButton.UseVisualStyleBackColor = true;
        simpleButton.Click += simpleButton_Click;
        //
        // normalFrame
        //
        normalFrame.Controls.Add(normalFramesLabel);
        normalFrame.Controls.Add(normalFrames);
        normalFrame.Controls.Add(normalColumnsLabel);
        normalFrame.Controls.Add(normalColumns);
        normalFrame.Controls.Add(normalRowsLabel);
        normalFrame.Controls.Add(normalRows);
        normalFrame.Controls.Add(normalTotalLabel);
        normalFrame.Controls.Add(normalTotal);
        normalFrame.Controls.Add(normalFirstIndexLabel);
        normalFrame.Controls.Add(normalFirstIndex);
        normalFrame.Controls.Add(normalSearchButton);
        normalFrame.Controls.Add(normalHeightLabel);
        normalFrame.Controls.Add(normalHeight);
        normalFrame.Controls.Add(normalWidthLabel);
        normalFrame.Controls.Add(normalWidth);
        normalFrame.Controls.Add(normalCalculateButton);
        normalFrame.Controls.Add(normalBmpLabel);
        normalFrame.Controls.Add(normalBmp);
        normalFrame.Controls.Add(sameBitmapOption);
        normalFrame.Controls.Add(consecutiveBitmapOption);
        normalFrame.Controls.Add(createButton);
        normalFrame.Location = new Point(0, 32);
        normalFrame.Name = "normalFrame";
        normalFrame.Size = new Size(425, 365);
        normalFrame.TabIndex = 3;
        normalFrame.TabStop = false;
        normalFrame.Text = "Animacion normal";
        //
        // normalFramesLabel
        //
        normalFramesLabel.AutoSize = true;
        normalFramesLabel.Location = new Point(48, 107);
        normalFramesLabel.Name = "normalFramesLabel";
        normalFramesLabel.Size = new Size(58, 15);
        normalFramesLabel.TabIndex = 0;
        normalFramesLabel.Text = "nºframes:";
        //
        // normalFrames
        //
        normalFrames.Location = new Point(128, 104);
        normalFrames.Name = "normalFrames";
        normalFrames.Size = new Size(45, 23);
        normalFrames.TabIndex = 1;
        normalFrames.Text = "1";
        //
        // normalColumnsLabel
        //
        normalColumnsLabel.AutoSize = true;
        normalColumnsLabel.Location = new Point(112, 83);
        normalColumnsLabel.Name = "normalColumnsLabel";
        normalColumnsLabel.Size = new Size(64, 15);
        normalColumnsLabel.TabIndex = 2;
        normalColumnsLabel.Text = "A lo ancho";
        //
        // normalColumns
        //
        normalColumns.Location = new Point(200, 104);
        normalColumns.Name = "normalColumns";
        normalColumns.Size = new Size(45, 23);
        normalColumns.TabIndex = 3;
        normalColumns.Text = "1";
        //
        // normalRowsLabel
        //
        normalRowsLabel.AutoSize = true;
        normalRowsLabel.Location = new Point(200, 83);
        normalRowsLabel.Name = "normalRowsLabel";
        normalRowsLabel.Size = new Size(51, 15);
        normalRowsLabel.TabIndex = 4;
        normalRowsLabel.Text = "A lo alto";
        //
        // normalRows
        //
        normalRows.Location = new Point(264, 104);
        normalRows.Name = "normalRows";
        normalRows.Size = new Size(45, 23);
        normalRows.TabIndex = 5;
        normalRows.Text = "1";
        //
        // normalTotalLabel
        //
        normalTotalLabel.AutoSize = true;
        normalTotalLabel.Location = new Point(320, 83);
        normalTotalLabel.Name = "normalTotalLabel";
        normalTotalLabel.Size = new Size(44, 15);
        normalTotalLabel.TabIndex = 6;
        normalTotalLabel.Text = "Totales";
        //
        // normalTotal
        //
        normalTotal.Location = new Point(319, 104);
        normalTotal.Name = "normalTotal";
        normalTotal.Size = new Size(45, 23);
        normalTotal.TabIndex = 7;
        normalTotal.Text = "1";
        //
        // normalFirstIndexLabel
        //
        normalFirstIndexLabel.AutoSize = true;
        normalFirstIndexLabel.Location = new Point(48, 139);
        normalFirstIndexLabel.Name = "normalFirstIndexLabel";
        normalFirstIndexLabel.Size = new Size(80, 15);
        normalFirstIndexLabel.TabIndex = 8;
        normalFirstIndexLabel.Text = "Primer indice:";
        //
        // normalFirstIndex
        //
        normalFirstIndex.Location = new Point(128, 136);
        normalFirstIndex.Name = "normalFirstIndex";
        normalFirstIndex.Size = new Size(75, 23);
        normalFirstIndex.Text = "0";
        normalFirstIndex.BackColor = Color.Red;
        normalFirstIndex.TabIndex = 9;
        //
        // normalSearchButton
        //
        normalSearchButton.Location = new Point(216, 136);
        normalSearchButton.Name = "normalSearchButton";
        normalSearchButton.Size = new Size(55, 23);
        normalSearchButton.TabIndex = 10;
        normalSearchButton.Text = "Buscar";
        normalSearchButton.Click += BrowseBitmap_Click;
        //
        // normalHeightLabel
        //
        normalHeightLabel.AutoSize = true;
        normalHeightLabel.Location = new Point(48, 171);
        normalHeightLabel.Name = "normalHeightLabel";
        normalHeightLabel.Size = new Size(32, 15);
        normalHeightLabel.TabIndex = 11;
        normalHeightLabel.Text = "Alto:";
        //
        // normalHeight
        //
        normalHeight.Location = new Point(128, 168);
        normalHeight.Name = "normalHeight";
        normalHeight.Size = new Size(75, 23);
        normalHeight.Text = "16";
        normalHeight.TabIndex = 12;
        //
        // normalWidthLabel
        //
        normalWidthLabel.AutoSize = true;
        normalWidthLabel.Location = new Point(48, 203);
        normalWidthLabel.Name = "normalWidthLabel";
        normalWidthLabel.Size = new Size(45, 15);
        normalWidthLabel.TabIndex = 13;
        normalWidthLabel.Text = "Ancho:";
        //
        // normalWidth
        //
        normalWidth.Location = new Point(128, 200);
        normalWidth.Name = "normalWidth";
        normalWidth.Size = new Size(75, 23);
        normalWidth.Text = "16";
        normalWidth.TabIndex = 14;
        //
        // normalCalculateButton
        //
        normalCalculateButton.Location = new Point(224, 200);
        normalCalculateButton.Name = "normalCalculateButton";
        normalCalculateButton.Size = new Size(80, 23);
        normalCalculateButton.TabIndex = 15;
        normalCalculateButton.Text = "AutoCalcular";
        normalCalculateButton.Click += normalCalculateButton_Click;
        //
        // normalBmpLabel
        //
        normalBmpLabel.AutoSize = true;
        normalBmpLabel.Location = new Point(48, 235);
        normalBmpLabel.Name = "normalBmpLabel";
        normalBmpLabel.Size = new Size(35, 15);
        normalBmpLabel.TabIndex = 16;
        normalBmpLabel.Text = "BMP:";
        //
        // normalBmp
        //
        normalBmp.Location = new Point(128, 232);
        normalBmp.Name = "normalBmp";
        normalBmp.Size = new Size(75, 23);
        normalBmp.Text = "1";
        normalBmp.BackColor = Color.Red;
        normalBmp.TabIndex = 17;
        //
        // sameBitmapOption
        //
        sameBitmapOption.AutoSize = true;
        sameBitmapOption.Checked = true;
        sameBitmapOption.Location = new Point(48, 45);
        sameBitmapOption.Name = "sameBitmapOption";
        sameBitmapOption.Size = new Size(90, 19);
        sameBitmapOption.TabIndex = 18;
        sameBitmapOption.TabStop = true;
        sameBitmapOption.Text = "Mismo BMP";
        //
        // consecutiveBitmapOption
        //
        consecutiveBitmapOption.AutoSize = true;
        consecutiveBitmapOption.Location = new Point(220, 45);
        consecutiveBitmapOption.Name = "consecutiveBitmapOption";
        consecutiveBitmapOption.Size = new Size(122, 19);
        consecutiveBitmapOption.TabIndex = 19;
        consecutiveBitmapOption.Text = "BMP consecutivos";
        //
        // createButton
        //
        createButton.Location = new Point(112, 264);
        createButton.Name = "createButton";
        createButton.Size = new Size(75, 23);
        createButton.TabIndex = 20;
        createButton.Text = "Crear";
        createButton.Click += Create_Click;
        //
        // advancedFrame
        //
        advancedFrame.Controls.Add(advancedType);
        advancedFrame.Controls.Add(advancedFramesLabel);
        advancedFrame.Controls.Add(advancedFrames);
        advancedFrame.Controls.Add(advancedColumnsLabel);
        advancedFrame.Controls.Add(advancedColumns);
        advancedFrame.Controls.Add(advancedRowsLabel);
        advancedFrame.Controls.Add(advancedRows);
        advancedFrame.Controls.Add(advancedTotalLabel);
        advancedFrame.Controls.Add(advancedTotal);
        advancedFrame.Controls.Add(advancedFirstIndexLabel);
        advancedFrame.Controls.Add(advancedFirstIndex);
        advancedFrame.Controls.Add(advancedSearchButton);
        advancedFrame.Controls.Add(advancedHeightLabel);
        advancedFrame.Controls.Add(advancedHeight);
        advancedFrame.Controls.Add(advancedWidthLabel);
        advancedFrame.Controls.Add(advancedWidth);
        advancedFrame.Controls.Add(advancedCalculateButton);
        advancedFrame.Controls.Add(advancedBmpLabel);
        advancedFrame.Controls.Add(advancedBmp);
        advancedFrame.Controls.Add(advancedCreateButton);
        advancedFrame.Controls.Add(advancedBodyLabel);
        advancedFrame.Controls.Add(advancedBodyCombo);
        advancedFrame.Controls.Add(advancedAutoIndexText);
        advancedFrame.Controls.Add(advancedOffsetLabel);
        advancedFrame.Controls.Add(advancedOffsetText);
        advancedFrame.Controls.Add(alkonOption);
        advancedFrame.Controls.Add(classicOption);
        advancedFrame.Controls.Add(npcOption);
        advancedFrame.Controls.Add(autoIndexCheck);
        advancedFrame.Location = new Point(0, 32);
        advancedFrame.Name = "advancedFrame";
        advancedFrame.Size = new Size(425, 365);
        advancedFrame.TabIndex = 4;
        advancedFrame.TabStop = false;
        advancedFrame.Text = "Animacion Especial";
        advancedFrame.Visible = false;
        //
        // advancedType
        //
        advancedType.DropDownStyle = ComboBoxStyle.DropDownList;
        advancedType.Items.AddRange(new object[] { "Animacion normal", "Animacion por grilla" });
        advancedType.Location = new Point(56, 32);
        advancedType.Name = "advancedType";
        advancedType.Size = new Size(155, 23);
        advancedType.TabIndex = 0;
        advancedType.Items.Clear();
        advancedType.Items.AddRange(new object[] { "Bodys/armas/Escudo", "Animacion con offset" });
        advancedType.SelectedIndex = 0;
        advancedBodyCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        advancedBodyCombo.Items.AddRange(new object[] { "Body", "Armas", "Escudo" });
        advancedBodyCombo.Location = new Point(112, 304);
        advancedBodyCombo.Name = "advancedBodyCombo";
        advancedBodyCombo.Size = new Size(80, 23);
        advancedBodyCombo.TabIndex = 20;
        advancedBodyCombo.SelectedIndex = 0;
        //
        // advancedFramesLabel
        //
        advancedFramesLabel.AutoSize = true;
        advancedFramesLabel.Location = new Point(48, 107);
        advancedFramesLabel.Name = "advancedFramesLabel";
        advancedFramesLabel.Size = new Size(58, 15);
        advancedFramesLabel.TabIndex = 1;
        advancedFramesLabel.Text = "nºframes:";
        //
        // advancedFrames
        //
        advancedFrames.Location = new Point(128, 104);
        advancedFrames.Name = "advancedFrames";
        advancedFrames.Size = new Size(45, 23);
        advancedFrames.TabIndex = 2;
        advancedFrames.Text = "6";
        //
        // advancedColumnsLabel
        //
        advancedColumnsLabel.AutoSize = true;
        advancedColumnsLabel.Location = new Point(112, 83);
        advancedColumnsLabel.Name = "advancedColumnsLabel";
        advancedColumnsLabel.Size = new Size(64, 15);
        advancedColumnsLabel.TabIndex = 3;
        advancedColumnsLabel.Text = "A lo ancho";
        //
        // advancedColumns
        //
        advancedColumns.Location = new Point(224, 104);
        advancedColumns.Name = "advancedColumns";
        advancedColumns.Size = new Size(45, 23);
        advancedColumns.TabIndex = 4;
        advancedColumns.Text = "6";
        //
        // advancedRowsLabel
        //
        advancedRowsLabel.AutoSize = true;
        advancedRowsLabel.Location = new Point(200, 83);
        advancedRowsLabel.Name = "advancedRowsLabel";
        advancedRowsLabel.Size = new Size(51, 15);
        advancedRowsLabel.TabIndex = 5;
        advancedRowsLabel.Text = "A lo alto";
        //
        // advancedRows
        //
        advancedRows.Location = new Point(288, 104);
        advancedRows.Name = "advancedRows";
        advancedRows.Size = new Size(45, 23);
        advancedRows.TabIndex = 6;
        advancedRows.Text = "4";
        //
        // advancedTotalLabel
        //
        advancedTotalLabel.AutoSize = true;
        advancedTotalLabel.Location = new Point(320, 83);
        advancedTotalLabel.Name = "advancedTotalLabel";
        advancedTotalLabel.Size = new Size(44, 15);
        advancedTotalLabel.TabIndex = 7;
        advancedTotalLabel.Text = "Totales";
        //
        // advancedTotal
        //
        advancedTotal.Location = new Point(352, 104);
        advancedTotal.Name = "advancedTotal";
        advancedTotal.Size = new Size(45, 23);
        advancedTotal.TabIndex = 8;
        advancedTotal.Text = "22";
        //
        // advancedFirstIndexLabel
        //
        advancedFirstIndexLabel.AutoSize = true;
        advancedFirstIndexLabel.Location = new Point(48, 139);
        advancedFirstIndexLabel.Name = "advancedFirstIndexLabel";
        advancedFirstIndexLabel.Size = new Size(80, 15);
        advancedFirstIndexLabel.TabIndex = 9;
        advancedFirstIndexLabel.Text = "Primer indice:";
        //
        // advancedFirstIndex
        //
        advancedFirstIndex.Location = new Point(128, 136);
        advancedFirstIndex.Name = "advancedFirstIndex";
        advancedFirstIndex.Size = new Size(75, 23);
        advancedFirstIndex.Text = "0";
        advancedFirstIndex.BackColor = Color.Red;
        advancedFirstIndex.TabIndex = 10;
        //
        // advancedSearchButton
        //
        advancedSearchButton.Location = new Point(216, 136);
        advancedSearchButton.Name = "advancedSearchButton";
        advancedSearchButton.Size = new Size(55, 23);
        advancedSearchButton.TabIndex = 11;
        advancedSearchButton.Text = "Buscar";
        advancedSearchButton.Click += BrowseBitmap_Click;
        //
        // advancedHeightLabel
        //
        advancedHeightLabel.AutoSize = true;
        advancedHeightLabel.Location = new Point(48, 171);
        advancedHeightLabel.Name = "advancedHeightLabel";
        advancedHeightLabel.Size = new Size(32, 15);
        advancedHeightLabel.TabIndex = 12;
        advancedHeightLabel.Text = "Alto:";
        //
        // advancedHeight
        //
        advancedHeight.Location = new Point(128, 168);
        advancedHeight.Name = "advancedHeight";
        advancedHeight.Size = new Size(75, 23);
        advancedHeight.Text = "46";
        advancedHeight.TabIndex = 13;
        //
        // advancedWidthLabel
        //
        advancedWidthLabel.AutoSize = true;
        advancedWidthLabel.Location = new Point(48, 203);
        advancedWidthLabel.Name = "advancedWidthLabel";
        advancedWidthLabel.Size = new Size(45, 15);
        advancedWidthLabel.TabIndex = 14;
        advancedWidthLabel.Text = "Ancho:";
        //
        // advancedWidth
        //
        advancedWidth.Location = new Point(128, 200);
        advancedWidth.Name = "advancedWidth";
        advancedWidth.Size = new Size(75, 23);
        advancedWidth.Text = "26";
        advancedWidth.TabIndex = 15;
        //
        // advancedCalculateButton
        //
        advancedCalculateButton.Location = new Point(224, 200);
        advancedCalculateButton.Name = "advancedCalculateButton";
        advancedCalculateButton.Size = new Size(80, 23);
        advancedCalculateButton.TabIndex = 16;
        advancedCalculateButton.Text = "AutoCalcular";
        advancedCalculateButton.Click += normalCalculateButton_Click;
        //
        // advancedBmpLabel
        //
        advancedBmpLabel.AutoSize = true;
        advancedBmpLabel.Location = new Point(48, 235);
        advancedBmpLabel.Name = "advancedBmpLabel";
        advancedBmpLabel.Size = new Size(35, 15);
        advancedBmpLabel.TabIndex = 17;
        advancedBmpLabel.Text = "BMP:";
        //
        // advancedBmp
        //
        advancedBmp.Location = new Point(128, 232);
        advancedBmp.Name = "advancedBmp";
        advancedBmp.Size = new Size(75, 23);
        advancedBmp.Text = "0";
        advancedBmp.BackColor = Color.Red;
        advancedBmp.TabIndex = 18;
        //
        // advancedCreateButton
        //
        advancedCreateButton.Location = new Point(112, 296);
        advancedCreateButton.Name = "advancedCreateButton";
        advancedCreateButton.Size = new Size(75, 23);
        advancedCreateButton.TabIndex = 19;
        advancedCreateButton.Text = "Crear";
        advancedCreateButton.Click += Create_Click;
        advancedBodyLabel.AutoSize = true;
        advancedBodyLabel.Location = new Point(40, 308);
        advancedBodyLabel.Name = "advancedBodyLabel";
        advancedBodyLabel.Text = "Autodindexar";
        advancedAutoIndexText.Location = new Point(198, 304);
        advancedAutoIndexText.Name = "advancedAutoIndexText";
        advancedAutoIndexText.Size = new Size(55, 23);
        advancedAutoIndexText.Text = "280";
        advancedOffsetLabel.AutoSize = true;
        advancedOffsetLabel.Location = new Point(258, 308);
        advancedOffsetLabel.Name = "advancedOffsetLabel";
        advancedOffsetLabel.Text = "offset:";
        advancedOffsetText.Location = new Point(300, 304);
        advancedOffsetText.Name = "advancedOffsetText";
        advancedOffsetText.Size = new Size(55, 23);
        advancedOffsetText.Text = "-38";
        //
        // alkonOption
        //
        alkonOption.AutoSize = true;
        alkonOption.Location = new Point(304, 185);
        alkonOption.Name = "alkonOption";
        alkonOption.Size = new Size(75, 19);
        alkonOption.TabIndex = 20;
        alkonOption.Text = "(pj) alkon";
        //
        // classicOption
        //
        classicOption.AutoSize = true;
        classicOption.Location = new Point(304, 208);
        classicOption.Name = "classicOption";
        classicOption.Size = new Size(82, 19);
        classicOption.TabIndex = 21;
        classicOption.Text = "(pj) clasico";
        //
        // npcOption
        //
        npcOption.AutoSize = true;
        npcOption.Location = new Point(304, 231);
        npcOption.Name = "npcOption";
        npcOption.Size = new Size(97, 19);
        npcOption.TabIndex = 22;
        npcOption.Text = "Normal(NPC)";
        //
        // autoIndexCheck
        //
        autoIndexCheck.AutoSize = true;
        autoIndexCheck.Location = new Point(24, 256);
        autoIndexCheck.Name = "autoIndexCheck";
        autoIndexCheck.Size = new Size(97, 19);
        autoIndexCheck.TabIndex = 23;
        autoIndexCheck.Text = "Autodindexar";
        //
        // simpleFrame
        //
        simpleFrame.Controls.Add(simpleBmpLabel);
        simpleFrame.Controls.Add(simpleBmp);
        simpleFrame.Controls.Add(simpleGrhLabel);
        simpleFrame.Controls.Add(simpleGrh);
        simpleFrame.Controls.Add(simpleSearchButton);
        simpleFrame.Controls.Add(simpleIndexButton);
        simpleFrame.Location = new Point(0, 32);
        simpleFrame.Name = "simpleFrame";
        simpleFrame.Size = new Size(425, 365);
        simpleFrame.TabIndex = 5;
        simpleFrame.TabStop = false;
        simpleFrame.Text = "Indexacion simple";
        simpleFrame.Visible = false;
        //
        // simpleBmpLabel
        //
        simpleBmpLabel.AutoSize = true;
        simpleBmpLabel.Location = new Point(56, 43);
        simpleBmpLabel.Name = "simpleBmpLabel";
        simpleBmpLabel.Size = new Size(35, 15);
        simpleBmpLabel.TabIndex = 0;
        simpleBmpLabel.Text = "Bmp:";
        //
        // simpleBmp
        //
        simpleBmp.Location = new Point(120, 40);
        simpleBmp.Name = "simpleBmp";
        simpleBmp.Size = new Size(115, 23);
        simpleBmp.TabIndex = 1;
        //
        // simpleGrhLabel
        //
        simpleGrhLabel.AutoSize = true;
        simpleGrhLabel.Location = new Point(40, 83);
        simpleGrhLabel.Name = "simpleGrhLabel";
        simpleGrhLabel.Size = new Size(76, 15);
        simpleGrhLabel.TabIndex = 2;
        simpleGrhLabel.Text = "Numero Grh:";
        //
        // simpleGrh
        //
        simpleGrh.Location = new Point(120, 80);
        simpleGrh.Name = "simpleGrh";
        simpleGrh.Size = new Size(115, 23);
        simpleGrh.TabIndex = 3;
        //
        // simpleSearchButton
        //
        simpleSearchButton.Location = new Point(264, 40);
        simpleSearchButton.Name = "simpleSearchButton";
        simpleSearchButton.Size = new Size(55, 23);
        simpleSearchButton.TabIndex = 4;
        simpleSearchButton.Text = "buscar";
        simpleSearchButton.Click += BrowseBitmap_Click;
        //
        // simpleIndexButton
        //
        simpleIndexButton.Location = new Point(144, 128);
        simpleIndexButton.Name = "simpleIndexButton";
        simpleIndexButton.Size = new Size(95, 23);
        simpleIndexButton.TabIndex = 5;
        simpleIndexButton.Text = "indexar";
        simpleIndexButton.Click += Create_Click;
        //
        // createAnimation
        //
        createAnimation.Location = new Point(0, 0);
        createAnimation.Name = "createAnimation";
        createAnimation.Size = new Size(104, 24);
        createAnimation.TabIndex = 0;
        //
        // AutoIndexerForm
        //
        ClientSize = new Size(425, 405);
        Controls.Add(normalButton);
        Controls.Add(advancedButton);
        Controls.Add(simpleButton);
        Controls.Add(normalFrame);
        Controls.Add(advancedFrame);
        Controls.Add(simpleFrame);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AutoIndexerForm";
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
        Text = "Indexador automatico";
        normalFrame.ResumeLayout(false);
        normalFrame.PerformLayout();
        advancedFrame.ResumeLayout(false);
        advancedFrame.PerformLayout();
        simpleFrame.ResumeLayout(false);
        simpleFrame.PerformLayout();
        ResumeLayout(false);
    }
}
