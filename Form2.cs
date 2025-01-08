using FirebirdSql.Data.FirebirdClient;
using System.Data;
using System.Windows.Forms;
using System;
namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        FbConnection fbCon;
        DataSet dsDiagnoses;
        readonly string sqlDiagnoses = "SELECT * FROM Diagnoses WHERE IsDeleted = 0";
        private DataTable orglDataTable1;
        private DataTable orglDataTable2;

        public Form2()
        {
            InitializeComponent();
            dateTimePickerStart.MinDate = new DateTime(2020, 1, 1);
            dateTimePickerEnd.MaxDate = new DateTime(2030, 12, 31);

            // Исправлено: подписка на событие Click кнопки
            this.button5.Click += new System.EventHandler(this.button5_Click_1);
        }

        void Form2_Load(object sender, EventArgs e)
        {
            // Инициализация строки подключения
            var fb_cons = new FbConnectionStringBuilder
            {
                DataSource = "localhost",
                Port = 3050,
                Role = "",
                Dialect = 3,
                Charset = "WIN1251",
                UserID = "SYSDBA",
                Password = "masterkey",
                Database = @"C:\Users\katerina\Desktop\Новая папка\3 курс\Курсовая работа. Базы данных\Базы данных\1.5.2\CLINICA.FDB"
            };
            fbCon = new FbConnection(fb_cons.ToString());
            try
            {
                fbCon.Open();
                LoadData();
                LoadDoctors();
                LoadPatients();
                LoadDiagnoses();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}");
            }
            finally
            {
                // Закрытие соединения, если оно открыто
                if (fbCon.State == ConnectionState.Open)
                {
                    fbCon.Close();
                }
            }
        }
        private void LoadDiagnoses()
        {
            using (FbDataAdapter adapterPatients = new FbDataAdapter(sqlDiagnoses, fbCon))
            {
                dsDiagnoses = new DataSet();
                adapterPatients.Fill(dsDiagnoses);
                orglDataTable2 = dsDiagnoses.Tables[0].Copy();
                // Устанавливаем источник данных для DataGridView2
                dataGridView2.DataSource = dsDiagnoses.Tables[0];

                // Настройка заголовков столбцов для DataGridView2
                dataGridView2.Columns["DiagnosisId"].HeaderText = "ID Диагноза";
                dataGridView2.Columns["DiagnosisName"].HeaderText = "Диагноз";
                dataGridView2.Columns["Description"].HeaderText = "Описание";

                // Ширина столбцов
                dataGridView2.Columns["Description"].Width = 415;
                dataGridView2.Columns["DiagnosisName"].Width = 105;
                dataGridView2.Columns["ISDELETED"].Visible = false;
                dataGridView2.Columns["DiagnosisId"].Visible = false;

                dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
        }
        private void LoadDoctors()
        {
            string sql = "SELECT DoctorId, Surname_D AS Surname_d, First_Name_d AS First_Name_d, Patronymic_d AS Patronymic_d FROM Doctors WHERE IsDeleted = 0;";
            if (fbCon.State != ConnectionState.Open)
            {
                fbCon.Open();
            }

            try
            {
                using (FbCommand command = new FbCommand(sql, fbCon))
                {
                    FbDataAdapter adapter = new FbDataAdapter(command);
                    DataTable doctorsTable = new DataTable();

                    adapter.Fill(doctorsTable);

                    doctorsTable.Columns.Add("FullName", typeof(string), "Surname_d + ' ' + First_Name_d + ' ' + Patronymic_d");


                    comboBoxDoctors.DataSource = doctorsTable;
                    comboBoxDoctors.DisplayMember = "FullName";
                    comboBoxDoctors.ValueMember = "DoctorId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке врачей: {ex.Message}");
            }
            finally
            {

                if (fbCon.State == ConnectionState.Open)
                {
                    fbCon.Close();
                }
            }
        }



        private void LoadPatients()
        {
            string sql = "SELECT PatientId, Surname_D AS Surname_d, First_Name_d AS First_Name_d, Patronymic_d AS Patronymic_d FROM Patients WHERE IsDeleted = 0;";
            if (fbCon.State != ConnectionState.Open)
            {
                fbCon.Open();
            }
            try
            {
                using (FbCommand command = new FbCommand(sql, fbCon))
                {
                    FbDataAdapter adapter = new FbDataAdapter(command);
                    DataTable patientsTable = new DataTable();

                    adapter.Fill(patientsTable);

                    // Добавление столбца FullName
                    patientsTable.Columns.Add("FullName", typeof(string), "Surname_d + ' ' + First_Name_d + ' ' + Patronymic_d");

                    // Создание строки "Все"
                    DataRow allPatientsRow = patientsTable.NewRow();
                    allPatientsRow["PatientId"] = -1;
                    allPatientsRow["FullName"] = "Все";

                    // Вставка строки в начало таблицы
                    patientsTable.Rows.InsertAt(allPatientsRow, 0);

                    // Настройка ComboBox
                    comboBoxPatients.DisplayMember = "FullName";
                    comboBoxPatients.ValueMember = "PatientId";
                    comboBoxPatients.DataSource = patientsTable;

                    // Установка выбранного значения по умолчанию
                    comboBoxPatients.SelectedValue = -1; // Установите выбранное значение на "Все"
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пациентов: {ex.Message}");
            }
            finally
            {
                if (fbCon.State == ConnectionState.Open)
                {
                    fbCon.Close();
                }
            }
        }



        void LoadData(int? doctorId = null, int? patientId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Начинаем формировать SQL-запрос
            string query = @"
            SELECT 
                v.VisitId,
                d.Surname_D || ' ' || d.First_Name_d || ' ' || d.Patronymic_d AS DoctorFullName,
                p.Surname_D || ' ' || p.First_Name_d || ' ' || p.Patronymic_d AS PatientFullName,
                Snils,
                v.VisitDate, 
                diag.DiagnosisName
            FROM 
                Visits v
            JOIN 
                Doctors d ON v.DoctorId = d.DoctorId
            JOIN 
                Patients p ON v.PatientId = p.PatientId
            JOIN 
                Diagnoses diag ON v.DiagnosisId = diag.DiagnosisId
            WHERE 
                d.IsDeleted = 0 AND p.IsDeleted = 0 AND v.IsDeleted = 0";


            // Добавляем условия для фильтрации
            if (doctorId.HasValue)
            {
                query += " AND v.DoctorId = @DoctorId";
            }

            if (patientId.HasValue)
            {
                query += " AND v.PatientId = @PatientId";
            }

            if (startDate.HasValue)
            {
                query += " AND v.VisitDate >= @StartDate";
            }

            if (endDate.HasValue)
            {
                query += " AND v.VisitDate <= @EndDate";
            }

            try
            {
                using (FbCommand command = new FbCommand(query, fbCon))
                {
                    // Добавляем параметры к команде
                    if (doctorId.HasValue)
                        command.Parameters.AddWithValue("@DoctorId", doctorId.Value);

                    if (patientId.HasValue)
                        command.Parameters.AddWithValue("@PatientId", patientId.Value);

                    if (startDate.HasValue)
                        command.Parameters.AddWithValue("@StartDate", startDate.Value);

                    if (endDate.HasValue)
                        command.Parameters.AddWithValue("@EndDate", endDate.Value);

                    using (FbDataAdapter adapter = new FbDataAdapter(command))
                    {
                        DataTable visitsTable = new DataTable();
                        adapter.Fill(visitsTable);
                        dataGridView1.DataSource = visitsTable;

                        orglDataTable1 = visitsTable.Copy();
                        // Настройка заголовков столбцов
                        dataGridView1.Columns["DoctorFullName"].HeaderText = "Врач";
                        dataGridView1.Columns["PatientFullName"].HeaderText = "Пациент";
                        dataGridView1.Columns["Snils"].HeaderText = "Снилс";
                        dataGridView1.Columns["VisitDate"].HeaderText = "Дата визита";
                        dataGridView1.Columns["DiagnosisName"].HeaderText = "Диагноз";
                        dataGridView1.Columns["VisitId"].Visible = false;


                        // Установка ширины столбцов
                        dataGridView1.Columns["DoctorFullName"].Width = 140;
                        dataGridView1.Columns["PatientFullName"].Width = 140;
                        dataGridView1.Columns["VisitDate"].Width = 100;
                        dataGridView1.Columns["DiagnosisName"].Width = 100;
                        string sqlDiagnoses = "SELECT DiagnosisId, DiagnosisName FROM Diagnoses WHERE IsDeleted = 0";

                        // Загружаем данные для DataGridView2 (Diagnoses)
                        using (FbDataAdapter adapterPatients = new FbDataAdapter(sqlDiagnoses, fbCon))
                        {
                            dsDiagnoses = new DataSet();
                            adapterPatients.Fill(dsDiagnoses);
                            orglDataTable2 = dsDiagnoses.Tables[0].Copy();
                            // Устанавливаем источник данных для DataGridView2
                            dataGridView2.DataSource = dsDiagnoses.Tables[0];

                            // Настройка заголовков столбцов для DataGridView2
                            dataGridView2.Columns["DiagnosisId"].HeaderText = "ID Диагноза";
                            dataGridView2.Columns["DiagnosisName"].HeaderText = "Диагноз";

                            // Ширина столбцов
                            dataGridView2.Columns["DiagnosisName"].Width = 545;

                            dataGridView2.Columns["DiagnosisId"].Visible = false;

                            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
            finally
            {
                // Закрытие соединения, если оно открыто
                if (fbCon.State == ConnectionState.Open)
                {
                    fbCon.Close();
                }
            }
        }
        void PerS()
        {
            using (FbCommand command = new FbCommand("GetSpecialistsList", fbCon))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (FbDataAdapter adapter = new FbDataAdapter(command))
                {
                    DataTable specialistsTable = new DataTable();
                    adapter.Fill(specialistsTable);

                    if (specialistsTable.Rows.Count > 0)
                    {
                        dataGridView3.DataSource = specialistsTable;

                        // Настройка заголовков столбцов
                        dataGridView3.Columns["Specialty"].HeaderText = "Специальность";
                        dataGridView3.Columns["FullName"].HeaderText = "ФИО";
                        dataGridView3.Columns["Specialty"].Width = 260;
                        dataGridView3.Columns["FullName"].Width = 260;

                        // Скрытие столбца DoctorId
                        if (dataGridView3.Columns.Contains("DoctorId"))
                        {
                            dataGridView3.Columns["DoctorId"].Visible = false;
                        }

                        dataGridView3.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    }
                    else
                    {
                        MessageBox.Show("Нет данных для отображения в таблице специалистов.");
                    }
                }
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            string inputText = textSearch22.Text;

            if (!string.IsNullOrEmpty(inputText))
            {
                SearchDiagnoses(inputText);
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите текст для поиска.");
            }
        }
        private void SearchDiagnoses(string inputText)
        {
            try
            {
                fbCon.Open();

                string query = @"
        SELECT 
            DiagnosisName,
            Description
        FROM  
            Diagnoses
        WHERE 
            IsDeleted = 0 AND 
            UPPER(DiagnosisName) LIKE @InputText";

                using (FbCommand command = new FbCommand(query, fbCon))
                {
                    string searchText = "%" + inputText.ToUpper() + "%";
                    command.Parameters.AddWithValue("@InputText", searchText);
                    FbDataAdapter adapter = new FbDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Проверка на наличие записей
                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("Запись не найдена.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        dataGridView2.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Подключение к базе данных: " + ex.Message);
            }
            finally
            {
                if (fbCon.State == ConnectionState.Open)
                    fbCon.Close();
            }
        }

        void Vis()
        {
            string query = @"
        SELECT d.FullName AS DoctorName, 
               COUNT(v.VisitId) AS VisitCount
        FROM Doctors d
        LEFT JOIN Visits v ON d.DoctorId = v.DoctorId
        WHERE d.IsDeleted = 0
        GROUP BY d.FullName
        ORDER BY VisitCount DESC;";

            using (FbCommand command = new FbCommand(query, fbCon))
            {
                try
                {
                    fbCon.Open();

                    using (FbDataAdapter adapter = new FbDataAdapter(command))
                    {
                        DataTable visitCountsTable = new DataTable();
                        adapter.Fill(visitCountsTable);

                        if (visitCountsTable.Rows.Count > 0)
                        {
                            dataGridView3.DataSource = visitCountsTable;

                            dataGridView3.Columns["DoctorName"].HeaderText = "ФИО врача";
                            dataGridView3.Columns["VisitCount"].HeaderText = "Количество посещений";
                            dataGridView3.Columns["DoctorName"].Width = 260;
                            dataGridView3.Columns["VisitCount"].Width = 260;

                            dataGridView3.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                        }
                        else
                        {
                            MessageBox.Show("Нет данных для отображения.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
                finally
                {
                    // Закрытие соединения, если оно открыто
                    if (fbCon.State == ConnectionState.Open)
                    {
                        fbCon.Close();
                    }
                }
                // Закрытие соединения происходит автоматически благодаря using
            }
        }

        void DisCounts()
        {
            string query = @"
            SELECT diag.DiagnosisName, 
                   COUNT(v.VisitId) AS DiagnosisCount
            FROM Diagnoses diag
            LEFT JOIN Visits v ON diag.DiagnosisId = v.DiagnosisId
            WHERE diag.IsDeleted = 0
            GROUP BY diag.DiagnosisName
            ORDER BY DiagnosisCount DESC;";

            using (FbCommand command = new FbCommand(query, fbCon))
            {
                try
                {
                    if (fbCon.State != ConnectionState.Open)
                    {
                        fbCon.Open();
                    }

                    using (FbDataAdapter adapter = new FbDataAdapter(command))
                    {
                        DataTable diagnosisCountsTable = new DataTable();
                        adapter.Fill(diagnosisCountsTable);

                        if (diagnosisCountsTable.Rows.Count > 0)
                        {
                            dataGridView3.DataSource = diagnosisCountsTable;

                            dataGridView3.Columns["DiagnosisName"].HeaderText = "Название диагноза";
                            dataGridView3.Columns["DiagnosisCount"].HeaderText = "Количество посещений";
                            dataGridView3.Columns["DiagnosisName"].Width = 260;
                            dataGridView3.Columns["DiagnosisCount"].Width = 260;

                            dataGridView3.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                        }
                        else
                        {
                            MessageBox.Show("Нет данных для отображения.");
                        }
                    }
                }
                catch (FbException fbEx)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + fbEx.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
                finally
                {
                    if (fbCon.State == ConnectionState.Open)
                    {
                        fbCon.Close();
                    }
                }
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = orglDataTable2;
            // Устанавливаем источник данных для DataGridView2
            dataGridView2.DataSource = dsDiagnoses.Tables[0];

            // Настройка заголовков столбцов для DataGridView2
            dataGridView2.Columns["DiagnosisId"].HeaderText = "ID Диагноза";
            dataGridView2.Columns["DiagnosisName"].HeaderText = "Диагноз";
            dataGridView2.Columns["Description"].HeaderText = "Описание";
            // Ширина столбцов
            dataGridView2.Columns["DiagnosisName"].Width = 220;
            dataGridView2.Columns["Description"].Width = 320;

            dataGridView2.Columns["DiagnosisId"].Visible = false;

            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            PerS();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Vis();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Получаем выбранные значения из ComboBox
            int selectedDoctorId = (int)(comboBoxDoctors.SelectedValue ?? -1);
            int selectedPatientId = (int)(comboBoxPatients.SelectedValue ?? -1);
            DateTime startDate = dateTimePickerStart.Value.Date;
            DateTime endDate = dateTimePickerEnd.Value.Date;

            // Формируем SQL-запрос с фильтрацией
            string sql = "SELECT v.VisitId, " +
                         "d.Surname_D || ' ' || d.First_Name_d || ' ' || d.Patronymic_d AS DoctorName, " +
                         "p.Surname_D || ' ' || p.First_Name_d || ' ' || p.Patronymic_d AS PatientName, " +
                         "p.Snils AS Snils, " +  
                         "v.VisitDate, diag.DiagnosisName " +
                         "FROM Visits v " +
                         "JOIN Doctors d ON v.DoctorId = d.DoctorId " +
                         "JOIN Patients p ON v.PatientId = p.PatientId " +
                         "JOIN Diagnoses diag ON v.DiagnosisId = diag.DiagnosisId " +
                         "WHERE v.VisitDate BETWEEN @startDate AND @endDate AND v.IsDeleted = 0";

            if (selectedDoctorId != -1)
            {
                sql += " AND v.DoctorId = @doctorId";
            }

            if (selectedPatientId != -1)
            {
                sql += " AND v.PatientId = @patientId";
            }

            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open();

                using (FbCommand command = new FbCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);

                    if (selectedDoctorId != -1)
                    {
                        command.Parameters.AddWithValue("@doctorId", selectedDoctorId);
                    }

                    if (selectedPatientId != -1)
                    {
                        command.Parameters.AddWithValue("@patientId", selectedPatientId);
                    }

                    FbDataAdapter adapter = new FbDataAdapter(command);
                    DataTable visitsTable = new DataTable();
                    adapter.Fill(visitsTable);

                    dataGridView1.DataSource = visitsTable;
                    dataGridView1.Columns["DoctorName"].HeaderText = "Врач";
                    dataGridView1.Columns["PatientName"].HeaderText = "Пациент";
                    dataGridView1.Columns["Snils"].HeaderText = "СНИЛС";
                    dataGridView1.Columns["VisitDate"].HeaderText = "Дата визита";
                    dataGridView1.Columns["DiagnosisName"].HeaderText = "Диагноз";

                    dataGridView1.Columns["DoctorName"].Width = 140;
                    dataGridView1.Columns["PatientName"].Width = 140;
                    dataGridView1.Columns["Snils"].Width = 100;
                    dataGridView1.Columns["VisitDate"].Width = 100;
                    dataGridView1.Columns["DiagnosisName"].Width = 100;
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DisCounts();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var DiagnosisName = dataGridView2.CurrentRow.Cells["DiagnosisName"].Value?.ToString();

            // Проверка, что название диагноза содержит только русские буквы
            if (!IsCyrillic(DiagnosisName))
            {
                MessageBox.Show("Диагноз должен содержать только русские буквы.");
                return;
            }

            // Проверка, что название диагноза не пустое и начинается с заглавной буквы
            if (string.IsNullOrEmpty(DiagnosisName) || !char.IsUpper(DiagnosisName[0]))
            {
                MessageBox.Show("Диагноз должен начинаться с заглавной буквы.");
                return;
            }

            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open();

                // Проверка на дубликаты
                using (FbCommand checkDuplicateCommand = new FbCommand("SELECT COUNT(*) FROM Diagnoses WHERE DiagnosisName = @DiagnosisName", connection))
                {
                    checkDuplicateCommand.Parameters.AddWithValue("@DiagnosisName", DiagnosisName);
                    var duplicateCount = Convert.ToInt32(checkDuplicateCommand.ExecuteScalar());

                    if (duplicateCount > 0)
                    {
                        MessageBox.Show("Диагноз с таким названием уже существует.");
                        return;
                    }
                }

                // Добавление нового диагноза в таблицу Diagnoses
                using (FbCommand command = new FbCommand("INSERT INTO Diagnoses (DiagnosisName) VALUES (@DiagnosisName)", connection))
                {
                    command.Parameters.AddWithValue("@DiagnosisName", DiagnosisName);

                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Запись успешно добавлена.");

                        // Обновление DataGridView
                        LoadDoctors(); // Можно перезагрузить данные или добавить строку в DataTable
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)

        {
        }

        // Метод для проверки наличия только русских букв
        private bool IsCyrillic(string str)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str, @"^[А-Яа-яЁёs]+$");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                string DiagnosisName = dataGridView2.SelectedRows[0].Cells["DiagnosisName"].Value.ToString();
                SoftDeleteDiagnoses(DiagnosisName);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите врача для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void SoftDeleteDiagnoses(string DiagnosisName)
        {
            using (var connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open(); // Открываем соединение с базой данных

                // Создаем команду для выполнения SQL-запроса
                using (var command = new FbCommand("UPDATE Diagnoses SET IsDeleted = 1 WHERE DiagnosisName = @DiagnosisName AND IsDeleted = 0", connection))
                {
                    command.Parameters.AddWithValue("@DiagnosisName", DiagnosisName);

                    int rowsAffected = command.ExecuteNonQuery(); // Выполняем команду и получаем количество затронутых строк

                    if (rowsAffected > 0)
                    {
                        // Обновляем DataGridView после удаления
                        LoadDiagnoses();

                        MessageBox.Show("Диагноз успешно удален.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Диагноз не найден или уже удален.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private int GetIdByName(string tableName, string idColumn, string nameColumn, string nameValue)
        {
            if (string.IsNullOrWhiteSpace(nameValue))
            {
                return -1; // Возвращаем -1, если имя пустое или null
            }
            try
            {
                using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
                {
                    connection.Open();
                    using (FbCommand command = new FbCommand($"SELECT {idColumn} FROM {tableName} WHERE {nameColumn} = @NameValue", connection))
                    {
                        command.Parameters.AddWithValue("@NameValue", nameValue);
                        var result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении ID: {ex.Message}");
                return -1; // Возвращаем -1 в случае ошибки
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Получаем значения из ячеек текущей строки
            var doctorName = dataGridView1.CurrentRow.Cells["DoctorFullName"].Value?.ToString();
            var patientName = dataGridView1.CurrentRow.Cells["PatientFullName"].Value?.ToString();
            var snils = dataGridView1.CurrentRow.Cells["Snils"].Value?.ToString();
            var visitDateString = dataGridView1.CurrentRow.Cells["VisitDate"].Value?.ToString();
            var diagnosisName = dataGridView1.CurrentRow.Cells["DiagnosisName"].Value?.ToString();

            // Проверка на пустые значения
            if (string.IsNullOrWhiteSpace(doctorName) ||
                string.IsNullOrWhiteSpace(patientName) ||
                string.IsNullOrWhiteSpace(diagnosisName))
            {
                MessageBox.Show("Пожалуйста, заполните все необходимые поля.");
                return;
            }

            // Проверка на корректность введенной даты
            if (!DateTime.TryParse(visitDateString, out DateTime visitDate))
            {
                MessageBox.Show("Введите корректную дату визита.");
                return;
            }

            // Получаем идентификаторы врача, пациента и диагноза
            int doctorId = GetIdByName("Doctors", "DoctorId", "DoctorFullName", doctorName);
            int patientId = GetIdByName("Patients", "PatientId", "PatientFullName", patientName);
            int diagnosisId = GetIdByName("Diagnoses", "DiagnosisId", "DiagnosisName", diagnosisName);

            // Проверка на успешное получение всех идентификаторов
            if (doctorId == -1 || patientId == -1 || diagnosisId == -1)
            {
                MessageBox.Show("Не удалось найти врача, пациента или диагноз.");
                return;
            }
            // Проверка диапазона даты визита
            if (visitDate < new DateTime(2020, 1, 1) || visitDate > new DateTime(2030, 12, 31))
            {
                MessageBox.Show("Дата визита должна быть в диапазоне от 1 января 2020 года до 31 декабря 2030 года.");
                return;
            }
            // Проверка на дубликаты
            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open(); // Открываем соединение с базой данных
                // Проверяем наличие дубликата
                using (FbCommand checkDuplicateCommand = new FbCommand("SELECT COUNT(*) FROM Visits WHERE PatientId = @patientId AND DoctorId = @doctorId AND VisitDate = @visitDate", connection))
                {
                    checkDuplicateCommand.Parameters.AddWithValue("@patientId", patientId);
                    checkDuplicateCommand.Parameters.AddWithValue("@doctorId", doctorId);
                    checkDuplicateCommand.Parameters.AddWithValue("@visitDate", visitDate);

                    var duplicateCount = Convert.ToInt32(checkDuplicateCommand.ExecuteScalar());
                    if (duplicateCount > 0)
                    {
                        MessageBox.Show("Запись с таким пациентом, врачом и датой визита уже существует.");
                        return;
                    }
                }
                // Добавление записи в таблицу Visits
                using (FbCommand command = new FbCommand("INSERT INTO Visits (PatientId, DoctorId, VisitDate, DiagnosisId) VALUES (@patientId, @doctorId, @visitDate, @diagnosisId)", connection))
                {
                    // Добавляем параметры к команде
                    command.Parameters.AddWithValue("@patientId", patientId);
                    command.Parameters.AddWithValue("@doctorId", doctorId);
                    command.Parameters.AddWithValue("@visitDate", visitDate);
                    command.Parameters.AddWithValue("@diagnosisId", diagnosisId);
                    try
                    {
                        command.ExecuteNonQuery(); // Выполняем команду вставки
                        MessageBox.Show("Запись успешно добавлена.");
                        // Обновление DataGridView с новыми данными
                        LoadData(); // Метод для перезагрузки данных из таблицы Visits
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
                    }
                }
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрана ли строка в DataGridView
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления.");
                return;
            }

            // Получаем идентификатор визита из текущей строки
            int visitId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["VisitId"].Value);

            // Подтверждение удаления
            var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                                                 "Подтверждение удаления",
                                                 MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
            {
                return; // Если пользователь отменил, выходим из метода
            }

            // Мягкое удаление записи из таблицы Visits
            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open(); // Открываем соединение с базой данных

                using (FbCommand command = new FbCommand("UPDATE Visits SET IsDeleted = 1 WHERE VisitId = @visitId AND IsDeleted = 0", connection))
                {
                    command.Parameters.AddWithValue("@visitId", visitId);

                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery(); // Выполняем команду обновления

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Запись успешно удалена.");
                            // Обновление DataGridView с новыми данными
                            LoadData(); // Метод для перезагрузки данных из таблицы Visits
                        }
                        else
                        {
                            MessageBox.Show("Запись не найдена или уже удалена.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            // Получаем значения из ячеек текущей строки
            var visitIdString = dataGridView1.CurrentRow.Cells["VisitId"].Value?.ToString();
            var doctorName = dataGridView1.CurrentRow.Cells["DoctorName"].Value?.ToString();
            var patientName = dataGridView1.CurrentRow.Cells["PatientName"].Value?.ToString();
            var visitDateString = dataGridView1.CurrentRow.Cells["VisitDate"].Value?.ToString();
            var diagnosisName = dataGridView1.CurrentRow.Cells["DiagnosisName"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(doctorName) || string.IsNullOrWhiteSpace(patientName) || string.IsNullOrWhiteSpace(diagnosisName))
            {
                MessageBox.Show("Пожалуйста, заполните все необходимые поля.");
                return;
            }

            if (!DateTime.TryParse(visitDateString, out DateTime visitDate))
            {
                MessageBox.Show("Введите корректную дату визита.");
                return;
            }

            int doctorId = GetIdByName("Doctors", "DoctorId", "FullName", doctorName);
            int patientId = GetIdByName("Patients", "PatientId", "FullName", patientName);
            int diagnosisId = GetIdByName("Diagnoses", "DiagnosisId", "DiagnosisName", diagnosisName);

            // Проверка на успешное получение всех идентификаторов
            if (doctorId == -1 || patientId == -1 || diagnosisId == -1)
            {
                MessageBox.Show("Не удалось найти врача, пациента или диагноз.");
                return;
            }

            // Проверяем, что VisitId не пустой
            if (!int.TryParse(visitIdString, out int visitId))
            {
                MessageBox.Show("Не удалось получить идентификатор визита.");
                return;
            }

            // Обновление записи в таблице Visits
            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open(); // Открываем соединение с базой данных

                using (FbCommand command = new FbCommand("UPDATE Visits SET PatientId = @patientId, DoctorId = @doctorId, VisitDate = @visitDate, DiagnosisId = @diagnosisId WHERE VisitId = @visitId", connection))
                {
                    // Добавляем параметры к команде
                    command.Parameters.AddWithValue("@patientId", patientId);
                    command.Parameters.AddWithValue("@doctorId", doctorId);
                    command.Parameters.AddWithValue("@visitDate", visitDate);
                    command.Parameters.AddWithValue("@diagnosisId", diagnosisId);
                    command.Parameters.AddWithValue("@visitId", visitId);

                    try
                    {
                        command.ExecuteNonQuery(); // Выполняем команду обновления
                        MessageBox.Show("Запись успешно обновлена.");
                        // Обновление DataGridView с новыми данными
                        LoadData(); // Метод для перезагрузки данных из таблицы Visits
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении записи: {ex.Message}");
                    }
                }
            }
        }
        private void comboBoxDoctors_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Проверяем, если выбран элемент, и если это не "Врач"
            if (comboBoxDoctors.SelectedIndex != -1 && comboBoxDoctors.Text == "Врач")
            {
                // Очищаем текст, чтобы слово "Врач" пропало
                comboBoxDoctors.Text = comboBoxDoctors.SelectedItem.ToString();
            }
        }
        private void ComboBoxDoctors_SelectedIndexChanged(object sender, EventArgs e)
        {
            // При фокусировке на ComboBox очищаем текст, если это текст по умолчанию
            if (comboBoxDoctors.Text == "Врач")
            {
                comboBoxDoctors.Text = string.Empty;
            }
        }
        private void button5_Click_1(object sender, EventArgs e)
        {
            if (orglDataTable1 != null)
            {
                // Возвращаем исходную таблицу без фильтрации
                dataGridView1.DataSource = orglDataTable1;
            }
            // Настройка заголовков столбцов
            dataGridView1.Columns["DoctorFullName"].HeaderText = "Врач";
            dataGridView1.Columns["PatientFullName"].HeaderText = "Пациент";
            dataGridView1.Columns["Snils"].HeaderText = "Снилс";
            dataGridView1.Columns["VisitDate"].HeaderText = "Дата визита";
            dataGridView1.Columns["DiagnosisName"].HeaderText = "Диагноз";
            dataGridView1.Columns["VisitId"].Visible = false;


            // Установка ширины столбцов
            dataGridView1.Columns["DoctorFullName"].Width = 140;
            dataGridView1.Columns["PatientFullName"].Width = 140;
            dataGridView1.Columns["VisitDate"].Width = 100;
            dataGridView1.Columns["DiagnosisName"].Width = 100;
        }
    }
}
