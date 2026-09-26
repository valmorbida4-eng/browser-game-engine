using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

public class LauncherForm : Form {
    private static Process serverProcess = null;
    private static string projectDir;
    private System.Windows.Forms.Timer statusTimer;
    private Label lblServerStatus;
    private CheckBox chkAppMode;
    private Button btnVoxel;
    private Button btnReinos;
    private Button btnSandbox;
    private Button btnTextures;
    private Button btnOpenFolder;
    private Button btnRestartServer;

    public LauncherForm() {
        projectDir = GetProjectDir();
        InitializeComponents();
        EnsureServerRunning();
    }

    private static string GetProjectDir() {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/');
        if (File.Exists(Path.Combine(baseDir, "package.json"))) {
            return baseDir;
        }
        string hardcoded = @"c:\Users\valmo\OneDrive\GitHub\browser-game-engine";
        if (File.Exists(Path.Combine(hardcoded, "package.json"))) {
            return hardcoded;
        }
        return baseDir;
    }

    private void InitializeComponents() {
        this.Text = "Aether Games — Launcher Oficial";
        this.ClientSize = new Size(740, 560);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(17, 19, 27);
        this.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

        string icoPath = Path.Combine(projectDir, "app.ico");
        if (File.Exists(icoPath)) {
            try {
                this.Icon = new Icon(icoPath);
            } catch {}
        }

        // Header Panel
        Panel pnlHeader = new Panel();
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Size = new Size(740, 85);
        pnlHeader.BackColor = Color.FromArgb(24, 28, 42);
        pnlHeader.Paint += delegate(object sender, PaintEventArgs e) {
            using (Pen p = new Pen(Color.FromArgb(40, 48, 72), 1)) {
                e.Graphics.DrawLine(p, 0, 84, 740, 84);
            }
        };

        Label lblTitle = new Label();
        lblTitle.Text = "⚔️ AETHER GAMES 🎮";
        lblTitle.Font = new Font("Segoe UI", 17f, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(24, 14);
        lblTitle.AutoSize = true;
        pnlHeader.Controls.Add(lblTitle);

        Label lblSubtitle = new Label();
        lblSubtitle.Text = "Engine 3D WebGL2 com Pipeline Deferred HDR • Escolha o jogo para iniciar:";
        lblSubtitle.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        lblSubtitle.ForeColor = Color.FromArgb(160, 172, 198);
        lblSubtitle.Location = new Point(26, 49);
        lblSubtitle.AutoSize = true;
        pnlHeader.Controls.Add(lblSubtitle);

        this.Controls.Add(pnlHeader);

        // Card 1: VoxelCraft
        Panel cardVoxel = CreateCard(24, 102, 336, 325, Color.FromArgb(24, 28, 40), Color.FromArgb(40, 167, 69));
        
        Label badgeVoxel = CreateBadge("VOXEL 3D • MUNDO ABERTO", Color.FromArgb(40, 167, 69), 16, 16);
        cardVoxel.Controls.Add(badgeVoxel);

        Label titleVoxel = new Label();
        titleVoxel.Text = "VoxelCraft";
        titleVoxel.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
        titleVoxel.ForeColor = Color.White;
        titleVoxel.Location = new Point(14, 46);
        titleVoxel.AutoSize = true;
        cardVoxel.Controls.Add(titleVoxel);

        Label descVoxel = new Label();
        descVoxel.Text = "• Mundo aberto procedural infinito com biomas e rios\n" +
                         "• Cavernas, minérios e iluminação HDR dinâmica\n" +
                         "• Física fluida de água, ciclo dia/noite e nuvens\n" +
                         "• Animais com IA, inventário criativo e ferramentas\n" +
                         "• Salvamento automático no navegador (IndexedDB)";
        descVoxel.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
        descVoxel.ForeColor = Color.FromArgb(185, 195, 215);
        descVoxel.Location = new Point(16, 85);
        descVoxel.Size = new Size(304, 165);
        cardVoxel.Controls.Add(descVoxel);

        btnVoxel = CreateActionButton("▶  JOGAR VOXELCRAFT", Color.FromArgb(40, 167, 69), Color.FromArgb(48, 190, 80), 16, 260, 304, 48);
        btnVoxel.Click += delegate { LaunchGame("/"); };
        cardVoxel.Controls.Add(btnVoxel);

        this.Controls.Add(cardVoxel);

        // Card 2: Reinos RTS
        Panel cardReinos = CreateCard(380, 102, 336, 325, Color.FromArgb(24, 28, 40), Color.FromArgb(220, 53, 69));
        
        Label badgeReinos = CreateBadge("RTS • ESTRATÉGIA MEDIEVAL", Color.FromArgb(220, 53, 69), 16, 16);
        cardReinos.Controls.Add(badgeReinos);

        Label titleReinos = new Label();
        titleReinos.Text = "Reinos (RTS)";
        titleReinos.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
        titleReinos.ForeColor = Color.White;
        titleReinos.Location = new Point(14, 46);
        titleReinos.AutoSize = true;
        cardReinos.Controls.Add(titleReinos);

        Label descReinos = new Label();
        descReinos.Text = "• Estratégia em tempo real estilo Age of Empires\n" +
                          "• Partida 1x1 completa contra IA desafiadora\n" +
                          "• Economia de aldeões: corte de madeira, ouro e fazendas\n" +
                          "• 3 Eras: Trevas, Feudal e Castelos com construções\n" +
                          "• Névoa de guerra, minimapa e exército estratégico";
        descReinos.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
        descReinos.ForeColor = Color.FromArgb(185, 195, 215);
        descReinos.Location = new Point(16, 85);
        descReinos.Size = new Size(304, 165);
        cardReinos.Controls.Add(descReinos);

        btnReinos = CreateActionButton("▶  JOGAR REINOS", Color.FromArgb(220, 53, 69), Color.FromArgb(240, 65, 80), 16, 260, 304, 48);
        btnReinos.Click += delegate { LaunchGame("/rts.html"); };
        cardReinos.Controls.Add(btnReinos);

        this.Controls.Add(cardReinos);

        // Options bar
        chkAppMode = new CheckBox();
        chkAppMode.Text = "Abrir como Janela de Jogo Dedicada (Modo App Edge/Chrome sem barra de navegador)";
        chkAppMode.Checked = true;
        chkAppMode.ForeColor = Color.FromArgb(200, 210, 230);
        chkAppMode.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
        chkAppMode.Location = new Point(26, 440);
        chkAppMode.Size = new Size(690, 24);
        this.Controls.Add(chkAppMode);

        // Secondary buttons
        btnSandbox = CreateSmallButton("🧪 Sandbox 3D", 24, 472, 160, 32);
        btnSandbox.Click += delegate { LaunchGame("/sandbox.html"); };
        this.Controls.Add(btnSandbox);

        btnTextures = CreateSmallButton("🎨 Lab de Texturas", 194, 472, 160, 32);
        btnTextures.Click += delegate { LaunchGame("/textures.html"); };
        this.Controls.Add(btnTextures);

        btnOpenFolder = CreateSmallButton("📁 Pasta do Projeto", 364, 472, 160, 32);
        btnOpenFolder.Click += delegate {
            try { Process.Start("explorer.exe", projectDir); } catch {}
        };
        this.Controls.Add(btnOpenFolder);

        btnRestartServer = CreateSmallButton("🔄 Reiniciar Servidor", 534, 472, 182, 32);
        btnRestartServer.Click += delegate {
            StopServer();
            EnsureServerRunning();
            UpdateStatusDisplay();
        };
        this.Controls.Add(btnRestartServer);

        // Bottom status label
        lblServerStatus = new Label();
        lblServerStatus.Text = "⏳ Verificando servidor local...";
        lblServerStatus.ForeColor = Color.FromArgb(255, 193, 7);
        lblServerStatus.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        lblServerStatus.Location = new Point(24, 518);
        lblServerStatus.AutoSize = true;
        this.Controls.Add(lblServerStatus);

        // Timer for polling server health
        statusTimer = new System.Windows.Forms.Timer();
        statusTimer.Interval = 1000;
        statusTimer.Tick += delegate { UpdateStatusDisplay(); };
        statusTimer.Start();
    }

    private Panel CreateCard(int x, int y, int w, int h, Color bg, Color accent) {
        Panel p = new Panel();
        p.Location = new Point(x, y);
        p.Size = new Size(w, h);
        p.BackColor = bg;
        p.Paint += delegate(object sender, PaintEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(Color.FromArgb(45, 52, 75), 1)) {
                e.Graphics.DrawRectangle(pen, 0, 0, w - 1, h - 1);
            }
            using (SolidBrush b = new SolidBrush(accent)) {
                e.Graphics.FillRectangle(b, 0, 0, w, 4);
            }
        };
        return p;
    }

