using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MaethrillianInstaller.GUI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.connectivityTimer = new System.Windows.Forms.Timer(this.components);
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.imageBox = new System.Windows.Forms.PictureBox();
            this.modGroup = new System.Windows.Forms.GroupBox();
            this.modListPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.modInfoGroup = new System.Windows.Forms.GroupBox();
            this.modInfoTextBox = new System.Windows.Forms.TextBox();
            this.bottomLayout = new System.Windows.Forms.TableLayoutPanel();
            this.linkDiscord = new System.Windows.Forms.LinkLabel();
            this.buttonPTR = new System.Windows.Forms.CheckBox();
            this.installButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.installProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.mainLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.modGroup.SuspendLayout();
            this.modInfoGroup.SuspendLayout();
            this.bottomLayout.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectivityTimer
            // 
            this.connectivityTimer.Interval = 5000;
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.imageBox, 0, 0);
            this.mainLayout.Controls.Add(this.modGroup, 0, 1);
            this.mainLayout.Controls.Add(this.modInfoGroup, 0, 2);
            this.mainLayout.Controls.Add(this.bottomLayout, 0, 3);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(12, 12);
            this.mainLayout.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.mainLayout.RowCount = 4;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayout.Size = new System.Drawing.Size(380, 705);
            this.mainLayout.TabIndex = 0;
            // 
            // imageBox
            // 
            this.imageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBox.Location = new System.Drawing.Point(0, 0);
            this.imageBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(380, 174);
            this.imageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBox.TabIndex = 0;
            this.imageBox.TabStop = false;
            // 
            // modGroup
            // 
            this.modGroup.Controls.Add(this.modListPanel);
            this.modGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modGroup.Location = new System.Drawing.Point(0, 186);
            this.modGroup.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.modGroup.Name = "modGroup";
            this.modGroup.Padding = new System.Windows.Forms.Padding(8);
            this.modGroup.Size = new System.Drawing.Size(380, 248);
            this.modGroup.TabIndex = 1;
            this.modGroup.TabStop = false;
            this.modGroup.Text = "Select Mod";
            // 
            // modListPanel
            // 
            this.modListPanel.AutoScroll = true;
            this.modListPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modListPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.modListPanel.Location = new System.Drawing.Point(8, 24);
            this.modListPanel.Margin = new System.Windows.Forms.Padding(0);
            this.modListPanel.Name = "modListPanel";
            this.modListPanel.Size = new System.Drawing.Size(364, 216);
            this.modListPanel.TabIndex = 0;
            this.modListPanel.WrapContents = false;
            // 
            // modInfoGroup
            // 
            this.modInfoGroup.Controls.Add(this.modInfoTextBox);
            this.modInfoGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modInfoGroup.Location = new System.Drawing.Point(0, 446);
            this.modInfoGroup.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.modInfoGroup.Name = "modInfoGroup";
            this.modInfoGroup.Padding = new System.Windows.Forms.Padding(8);
            this.modInfoGroup.Size = new System.Drawing.Size(380, 201);
            this.modInfoGroup.TabIndex = 2;
            this.modInfoGroup.TabStop = false;
            this.modInfoGroup.Text = "Mod Details";
            // 
            // modInfoTextBox
            // 
            this.modInfoTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.modInfoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.modInfoTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modInfoTextBox.Location = new System.Drawing.Point(8, 24);
            this.modInfoTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.modInfoTextBox.Multiline = true;
            this.modInfoTextBox.Name = "modInfoTextBox";
            this.modInfoTextBox.ReadOnly = true;
            this.modInfoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.modInfoTextBox.Size = new System.Drawing.Size(364, 169);
            this.modInfoTextBox.TabIndex = 0;
            // 
            // bottomLayout
            // 
            this.bottomLayout.ColumnCount = 3;
            this.bottomLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bottomLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.bottomLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.bottomLayout.Controls.Add(this.linkDiscord, 0, 0);
            this.bottomLayout.Controls.Add(this.buttonPTR, 1, 0);
            this.bottomLayout.Controls.Add(this.installButton, 2, 0);
            this.bottomLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomLayout.Location = new System.Drawing.Point(0, 659);
            this.bottomLayout.Margin = new System.Windows.Forms.Padding(0);
            this.bottomLayout.Name = "bottomLayout";
            this.bottomLayout.RowCount = 1;
            this.bottomLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bottomLayout.Size = new System.Drawing.Size(380, 34);
            this.bottomLayout.TabIndex = 3;
            // 
            // linkDiscord
            // 
            this.linkDiscord.AutoSize = true;
            this.linkDiscord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkDiscord.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkDiscord.Location = new System.Drawing.Point(0, 0);
            this.linkDiscord.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.linkDiscord.Name = "linkDiscord";
            this.linkDiscord.Size = new System.Drawing.Size(160, 34);
            this.linkDiscord.TabIndex = 0;
            this.linkDiscord.TabStop = true;
            this.linkDiscord.Text = "discord.gg/Gd7wmjfrSS";
            this.linkDiscord.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonPTR
            // 
            this.buttonPTR.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonPTR.AutoSize = true;
            this.buttonPTR.Location = new System.Drawing.Point(172, 7);
            this.buttonPTR.Margin = new System.Windows.Forms.Padding(0, 6, 12, 6);
            this.buttonPTR.Name = "buttonPTR";
            this.buttonPTR.Size = new System.Drawing.Size(46, 19);
            this.buttonPTR.TabIndex = 1;
            this.buttonPTR.Text = "PTR";
            this.buttonPTR.UseVisualStyleBackColor = true;
            // 
            // installButton
            // 
            this.installButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.installButton.AutoSize = true;
            this.installButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.installButton.Location = new System.Drawing.Point(230, 0);
            this.installButton.Margin = new System.Windows.Forms.Padding(0);
            this.installButton.MinimumSize = new System.Drawing.Size(150, 32);
            this.installButton.Name = "installButton";
            this.installButton.Padding = new System.Windows.Forms.Padding(12, 4, 12, 4);
            this.installButton.Size = new System.Drawing.Size(150, 33);
            this.installButton.TabIndex = 2;
            this.installButton.Text = "Install";
            this.installButton.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.installProgressBar});
            this.statusStrip.Location = new System.Drawing.Point(12, 717);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip.Size = new System.Drawing.Size(380, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            // 
            // statusLabel
            // 
            this.statusLabel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(42, 16);
            this.statusLabel.Text = "Ready.";
            // 
            // installProgressBar
            // 
            this.installProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.installProgressBar.AutoSize = false;
            this.installProgressBar.Margin = new System.Windows.Forms.Padding(8, 3, 1, 3);
            this.installProgressBar.Name = "installProgressBar";
            this.installProgressBar.Size = new System.Drawing.Size(180, 16);
            this.installProgressBar.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 739);
            this.Controls.Add(this.mainLayout);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(420, 640);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 0);
            this.Text = "Maethrillian";
            this.mainLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            this.modGroup.ResumeLayout(false);
            this.modInfoGroup.ResumeLayout(false);
            this.modInfoGroup.PerformLayout();
            this.bottomLayout.ResumeLayout(false);
            this.bottomLayout.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolTip toolTip1;
        private TableLayoutPanel mainLayout;
        private PictureBox imageBox;
        private GroupBox modGroup;
        private FlowLayoutPanel modListPanel;
        private GroupBox modInfoGroup;
        private TextBox modInfoTextBox;
        private TableLayoutPanel bottomLayout;
        private LinkLabel linkDiscord;
        private CheckBox buttonPTR;
        private Button installButton;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolStripProgressBar installProgressBar;
        private System.Windows.Forms.Timer connectivityTimer;
    }
}
