namespace OrderPrint
{
    partial class FormView2
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.listView2 = new System.Windows.Forms.ListView();
            this.listView1 = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnQueryAll = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnPrintS = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView2
            // 
            this.listView2.Location = new System.Drawing.Point(12, 436);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(1319, 213);
            this.listView2.TabIndex = 6;
            this.listView2.UseCompatibleStateImageBehavior = false;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(12, 68);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1319, 347);
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.Click += new System.EventHandler(this.listView1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "患者姓名/编号/处方号：";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(193, 21);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(220, 26);
            this.textBox1.TabIndex = 9;
            // 
            // btnQueryAll
            // 
            this.btnQueryAll.Image = global::OrderPrint.Properties.Resources.book_magnify;
            this.btnQueryAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnQueryAll.Location = new System.Drawing.Point(547, 15);
            this.btnQueryAll.Name = "btnQueryAll";
            this.btnQueryAll.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.btnQueryAll.Size = new System.Drawing.Size(102, 35);
            this.btnQueryAll.TabIndex = 11;
            this.btnQueryAll.Text = "检索全部";
            this.btnQueryAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnQueryAll.UseVisualStyleBackColor = true;
            this.btnQueryAll.Click += new System.EventHandler(this.btnQueryAll_Click);
            // 
            // btnQuery
            // 
            this.btnQuery.Image = global::OrderPrint.Properties.Resources.Zoom_query_16px_525305_easyicon_net;
            this.btnQuery.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnQuery.Location = new System.Drawing.Point(426, 15);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.btnQuery.Size = new System.Drawing.Size(102, 35);
            this.btnQuery.TabIndex = 10;
            this.btnQuery.Text = " 检  索";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnPrintS
            // 
            this.btnPrintS.Image = global::OrderPrint.Properties.Resources.printer_mono;
            this.btnPrintS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrintS.Location = new System.Drawing.Point(997, 15);
            this.btnPrintS.Name = "btnPrintS";
            this.btnPrintS.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.btnPrintS.Size = new System.Drawing.Size(102, 35);
            this.btnPrintS.TabIndex = 7;
            this.btnPrintS.Text = " 打  印";
            this.btnPrintS.UseVisualStyleBackColor = true;
            this.btnPrintS.Click += new System.EventHandler(this.btnPrintS_Click);
            // 
            // FormView2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 750);
            this.ControlBox = false;
            this.Controls.Add(this.btnQueryAll);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPrintS);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.listView1);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormView2";
            this.Text = "处方补打";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormView2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPrintS;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnQueryAll;
    }
}