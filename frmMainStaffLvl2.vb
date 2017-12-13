Imports System.Security.Cryptography 'Needed for HASHING
Imports System.Text 'Needed for HASHING
Imports MySql.Data.MySqlClient
Public Class frmMainStaffLvl2
    'MySql Variables
    Dim mysqlserver As String
    Dim mysqlport As Integer
    Dim mysqluser As String
    Dim mysqlpassword As String
    Dim con As New MySqlConnection()
    Dim cmd As New MySqlCommand()
    'Hashing functions
    Public Function hash256(ByVal password As String, ByVal salt As String) As String
        Dim convertedtobytes As Byte() = Encoding.UTF8.GetBytes(password & salt)
        Dim hashtype As HashAlgorithm = New SHA256Managed()
        Dim generatehash As Byte() = hashtype.ComputeHash(convertedtobytes)
        Dim finalhash As String = Convert.ToBase64String(generatehash)
        Return finalhash
    End Function
    'Salt generation function
    Public Function CreateAdminLevelCode()
        Dim rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider
        Dim buff(15) As Byte
        rng.GetBytes(buff)
        Return Convert.ToBase64String(buff)
    End Function
    Private Sub frmMainStaffLvl2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Setting MySql variables for connection purposes
        mysqlserver = My.Settings.mysqlip
        mysqlport = My.Settings.mysqlport
        mysqluser = My.Settings.mysqluser
        mysqlpassword = My.Settings.mysqlpass
        Dim newpassword As String
        'Addings tables to combo box to allow user to select which table to view
        cbTables.Items.Add("users")
        cbTables.Items.Add("admincode")
        cbTables.Items.Add("games")
        cbTables.Items.Add("questions")
        'Force change Tech Admin pass if it is set as default, this is when the first user is inserted to the table, only happens on fresh install of program on network
        If My.Settings.password = "tGp4rfVxe1jwQjpd7yx8GwiMVqsP88n9Vcs93x/D3WU=" Then
            newpassword = InputBox("Enter New Password", "Change default password", "")
            Dim salt As String
            'Grab salt from database to set append to new password
            Try
                con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
                cmd.Connection = con
                cmd.CommandText = "SELECT * FROM users WHERE username = @userFind"
                cmd.Parameters.Add("@userFind", MySqlDbType.VarChar, 255).Value = "MainAdmin123"
                con.Open()
                cmd.ExecuteNonQuery()
                Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
                While sqlreader.Read()
                    salt = sqlreader("salt").ToString
                End While
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
                Exit Sub
            Finally
                con.Close()
            End Try
            'Hash new password with existing salt in database that has been pulled to keep consistancy 
            newpassword = hash256(newpassword, salt)
            'Update new pass on database so lvl2 staff can use to login
            Try
                con.Open()
                cmd.CommandText = "UPDATE users SET hash = @newhash WHERE username = @user"
                cmd.Parameters.Add("@newhash", MySqlDbType.VarChar, 255).Value = newpassword
                cmd.Parameters.Add("@user", MySqlDbType.VarChar, 255).Value = "MainAdmin123"
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                con.Close()
            End Try
        End If
    End Sub
    Private Sub btnCreateDatabase_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateDatabase.Click
        btnCreateDatabase.Enabled = False 'Filters out button spamming
        Dim con As New MySqlConnection()
        Dim cmd As New MySqlCommand()
        'Create database button, can change database name if wanted, howwever all con.connectionstrings would have to be modified too.
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=;")
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "CREATE DATABASE main"
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        btnCreateDatabase.Enabled = True
    End Sub

    Private Sub btnConnectToMySql_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnectToMySql.Click
        btnConnectToMySql.Enabled = False 'Filters out button spamming
        'Allows debugging connection to MySql DB or to set a new MySql server to run on
        Dim con As New MySqlConnection()
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=;")
            con.Open()
            MessageBox.Show("Database connection was successful. New details set.")
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
            con.Dispose()
        End Try
        'Save all information in program settings as perm variables
        My.Settings.mysqluser = tbMySqlUser.Text
        My.Settings.mysqlpass = tbMySqlPass.Text
        My.Settings.mysqlip = tbMySqlServer.Text
        My.Settings.mysqlport = tbMySqlPort.Text
        My.Settings.Save()
        My.Settings.Reload()
        btnConnectToMySql.Enabled = True
    End Sub

    Private Sub btnCreateUserTable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateUserTable.Click
        Dim con As New MySqlConnection()
        Dim cmd As New MySqlCommand()
        btnCreateUserTable.Enabled = False 'Filters out button spamming
        'Allows creation of USERS table, same code as startup
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS users(ID int NOT NULL AUTO_INCREMENT, username varchar(255),hash varchar(255),salt varchar(255),fname char(255),sname char(255),adminlevel int, PRIMARY KEY(id))"
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        Try
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "INSERT INTO users(username, hash, salt, fname, sname, adminlevel) VALUES(@user, @hash, @salt, @fname, @sname, @alvl)"
            cmd.Parameters.Add("@user", MySqlDbType.VarChar, 255).Value = "MainAdmin123"
            cmd.Parameters.Add("@hash", MySqlDbType.VarChar, 255).Value = "tGp4rfVxe1jwQjpd7yx8GwiMVqsP88n9Vcs93x/D3WU="
            cmd.Parameters.Add("@salt", MySqlDbType.VarChar, 255).Value = "+TY2ktlNH5LcgT0m5rvyEw=="
            cmd.Parameters.Add("@fname", MySqlDbType.String, 255).Value = "Main"
            cmd.Parameters.Add("@sname", MySqlDbType.String, 255).Value = "Admin"
            cmd.Parameters.Add("@alvl", MySqlDbType.Int16, 255).Value = 2
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        btnCreateUserTable.Enabled = True
    End Sub

    Private Sub btnCreateTableAdmin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateTableAdmin.Click
        btnCreateTableAdmin.Enabled = False 'Filters out button spamming
        Dim con As New MySqlConnection()
        Dim cmd As New MySqlCommand()
        'Creation of ADMINCODE table, same code as startup
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS admincode(admincode varchar(255))"
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        btnCreateTableAdmin.Enabled = True
    End Sub
    Private Sub btnGenerateStaffCode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerateStaffCode.Click
        btnGenerateStaffCode.Enabled = False 'Filters out button spamming
        Dim con As New MySqlConnection()
        Dim cmd As New MySqlCommand()
        Dim newcode As String = CreateAdminLevelCode()
        Dim oldcode As String
        'Generates new Reg Code for Lvl1 customer support members
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            cmd.Connection = con
            cmd.CommandText = "SELECT * FROM admincode"
            con.Open()
            cmd.ExecuteNonQuery()
            Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
            While sqlreader.Read()
                oldcode = sqlreader("admincode").ToString()
            End While
            con.Close()
            'overwrites existing code so only one code exists at a time
            cmd.CommandText = "UPDATE admincode SET admincode = @code WHERE admincode = @oldcode"
            cmd.Parameters.Add("@code", MySqlDbType.VarChar, 255).Value = newcode
            cmd.Parameters.Add("@oldcode", MySqlDbType.VarChar, 255).Value = oldcode
            con.Open()
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        tbNewCode.Text = newcode
        btnGenerateStaffCode.Enabled = True
    End Sub

    Private Sub btnLogout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogout.Click
        'Allows user to logout
        frmLogin.Show()
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles btnCreateGamesTable.Click
        btnCreateGamesTable.Enabled = False
        Dim con As New MySqlConnection()
        Dim cmd As New MySqlCommand()
        'Creation of Games table, same code as startup file
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS games(game varchar(255))"
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        btnCreateGamesTable.Enabled = True
    End Sub

    Private Sub btnCreateQuestionsTable_Click(sender As System.Object, e As System.EventArgs) Handles btnCreateQuestionsTable.Click
        btnCreateQuestionsTable.Enabled = False
        Dim con As New MySqlConnection
        Dim cmd As New MySqlCommand
        'CREATE QUESTIONS TABLE, same code as startup
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS questions(ID int NOT NULL AUTO_INCREMENT,issuetocompare varchar(255),issuetoshow varchar(255),game varchar(255),fix varchar(255),PRIMARY KEY(id))"
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        btnCreateQuestionsTable.Enabled = True
    End Sub

    Private Sub btnShowTable_Click(sender As System.Object, e As System.EventArgs) Handles btnShowTable.Click
        btnShowTable.Enabled = False 'Filters out button spamming
        'Populate DGV with table data
        If cbTables.Text = "" Then 'If no table selected in combo box, notify user and close
            MessageBox.Show("No table selected")
            Exit Sub
        End If
        Dim Adapter As New MySqlDataAdapter
        Dim data As New DataTable
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            con.Open()
            cmd.Connection = con
            'Grab all from specified table the user requests
            cmd.CommandText = "SELECT * FROM " & cbTables.Text
            Adapter.SelectCommand = cmd
            'Fill the DGV
            Adapter.Fill(data)
            DGVAdmin2.DataSource = data
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try
        btnShowTable.Enabled = True
    End Sub
End Class