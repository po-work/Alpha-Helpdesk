Imports System.Security.Cryptography 'Needed for HASHING
Imports System.Text 'Needed for HASHING
Imports MySql.Data.MySqlClient

Public Class frmLogin
    'Variable definitions
    Dim mysqlserver As String
    Dim mysqlport As Integer
    Dim mysqluser As String
    Dim mysqlpassword As String
    Dim username As String
    Dim plainpass As String
    Dim hashpass As String
    Dim salt As String
    Dim adminlvl As Integer
    'Hashing function
    Public Function hash256(ByVal password As String, ByVal salt As String) As String
        Dim convertedtobytes As Byte() = Encoding.UTF8.GetBytes(password & salt)
        Dim hashtype As HashAlgorithm = New SHA256Managed()
        Dim generatehash As Byte() = hashtype.ComputeHash(convertedtobytes)
        Dim finalhash As String = Convert.ToBase64String(generatehash)
        Return finalhash
    End Function

    Private Sub btnLogin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        username = tbUsername.Text
        plainpass = tbPassword.Text

        Dim con As New MySqlConnection()
        Dim cmd As New MySqlCommand()
        Try
            con.ConnectionString = ("server=" & mysqlserver & ";Port=" & mysqlport & ";userid=" & mysqluser & ";password=" & mysqlpassword & ";database=main;")
            cmd.Connection = con
            cmd.CommandText = "SELECT * FROM users WHERE username = @userLogin"
            cmd.Parameters.AddWithValue("@userLogin", username)
            con.Open()
            cmd.ExecuteNonQuery()
            Dim sqlreader As MySqlDataReader = cmd.ExecuteReader
            While sqlreader.Read()
                hashpass = sqlreader("hash").ToString()
                salt = sqlreader("salt").ToString
                'fname = sqlreader("fname").ToString
                'sname = sqlreader("sname").ToString
                adminlvl = sqlreader("adminlevel").ToString
            End While
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            Exit Sub
        Finally
            con.Close()
        End Try
        'Hash entered password
        plainpass = hash256(plainpass, salt)
        'Check if a password has been grabbed from DB
        If hashpass = "" Then
            MsgBox("Account doesn't exist in the database")
        ElseIf plainpass = hashpass And adminlvl = 0 Then
            If cbRemember.Checked = True Then
                My.Settings.username = tbUsername.Text
                My.Settings.Save()
                My.Settings.Reload()
            End If
            My.Settings.password = plainpass
            My.Settings.usernameforinfo = username
            My.Settings.Save()
            My.Settings.Reload()
            frmMainCustomer.Show()
            Me.Close()
            'Compare entered pass with stored pass and check admin level
        ElseIf plainpass = hashpass And adminlvl = 1 Then
            If cbRemember.Checked = True Then
                My.Settings.username = tbUsername.Text
                My.Settings.Save()
                My.Settings.Reload()
            End If
            My.Settings.password = plainpass
            My.Settings.usernameforinfo = username
            My.Settings.Save()
            My.Settings.Reload()
            My.Settings.username = username
            frmMainStaffLvl1.Show()
            Me.Close()
            'Compare entered pass with stored pass and check admin level
        ElseIf plainpass = hashpass And adminlvl = 2 Then
            If cbRemember.Checked = True Then
                My.Settings.username = username
                My.Settings.Save()
                My.Settings.Reload()
            End If
            My.Settings.password = plainpass
            My.Settings.usernameforinfo = username
            My.Settings.Save()
            My.Settings.Reload()
            frmMainStaffLvl2.Show()
            Me.Close()
            'Compare entered pass with stored pass and check admin level
        Else
            MsgBox("Incorrect Login Details")
            hashpass = ""
            salt = ""
            'fname = ""
            'sname = ""
            adminlvl = 0
        End If
    End Sub

    Private Sub btnRegister_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRegister.Click
        frmRegister.Show()
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        'Reset TB's
        tbPassword.Text = ""
        tbUsername.Text = ""
    End Sub

    Private Sub frmLogin_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        'Define MySql variables
        mysqlserver = My.Settings.mysqlip
        mysqlport = My.Settings.mysqlport
        mysqluser = My.Settings.mysqluser
        mysqlpassword = My.Settings.mysqlpass
        tbUsername.Text = My.Settings.username
    End Sub

End Class
