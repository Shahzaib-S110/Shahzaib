using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PredictiveMaintenanceSystem
{
    // -------------------------------
    // User Model
    // -------------------------------
    public class User
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    // -------------------------------
    // Repository for persistent user data.
    // -------------------------------
    public static class UserRepository
    {
        private static readonly string usersFile = "users.txt";
        public static List<User> Users { get; } = new List<User>();

        public static void LoadUsers()
        {
            if (File.Exists(usersFile))
            {
                var lines = File.ReadAllLines(usersFile);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 3)
                    {
                        Users.Add(new User { Name = parts[0], Email = parts[1], Password = parts[2] });
                    }
                }
            }
        }

        public static void SaveUsers()
        {
            var lines = Users.Select(u => $"{u.Name},{u.Email},{u.Password}");
            File.WriteAllLines(usersFile, lines);
        }

        public static bool AddUser(User user)
        {
            if (Users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                return false;
            Users.Add(user);
            SaveUsers();
            return true;
        }

        public static User GetUser(string email, string password)
        {
            return Users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.Password == password);
        }
    }

    // -------------------------------
    // Session Manager to persist login session.
    // -------------------------------
    public static class SessionManager
    {
        private static readonly string sessionFile = "session.txt";

        public static void SaveSession(string email)
        {
            File.WriteAllText(sessionFile, email);
        }

        public static void ClearSession()
        {
            if (File.Exists(sessionFile))
                File.Delete(sessionFile);
        }

        public static string GetSession()
        {
            if (File.Exists(sessionFile))
                return File.ReadAllText(sessionFile).Trim();
            return "";
        }
    }

    // -------------------------------
    // Custom Gradient Panel for attractive background.
    // -------------------------------
    public class GradientPanel : Panel
    {
        public Color ColorTop { get; set; } = Color.FromArgb(0, 120, 215);
        public Color ColorBottom { get; set; } = Color.White;

        protected override void OnPaint(PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, ColorTop, ColorBottom, 90F))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
            base.OnPaint(e);
        }
    }

    // -------------------------------
    // Login Form
    // -------------------------------
    public class LoginForm : Form
    {
        private Label lblWelcome;
        private Label lblTitle;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblPassword;
        private TextBox txtPassword;
        private CheckBox chkShowPassword;
        private Button btnLogin;
        private LinkLabel lnkSignup;

        public LoginForm()
        {
            InitializeForm();
            CreateControls();
            UserRepository.LoadUsers();

            // Auto-login if a session exists.
            string sessionEmail = SessionManager.GetSession();
            if (!string.IsNullOrWhiteSpace(sessionEmail))
            {
                var user = UserRepository.Users.FirstOrDefault(u =>
                    u.Email.Equals(sessionEmail, StringComparison.OrdinalIgnoreCase));
                if (user != null)
                {
                    OpenDashboard(user);
                }
            }
        }

        private void InitializeForm()
        {
            this.Text = "Predictive Maintenance System - Login";
            this.Size = new Size(500, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
        }

        private void CreateControls()
        {
            // Gradient Background
            GradientPanel gradientPanel = new GradientPanel()
            {
                Dock = DockStyle.Fill,
                ColorTop = Color.FromArgb(0, 120, 215),
                ColorBottom = Color.White
            };
            this.Controls.Add(gradientPanel);

            // Prominent Welcome Label
            lblWelcome = new Label()
            {
                Text = "Welcome to Predictive Maintenance System",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 100
            };
            gradientPanel.Controls.Add(lblWelcome);

            // Login Panel (Card)
            Panel loginPanel = new Panel()
            {
                Size = new Size(350, 350),
                BackColor = Color.White,
                Location = new Point((this.ClientSize.Width - 350) / 2, (this.ClientSize.Height - 350) / 2),
                Anchor = AnchorStyles.None,
                BorderStyle = BorderStyle.Fixed3D
            };
            gradientPanel.Controls.Add(loginPanel);

            // Title Label for Login Card
            lblTitle = new Label()
            {
                Text = "Login",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(140, 20)
            };
            loginPanel.Controls.Add(lblTitle);

            // Email Label
            lblEmail = new Label()
            {
                Text = "Email",
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 70),
                AutoSize = true
            };
            loginPanel.Controls.Add(lblEmail);

            // Email TextBox
            txtEmail = new TextBox()
            {
                Location = new Point(30, 90),
                Width = 280,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            loginPanel.Controls.Add(txtEmail);

            // Password Label
            lblPassword = new Label()
            {
                Text = "Password",
                Font = new Font("Segoe UI", 10),
                Location = new Point(30, 130),
                AutoSize = true
            };
            loginPanel.Controls.Add(lblPassword);

            // Password TextBox
            txtPassword = new TextBox()
            {
                Location = new Point(30, 150),
                Width = 280,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };
            loginPanel.Controls.Add(txtPassword);

            // Show Password CheckBox
            chkShowPassword = new CheckBox()
            {
                Text = "Show Password",
                Font = new Font("Segoe UI", 9),
                Location = new Point(30, 180),
                AutoSize = true
            };
            chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;
            loginPanel.Controls.Add(chkShowPassword);

            // Login Button
            btnLogin = new Button()
            {
                Text = "Login",
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Width = 280,
                Height = 40,
                Location = new Point(30, 220),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnLogin.Click += BtnLogin_Click;
            loginPanel.Controls.Add(btnLogin);

            // Signup Link
            lnkSignup = new LinkLabel()
            {
                Text = "Don't have an account? Sign up here",
                Font = new Font("Segoe UI", 9),
                Location = new Point(80, 280),
                AutoSize = true
            };
            lnkSignup.Click += LnkSignup_Click;
            loginPanel.Controls.Add(lnkSignup);
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both email and password", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            User user = UserRepository.GetUser(txtEmail.Text, txtPassword.Text);
            if (user == null)
            {
                MessageBox.Show("Invalid email or password", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Save session so that user is remembered.
            SessionManager.SaveSession(user.Email);
            MessageBox.Show("Login successful!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            OpenDashboard(user);
        }

        private void LnkSignup_Click(object sender, EventArgs e)
        {
            SignupForm signupForm = new SignupForm();
            signupForm.ShowDialog();
        }

        private void OpenDashboard(User user)
        {
            DashboardForm dashboard = new DashboardForm(user);
            this.Hide();
            dashboard.ShowDialog();
            // On logout, show the login form again.
            this.Show();
        }
    }

    // -------------------------------
    // Signup Form (Enhanced Design)
    // -------------------------------
    public class SignupForm : Form
    {
        private Label lblTitle;
        private Label lblName;
        private TextBox txtName;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnSignUp;

        public SignupForm()
        {
            InitializeForm();
            CreateControls();
        }

        private void InitializeForm()
        {
            this.Text = "Sign Up - Predictive Maintenance System";
            this.Size = new Size(500, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            // Use a gradient background.
            GradientPanel background = new GradientPanel()
            {
                Dock = DockStyle.Fill,
                ColorTop = Color.FromArgb(0, 120, 215),
                ColorBottom = Color.White
            };
            this.Controls.Add(background);
        }

        private void CreateControls()
        {
            // Create a 3D card panel for signup.
            Panel signupPanel = new Panel()
            {
                Size = new Size(350, 400),
                BackColor = Color.White,
                Location = new Point((this.ClientSize.Width - 350) / 2, (this.ClientSize.Height - 400) / 2),
                Anchor = AnchorStyles.None,
                BorderStyle = BorderStyle.Fixed3D
            };
            // Add to the first control (gradient background).
            this.Controls[0].Controls.Add(signupPanel);

            // Title Label with attractive font.
            lblTitle = new Label()
            {
                Text = "Create Your Account",
                Font = new Font("Segoe UI Semibold", 20, FontStyle.Bold),
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };
            signupPanel.Controls.Add(lblTitle);

            // Name Label.
            lblName = new Label()
            {
                Text = "Name",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.Black,
                Location = new Point(30, 80),
                AutoSize = true
            };
            signupPanel.Controls.Add(lblName);

            // Name TextBox.
            txtName = new TextBox()
            {
                Location = new Point(30, 105),
                Width = 280,
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle
            };
            signupPanel.Controls.Add(txtName);

            // Email Label.
            lblEmail = new Label()
            {
                Text = "Email",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.Black,
                Location = new Point(30, 145),
                AutoSize = true
            };
            signupPanel.Controls.Add(lblEmail);

            // Email TextBox.
            txtEmail = new TextBox()
            {
                Location = new Point(30, 170),
                Width = 280,
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle
            };
            signupPanel.Controls.Add(txtEmail);

            // Password Label.
            lblPassword = new Label()
            {
                Text = "Password",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.Black,
                Location = new Point(30, 210),
                AutoSize = true
            };
            signupPanel.Controls.Add(lblPassword);

            // Password TextBox.
            txtPassword = new TextBox()
            {
                Location = new Point(30, 235),
                Width = 280,
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };
            signupPanel.Controls.Add(txtPassword);

            // Signup Button with modern flat design.
            btnSignUp = new Button()
            {
                Text = "Sign Up",
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                Width = 280,
                Height = 45,
                Location = new Point(30, 300),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnSignUp.Click += BtnSignUp_Click;
            signupPanel.Controls.Add(btnSignUp);
        }

        private void BtnSignUp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (UserRepository.Users.Any(u => u.Email.Equals(txtEmail.Text, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Email is already registered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            User newUser = new User
            {
                Name = txtName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Password = txtPassword.Text
            };
            UserRepository.AddUser(newUser);
            MessageBox.Show("Signup successful!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }

    // -------------------------------
    // Updated Machine Model with additional properties.
    // -------------------------------
    public class Machine
    {
        public string Name { get; set; }
        public string MachineCode { get; set; }
        public string CNIC { get; set; }
        public string Model { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Condition { get; set; }
        public string ExpectedRecoveryTime { get; set; } // e.g. "7 days" or "3 days" (if applicable)
        public string RegisteredBy { get; set; }
    }

    // -------------------------------
    // Repository for Machines (Updated for additional fields).
    // -------------------------------
    public static class MachineRepository
    {
        private static readonly string machinesFile = "machines.txt";
        public static List<Machine> Machines { get; } = new List<Machine>();

        public static void LoadMachines()
        {
            Machines.Clear();
            if (File.Exists(machinesFile))
            {
                var lines = File.ReadAllLines(machinesFile);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 8)
                    {
                        DateTime regDate;
                        DateTime.TryParse(parts[4], out regDate);
                        Machines.Add(new Machine
                        {
                            Name = parts[0],
                            MachineCode = parts[1],
                            CNIC = parts[2],
                            Model = parts[3],
                            RegistrationDate = regDate,
                            Condition = parts[5],
                            ExpectedRecoveryTime = parts[6],
                            RegisteredBy = parts[7]
                        });
                    }
                }
            }
        }

        public static void SaveMachines()
        {
            var lines = Machines.Select(m =>
                $"{m.Name},{m.MachineCode},{m.CNIC},{m.Model},{m.RegistrationDate},{m.Condition},{m.ExpectedRecoveryTime},{m.RegisteredBy}");
            File.WriteAllLines(machinesFile, lines);
        }

        public static void AddMachine(Machine machine)
        {
            Machines.Add(machine);
            SaveMachines();
        }

        // Search returns a machine registered by the given user with a specific machine code and name.
        public static Machine SearchMachine(string machineCode, string machineName, string registeredBy)
        {
            return Machines.FirstOrDefault(m =>
                m.MachineCode.Equals(machineCode, StringComparison.OrdinalIgnoreCase) &&
                m.Name.Equals(machineName, StringComparison.OrdinalIgnoreCase) &&
                m.RegisteredBy.Equals(registeredBy, StringComparison.OrdinalIgnoreCase));
        }
    }

    // -------------------------------
    // Dashboard Form (MODIFIED: Removed Assess Equipment button)
    // -------------------------------
    public class DashboardForm : Form
    {
        private Label lblGreeting;
        private Button btnLogout;
        private Button btnManageMachines;
        private User loggedInUser;

        public DashboardForm(User user)
        {
            loggedInUser = user;
            InitializeForm();
            CreateControls();
        }

        private void InitializeForm()
        {
            this.Text = "Dashboard - Predictive Maintenance System";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = Color.White;
        }

        private void CreateControls()
        {
            lblGreeting = new Label()
            {
                Text = $"Hello, {loggedInUser.Name}! Welcome to your Dashboard.",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80
            };
            this.Controls.Add(lblGreeting);

            // Button for managing machines (existing functionality + assessment)
            btnManageMachines = new Button()
            {
                Text = "Manage Machines",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.Blue,
                ForeColor = Color.White,
                Size = new Size(150, 40),
                Location = new Point((this.ClientSize.Width - 150) / 2, this.ClientSize.Height - 150),
                Anchor = AnchorStyles.Bottom
            };
            btnManageMachines.Click += BtnManageMachines_Click;
            this.Controls.Add(btnManageMachines);

            btnLogout = new Button()
            {
                Text = "Logout",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.Red,
                ForeColor = Color.White,
                Size = new Size(100, 40),
                Location = new Point((this.ClientSize.Width - 100) / 2, this.ClientSize.Height - 100),
                Anchor = AnchorStyles.Bottom
            };
            btnLogout.Click += BtnLogout_Click;
            this.Controls.Add(btnLogout);
        }

        private void BtnManageMachines_Click(object sender, EventArgs e)
        {
            MachineManagementForm machineForm = new MachineManagementForm(loggedInUser);
            machineForm.ShowDialog();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            SessionManager.ClearSession();
            this.Close();
        }
    }

    // -------------------------------
    // Machine Management Form with integrated assessment functionality
    // -------------------------------
    public class MachineManagementForm : Form
    {
        private TabControl tabControl;
        private TabPage tabAdd;
        private TabPage tabSearch;
        private TabPage tabAssess; // New tab for equipment assessment
        private User currentUser;

        // Controls for Add Machine Tab
        private TextBox txtMachineName;
        private TextBox txtMachineCode;
        private TextBox txtCNIC;
        private TextBox txtModel;
        private ComboBox cmbCondition;
        private TextBox txtRecoveryTime;
        private Button btnAddMachine;
        private ListView lvMachines;
        private ImageList conditionImages;

        // Controls for Search Machine Tab
        private TextBox txtSearchMachineCode;
        private TextBox txtSearchCNIC;
        private Button btnSearch;
        private Label lblSearchResult;

        // Controls for Assess Equipment Tab
        private TextBox txtAssessMachineName;
        private TextBox txtAssessMachineCode;
        private TextBox txtAssessCNIC;
        private TextBox txtAssessModel;
        private ComboBox cmbEquipType;
        private ComboBox cmbSubType;
        private Panel pnlProblems;
        private List<CheckBox> problemCheckBoxes = new List<CheckBox>();
        private PictureBox picCondition;
        private Button btnAssess;
        private Label lblInstructions;

        public MachineManagementForm(User user)
        {
            currentUser = user;
            this.Text = "Machine Management";
            this.Size = new Size(600, 500);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeTabs();
            MachineRepository.LoadMachines();
            InitializeConditionImages();
            PopulateMachineList();
        }

        private void InitializeTabs()
        {
            tabControl = new TabControl { Dock = DockStyle.Fill };
            tabAdd = new TabPage("Add Machine");
            tabSearch = new TabPage("Search Machine");
            tabAssess = new TabPage("Assess Equipment"); // New tab

            // Build Add Machine tab
            BuildAddMachineTab();
            // Build Search Machine tab
            BuildSearchMachineTab();
            // Build Assess Equipment tab (new)
            BuildAssessEquipmentTab();

            tabControl.TabPages.Add(tabAdd);
            tabControl.TabPages.Add(tabSearch);
            tabControl.TabPages.Add(tabAssess);
            this.Controls.Add(tabControl);
        }

        private void BuildAddMachineTab()
        {
            // Labels and Textboxes for machine details.
            Label lblName = new Label { Text = "Machine Name:", Left = 20, Top = 20, Width = 120 };
            txtMachineName = new TextBox { Left = 150, Top = 20, Width = 200 };

            Label lblCode = new Label { Text = "Machine Code:", Left = 20, Top = 60, Width = 120 };
            txtMachineCode = new TextBox { Left = 150, Top = 60, Width = 200 };

            Label lblCNIC = new Label { Text = "CNIC:", Left = 20, Top = 100, Width = 120 };
            txtCNIC = new TextBox { Left = 150, Top = 100, Width = 200 };

            Label lblModel = new Label { Text = "Model:", Left = 20, Top = 140, Width = 120 };
            txtModel = new TextBox { Left = 150, Top = 140, Width = 200 };

            Label lblCondition = new Label { Text = "Condition:", Left = 20, Top = 180, Width = 120 };
            cmbCondition = new ComboBox { Left = 150, Top = 180, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCondition.Items.AddRange(new string[] { "Operational", "Under Maintenance", "Out of Order" });
            cmbCondition.SelectedIndex = 0;
            cmbCondition.SelectedIndexChanged += CmbCondition_SelectedIndexChanged;

            Label lblRecovery = new Label { Text = "Expected Recovery Time:", Left = 20, Top = 220, Width = 120 };
            txtRecoveryTime = new TextBox { Left = 150, Top = 220, Width = 200 };
            txtRecoveryTime.Enabled = false; // Only enabled for non-operational machines.

            btnAddMachine = new Button
            {
                Text = "Add Machine",
                Left = 150,
                Top = 260,
                Width = 200,
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddMachine.Click += BtnAddMachine_Click;

            // ListView to show added machines with condition indicator.
            lvMachines = new ListView
            {
                Left = 20,
                Top = 310,
                Width = 540,
                Height = 130,
                View = View.Details,
                FullRowSelect = true,
                SmallImageList = new ImageList()
            };
            lvMachines.Columns.Add("Machine Name", 120);
            lvMachines.Columns.Add("Machine Code", 100);
            lvMachines.Columns.Add("Model", 100);
            lvMachines.Columns.Add("Registered Date", 120);
            lvMachines.Columns.Add("Condition", 100);

            tabAdd.Controls.Add(lblName);
            tabAdd.Controls.Add(txtMachineName);
            tabAdd.Controls.Add(lblCode);
            tabAdd.Controls.Add(txtMachineCode);
            tabAdd.Controls.Add(lblCNIC);
            tabAdd.Controls.Add(txtCNIC);
            tabAdd.Controls.Add(lblModel);
            tabAdd.Controls.Add(txtModel);
            tabAdd.Controls.Add(lblCondition);
            tabAdd.Controls.Add(cmbCondition);
            tabAdd.Controls.Add(lblRecovery);
            tabAdd.Controls.Add(txtRecoveryTime);
            tabAdd.Controls.Add(btnAddMachine);
            tabAdd.Controls.Add(lvMachines);
        }

        private void CmbCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Enable recovery time input only if condition is Under Maintenance or Out of Order.
            if (cmbCondition.SelectedItem.ToString() == "Operational")
                txtRecoveryTime.Enabled = false;
            else
                txtRecoveryTime.Enabled = true;
        }

        private void BtnAddMachine_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMachineName.Text) ||
                string.IsNullOrWhiteSpace(txtMachineCode.Text) ||
                string.IsNullOrWhiteSpace(txtCNIC.Text) ||
                string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Please fill in Machine Name, Code, CNIC and Model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if ((cmbCondition.SelectedItem.ToString() != "Operational") && string.IsNullOrWhiteSpace(txtRecoveryTime.Text))
            {
                MessageBox.Show("Please provide the expected recovery time for non-operational machines.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Machine newMachine = new Machine
            {
                Name = txtMachineName.Text.Trim(),
                MachineCode = txtMachineCode.Text.Trim(),
                CNIC = txtCNIC.Text.Trim(),
                Model = txtModel.Text.Trim(),
                RegistrationDate = DateTime.Now,
                Condition = cmbCondition.SelectedItem.ToString(),
                ExpectedRecoveryTime = (cmbCondition.SelectedItem.ToString() == "Operational") ? "" : txtRecoveryTime.Text.Trim(),
                RegisteredBy = currentUser.Email
            };

            MachineRepository.AddMachine(newMachine);
            MessageBox.Show("Machine added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearAddMachineInputs();
            PopulateMachineList();
        }

        private void ClearAddMachineInputs()
        {
            txtMachineName.Clear();
            txtMachineCode.Clear();
            txtCNIC.Clear();
            txtModel.Clear();
            cmbCondition.SelectedIndex = 0;
            txtRecoveryTime.Clear();
        }

        private void InitializeConditionImages()
        {
            // Create an ImageList for the condition icons (circles).
            conditionImages = new ImageList();
            conditionImages.ImageSize = new Size(16, 16);
            conditionImages.Images.Add("Operational", CreateColoredCircle(Color.Green));
            conditionImages.Images.Add("Under Maintenance", CreateColoredCircle(Color.Yellow));
            conditionImages.Images.Add("Out of Order", CreateColoredCircle(Color.Red));

            lvMachines.SmallImageList = conditionImages;
        }
        private Image CreateColoredCircle(Color color)
        {
            // Create a circle with the given color.
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, 0, 0, 15, 15);
                }
                using (Pen pen = new Pen(Color.Black, 1))
                {
                    g.DrawEllipse(pen, 0, 0, 15, 15);
                }
            }
            return bmp;
        }

        private void PopulateMachineList()
        {
            lvMachines.Items.Clear();
            foreach (var machine in MachineRepository.Machines.Where(m => m.RegisteredBy == currentUser.Email))
            {
                ListViewItem item = new ListViewItem(machine.Name);
                item.SubItems.Add(machine.MachineCode);
                item.SubItems.Add(machine.Model);
                item.SubItems.Add(machine.RegistrationDate.ToShortDateString());
                item.SubItems.Add(machine.Condition);

                // Set the image based on the condition.
                switch (machine.Condition)
                {
                    case "Operational":
                        item.ImageKey = "Operational";
                        break;
                    case "Under Maintenance":
                        item.ImageKey = "Under Maintenance";
                        break;
                    case "Out of Order":
                        item.ImageKey = "Out of Order";
                        break;
                }

                lvMachines.Items.Add(item);
            }
        }

        private void BuildSearchMachineTab()
        {
            // Create search controls.
            Label lblSearchCode = new Label { Text = "Machine Code:", Left = 20, Top = 30, Width = 120 };
            txtSearchMachineCode = new TextBox { Left = 150, Top = 30, Width = 200 };

            Label lblSearchCNIC = new Label { Text = "CNIC:", Left = 20, Top = 70, Width = 120 };
            txtSearchCNIC = new TextBox { Left = 150, Top = 70, Width = 200 };

            btnSearch = new Button
            {
                Text = "Search",
                Left = 150,
                Top = 110,
                Width = 200,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.Click += BtnSearch_Click;

            lblSearchResult = new Label
            {
                Text = "",
                Left = 20,
                Top = 160,
                Width = 550,
                Height = 200,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9),
                AutoSize = false
            };

            tabSearch.Controls.Add(lblSearchCode);
            tabSearch.Controls.Add(txtSearchMachineCode);
            tabSearch.Controls.Add(lblSearchCNIC);
            tabSearch.Controls.Add(txtSearchCNIC);
            tabSearch.Controls.Add(btnSearch);
            tabSearch.Controls.Add(lblSearchResult);
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchMachineCode.Text) && string.IsNullOrWhiteSpace(txtSearchCNIC.Text))
            {
                MessageBox.Show("Please enter either Machine Code or CNIC to search", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblSearchResult.Text = "";

            var results = MachineRepository.Machines.Where(m =>
                m.RegisteredBy == currentUser.Email &&
                ((!string.IsNullOrWhiteSpace(txtSearchMachineCode.Text) &&
                  m.MachineCode.Equals(txtSearchMachineCode.Text, StringComparison.OrdinalIgnoreCase)) ||
                 (!string.IsNullOrWhiteSpace(txtSearchCNIC.Text) &&
                  m.CNIC.Equals(txtSearchCNIC.Text, StringComparison.OrdinalIgnoreCase))));

            if (!results.Any())
            {
                lblSearchResult.Text = "No machines found matching the search criteria.";
                return;
            }

            // Display search results.
            foreach (var machine in results)
            {
                lblSearchResult.Text += $"Name: {machine.Name}\r\n";
                lblSearchResult.Text += $"Machine Code: {machine.MachineCode}\r\n";
                lblSearchResult.Text += $"CNIC: {machine.CNIC}\r\n";
                lblSearchResult.Text += $"Model: {machine.Model}\r\n";
                lblSearchResult.Text += $"Registration Date: {machine.RegistrationDate.ToShortDateString()}\r\n";
                lblSearchResult.Text += $"Condition: {machine.Condition}\r\n";
                if (!string.IsNullOrWhiteSpace(machine.ExpectedRecoveryTime))
                    lblSearchResult.Text += $"Expected Recovery Time: {machine.ExpectedRecoveryTime}\r\n";
                lblSearchResult.Text += "-------------------------------------\r\n";
            }
        }

        private void BuildAssessEquipmentTab()
        {
            // Create assessment controls.
            Label lblAssessInstructions = new Label
            {
                Text = "Enter your machine details and identify issues to assess maintenance needs:",
                Left = 20,
                Top = 10,
                Width = 550,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Label lblAssessName = new Label { Text = "Machine Name:", Left = 20, Top = 40, Width = 120 };
            txtAssessMachineName = new TextBox { Left = 150, Top = 40, Width = 200 };

            Label lblAssessCode = new Label { Text = "Machine Code:", Left = 20, Top = 70, Width = 120 };
            txtAssessMachineCode = new TextBox { Left = 150, Top = 70, Width = 200 };

            Label lblAssessCNIC = new Label { Text = "CNIC:", Left = 20, Top = 100, Width = 120 };
            txtAssessCNIC = new TextBox { Left = 150, Top = 100, Width = 200 };

            Label lblAssessModel = new Label { Text = "Model:", Left = 20, Top = 130, Width = 120 };
            txtAssessModel = new TextBox { Left = 150, Top = 130, Width = 200 };

            Label lblEquipType = new Label { Text = "Equipment Type:", Left = 20, Top = 160, Width = 120 };
            cmbEquipType = new ComboBox
            {
                Left = 150,
                Top = 160,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbEquipType.Items.AddRange(new string[] { "Hydraulic Machine", "Electrical Machine", "Mechanical Machine" });
            cmbEquipType.SelectedIndexChanged += CmbEquipType_SelectedIndexChanged;

            Label lblSubType = new Label { Text = "Sub Type:", Left = 20, Top = 190, Width = 120 };
            cmbSubType = new ComboBox
            {
                Left = 150,
                Top = 190,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblProblems = new Label { Text = "Problems:", Left = 20, Top = 220, Width = 120 };
            pnlProblems = new Panel
            {
                Left = 150,
                Top = 220,
                Width = 400,
                Height = 100,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            picCondition = new PictureBox
            {
                Left = 380,
                Top = 40,
                Width = 120,
                Height = 120,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            lblInstructions = new Label
            {
                Left = 20,
                Top = 340,
                Width = 550,
                Height = 60,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.PaleGoldenrod,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9)
            };

            btnAssess = new Button
            {
                Text = "Assess Equipment",
                Left = 150,
                Top = 410,
                Width = 200,
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAssess.Click += BtnAssess_Click;

            tabAssess.Controls.Add(lblAssessInstructions);
            tabAssess.Controls.Add(lblAssessName);
            tabAssess.Controls.Add(txtAssessMachineName);
            tabAssess.Controls.Add(lblAssessCode);
            tabAssess.Controls.Add(txtAssessMachineCode);
            tabAssess.Controls.Add(lblAssessCNIC);
            tabAssess.Controls.Add(txtAssessCNIC);
            tabAssess.Controls.Add(lblAssessModel);
            tabAssess.Controls.Add(txtAssessModel);
            tabAssess.Controls.Add(lblEquipType);
            tabAssess.Controls.Add(cmbEquipType);
            tabAssess.Controls.Add(lblSubType);
            tabAssess.Controls.Add(cmbSubType);
            tabAssess.Controls.Add(lblProblems);
            tabAssess.Controls.Add(pnlProblems);
            tabAssess.Controls.Add(picCondition);
            tabAssess.Controls.Add(lblInstructions);
            tabAssess.Controls.Add(btnAssess);
        }

        private void CmbEquipType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbSubType.Items.Clear();
            problemCheckBoxes.Clear();
            pnlProblems.Controls.Clear();

            if (cmbEquipType.SelectedItem == null)
                return;

            string equipType = cmbEquipType.SelectedItem.ToString();
            switch (equipType)
            {
                case "Hydraulic Machine":
                    cmbSubType.Items.AddRange(new string[] { "Pump", "Valve", "Cylinder", "Motor" });
                    break;
                case "Electrical Machine":
                    cmbSubType.Items.AddRange(new string[] { "Generator", "Transformer", "Motor", "Control Panel" });
                    break;
                case "Mechanical Machine":
                    cmbSubType.Items.AddRange(new string[] { "Engine", "Compressor", "Conveyor", "Gearbox" });
                    break;
            }

            if (cmbSubType.Items.Count > 0)
                cmbSubType.SelectedIndex = 0;

            // Show relevant problems based on equipment type
            PopulateProblems();
        }

        private void PopulateProblems()
        {
            pnlProblems.Controls.Clear();
            problemCheckBoxes.Clear();

            if (cmbEquipType.SelectedItem == null || cmbSubType.SelectedItem == null)
                return;

            string equipType = cmbEquipType.SelectedItem.ToString();
            string subType = cmbSubType.SelectedItem.ToString();
            List<string> problems = new List<string>();

            // Define problems for each equipment type and subtype
            if (equipType == "Hydraulic Machine")
            {
                problems.Add("Fluid Leakage");
                problems.Add("Pressure Loss");
                problems.Add("Overheating");
                problems.Add("Unusual Noise");
                problems.Add("Slow Operation");

                if (subType == "Pump")
                {
                    problems.Add("Cavitation");
                    problems.Add("Bearing Failure");
                }
                else if (subType == "Valve")
                {
                    problems.Add("Sticking");
                    problems.Add("Contamination");
                }
                else if (subType == "Cylinder")
                {
                    problems.Add("Seal Damage");
                    problems.Add("Rod Bending");
                }
            }
            else if (equipType == "Electrical Machine")
            {
                problems.Add("Overheating");
                problems.Add("Unusual Noise");
                problems.Add("Vibration");
                problems.Add("Power Fluctuations");

                if (subType == "Generator")
                {
                    problems.Add("Output Voltage Fluctuation");
                    problems.Add("Bearing Failure");
                }
                else if (subType == "Transformer")
                {
                    problems.Add("Insulation Breakdown");
                    problems.Add("Oil Leakage");
                }
                else if (subType == "Motor")
                {
                    problems.Add("Winding Damage");
                    problems.Add("Rotor Imbalance");
                }
            }
            else if (equipType == "Mechanical Machine")
            {
                problems.Add("Vibration");
                problems.Add("Unusual Noise");
                problems.Add("Overheating");
                problems.Add("Misalignment");

                if (subType == "Engine")
                {
                    problems.Add("Low Compression");
                    problems.Add("Fuel System Issues");
                }
                else if (subType == "Compressor")
                {
                    problems.Add("Pressure Loss");
                    problems.Add("Belt Issues");
                }
                else if (subType == "Gearbox")
                {
                    problems.Add("Gear Tooth Wear");
                    problems.Add("Oil Contamination");
                }
            }

            // Create checkboxes for each problem
            int yPosition = 10;
            foreach (var problem in problems)
            {
                CheckBox chkProblem = new CheckBox
                {
                    Text = problem,
                    Left = 10,
                    Top = yPosition,
                    Width = 360,
                    AutoSize = true
                };
                pnlProblems.Controls.Add(chkProblem);
                problemCheckBoxes.Add(chkProblem);
                yPosition += 25;
            }
        }

        private void BtnAssess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAssessMachineName.Text) ||
                string.IsNullOrWhiteSpace(txtAssessMachineCode.Text) ||
                cmbEquipType.SelectedItem == null ||
                cmbSubType.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all required fields.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if the machine exists for this user
            var machine = MachineRepository.SearchMachine(
                txtAssessMachineCode.Text,
                txtAssessMachineName.Text,
                currentUser.Email);

            if (machine == null)
            {
                MessageBox.Show("Machine not found. Please register this machine first.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Count checked problems
            int checkedProblems = problemCheckBoxes.Count(c => c.Checked);
            if (checkedProblems == 0)
            {
                MessageBox.Show("Please select at least one problem to assess.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Perform assessment
            string condition;
            string recoveryTime;
            string instructions;

            if (checkedProblems >= 4)
            {
                condition = "Out of Order";
                recoveryTime = "14 days";
                instructions = "Critical issues detected. Machine must be taken out of service immediately. " +
                              "Contact maintenance team for a complete overhaul.";
                picCondition.Image = CreateColoredCircle(Color.Red);
            }
            else if (checkedProblems >= 2)
            {
                condition = "Under Maintenance";
                recoveryTime = "7 days";
                instructions = "Significant issues detected. Schedule maintenance within 48 hours. " +
                              "Reduce operational load until repairs are completed.";
                picCondition.Image = CreateColoredCircle(Color.Yellow);
            }
            else
            {
                condition = "Operational with Caution";
                recoveryTime = "3 days";
                instructions = "Minor issues detected. Machine can remain operational but schedule " +
                              "maintenance within the week. Monitor the identified problems closely.";
                picCondition.Image = CreateColoredCircle(Color.Green);
            }

            // Update the machine's condition and recovery time
            machine.Condition = condition;
            machine.ExpectedRecoveryTime = recoveryTime;
            MachineRepository.SaveMachines();

            // Display assessment results
            lblInstructions.Text = instructions;

            // Refresh the machine list to show updated status
            PopulateMachineList();

            MessageBox.Show($"Assessment completed. Machine condition: {condition}", "Assessment Result",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // ------------------------------- 
    // Entry Point of the application.
    // -------------------------------
    public class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}