    private Label CreateBadge(string text, Color bg, int x, int y) {
        Label l = new Label();
        l.Text = text;
        l.Font = new Font("Segoe UI", 7.5f, FontStyle.Bold);
        l.ForeColor = Color.White;
        l.BackColor = bg;
        l.Location = new Point(x, y);
        l.Padding = new Padding(6, 2, 6, 2);
        l.AutoSize = true;
        return l;
    }

    private Button CreateActionButton(string text, Color normal, Color hover, int x, int y, int w, int h) {
        Button b = new Button();
        b.Text = text;
        b.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
        b.ForeColor = Color.White;
        b.BackColor = normal;
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize = 0;
        b.Cursor = Cursors.Hand;
        b.Location = new Point(x, y);
        b.Size = new Size(w, h);
        b.MouseEnter += delegate { b.BackColor = hover; };
        b.MouseLeave += delegate { b.BackColor = normal; };
        return b;
    }

    private Button CreateSmallButton(string text, int x, int y, int w, int h) {
        Button b = new Button();
        b.Text = text;
        b.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
        b.ForeColor = Color.FromArgb(215, 225, 245);
        b.BackColor = Color.FromArgb(32, 38, 56);
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderColor = Color.FromArgb(50, 60, 88);
        b.FlatAppearance.BorderSize = 1;
        b.Cursor = Cursors.Hand;
        b.Location = new Point(x, y);
        b.Size = new Size(w, h);
        b.MouseEnter += delegate { b.BackColor = Color.FromArgb(45, 54, 80); };
        b.MouseLeave += delegate { b.BackColor = Color.FromArgb(32, 38, 56); };
        return b;
    }

