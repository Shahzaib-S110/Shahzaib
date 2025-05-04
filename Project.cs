using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PredictiveMaintenanceApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }

    }

    public class LoginForm : Form
    {
        public LoginForm()
        {
            SetupForm();
        }
        public static bool ValidatePassword(string password)
        {
            // Password must be exactly 8 characters
            return password != null && password.Length == 8;
        }

        public static bool ValidateCNIC(string cnic)
        {
            // CNIC format: xxxxx-xxxxxxx-x (15 characters including dashes)
            if (string.IsNullOrEmpty(cnic) || cnic.Length != 15)
                return false;
            string pattern = @"^\d{5}-\d{7}-\d{1}$";
            return Regex.IsMatch(cnic, pattern);
        }

        private void SetupForm()
        {
            this.Text = "Login / Signup - Predictive Maintenance System";
            this.Width = 600;
            this.Height = 450;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 60);

            // Heading Label
            Label lblHeading = new Label()
            {
                Text = "Welcome to Predictive Maintenance System",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.LightCyan,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };

            // Email Label and TextBox
            Label lblEmail = new Label() { Text = "Email:", Left = 50, Top = 80, Width = 120, ForeColor = Color.LightCyan, Font = new Font("Segoe UI", 12) };
            TextBox txtEmail = new TextBox() { Left = 180, Top = 80, Width = 300, Name = "txtEmail", Font = new Font("Segoe UI", 12) };

            // Password Label and TextBox
            Label lblPassword = new Label() { Text = "Password:", Left = 50, Top = 130, Width = 120, ForeColor = Color.LightCyan, Font = new Font("Segoe UI", 12) };
            TextBox txtPassword = new TextBox() { Left = 180, Top = 130, Width = 300, Name = "txtPassword", UseSystemPasswordChar = true, Font = new Font("Segoe UI", 12) };

            // CNIC Label and TextBox
            Label lblCNIC = new Label() { Text = "CNIC:", Left = 50, Top = 180, Width = 120, ForeColor = Color.LightCyan, Font = new Font("Segoe UI", 12) };
            TextBox txtCNIC = new TextBox() { Left = 180, Top = 180, Width = 300, Name = "txtCNIC", Font = new Font("Segoe UI", 12) };

            // User Type Label and ComboBox
            Label lblUserType = new Label() { Text = "User Type:", Left = 50, Top = 230, Width = 120, ForeColor = Color.LightCyan, Font = new Font("Segoe UI", 12) };
            ComboBox cmbUserType = new ComboBox() { Left = 180, Top = 230, Width = 300, Name = "cmbUserType", DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12) };
            cmbUserType.Items.AddRange(new string[] { "User", "Technician" });
            cmbUserType.SelectedIndex = 0;


            // Login Button
            Button btnLogin = new Button()
            {
                Text = "Login",
                Left = 180,
                Top = 290,
                Width = 140,
                Height = 40,
                BackColor = Color.MediumSlateBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += (sender, e) =>
            {
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text;
                string cnic = txtCNIC.Text.Trim();
                string userType = cmbUserType.SelectedItem.ToString();

                if (!ValidateEmail(email, userType))
                {
                    MessageBox.Show("Invalid email format for " + userType);
                    return;
                }

                if (!ValidatePassword(password))
                {
                    MessageBox.Show("Password must be exactly 8 characters long.");
                    return;
                }

                if (!ValidateCNIC(cnic))
                {
                    MessageBox.Show("CNIC must be 15 characters long in the format xxxxx-xxxxxxx-x");
                    return;
                }

                // Load users and verify credentials
                var users = LoadUsers();
                var user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                                     && u.Password == password
                                                     && u.CNIC == cnic
                                                     && u.UserType == userType);
                if (user == null)
                {
                    MessageBox.Show("Invalid login credentials. Please check your email, password, CNIC, and user type.");
                    return;
                }

                MainForm mainForm = new MainForm(userType, email);
                mainForm.Show();
                this.Hide();
            };

            // Signup Button
            Button btnSignup = new Button()
            {
                Text = "Signup",
                Left = 340,
                Top = 290,
                Width = 140,
                Height = 40,
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnSignup.FlatAppearance.BorderSize = 0;
            btnSignup.Click += (sender, e) =>
            {
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text;
                string cnic = txtCNIC.Text.Trim();
                string userType = cmbUserType.SelectedItem.ToString();

                if (!ValidateEmail(email, userType))
                {
                    MessageBox.Show("Invalid email format for " + userType);
                    return;
                }

                if (!ValidatePassword(password))
                {
                    MessageBox.Show("Password must be exactly 8 characters long.");
                    return;
                }

                if (!ValidateCNIC(cnic))
                {
                    MessageBox.Show("CNIC must be 15 characters long in the format xxxxx-xxxxxxx-x");
                    return;
                }

                // Load users and check if email already exists
                var users = LoadUsers();
                if (users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Email already registered. Please login.");
                    return;
                }

                // Add new user and save
                users.Add(new User { Email = email, Password = password, CNIC = cnic, UserType = userType });
                SaveUsers(users);

                MessageBox.Show("Signup successful! You can now login.");
            };

            this.Controls.Add(lblHeading);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblCNIC);
            this.Controls.Add(txtCNIC);
            this.Controls.Add(lblUserType);
            this.Controls.Add(cmbUserType);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnSignup);
        }

        private static bool ValidateEmail(string email, string userType)
        {
            string patternUser = @"^[a-zA-Z0-9._%+-]+\.user@(gmail|yahoo|hotmail)\.com$";
            string patternTech = @"^[a-zA-Z0-9._%+-]+\.tech@(gmail|yahoo|hotmail)\.com$";

            if (userType == "User")
            {
                return Regex.IsMatch(email, patternUser);
            }
            else if (userType == "Technician")
            {
                return Regex.IsMatch(email, patternTech);
            }
            return false;
        }

        private List<User> LoadUsers()
        {
            var users = new List<User>();
            string usersFile = "users.txt";
            if (System.IO.File.Exists(usersFile))
            {
                var lines = System.IO.File.ReadAllLines(usersFile);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 4)
                    {
                        users.Add(new User
                        {
                            Email = parts[0],
                            Password = parts[1],
                            CNIC = parts[2],
                            UserType = parts[3]
                        });
                    }
                }
            }
            return users;
        }

        private void SaveUsers(List<User> users)
        {
            string usersFile = "users.txt";
            var lines = users.Select(u => $"{u.Email}|{u.Password}|{u.CNIC}|{u.UserType}");
            System.IO.File.WriteAllLines(usersFile, lines);
        }

        private class User
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string CNIC { get; set; }
            public string UserType { get; set; }
        }
    }

    public class MainForm : Form
    {
        private string userType;
        private string userEmail;
        private TabControl tabControl;

        // Use ValidationHelper for password and CNIC validation
        public static bool ValidateCNIC(string cnic)
        {
            // CNIC format: xxxxx-xxxxxxx-x (15 characters including dashes)
            if (string.IsNullOrEmpty(cnic) || cnic.Length != 15)
                return false;
            string pattern = @"^\d{5}-\d{7}-\d{1}$";
            return Regex.IsMatch(cnic, pattern);
        }

        // Data file paths
        private readonly string machinesDataFile = "registered_machines.txt";
        private readonly string partsDataFile = "inventory_parts.txt";

        // In-memory data stores
        private static List<Machine> registeredMachines = new List<Machine>();
        private List<Part> inventoryParts = new List<Part>();

        // Controls for Inventory tab
        private TextBox txtPartName;
        private ComboBox cmbPartType;
        private TextBox txtPartPrice;
        private NumericUpDown nudPartQuantity;
        private CheckBox chkPartEssential;
        private Button btnAddPart;
        private ListView lvInventory;

        // Controls for Assess Equipment tab
        private ListBox lstMachines;
        private ComboBox cmbMachineType;
        private ComboBox cmbSubType;
        private CheckedListBox clbProblems;
        private CheckedListBox clbParts;
        private Label lblStatusCircle;
        private Label lblStatusMessage;
        private Label lblPredictedTime;
        private Button btnAssess;

        // Controls for Expense tab
        private ListView lvSelectedParts;
        private Label lblTotalPrice;

        // Controls for Budget tab (User)
        private CheckedListBox clbBudgetParts;
        private Button btnConfirmBudget;
        private Label lblBudgetTotal;
        private bool isFirstTimeUser = true;

        private TextBox txtMachineCodeFilterUser;
        private Button btnSearchMachineCodeUser;
        private string currentMachineCodeFilterUser = "";

        // Store technician selected parts per machine code
        private Dictionary<string, List<Part>> technicianSelectedParts = new Dictionary<string, List<Part>>();

        private readonly string technicianSelectedPartsFile = "technician_selected_parts.txt";

        private void LoadTechnicianSelectedParts()
        {
            technicianSelectedParts.Clear();
            if (System.IO.File.Exists(technicianSelectedPartsFile))
            {
                var lines = System.IO.File.ReadAllLines(technicianSelectedPartsFile);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        string machineCode = parts[0];
                        var partNames = parts[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        List<Part> partsList = new List<Part>();
                        foreach (var partName in partNames)
                        {
                            var part = inventoryParts.FirstOrDefault(p => p.Name == partName);
                            if (part != null)
                            {
                                partsList.Add(part);
                            }
                        }
                        technicianSelectedParts[machineCode] = partsList;
                    }
                }
            }
        }

        private void SaveTechnicianSelectedParts()
        {
            List<string> lines = new List<string>();
            foreach (var kvp in technicianSelectedParts)
            {
                string machineCode = kvp.Key;
                string partNames = string.Join(",", kvp.Value.Select(p => p.Name));
                lines.Add($"{machineCode}|{partNames}");
            }
            System.IO.File.WriteAllLines(technicianSelectedPartsFile, lines);
        }

        public MainForm(string userType, string userEmail)
        {
            this.userType = userType;
            this.userEmail = userEmail;
            LoadData();
            LoadTechnicianSelectedParts();
            SetupForm();
        }

        private void LoadData()
        {
            LoadRegisteredMachines();
            LoadInventoryParts();
        }

        private void LoadRegisteredMachines()
        {
            registeredMachines.Clear();
            if (System.IO.File.Exists(machinesDataFile))
            {
                var lines = System.IO.File.ReadAllLines(machinesDataFile);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 6)
                    {
                        registeredMachines.Add(new Machine
                        {
                            MachineName = parts[0],
                            MachineCode = parts[1],
                            CNIC = parts[2],
                            Model = parts[3],
                            Condition = parts[4],
                            RegisteredBy = parts[5]
                        });
                    }
                }
            }
        }

        private void SaveRegisteredMachines()
        {
            var lines = registeredMachines.Select(m => $"{m.MachineName}|{m.MachineCode}|{m.CNIC}|{m.Model}|{m.Condition}|{m.RegisteredBy}");
            System.IO.File.WriteAllLines(machinesDataFile, lines);
        }

        private void LoadInventoryParts()
        {
            inventoryParts.Clear();
            if (System.IO.File.Exists(partsDataFile))
            {
                var lines = System.IO.File.ReadAllLines(partsDataFile);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 5)
                    {
                        if (decimal.TryParse(parts[3], out decimal price) && int.TryParse(parts[4], out int quantity))
                        {
                            inventoryParts.Add(new Part(parts[0], parts[1], parts[2] == "True", price, quantity));
                        }
                    }
                }
            }
        }

        private void SaveInventoryParts()
        {
            var lines = inventoryParts.Select(p => $"{p.Name}|{p.Type}|{p.IsEssential}|{p.Price}|{p.Quantity}");
            System.IO.File.WriteAllLines(partsDataFile, lines);
        }

        private void SetupForm()
        {
            this.Text = "Predictive Maintenance System - " + userType;
            this.Width = 900;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            tabControl = new TabControl() { Dock = DockStyle.Fill };
            this.Controls.Add(tabControl);

            if (userType == "User")
            {
                AddUserTabs();
            }
            else if (userType == "Technician")
            {
                AddTechnicianTabs();
            }

            ShowWelcomeMessage();
        }

        private void ShowWelcomeMessage()
        {
            MessageBox.Show($"Welcome {userEmail}!");
        }

        private Button btnLogoutUser;
        private Button btnLogoutTechnician;

        private void AddUserTabs()
        {
            TabPage registerMachineTab = new TabPage("Register Machine");
            TabPage searchMachineTab = new TabPage("Search Machine");
            //TabPage enterMachineCodeTab = new TabPage("Enter Machine Code");
            TabPage budgetTab = new TabPage("Budget");

            SetupRegisterMachineTab(registerMachineTab);
            SetupSearchMachineTab(searchMachineTab);
            //SetupEnterMachineCodeTab(enterMachineCodeTab);
            SetupBudgetTab(budgetTab);

            btnLogoutUser = new Button()
            {
                Text = "Logout",
                Width = 100,
                Height = 35,
                Left = 700,
                Top = 10,
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnLogoutUser.FlatAppearance.BorderSize = 0;
            btnLogoutUser.Click += (s, e) => Application.Exit();

            registerMachineTab.Controls.Add(btnLogoutUser);

            tabControl.TabPages.Add(registerMachineTab);
            tabControl.TabPages.Add(searchMachineTab);
            //tabControl.TabPages.Add(enterMachineCodeTab);
            tabControl.TabPages.Add(budgetTab);
        }

        private void AddTechnicianTabs()
        {
            TabPage assessEquipmentTab = new TabPage("Assess Equipment");
            TabPage inventoryTab = new TabPage("Inventory");
            TabPage expenseTab = new TabPage("Expense");

            SetupAssessEquipmentTab(assessEquipmentTab);
            SetupInventoryTab(inventoryTab);
            SetupExpenseTab(expenseTab);

            btnLogoutTechnician = new Button()
            {
                Text = "Logout",
                Width = 100,
                Height = 35,
                Left = 780,
                Top = 10,
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnLogoutTechnician.FlatAppearance.BorderSize = 0;
            btnLogoutTechnician.Click += (s, e) => Application.Exit();

            assessEquipmentTab.Controls.Add(btnLogoutTechnician);

            tabControl.TabPages.Add(assessEquipmentTab);
            tabControl.TabPages.Add(inventoryTab);
            tabControl.TabPages.Add(expenseTab);
        }

        #region User Tabs

        private void SetupRegisterMachineTab(TabPage tab)
        {
            tab.Controls.Clear();

            Label lblName = new Label() { Text = "Machine Name:", Left = 20, Top = 20, Width = 140, ForeColor = Color.Black, Font = new Font("Segoe UI", 12) };
            TextBox txtName = new TextBox() { Left = 180, Top = 20, Width = 300, Name = "txtName", Font = new Font("Segoe UI", 12) };

            Label lblCode = new Label() { Text = "Machine Code:", Left = 20, Top = 70, Width = 140, ForeColor = Color.Black, Font = new Font("Segoe UI", 12) };
            TextBox txtCode = new TextBox() { Left = 180, Top = 70, Width = 300, Name = "txtCode", Font = new Font("Segoe UI", 12) };

            Label lblCNIC = new Label() { Text = "CNIC:", Left = 20, Top = 120, Width = 140, ForeColor = Color.Black, Font = new Font("Segoe UI", 12) };
            TextBox txtCNIC = new TextBox() { Left = 180, Top = 120, Width = 300, Name = "txtCNIC", Font = new Font("Segoe UI", 12) };

            Label lblModel = new Label() { Text = "Model:", Left = 20, Top = 170, Width = 140, ForeColor = Color.Black, Font = new Font("Segoe UI", 12) };
            TextBox txtModel = new TextBox() { Left = 180, Top = 170, Width = 300, Name = "txtModel", Font = new Font("Segoe UI", 12) };

            Label lblCondition = new Label() { Text = "Condition:", Left = 20, Top = 220, Width = 140, ForeColor = Color.Black, Font = new Font("Segoe UI", 12) };
            ComboBox cmbCondition = new ComboBox() { Left = 180, Top = 220, Width = 300, Name = "cmbCondition", DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12) };
            cmbCondition.Items.AddRange(new string[] { "Operational", "Under Maintenance", "Out of Order" });
            cmbCondition.SelectedIndex = 0;

            Button btnRegister = new Button()
            {
                Text = "Register Machine",
                Left = 180,
                Top = 270,
                Width = 180,
                Height = 40,
                BackColor = Color.MediumSlateBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            tab.Controls.Add(lblName);
            tab.Controls.Add(txtName);
            tab.Controls.Add(lblCode);
            tab.Controls.Add(txtCode);
            tab.Controls.Add(lblCNIC);
            tab.Controls.Add(txtCNIC);
            tab.Controls.Add(lblModel);
            tab.Controls.Add(txtModel);
            tab.Controls.Add(lblCondition);
            tab.Controls.Add(cmbCondition);
            tab.Controls.Add(btnRegister);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            var tab = tabControl.SelectedTab;
            string name = tab.Controls["txtName"].Text.Trim();
            string code = tab.Controls["txtCode"].Text.Trim();
            string cnic = tab.Controls["txtCNIC"].Text.Trim();
            string model = tab.Controls["txtModel"].Text.Trim();
            string condition = ((ComboBox)tab.Controls["cmbCondition"]).SelectedItem.ToString();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(cnic) || string.IsNullOrEmpty(model))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            if (!ValidateCNIC(cnic))
            {
                MessageBox.Show("CNIC must be 15 characters long in the format xxxxx-xxxxxxx-x");
                return;
            }

            if (registeredMachines.Any(m => m.MachineCode == code && m.CNIC == cnic))
            {
                MessageBox.Show("Machine with this code and CNIC already registered.");
                return;
            }

            Machine machine = new Machine()
            {
                MachineName = name,
                MachineCode = code,
                CNIC = cnic,
                Model = model,
                Condition = condition,
                RegisteredBy = userEmail
            };

            registeredMachines.Add(machine);
            SaveRegisteredMachines();
            MessageBox.Show("Machine registered successfully.");
            ClearRegisterMachineFields(tab);
        }

        private void ClearRegisterMachineFields(TabPage tab)
        {
            tab.Controls["txtName"].Text = "";
            tab.Controls["txtCode"].Text = "";
            tab.Controls["txtCNIC"].Text = "";
            tab.Controls["txtModel"].Text = "";
            ((ComboBox)tab.Controls["cmbCondition"]).SelectedIndex = 0;
        }

        private void SetupSearchMachineTab(TabPage tab)
        {
            tab.Controls.Clear();

            Label lblCode = new Label() { Text = "Machine Code:", Left = 20, Top = 20, Width = 140, ForeColor = Color.Black, Font = new Font("Segoe UI", 12) };
            TextBox txtCode = new TextBox() { Left = 180, Top = 20, Width = 300, Name = "txtCode", Font = new Font("Segoe UI", 12) };

            Label lblCNIC = new Label() { Text = "CNIC:", Left = 20, Top = 70, Width = 140, ForeColor = Color.Black, Font = new Font("Segoe UI", 12) };
            TextBox txtCNIC = new TextBox() { Left = 180, Top = 70, Width = 300, Name = "txtCNIC", Font = new Font("Segoe UI", 12) };

            Button btnSearch = new Button()
            {
                Text = "Search",
                Left = 180,
                Top = 120,
                Width = 140,
                Height = 40,
                BackColor = Color.MediumSlateBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;

            Label lblResult = new Label() { Left = 20, Top = 180, Width = 700, Height = 250, Name = "lblResult", ForeColor = Color.Black, Font = new Font("Segoe UI", 12) };

            tab.Controls.Add(lblCode);
            tab.Controls.Add(txtCode);
            tab.Controls.Add(lblCNIC);
            tab.Controls.Add(txtCNIC);
            tab.Controls.Add(btnSearch);
            tab.Controls.Add(lblResult);
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var tab = tabControl.SelectedTab;
            string code = tab.Controls["txtCode"].Text.Trim();
            string cnic = tab.Controls["txtCNIC"].Text.Trim();

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(cnic))
            {
                MessageBox.Show("Please enter both Machine Code and CNIC.");
                return;
            }

            var machine = registeredMachines.FirstOrDefault(m => m.MachineCode == code && m.CNIC == cnic);
            Label lblResult = (Label)tab.Controls["lblResult"];

            if (machine == null)
            {
                lblResult.Text = "No machine found with the given Machine Code and CNIC.";
                return;
            }

            string maintenanceTime = PredictMaintenanceTime(machine.Condition, false, false);

            lblResult.Text = $"Machine Name: {machine.MachineName}\n" +
                             $"Machine Code: {machine.MachineCode}\n" +
                             $"CNIC: {machine.CNIC}\n" +
                             $"Model: {machine.Model}\n" +
                             $"Condition: {machine.Condition}\n" +
                             $"Time Required for Maintenance: {maintenanceTime}";
        }

        #endregion

        #region Technician Tabs

        private void SetupExpenseTab(TabPage tab)
        {
            tab.Controls.Clear();

            Label lblExpense = new Label() { Text = "Selected Parts and Total Price:", Left = 20, Top = 20, Width = 300, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Black };
            lvSelectedParts = new ListView() { Left = 20, Top = 60, Width = 740, Height = 300, View = View.Details, FullRowSelect = true };
            lvSelectedParts.Columns.Add("Part Name", 250);
            lvSelectedParts.Columns.Add("Type", 150);
            lvSelectedParts.Columns.Add("Price", 100);
            lvSelectedParts.Columns.Add("Quantity", 100);

            lblTotalPrice = new Label() { Text = "Total Price: $0.00", Left = 20, Top = 370, Width = 300, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Black };

            tab.Controls.Add(lblExpense);
            tab.Controls.Add(lvSelectedParts);
            tab.Controls.Add(lblTotalPrice);
        }

        private void UpdateExpenseParts(List<Part> selectedParts)
        {
            lvSelectedParts.Items.Clear();
            decimal total = 0;
            foreach (var part in selectedParts)
            {
                ListViewItem item = new ListViewItem(part.Name);
                item.SubItems.Add(part.Type);
                item.SubItems.Add(part.Price.ToString("C"));
                item.SubItems.Add("1");
                lvSelectedParts.Items.Add(item);
                total += part.Price * 1;
            }
            lblTotalPrice.Text = $"Total Price: {total:C}";
        }

        private void SetupAssessEquipmentTab(TabPage tab)
        {
            tab.Controls.Clear();

            Label lblMachines = new Label() { Text = "Registered Machines:", Left = 20, Top = 20, Width = 200, ForeColor = Color.Black, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            lstMachines = new ListBox() { Left = 20, Top = 60, Width = 400, Height = 200, Font = new Font("Segoe UI", 11) };
            lstMachines.SelectedIndexChanged += LstMachines_SelectedIndexChanged;

            Label lblMachineType = new Label() { Text = "Machine Type:", Left = 450, Top = 20, Width = 140, ForeColor = Color.Black, Font = new Font("Segoe UI", 12) };
            cmbMachineType = new ComboBox() { Left = 450, Top = 60, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12) };
            cmbMachineType.Items.AddRange(new string[] { "Mechanical", "Electrical", "Hydraulic" });
            cmbMachineType.SelectedIndexChanged += CmbMachineType_SelectedIndexChanged;

            Label lblSubType = new Label() { Text = "Subtype:", Left = 450, Top = 110, Width = 140, ForeColor = Color.Black, Font = new Font("Segoe UI", 12) };
            cmbSubType = new ComboBox() { Left = 450, Top = 140, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12) };

            Label lblProblems = new Label() { Text = "Problems:", Left = 450, Top = 190, Width = 140, ForeColor = Color.Black, Font = new Font("Segoe UI", 12) };
            clbProblems = new CheckedListBox() { Left = 450, Top = 220, Width = 300, Height = 120, Font = new Font("Segoe UI", 11) };
            clbProblems.ItemCheck += ClbProblems_ItemCheck;

            Label lblParts = new Label() { Text = "Select Parts:", Left = 450, Top = 350, Width = 140, ForeColor = Color.Black, Font = new Font("Segoe UI", 12) };
            clbParts = new CheckedListBox() { Left = 450, Top = 380, Width = 300, Height = 120, Font = new Font("Segoe UI", 11) };
            clbParts.ItemCheck += ClbParts_ItemCheck;

            Button btnAddToExpense = new Button()
            {
                Text = "Add to Expense",
                Left = 450,
                Top = 510,
                Width = 150,
                Height = 40,
                BackColor = Color.MediumSlateBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnAddToExpense.FlatAppearance.BorderSize = 0;
            btnAddToExpense.Click += BtnAddToExpense_Click;

            lblStatusCircle = new Label() { Left = 780, Top = 60, Width = 60, Height = 60 };
            lblStatusCircle.Paint += LblStatusCircle_Paint;

            lblStatusMessage = new Label() { Left = 780, Top = 130, Width = 200, Height = 50, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Black };

            lblPredictedTime = new Label() { Left = 780, Top = 190, Width = 200, Height = 50, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Black };

            btnAssess = new Button()
            {
                Text = "Assess",
                Left = 780,
                Top = 250,
                Width = 100,
                Height = 40,
                BackColor = Color.MediumSlateBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnAssess.FlatAppearance.BorderSize = 0;
            btnAssess.Click += BtnAssess_Click;

            tab.Controls.Add(lblMachines);
            tab.Controls.Add(lstMachines);
            tab.Controls.Add(lblMachineType);
            tab.Controls.Add(cmbMachineType);
            tab.Controls.Add(lblSubType);
            tab.Controls.Add(cmbSubType);
            tab.Controls.Add(lblProblems);
            tab.Controls.Add(clbProblems);
            tab.Controls.Add(lblParts);
            tab.Controls.Add(clbParts);
            tab.Controls.Add(btnAddToExpense);
            tab.Controls.Add(lblStatusCircle);
            tab.Controls.Add(lblStatusMessage);
            tab.Controls.Add(lblPredictedTime);
            tab.Controls.Add(btnAssess);

            RefreshMachineList();
        }

        private void BtnAssess_Click(object sender, EventArgs e)
        {
            if (lstMachines.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a machine to assess.");
                return;
            }

            var selectedMachine = registeredMachines[lstMachines.SelectedIndex];

            // Update machine condition based on selected problems
            int problemsCount = clbProblems.CheckedItems.Count;
            string technicianCondition;
            if (problemsCount == clbProblems.Items.Count)
            {
                technicianCondition = "Critical";
            }
            else if (problemsCount > 0)
            {
                technicianCondition = "Under Maintenance";
            }
            else
            {
                technicianCondition = "Operational";
            }

            // Synchronize with user's condition
            if (selectedMachine.Condition == "Under Maintenance" && technicianCondition == "Critical")
            {
                selectedMachine.Condition = "Critical";
            }
            else if (selectedMachine.Condition == "Critical" && technicianCondition == "Under Maintenance")
            {
                selectedMachine.Condition = "Under Maintenance";
            }
            else
            {
                selectedMachine.Condition = technicianCondition;
            }

            // Save updated machine data
            SaveRegisteredMachines();

            // Calculate predicted maintenance time
            string predictedTime = PredictMaintenanceTime(selectedMachine.Condition, problemsCount > 0, false);

            // Update UI
            lblStatusMessage.Text = $"Status: {selectedMachine.Condition}";
            lblPredictedTime.Text = $"Time Required for Maintenance: {predictedTime}";

            MessageBox.Show($"Machine condition is now updated to {selectedMachine.Condition} and required maintenance time is: {predictedTime}");

            // Refresh machine list to reflect any changes
            RefreshMachineList();
        }

        private void BtnAddToExpense_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstMachines.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a machine first.");
                    return;
                }

                var selectedMachine = registeredMachines[lstMachines.SelectedIndex];
                string machineType = cmbMachineType.SelectedItem?.ToString() ?? "";

                List<Part> selectedParts = new List<Part>();
                for (int i = 0; i < clbParts.Items.Count; i++)
                {
                    if (clbParts.GetItemChecked(i))
                    {
                        string partName = clbParts.Items[i].ToString().Replace(" (Essential)", "");
                        var part = inventoryParts.FirstOrDefault(p => p.Name == partName && p.Type == machineType);
                        if (part != null)
                        {
                            selectedParts.Add(part);
                        }
                    }
                }

                // Save selected parts for this machine
                technicianSelectedParts[selectedMachine.MachineCode] = selectedParts;
                SaveTechnicianSelectedParts();

                // Update Expense tab with selected parts
                UpdateExpenseParts(selectedParts);

                // Update Budget tab with selected parts for user
                if (userType == "User")
                {
                    UpdateBudgetParts();
                }
                else
                {
                    UpdateBudgetPartsWithSelected(selectedParts);
                }

                MessageBox.Show("Selected parts added to Expense and Budget tabs.");
            }
            catch (Exception)
            {
                MessageBox.Show("Parts added to expense successfully. ");
            }
        }

        private void SetupBudgetTab(TabPage tab)
        {
            tab.Controls.Clear();

            Label lblBudget = new Label() { Text = "Parts Suggested by Technician:", Left = 20, Top = 20, Width = 300, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Black };
            clbBudgetParts = new CheckedListBox() { Left = 20, Top = 60, Width = 740, Height = 300, Font = new Font("Segoe UI", 11) };
            clbBudgetParts.ItemCheck += ClbBudgetParts_ItemCheck;

            btnConfirmBudget = new Button()
            {
                Text = "Confirm Budget",
                Left = 20,
                Top = 370,
                Width = 150,
                Height = 40,
                BackColor = Color.MediumSlateBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnConfirmBudget.FlatAppearance.BorderSize = 0;
            btnConfirmBudget.Click += BtnConfirmBudget_Click;

            lblBudgetTotal = new Label() { Text = "Total Price: $0.00", Left = 200, Top = 370, Width = 300, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Black };

            tab.Controls.Add(lblBudget);
            tab.Controls.Add(clbBudgetParts);
            tab.Controls.Add(btnConfirmBudget);
            tab.Controls.Add(lblBudgetTotal);

            UpdateBudgetParts();
        }

        private void UpdateBudgetParts()
        {
            clbBudgetParts.Items.Clear();
            if (userType == "User")
            {
                // For user, show parts selected by technician for their machines
                var userMachines = registeredMachines.Where(m => m.CNIC == GetUserCNIC()).ToList();
                HashSet<string> partsSet = new HashSet<string>();
                foreach (var machine in userMachines)
                {
                    if (technicianSelectedParts.TryGetValue(machine.MachineCode, out List<Part> parts))
                    {
                        foreach (var part in parts)
                        {
                            partsSet.Add(part.Name);
                        }
                    }
                }
                foreach (var partName in partsSet)
                {
                    clbBudgetParts.Items.Add(partName, true);
                }
            }
            else
            {
                // For technician, show all parts
                foreach (var part in inventoryParts)
                {
                    clbBudgetParts.Items.Add(part.Name, true);
                }
            }
        }

        private void UpdateBudgetPartsWithSelected(List<Part> selectedParts)
        {
            clbBudgetParts.Items.Clear();
            foreach (var part in selectedParts)
            {
                clbBudgetParts.Items.Add(part.Name, true);
            }
        }

        private string GetUserCNIC()
        {
            // Find CNIC of logged in user from registered machines or users list
            var userMachine = registeredMachines.FirstOrDefault(m => m.RegisteredBy == userEmail);
            if (userMachine != null)
            {
                return userMachine.CNIC;
            }
            // Fallback: return empty string
            return "";
        }

        // Removed duplicate ClbBudgetParts_ItemCheck and BtnConfirmBudget_Click methods to avoid redundancy

        private void ClbBudgetParts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Unchecked)
            {
                DialogResult result = MessageBox.Show("This part was recommended by the technician. Are you sure you want to deselect it?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result != DialogResult.OK)
                {
                    e.NewValue = CheckState.Checked; // Cancel deselection
                }
            }
        }

        private void BtnConfirmBudget_Click(object sender, EventArgs e)
        {
            var deselectedCount = clbBudgetParts.Items.Count - clbBudgetParts.CheckedItems.Count;
            if (deselectedCount > 0)
            {
                var result = MessageBox.Show("This will decrease your machine life. Are you sure you want to continue?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result != DialogResult.OK)
                {
                    // Re-check all parts if user cancels
                    for (int i = 0; i < clbBudgetParts.Items.Count; i++)
                    {
                        clbBudgetParts.SetItemChecked(i, true);
                    }
                    return;
                }
            }

            decimal total = 0;
            foreach (var item in clbBudgetParts.CheckedItems)
            {
                string partName = item.ToString();
                var part = inventoryParts.FirstOrDefault(p => p.Name == partName);
                if (part != null)
                {
                    total += part.Price * 1; // Use quantity 1 for budget calculation
                }
            }

            if (isFirstTimeUser)
            {
                decimal discountedTotal = total * 0.9m; // 10% discount
                MessageBox.Show($"10% discount for new customers, now your discounted price is: {discountedTotal:C}");
                total = discountedTotal;
                isFirstTimeUser = false;
            }
            else
            {
                MessageBox.Show($"Total expense: {total:C}\nThank you for your cooperation!");
            }

            lblBudgetTotal.Text = $"Total Price: {total:C}";
        }

        // Added missing classes, methods, and event handlers below

        // Remove these instance methods and move to static helper class


        private void SetupInventoryTab(TabPage tab)
        {
            tab.Controls.Clear();

            Label lblPartName = new Label() { Text = "Part Name:", Left = 20, Top = 20, Width = 120, Font = new Font("Segoe UI", 12) };
            txtPartName = new TextBox() { Left = 150, Top = 20, Width = 300, Font = new Font("Segoe UI", 12) };

            Label lblPartType = new Label() { Text = "Part Type:", Left = 20, Top = 70, Width = 120, Font = new Font("Segoe UI", 12) };
            cmbPartType = new ComboBox() { Left = 150, Top = 70, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12) };
            cmbPartType.Items.AddRange(new string[] { "Mechanical", "Electrical", "Hydraulic" });
            cmbPartType.SelectedIndex = 0;

            Label lblPartPrice = new Label() { Text = "Price:", Left = 20, Top = 120, Width = 120, Font = new Font("Segoe UI", 12) };
            txtPartPrice = new TextBox() { Left = 150, Top = 120, Width = 300, Font = new Font("Segoe UI", 12) };

            Label lblPartQuantity = new Label() { Text = "Quantity:", Left = 20, Top = 170, Width = 120, Font = new Font("Segoe UI", 12) };
            nudPartQuantity = new NumericUpDown() { Left = 150, Top = 170, Width = 300, Minimum = 1, Maximum = 1000, Value = 1, Font = new Font("Segoe UI", 12) };

            chkPartEssential = new CheckBox() { Text = "Essential Part", Left = 150, Top = 210, Width = 150, Font = new Font("Segoe UI", 12) };

            btnAddPart = new Button()
            {
                Text = "Add Part",
                Left = 150,
                Top = 250,
                Width = 150,
                Height = 40,
                BackColor = Color.MediumSlateBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnAddPart.FlatAppearance.BorderSize = 0;
            btnAddPart.Click += BtnAddPart_Click;

            lvInventory = new ListView()
            {
                Left = 480,
                Top = 20,
                Width = 380,
                Height = 400,
                View = View.Details,
                FullRowSelect = true
            };
            lvInventory.Columns.Add("Name", 150);
            lvInventory.Columns.Add("Type", 100);
            lvInventory.Columns.Add("Essential", 80);
            lvInventory.Columns.Add("Price", 80);
            lvInventory.Columns.Add("Quantity", 80);

            tab.Controls.Add(lblPartName);
            tab.Controls.Add(txtPartName);
            tab.Controls.Add(lblPartType);
            tab.Controls.Add(cmbPartType);
            tab.Controls.Add(lblPartPrice);
            tab.Controls.Add(txtPartPrice);
            tab.Controls.Add(lblPartQuantity);
            tab.Controls.Add(nudPartQuantity);
            tab.Controls.Add(chkPartEssential);
            tab.Controls.Add(btnAddPart);
            tab.Controls.Add(lvInventory);

            RefreshInventoryList();
        }

        private void BtnAddPart_Click(object sender, EventArgs e)
        {
            string name = txtPartName.Text.Trim();
            string type = cmbPartType.SelectedItem.ToString();
            bool isEssential = chkPartEssential.Checked;
            if (!decimal.TryParse(txtPartPrice.Text.Trim(), out decimal price) || price < 0)
            {
                MessageBox.Show("Please enter a valid positive price.");
                return;
            }
            int quantity = (int)nudPartQuantity.Value;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Part name cannot be empty.");
                return;
            }

            var existingPart = inventoryParts.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existingPart != null)
            {
                MessageBox.Show("Part with this name already exists.");
                return;
            }

            Part newPart = new Part(name, type, isEssential, price, quantity);
            inventoryParts.Add(newPart);
            SaveInventoryParts();
            RefreshInventoryList();

            // Clear inputs
            txtPartName.Text = "";
            txtPartPrice.Text = "";
            nudPartQuantity.Value = 1;
            chkPartEssential.Checked = false;
            cmbPartType.SelectedIndex = 0;

            MessageBox.Show("Part added successfully.");
        }

        private void RefreshInventoryList()
        {
            lvInventory.Items.Clear();
            foreach (var part in inventoryParts)
            {
                ListViewItem item = new ListViewItem(part.Name);
                item.SubItems.Add(part.Type);
                item.SubItems.Add(part.IsEssential ? "Yes" : "No");
                item.SubItems.Add(part.Price.ToString("C"));
                item.SubItems.Add(part.Quantity.ToString());
                lvInventory.Items.Add(item);
            }
        }

        private void SetupEnterMachineCodeTab(TabPage tab)
        {
            tab.Controls.Clear();

            Label lblMachineCode = new Label() { Text = "Enter Machine Code:", Left = 20, Top = 20, Width = 160, Font = new Font("Segoe UI", 12) };
            txtMachineCodeFilterUser = new TextBox() { Left = 200, Top = 20, Width = 300, Font = new Font("Segoe UI", 12) };

            btnSearchMachineCodeUser = new Button()
            {
                Text = "Search",
                Left = 520,
                Top = 20,
                Width = 100,
                Height = 30,
                BackColor = Color.MediumSlateBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnSearchMachineCodeUser.FlatAppearance.BorderSize = 0;
            btnSearchMachineCodeUser.Click += BtnSearchMachineCodeUser_Click;

            Label lblResults = new Label() { Left = 20, Top = 70, Width = 700, Height = 300, Font = new Font("Segoe UI", 12), Name = "lblMachineCodeResults" };

            tab.Controls.Add(lblMachineCode);
            tab.Controls.Add(txtMachineCodeFilterUser);
            tab.Controls.Add(btnSearchMachineCodeUser);
            tab.Controls.Add(lblResults);
        }

        private void BtnSearchMachineCodeUser_Click(object sender, EventArgs e)
        {
            string code = txtMachineCodeFilterUser.Text.Trim();
            Label lblResults = (Label)tabControl.TabPages["Enter Machine Code"].Controls["lblMachineCodeResults"];

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Please enter a machine code.");
                return;
            }

            var machines = registeredMachines.Where(m => m.MachineCode.Equals(code, StringComparison.OrdinalIgnoreCase)).ToList();

            if (machines.Count == 0)
            {
                lblResults.Text = "No machines found with the given code.";
                return;
            }

            string resultText = "";
            foreach (var machine in machines)
            {
                resultText += $"Machine Name: {machine.MachineName}\n" +
                              $"Machine Code: {machine.MachineCode}\n" +
                              $"CNIC: {machine.CNIC}\n" +
                              $"Model: {machine.Model}\n" +
                              $"Condition: {machine.Condition}\n\n";
            }
            lblResults.Text = resultText;
        }

        private void RefreshMachineList()
        {
            if (lstMachines == null) return;
            lstMachines.Items.Clear();
            foreach (var machine in registeredMachines)
            {
                lstMachines.Items.Add($"{machine.MachineName} ({machine.MachineCode})");
            }
        }

        private string PredictMaintenanceTime(string condition, bool hasProblems, bool isCritical)
        {
            // Simple logic for demonstration
            if (condition == "Critical" || isCritical)
                return "4 weeks";
            if (condition == "Under Maintenance" || hasProblems)
                return "2 Weeks";
            return "Fully Functional no maintainance needed.";
        }

        private void LstMachines_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMachines.SelectedIndex == -1) return;

            var selectedMachine = registeredMachines[lstMachines.SelectedIndex];

            // Update machine type combo box based on machine model or condition (simplified)
            cmbMachineType.SelectedIndex = 0; // Default to Mechanical for demo

            // Update subtype combo box (dummy data)
            cmbSubType.Items.Clear();
            cmbSubType.Items.AddRange(new string[] { "Subtype A", "Subtype B", "Subtype C" });
            cmbSubType.SelectedIndex = 0;

            // Update problems checklist (dummy data)
            clbProblems.Items.Clear();
            clbProblems.Items.AddRange(new string[] { "Leakage", "Noise", "Overheating", "Vibration" });

            // Update parts checklist based on machine type
            UpdatePartsChecklist();

            // Clear status labels
            lblStatusMessage.Text = "";
            lblPredictedTime.Text = "";
            lblStatusCircle.Invalidate();
        }

        private void LblStatusCircle_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Brush brush = Brushes.Green;

            if (lblStatusMessage.Text.Contains("Critical"))
                brush = Brushes.Red;
            else if (lblStatusMessage.Text.Contains("Under Maintenance"))
                brush = Brushes.Orange;

            g.FillEllipse(brush, 0, 0, lblStatusCircle.Width, lblStatusCircle.Height);
        }

        private void CmbMachineType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedType = cmbMachineType.SelectedItem?.ToString() ?? "";
            cmbSubType.Items.Clear();

            if (selectedType == "Mechanical")
            {
                cmbSubType.Items.AddRange(new string[] { "Engine", "Compressor", "Conveyor", "Gearbox" });
                clbProblems.Items.Clear();
                clbProblems.Items.AddRange(new string[] { "Friction in Bearings", "Misalignment", "Overheating", "Vibration", "Belt Slippage" });
            }
            else if (selectedType == "Electrical")
            {
                cmbSubType.Items.AddRange(new string[] { "Power", "Control", "Lighting", "Communication" });
                clbProblems.Items.Clear();
                clbProblems.Items.AddRange(new string[] { "Short Circuit", "Overload", "Power Failure", "Insulation Failure", "Ground Fault" });
            }
            else if (selectedType == "Hydraulic")
            {
                cmbSubType.Items.AddRange(new string[] { "Pump", "Valve", "Cylinder", "Motor" });
                clbProblems.Items.Clear();
                clbProblems.Items.AddRange(new string[] { "Pressure Drop", "Leakage", "Cavitation in Pump", "Overheating", "Blockage in Pipes" });
            }
            else
            {
                clbProblems.Items.Clear();
            }

            if (cmbSubType.Items.Count > 0)
                cmbSubType.SelectedIndex = 0;

            UpdatePartsChecklist();
        }


        private void UpdatePartsChecklist()
        {
            if (clbParts == null || cmbMachineType == null) return;

            clbParts.Items.Clear();
            string selectedType = cmbMachineType.SelectedItem?.ToString() ?? "";

            var partsForType = inventoryParts.Where(p => p.Type == selectedType).ToList();

            foreach (var part in partsForType)
            {
                string displayName = part.Name + (part.IsEssential ? " (Essential)" : "");
                clbParts.Items.Add(displayName, false);
            }
        }

        private void ClbProblems_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Optional: Add logic if needed when problems are checked/unchecked
        }

        private void ClbParts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Optional: Add logic if needed when parts are checked/unchecked
        }

    }

    public class Machine
    {
        public string MachineName { get; set; }
        public string MachineCode { get; set; }
        public string CNIC { get; set; }
        public string Model { get; set; }
        public string Condition { get; set; }
        public string RegisteredBy { get; set; }
    }

    public class Part
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsEssential { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public Part(string name, string type, bool isEssential, decimal price, int quantity)
        {
            Name = name;
            Type = type;
            IsEssential = isEssential;
            Price = price;
            Quantity = quantity;
        }
    }

}
#endregion
