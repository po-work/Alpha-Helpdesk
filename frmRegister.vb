Imports System.Security.Cryptography 'Needed for HASHING
Imports System.Text 'Needed for HASHING
Imports MySql.Data.MySqlClient

Public Class frmRegister
    'MySql/Needed variable definitions
    Dim StaffRegCode As String 'staff registration code
    Dim gainedstafflvl As Integer 'Gained staff level based upon entry to the inputbox
    Dim mysqlserver As String 'MySQL sever IP
    Dim mysqlport As String 'MySQL server port
    Dim mysqluser As String 'MySQL username
    Dim mysqlpassword As String 'MySQL password
    'Salt generation function
    Public Function createrandomsalt()
        Dim rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider
        Dim buff(15) As Byte
        rng.GetBytes(buff)
        Return Convert.ToBase64String(buff)
    End Function
    'Hash function
    Public Function hash256(ByVal password As String, ByVal salt As String) As String
        Dim convertedtobytes As Byte() = Encoding.UTF8.GetBytes(password & salt)
        Dim hashtype As HashAlgorithm = New SHA256Managed()
        Dim generatehash As Byte() = hashtype.ComputeHash(convertedtobytes)
        Dim finalhash As String = Convert.ToBase64String(generatehash)
        Return finalhash
    End Function

    Private Sub cbRegisterStaffYes_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbRegisterStaffYes.CheckedChanged
        'Define reg code variables
        Dim message, title As String
        Dim serveradmincode As String
        message = "Enter your staff registration code here"
        title = "Staff Registration Code"
        'show input box to enter the reg code
        StaffRegCode = InputBox(message, title)

        Dim con As New MySqlConnection()
        Dim cmd As New MySqlCommand()
        'select the existing code from DB
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            cmd.Connection = con
            cmd.CommandText = "SELECT * FROM admincode"
            'cmd.Parameters.AddWithValue("@number", 1)
            con.Open()
            cmd.ExecuteNonQuery()
            Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
            While sqlreader.Read()
                serveradmincode = sqlreader("admincode").ToString()
            End While
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            Exit Sub
        Finally
            con.Close()
        End Try
        'Check if codes match, if yes then assign staff level
        If StaffRegCode = serveradmincode Then
            gainedstafflvl = 1
        Else
            gainedstafflvl = 0
        End If
    End Sub

    Private Sub cbRegisterStaffNo_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbRegisterStaffNo.CheckedChanged
        'Set regcode to nothing if not signing up as staff
        StaffRegCode = ""
    End Sub
    Private Sub btnRegisterCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRegisterCancel.Click
        'Reset all text boxes
        tbRegisterUsername.Text = ""
        tbRegisterPasswordConfirm.Text = ""
        tbRegisterFN.Text = ""
        tbRegisterSN.Text = ""
        StaffRegCode = ""
        frmLogin.Show()
        Me.Close()
    End Sub

    Private Sub btnRegisterSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRegisterSubmit.Click
        'Defining variables
        Dim user As String = tbRegisterUsername.Text
        Dim fname As String = tbRegisterFN.Text
        Dim sname As String = tbRegisterSN.Text
        Dim salt As String = createrandomsalt()
        'Check if passwords match
        If tbRegisterPassword.Text = tbRegisterPasswordConfirm.Text Then
            'If yes then hash with generated salt
            Dim DBHashedPW As String = (hash256(tbRegisterPasswordConfirm.Text, salt))
            Dim DBSalt As String = salt
            Dim con As New MySqlConnection()
            Dim cmd As New MySqlCommand()
            'Insert new user into USERS table
            Try
                con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
                con.Open()
                cmd.Connection = con
                cmd.CommandText = "INSERT INTO users(username, hash, salt, fname, sname, adminlevel) VALUES(@user, @hash, @salt, @fname, @sname, @alvl)"
                cmd.Parameters.Add("@user", MySqlDbType.VarChar, 255).Value = user
                cmd.Parameters.Add("@hash", MySqlDbType.VarChar, 255).Value = DBHashedPW
                cmd.Parameters.Add("@salt", MySqlDbType.VarChar, 255).Value = DBSalt
                cmd.Parameters.Add("@fname", MySqlDbType.String, 255).Value = fname
                cmd.Parameters.Add("@sname", MySqlDbType.String, 255).Value = sname
                cmd.Parameters.Add("@alvl", MySqlDbType.Int16, 255).Value = gainedstafflvl
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                con.Close()
            End Try
            MsgBox("Registration Successful!")
            frmLogin.Show()
            Me.Close()
        Else
            MsgBox("Passwords do not match")
        End If
    End Sub

    Private Sub frmRegister_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Defining MySql variables
        mysqlserver = My.Settings.mysqlip
        mysqlport = My.Settings.mysqlport
        mysqluser = My.Settings.mysqluser
        mysqlpassword = My.Settings.mysqlpass
    End Sub
End Class