    private void UpdateStatusDisplay() {
        bool online = IsPortOpen(5173);
        if (online) {
            lblServerStatus.Text = "🟢 Servidor local ativo: http://localhost:5173 (Pronto para jogar)";
            lblServerStatus.ForeColor = Color.FromArgb(46, 189, 78);
        } else {
            lblServerStatus.Text = "⏳ Servidor local iniciando na porta 5173...";
            lblServerStatus.ForeColor = Color.FromArgb(255, 193, 7);
        }
    }

    private void LaunchGame(string path) {
        if (!IsPortOpen(5173)) {
            lblServerStatus.Text = "⏳ Aguardando servidor iniciar antes de abrir o jogo...";
            lblServerStatus.ForeColor = Color.FromArgb(255, 193, 7);
            this.Refresh();
            EnsureServerRunning();
            for (int i = 0; i < 20; i++) {
                if (IsPortOpen(5173)) break;
                Thread.Sleep(300);
                Application.DoEvents();
            }
        }

        string url = "http://localhost:5173" + path;

        if (chkAppMode.Checked) {
            string edge = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
            if (!File.Exists(edge)) {
                edge = @"C:\Program Files\Microsoft\Edge\Application\msedge.exe";
            }
            string chrome = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

            if (File.Exists(edge)) {
                try {
                    Process.Start(edge, "--app=\"" + url + "\"");
                    return;
                } catch {}
            } else if (File.Exists(chrome)) {
                try {
                    Process.Start(chrome, "--app=\"" + url + "\"");
                    return;
                } catch {}
            }
        }

        try {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        } catch (Exception ex) {
            MessageBox.Show("Erro ao abrir navegador: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static bool IsPortOpen(int port) {
        try {
            using (TcpClient client = new TcpClient()) {
                IAsyncResult res = client.BeginConnect("127.0.0.1", port, null, null);
                bool success = res.AsyncWaitHandle.WaitOne(300);
                if (!success) return false;
                client.EndConnect(res);
                return true;
            }
        } catch {
            return false;
        }
    }

    private static void EnsureServerRunning() {
        if (IsPortOpen(5173)) return;

        try {
            string viteScript = Path.Combine(projectDir, @"node_modules\vite\bin\vite.js");
            ProcessStartInfo psi;
            if (File.Exists(viteScript)) {
                psi = new ProcessStartInfo("node.exe", "\"" + viteScript + "\"");
            } else {
                psi = new ProcessStartInfo("cmd.exe", "/c npm run dev");
            }
            psi.WorkingDirectory = projectDir;
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            serverProcess = Process.Start(psi);
        } catch {}
    }

    private static void StopServer() {
        if (serverProcess != null) {
            try {
                if (!serverProcess.HasExited) {
                    ProcessStartInfo psi = new ProcessStartInfo("taskkill", "/F /T /PID " + serverProcess.Id);
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    Process.Start(psi);
                }
            } catch {}
            serverProcess = null;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e) {
        base.OnFormClosing(e);
        if (statusTimer != null) {
            statusTimer.Stop();
        }
        StopServer();
    }

    [STAThread]
    public static void Main() {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new LauncherForm());
    }
}
